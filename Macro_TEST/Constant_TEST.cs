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
         var expression = ConstantExpressions.Create(true);
         Assert.AreEqual(expression, ConstantExpressions.Create(true));
         Assert.AreNotEqual(expression, ConstantExpressions.Create(false));
      }

      [TestMethod]
      public void StringExpression_TEST()
      {
         var expression = ConstantExpressions.Create("foo");
         Assert.AreEqual(expression, ConstantExpressions.Create("foo"));
         Assert.AreNotEqual(expression, ConstantExpressions.Create("bar"));
         expression.Value = null;
         Assert.AreEqual(expression, ConstantExpressions.Create<string>(null));
         Assert.AreNotEqual(expression, ConstantExpressions.Create(""));
      }

      [TestMethod]
      public void IntegerExpression_TEST()
      {
         var expression = ConstantExpressions.Create(4711);
         Assert.AreEqual(expression, ConstantExpressions.Create(4711));
         Assert.AreNotEqual(expression, ConstantExpressions.Create(-4711));
      }
   }
}
