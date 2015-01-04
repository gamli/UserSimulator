using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Constant_TEST
   {
      [TestMethod]
      public void BooleanExpression_TEST()
      {
         var expression = new Constant(true);
         Assert.AreEqual(expression, new Constant(true));
         Assert.AreNotEqual(expression, new Constant(false));
      }

      [TestMethod]
      public void StringExpression_TEST()
      {
         var expression = new Constant("foo");
         Assert.AreEqual(expression, new Constant("foo"));
         Assert.AreNotEqual(expression, new Constant("bar"));
         expression.Value = null;
         Assert.AreEqual(expression, new Constant(null));
         Assert.AreNotEqual(expression, new Constant(""));
      }

      [TestMethod]
      public void IntegerExpression_TEST()
      {
         var expression = new Constant(4711);
         Assert.AreEqual(expression, new Constant(4711));
         Assert.AreNotEqual(expression, new Constant(-4711));
      }

      [TestMethod]
      public void DoubleExpression_TEST()
      {
         var expression = new Constant(4711.0);
         Assert.AreEqual(expression, new Constant(4711.0));
         Assert.AreNotEqual(expression, new Constant(-4711.0));
      }
   }
}
