using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroLanguage;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class Procedure_TEST
   {
      [TestMethod]
      public void Call_TEST()
      {
         var context = new MockContext();

         new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(define var1 ((lambda () (quote someSymbol))))"));
         Assert.AreEqual(new Symbol("someSymbol"), context.GetValue(new Symbol("var1")));

         new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2 3))"));
         Assert.AreEqual(new Constant(3), context.GetValue(new Symbol("var2")));
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void ArgumentCount_TEST()
      {
         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(define var2 ((lambda () 4711) 1))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 0 argument(s) but got 1", e.Message);
         }

         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2 3 4))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 3 argument(s) but got 4", e.Message);
         }

         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 3 argument(s) but got 2", e.Message);
         }
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var context = new MockContext();

         var procedure1 = new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(lambda (x y z) z)"));
         var procedure2 = new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(lambda (x y z) z)"));
         Assert.AreEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(lambda (x z) z)"));
         Assert.AreNotEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(new MockContext()).Evaluate((ExpressionBase)new MacroParser().Parse("(lambda (x y z) z)"));
         Assert.AreNotEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(context).Evaluate((ExpressionBase)new MacroParser().Parse("(lambda (x y z) x)"));
         Assert.AreNotEqual(procedure1, procedure2);
      }
   }
}
