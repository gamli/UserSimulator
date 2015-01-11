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

         var intrinsicProcedure = ((IntrinsicProcedure)evaluator.Evaluate(ParseExpression("pause")));
         var proc1 = 
            new IntrinsicProcedure
               {
                  Function = intrinsicProcedure.Function,
                  ArgumentSymbols = MacroCloner.Clone(intrinsicProcedure.ArgumentSymbols),
                  DefiningContext = context
               };
         var proc2 =
            new IntrinsicProcedure
            {
               Function = intrinsicProcedure.Function,
               ArgumentSymbols = MacroCloner.Clone(intrinsicProcedure.ArgumentSymbols),
               DefiningContext = context
            };
         Assert.AreEqual(proc1, proc2);

         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(move 0 0)")));

         Assert.AreEqual(new Constant(true), evaluator.Evaluate(ParseExpression("(position 0 0)")));

         var sw = new Stopwatch();
         sw.Start();
         Assert.AreEqual(new Constant(100), evaluator.Evaluate(ParseExpression("(pause 100)")));
         sw.Stop();
         Assert.IsTrue(sw.ElapsedMilliseconds >= 100);

         // TODO this can really hurt: Assert.AreEqual(true, evaluator.Evaluate(ParseExpression("(click)")));

         // TODO cannot test this without valid window => refactoring: Assert.AreEqual(false, evaluator.Evaluate(ParseExpression("(windowshot 0 0 \"PATH TO A FILE\")")));
         
         try
         {
            evaluator.Evaluate(ParseExpression("(pause)"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 1 argument(s) but got 0", e.Message);
         }
      }
      private ExpressionBase ParseExpression(string Expression)
      {
         return (ExpressionBase)new MacroParser().Parse(Expression);
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void InvalidCastException_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);
         var evaluator = new ExpressionEvaluator(context);

         evaluator.Evaluate((ExpressionBase)new MacroParser().Parse("(pause 10)"));

         try
         {
            evaluator.Evaluate((ExpressionBase)new MacroParser().Parse("(pause 10.05)"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Symbol >> Duration <<: expected type was >> System.Int32 << but got >> System.Double <<", e.Message);
         }

         try
         {
            evaluator.Evaluate((ExpressionBase) new MacroParser().Parse("(pause (quote (fun)))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Symbol >> Duration <<: expected [Macro.Constant] but got >> (fun) [Macro.ProcedureCall] <<", e.Message);
         }

         try
         {
            evaluator.Evaluate((ExpressionBase)new MacroParser().Parse("(pause \"10\")"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Symbol >> Duration <<: expected type was >> System.Int32 << but got >> System.String <<", e.Message);
         }
      }
   }
}
