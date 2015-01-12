using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroLanguage;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class IntrinsicProcedure_TEST
   {
      [TestMethod]
      public void Call_TEST()
      {
         var context = new MockContext();

         new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(define var1 ((lambda () (quote someSymbol))))"));
         Assert.AreEqual(new Symbol("someSymbol"), context.GetValue(new Symbol("var1")));

         new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2 3))"));
         Assert.AreEqual(new Constant(3), context.GetValue(new Symbol("var2")));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);

         var intrinsicProcedure1 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("move"));
         var intrinsicProcedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("move"));
         Assert.AreEqual(intrinsicProcedure1, intrinsicProcedure2);

         intrinsicProcedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("position"));
         Assert.AreNotEqual(intrinsicProcedure1, intrinsicProcedure2);

         intrinsicProcedure2 = new ExpressionEvaluator(new RuntimeContext(IntPtr.Zero)).Evaluate((Expression)new MacroParser().Parse("pause"));
         Assert.AreNotEqual(intrinsicProcedure1, intrinsicProcedure2);
      }
   }
}
