using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroLanguage;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   [ExcludeFromCodeCoverage]
   public class RuntimeContext_TEST
   {
      [TestMethod]
      public void SymbolNotFoundGetValue_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);         

         try
         {
            context.GetValue(new Symbol("undefinedVar"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
      }

      [TestMethod]
      public void IntrinsicProcedures_TEST() 
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(context);

         var intrinsicProcedure = ((IntrinsicProcedure)EvaluateExpression(evaluator, "pause"));
         var proc1 = 
            new IntrinsicProcedure
               {
                  Function = intrinsicProcedure.Function,
                  FormalArguments = MacroCloner.Clone(intrinsicProcedure.FormalArguments),
                  DefiningContext = context
               };
         var proc2 =
            new IntrinsicProcedure
            {
               Function = intrinsicProcedure.Function,
               FormalArguments = MacroCloner.Clone(intrinsicProcedure.FormalArguments),
               DefiningContext = context
            };
         Assert.AreEqual(proc1, proc2);

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(eval '(+ 4700 11))"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(= '(a b c) '(a b c))"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(= '(a b c) '(a b c d))"));

         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(! true)"));
         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(! false)"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(constant? 4711)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(constant? '(a b c))"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(list? '(a b c))"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(list? (lambda () nil))"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(symbol? 'i-am-a-symbol)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(symbol? \"i-am-not-a-symbol\")"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(< 1 2)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(< 2 1)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(< 1 1)"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(> 2 1)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(> 1 2)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(> 1 1)"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(or false true)"));
         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(or true false)"));
         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(or true true)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(or false false)"));

         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(and false true)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(and true false)"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(and false false)"));
         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(and true true)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(+ 4700 11)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(- 4712 1)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(* 47.11 100)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(/ 9422 2)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(% 14711 10000)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(abs 4711)"));
         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(abs -4711)"));

         Assert.AreEqual(new Constant(4711m), EvaluateExpression(evaluator, "(car '(4711 a b c))"));
         try
         {
            EvaluateExpression(evaluator, "(car nil)");
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(ParseExpression("(a b c)"), EvaluateExpression(evaluator, "(cdr '(4711 a b c))"));
         try
         {
            EvaluateExpression(evaluator, "(cdr nil)");
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(ParseExpression("(a b c d e)"), EvaluateExpression(evaluator, "(append '(a b) '(c d e))"));
         
         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(move 0 0)")));
         
         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(position 0 0)")));
         
         var sw = new Stopwatch();
         sw.Start();
         Assert.AreEqual(new Constant(100m), evaluator.Evaluate(ParseExpression("(pause 100)")));
         sw.Stop();
         Assert.IsTrue(sw.ElapsedMilliseconds >= 99); // the sleep function is not that exact

         try
         {
            evaluator.Evaluate(ParseExpression("(pause)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
      }

      private static Expression EvaluateExpression(ExpressionEvaluator Evaluator, string Expression)
      {
         return Evaluator.Evaluate(ParseExpression(Expression));
      }

      private static Expression ParseExpression(string Expression)
      {
         return (Expression)new MacroParser().Parse(Expression);
      }
      
      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void InvalidCastException_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(context);

         evaluator.Evaluate((Expression)new MacroParser().Parse("(pause 10)"));

         try
         {
            evaluator.Evaluate((Expression) new MacroParser().Parse("(pause (quote (fun)))"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(new Constant(30m), evaluator.Evaluate((Expression)new MacroParser().Parse("(* 3 \"10\")")));
      }
   }
}
