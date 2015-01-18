using System;
using System.Collections.Generic;
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
         Assert.AreEqual(new Constant(3m), context.GetValue(new Symbol("var2")));
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

      [TestMethod]
      public void MacroGetHashCode_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);

         var intrinsicProcedure1 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("move"));
         var intrinsicProcedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("move"));
         var intrinsicProcedure3 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("position"));

         var set = new HashSet<Expression> { intrinsicProcedure1 };
         Assert.IsTrue(set.Contains(intrinsicProcedure1));
         Assert.IsTrue(set.Contains(intrinsicProcedure2));
         Assert.IsFalse(set.Contains(intrinsicProcedure3));

         set.Add(intrinsicProcedure2);
         Assert.AreEqual(set.Count, 1);

         set.Add(intrinsicProcedure3);
         Assert.IsTrue(set.Contains(intrinsicProcedure1));
         Assert.IsTrue(set.Contains(intrinsicProcedure2));
         Assert.IsTrue(set.Contains(intrinsicProcedure3));
      }
   }
}
