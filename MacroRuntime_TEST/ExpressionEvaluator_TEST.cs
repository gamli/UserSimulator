using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
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
         AssertExpressionEvaluatesTo(new Constant(true), "True");
         AssertExpressionEvaluatesTo(new Constant(false), "False");

         AssertExpressionEvaluatesTo(new Constant("test with >> \" <<"), "\"test with >> \\\" <<\"");

         AssertExpressionEvaluatesTo(new Constant(4711), "4711");
         AssertExpressionEvaluatesTo(new Constant(-4711), "-4711");

         AssertExpressionEvaluatesTo(new Constant(-4711.1174), "-4711.1174");
         AssertExpressionEvaluatesTo(new Constant(4711.1174), "4711.1174");
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
         var expressionList = new ExpressionList();
         Assert.AreEqual(expressionList, new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate(expressionList));
      }

      [TestMethod]
      public void If_TEST()
      {
         AssertExpressionEvaluatesTo(new Constant(1), "(if True (pause 1) (pause 2))");
         AssertExpressionEvaluatesTo(new Constant(2), "(if False (pause 1) (pause 2))");
      }

      [TestMethod]
      public void Lambda_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var argumentSymbols = new SymbolList();
         var lambda = new Lambda { ArgumentSymbols = argumentSymbols, Body = new Symbol("y")};
         AssertExpressionEvaluatesTo(new Procedure { DefiningContext = context, ArgumentSymbols = argumentSymbols, Lambda = lambda }, "(lambda () y)", context);
         argumentSymbols.Symbols.Add(new Symbol("x"));
         argumentSymbols.Symbols.Add(new Symbol("y"));
         AssertExpressionEvaluatesTo(new Procedure { DefiningContext = context, ArgumentSymbols = argumentSymbols, Lambda = lambda }, "(lambda (x y) y)", context);
      }


      [TestMethod]
      public void SymbolList_TEST()
      {
         var symbolList = new SymbolList();
         Assert.AreEqual(symbolList, new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate(symbolList));
      }

      [TestMethod]
      public void Loop_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var counter = 10;
         context.DefineValue(
            new Symbol("loopTestCondition"),
            new IntrinsicProcedure { Function = ContextBase => new Constant(counter-- > 0), ArgumentSymbols = new SymbolList(), DefiningContext = context });
         var result = 0;
         context.DefineValue(
            new Symbol("loopTestBody"),
            new IntrinsicProcedure { Function = ContextBase => new Constant(++result), ArgumentSymbols = new SymbolList(), DefiningContext = context });

         AssertExpressionEvaluatesTo(new Constant(10), "(loop (loopTestCondition) (loopTestBody))", context);
         Assert.AreEqual(-1, counter);
         Assert.AreEqual(10, result);
      }

      [TestMethod]
      public void ProcedureCall_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         AssertExpressionEvaluatesTo(new Constant(10), "(define var1 (pause 10))", context);
         Assert.AreEqual(new Constant(10), context.GetValue(new Symbol("var1")));
         AssertExpressionEvaluatesTo(new Constant(10), "(define var2 ((lambda (duration) (pause duration)) 10))", context);
         Assert.AreEqual(new Constant(10), context.GetValue(new Symbol("var2")));
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
      public void Symbol_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var quotedExpression = ParseExpression("xyz");
         Assert.IsTrue(quotedExpression is Symbol);
         AssertExpressionEvaluatesTo(quotedExpression, "(define var (quote xyz))", context);
         Assert.AreEqual(quotedExpression, context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void ExceptionHandling_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         context.DefineValue(
            new Symbol("SomeFun"), 
            new IntrinsicProcedure { Function = SomeFun, ArgumentSymbols = new SymbolList(), DefiningContext = context });
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
      }

      [ExcludeFromCodeCoverage]
      private ExpressionBase SomeFun(ContextBase Context)
      {
         throw new NotImplementedException("SomeFun");
      }

      private void AssertExpressionEvaluatesTo(ExpressionBase ExpectedValue, string Expression, ContextBase Context = null)
      {
         if(Context == null)
            Context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(Context);
         var value = evaluator.Evaluate(ParseExpression(Expression));
         Assert.AreEqual(ExpectedValue, value);
      }

      private ExpressionBase ParseExpression(string Expression)
      {
         return (ExpressionBase)new MacroParser().Parse(Expression);
      }
   }
}
