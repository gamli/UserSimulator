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
   public class Procedure_TEST
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
      [ExcludeFromCodeCoverage]
      public void ArgumentCount_TEST()
      {
         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(define var2 ((lambda () 4711) 1))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 0 argument(s) but got 1", e.InnerException.InnerException.Message);
         }

         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2 3 4))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 3 argument(s) but got 4", e.InnerException.InnerException.Message);
         }

         try
         {
            var context = new MockContext();
            new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(define var2 ((lambda (x y z) z) 1 2))"));
            Assert.Fail();
         }
         catch (RuntimeException e)
         {
            Assert.AreEqual("Expected 3 argument(s) but got 2", e.InnerException.InnerException.Message);
         }
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var context = new MockContext();

         var procedure1 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) z)"));
         var procedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) z)"));
         Assert.AreEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x z) z)"));
         Assert.AreNotEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(new MockContext()).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) z)"));
         Assert.AreNotEqual(procedure1, procedure2);

         procedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) x)"));
         Assert.AreNotEqual(procedure1, procedure2);
      }

      [TestMethod]
      public void MacroGetHashCode_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);

         var procedure1 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) z)"));
         var procedure2 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x y z) z)"));
         var procedure3 = new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("(lambda (x z) z)"));

         var set = new HashSet<Expression> { procedure1 };
         Assert.IsTrue(set.Contains(procedure1));
         Assert.IsTrue(set.Contains(procedure2));
         Assert.IsFalse(set.Contains(procedure3));

         set.Add(procedure2);
         Assert.AreEqual(1, set.Count);
         Assert.IsTrue(set.Contains(procedure1));
         Assert.IsTrue(set.Contains(procedure2));
         Assert.IsFalse(set.Contains(procedure3));

         set.Add(procedure3);
         Assert.IsTrue(set.Contains(procedure1));
         Assert.IsTrue(set.Contains(procedure2));
         Assert.IsTrue(set.Contains(procedure3));
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void VarArg_TEST()
      {
         var context = new RuntimeContext(IntPtr.Zero);

         var xy = (List)new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("((lambda (.) .) 'x 'y)"));
         Assert.AreEqual(2, xy.Expressions.Count);
         Assert.AreEqual(new Symbol("x"), xy.Expressions[0]);
         Assert.AreEqual(new Symbol("y"), xy.Expressions[1]);

         var y = (List)new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("((lambda (arg .) .) 'x 'y)"));
         Assert.AreEqual(1, y.Expressions.Count);
         Assert.AreEqual(new Symbol("y"), y.Expressions[0]);

         var empty = (List)new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("((lambda (arg .) .) 'x)"));
         Assert.AreEqual(0, empty.Expressions.Count);

         try
         {
            new ExpressionEvaluator(context).Evaluate((Expression)new MacroParser().Parse("((lambda (arg1 arg2 .) .) 'x)"));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok - not enough arguments
         }
      }
   }
}
