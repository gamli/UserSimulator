using System;
using System.Collections.Generic;
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

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(eval '(+ 4700 11))"));

         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(= '(a b c) '(a b c))"));
         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(= '(a b c) '(a b c d))"));

         Assert.AreEqual(new Constant(false), EvaluateExpression(evaluator, "(not true)"));
         Assert.AreEqual(new Constant(true), EvaluateExpression(evaluator, "(not false)"));

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

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(+ 4700 11)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(- 4712 1)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(* 47.11 100)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(/ 9422 2)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(% 14711 10000)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(abs 4711)"));
         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(abs -4711)"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(car '(4711 a b c))"));
         try
         {
            EvaluateExpression(evaluator, "(car nil)");
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
         Assert.AreEqual(EvaluateExpression(evaluator, "(first '(4711 a b c))"), EvaluateExpression(evaluator, "(car '(4711 a b c))"));

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
         Assert.AreEqual(EvaluateExpression(evaluator, "(rest '(4711 a b c))"), EvaluateExpression(evaluator, "(cdr '(4711 a b c))"));

         Assert.AreEqual(new Symbol("e"), EvaluateExpression(evaluator, "(last '(a b c d e))"));

         Assert.AreEqual(ParseExpression("(a b c d e)"), EvaluateExpression(evaluator, "(append '(a b) '(c d e))"));
         
         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(move 0 0)")));
         
         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(position 0 0)")));
         
         var sw = new Stopwatch();
         sw.Start();
         Assert.AreEqual(Constant.Number(100), evaluator.Evaluate(ParseExpression("(pause 100)")));
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

         Assert.AreEqual(new List(Constant.Number(8), Constant.Number(16)), EvaluateExpression(evaluator, "(map '(2 4) (lambda (num) (* 4 num)))"));

         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(max -4711 4711)"));
         Assert.AreEqual(Constant.Number(4711), EvaluateExpression(evaluator, "(max (list -4711 4711))"));

         try
         {
            evaluator.Evaluate(ParseExpression("(max nil)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         try
         {
            evaluator.Evaluate(ParseExpression("(max)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(Constant.Number(-4711), EvaluateExpression(evaluator, "(min -4711 4711)"));
         Assert.AreEqual(Constant.Number(-4711), EvaluateExpression(evaluator, "(min (list -4711 4711))"));

         try
         {
            evaluator.Evaluate(ParseExpression("(min nil)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         try
         {
            evaluator.Evaluate(ParseExpression("(min)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(Constant.Number(1), EvaluateExpression(evaluator, "(arg-max '(1 2 3 4 5) (lambda (num) (* -1 num)))"));
         Assert.AreEqual(Constant.Number(1), EvaluateExpression(evaluator, "(arg-max '(1 2 1 1 3 4 5) (lambda (num) (* -1 num)))"));

         try
         {
            Assert.AreEqual(Constant.Number(1), EvaluateExpression(evaluator, "(arg-max nil (lambda (num) (* -1 num)))"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }

         Assert.AreEqual(new List(Constant.Number(1)), EvaluateExpression(evaluator, "(args-max '(1 2 3 4 5) (lambda (num) (* -1 num)))"));
         Assert.AreEqual(
            new List(Constant.Number(2), Constant.Number(2), Constant.Number(4)), 
            EvaluateExpression(evaluator, "(args-max '(1 2 3 2 4) (lambda (num) (if (= (% num 2) 0) 1 -1)))"));

         try
         {
            Assert.AreEqual(new List(), EvaluateExpression(evaluator, "(args-max nil (lambda (num) (* -1 num)))"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
         
         Assert.AreEqual(new List(Constant.Number(4711)), EvaluateExpression(evaluator, "(filter '(-4711 4711) (lambda (num) (> num 0)))"));
         Assert.AreEqual(new List(), EvaluateExpression(evaluator, "(filter '(-4711 -4812) (lambda (num) (> num 0)))"));
         Assert.AreEqual(new List(), EvaluateExpression(evaluator, "(filter nil (lambda (num) (> num 0)))"));
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

         Assert.AreEqual(Constant.Number(30), evaluator.Evaluate((Expression)new MacroParser().Parse("(* 3 \"10\")")));
      }
   }
}
