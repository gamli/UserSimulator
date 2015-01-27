using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroLanguage;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class ExpressionEvaluator_TEST
   {
      [TestMethod]
      public void Constant_TEST()
      {
         AssertExpressionEvaluatesTo(new Constant(null), "null");

         AssertExpressionEvaluatesTo(new Constant(true), "true");
         AssertExpressionEvaluatesTo(new Constant(false), "false");

         AssertExpressionEvaluatesTo(new Constant("test with >> \" <<"), "\"test with >> \\\" <<\"");

         AssertExpressionEvaluatesTo(new Constant(4711m), "4711");
         AssertExpressionEvaluatesTo(new Constant(-4711m), "-4711");

         AssertExpressionEvaluatesTo(new Constant(-4711.1174m), "-4711.1174");
         AssertExpressionEvaluatesTo(new Constant(4711.1174m), "4711.1174");
      }

      [TestMethod]
      public void Definition_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         AssertExpressionEvaluatesTo(new Constant("Hello World"), "(define var \"Hello World\")", context);
         Assert.AreEqual(new Constant("Hello World"), context.GetValue(new Symbol("var")));
      }


      [TestMethod]
      public void ExpressionList_TEST()
      {
         var expressionList = new List();
         Assert.AreEqual(expressionList, new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate(expressionList));
      }

      [TestMethod]
      public void If_TEST()
      {
         AssertExpressionEvaluatesTo(new Constant(1m), "(if true (pause 1) (pause 2))");
         AssertExpressionEvaluatesTo(new Constant(2m), "(if false (pause 1) (pause 2))");
      }

      [TestMethod]
      public void Lambda_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var argumentSymbols = new List();
         var lambda = SpecialForms.Lambda(argumentSymbols, new Symbol("y"));
         AssertExpressionEvaluatesTo(
            new Procedure { DefiningContext = context, FormalArguments = argumentSymbols, Lambda = lambda }, 
            "(lambda () y)", 
            context);

         argumentSymbols.Expressions.Add(new Symbol("x"));
         argumentSymbols.Expressions.Add(new Symbol("y"));
         AssertExpressionEvaluatesTo(
            new Procedure { DefiningContext = context, FormalArguments = argumentSymbols, Lambda = lambda }, 
            "(lambda (x y) y)", 
            context);
      }


      [TestMethod]
      public void List_TEST()
      {
         var emptyList = new List();
         Assert.AreEqual(emptyList, new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate(emptyList));
      }

      [TestMethod]
      public void Loop_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var counter = 10;
         context.DefineValue(
            new Symbol("loopTestCondition"),
            new IntrinsicProcedure { Function = ContextBase => new Constant(counter-- > 0), FormalArguments = new List(), DefiningContext = context });
         var result = 0;
         context.DefineValue(
            new Symbol("loopTestBody"),
            new IntrinsicProcedure { Function = ContextBase => new Constant(++result), FormalArguments = new List(), DefiningContext = context });

         AssertExpressionEvaluatesTo(new Constant(10), "(loop (loopTestCondition) (loopTestBody))", context);
         Assert.AreEqual(-1, counter);
         Assert.AreEqual(10, result);

         // loop is not executed => nil return
         AssertExpressionEvaluatesTo(new List(), "(loop (loopTestCondition) (loopTestBody))", context);
      }

      [TestMethod]
      public void ProcedureCall_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         AssertExpressionEvaluatesTo(new Constant(10m), "(define var1 (pause 10))", context);
         Assert.AreEqual(new Constant(10m), context.GetValue(new Symbol("var1")));
         AssertExpressionEvaluatesTo(new Constant(10m), "(define var2 ((lambda (duration) (pause duration)) 10))", context);
         Assert.AreEqual(new Constant(10m), context.GetValue(new Symbol("var2")));
      }

      [TestMethod]
      public void Quote_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var quotedExpression = ParseExpression("(fun arg1 arg2 arg3)");
         AssertExpressionEvaluatesTo(quotedExpression, "(define var (quote (fun arg1 arg2 arg3)))", context);
         Assert.AreEqual(quotedExpression, context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      public void SetValue_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var expectedValue = new Constant(4711m);
         AssertExpressionEvaluatesTo(expectedValue, "(begin (define var 4712) (set! var (- var 1)))", context);
         Assert.AreEqual(expectedValue, context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      public void Symbol_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var quotedExpression = ParseExpression("xyz");
         Assert.IsTrue(quotedExpression is Symbol);
         AssertExpressionEvaluatesTo(quotedExpression, "(define var (quote xyz))", context);
         Assert.AreEqual(quotedExpression, context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      public void BooleanConversion_TEST()
      {
         AssertExpressionEvaluatesTo(new Constant(2m), "(if nil 1 2)");
         AssertExpressionEvaluatesTo(new Constant(2m), "(if () 1 2)");
         AssertExpressionEvaluatesTo(new Constant(1m), "(if (quote (any valid list)) 1 2)");
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void ExceptionHandling_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         context.DefineValue(
            new Symbol("SomeFun"), 
            new IntrinsicProcedure { Function = SomeFun, FormalArguments = new List(), DefiningContext = context });
         var evaluator = new ExpressionEvaluator(context);

         try
         {
            evaluator.Evaluate(ParseExpression("(SomeFun)"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("SomeFun", e.InnerException.Message);
            Assert.IsTrue(e.InnerException is NotImplementedException);
         }

         try
         {
            evaluator.Evaluate(ParseExpression("(if \"somethingThatIsNotConvertibleToBoolean\" 1 2)"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.IsTrue(e.InnerException is FormatException);
         }

         try
         {
            evaluator.Evaluate(SpecialForms.If(SpecialForms.Quote(new Symbol("doesNotEvaluateToBoolean")), new Constant(1), new Constant(2)));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
      }

      [ExcludeFromCodeCoverage]
      private Expression SomeFun(ContextBase Context)
      {
         throw new NotImplementedException("SomeFun");
      }

      private void AssertExpressionEvaluatesTo(Expression ExpectedValue, string Expression, ContextBase Context = null)
      {
         if(Context == null)
            Context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(Context);
         var value = evaluator.Evaluate(ParseExpression(Expression));
         Assert.AreEqual(ExpectedValue, value);
      }

      private Expression ParseExpression(string Expression)
      {
         return (Expression)new MacroParser().Parse(Expression);
      }
   }
}
