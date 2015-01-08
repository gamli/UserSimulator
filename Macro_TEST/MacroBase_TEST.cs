using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class MacroBase_TEST
   {
      [TestMethod]
      public void Test_Event_MacroChanged()
      {
         var mock = new MockExpressionWithSomeProperty();
         var list = new List();
         list.Expressions.Add(mock);

         var macroChangedFired = false;
         mock.MacroChanged += (Sender, Args) => macroChangedFired = true;
         Assert.IsFalse(macroChangedFired);

         mock.SomeProperty = true;
         Assert.IsTrue(macroChangedFired);
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var mockStatement1 = new MockExpressionWithSomeProperty { SomeProperty = true };
         Assert.IsFalse(mockStatement1.Equals(null));
         Assert.AreNotEqual(mockStatement1, 4711);

         var mockStatement2 = new MockExpressionWithSomeProperty { SomeProperty = false };
         Assert.AreNotEqual(mockStatement1, mockStatement2);

         mockStatement1.SomeProperty = false;
         Assert.AreEqual(mockStatement1, mockStatement2);
      }

      private class MockExpressionWithSomeProperty : ExpressionBase
      {
         private bool _someProperty;
         [ExcludeFromCodeCoverage]
         public bool SomeProperty { private get { return _someProperty; } set { SetPropertyValue(ref _someProperty, value); } }

         [ExcludeFromCodeCoverage]
         public override void Accept(IVisitor Visitor)
         {
            throw new NotImplementedException();
         }

         protected override bool MacroEquals(MacroBase OtherMacro)
         {
            return Equals(SomeProperty, ((MockExpressionWithSomeProperty)OtherMacro).SomeProperty);
         }
      }

      [TestMethod]
      public void ToString_TEST()
      {
         var list = new List();
         list.Expressions.Add(new Symbol("aSymbol"));
         list.Expressions.Add(new Constant("a string"));
         list.Expressions.Add(new Constant(4711));
         list.Expressions.Add(new Constant(-4711.1174));
         list.Expressions.Add(new Constant(true));
         var functionCall = new FunctionCall { Function = new Symbol("aFunctionCall") };
         functionCall.Expressions.Add(new Quote { Expression = new Symbol("aQuotedSymbol") });
         list.Expressions.Add(functionCall);
         Assert.AreEqual("(aSymbol \"a string\" 4711 -4711.1174 True (aFunctionCall (quote aQuotedSymbol)))", list.ToString());
      }
   }
}
