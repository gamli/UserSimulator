using System;
using System.Collections.Generic;
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
         AssertExpressionEvaluatesTo(true, "True");
         AssertExpressionEvaluatesTo(false, "False");

         AssertExpressionEvaluatesTo("test with >> \" <<", "\"test with >> \\\" <<\"");

         AssertExpressionEvaluatesTo(4711, "4711");
         AssertExpressionEvaluatesTo(-4711, "-4711");

         AssertExpressionEvaluatesTo(-4711.1174, "-4711.1174");
         AssertExpressionEvaluatesTo(4711.1174, "4711.1174");
      }

      [TestMethod]
      public void Definition_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         AssertExpressionEvaluatesTo("Hello World", "(define var \"Hello World\")", context);
         Assert.AreEqual("Hello World", context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      public void FunctionCall_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         AssertExpressionEvaluatesTo(10, "(define var (pause 10))", context);
         Assert.AreEqual(10, context.GetValue(new Symbol("var")));
      }

      [TestMethod]
      public void If_TEST()
      {
         AssertExpressionEvaluatesTo(1, "(if True (pause 1) (pause 2))");
         AssertExpressionEvaluatesTo(2, "(if False (pause 1) (pause 2))");
      }

      [TestMethod]
      [ExpectedException(typeof(RuntimeException))]
      [ExcludeFromCodeCoverage]
      public void List_TEST()
      {
         new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate(new List());
      }

      [TestMethod]
      public void Loop_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var counter = 10;
         context.DefineValue(new Symbol("loopTestCondition"), new Func<List<object>, object>(Args => counter-- > 0));
         var result = 0;
         context.DefineValue(new Symbol("loopTestBody"), new Func<List<object>, object>(Args => ++result));

         AssertExpressionEvaluatesTo(10, "(loop (loopTestCondition) (loopTestBody))", context);
         Assert.AreEqual(-1, counter);
         Assert.AreEqual(10, result);
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
         context.DefineValue(new Symbol("SomeFun"), (Func<List<object>, object>)SomeFun);
         var evaluator = new ExpressionEvaluator(context);

         try
         {
            evaluator.Evaluate(ParseExpression("(SomeFun)"));
            Assert.Fail();
         }
         catch (RuntimeException E)
         {
            Assert.AreEqual("SomeFun", E.InnerException.Message);
            Assert.IsTrue(E.InnerException is NotImplementedException);
         }
      }

      [ExcludeFromCodeCoverage]
      private object SomeFun(List<object> Args)
      {
         throw new NotImplementedException("SomeFun");
      }

      private void AssertExpressionEvaluatesTo(object ExpectedValue, string Expression, ContextBase Context = null)
      {
         if(Context == null)
            Context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(Context);
         Assert.AreEqual(ExpectedValue, evaluator.Evaluate(ParseExpression(Expression)));
      }

      private ExpressionBase ParseExpression(string Expression)
      {
         return (ExpressionBase)new MacroParser().Parse(Expression);
      }
   }
}
