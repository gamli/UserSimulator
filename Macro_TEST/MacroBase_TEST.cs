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
         var mockStatement = new MockStatementWithMacroProperty();
         var macroChangedFired = false;
         mockStatement.MacroChanged += (Sender, Args) => macroChangedFired = true;
         Assert.IsFalse(macroChangedFired);
         var position = new Position { X = new Constant(4711), Y = new Constant(-4711) };
         var block = new Block();
         block.Items.Add(position);
         mockStatement.Body = block;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         ((Constant)position.X).Value = 0;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         position.X = new Constant(4711);
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         mockStatement.Body = null;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         position.X = new Constant(0);
         Assert.IsFalse(macroChangedFired);
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var mockStatement1 = new MockStatementWithMacroProperty { SomeProperty = true, Body = new Block() };
         Assert.IsFalse(mockStatement1.Equals(null));
         Assert.AreNotEqual(mockStatement1, 4711);
         var mockStatement2 = new MockStatementWithMacroProperty { SomeProperty = false, Body = new Block() };
         Assert.AreNotEqual(mockStatement1, mockStatement2);
         mockStatement1.SomeProperty = false;
         Assert.AreEqual(mockStatement1, mockStatement2);
      }

      private class MockStatementWithMacroProperty : StatementWithBodyBase
      {
         private bool _someProperty;
         [ExcludeFromCodeCoverage]
         public bool SomeProperty { get { return _someProperty; } set { SetPropertyValue(ref _someProperty, value); } }

         [ExcludeFromCodeCoverage]
         public override void Accept(IVisitor Visitor)
         {
            throw new NotImplementedException();
         }

         protected override bool MacroEquals(MacroBase OtherMacro)
         {
            return SomeProperty == ((MockStatementWithMacroProperty)OtherMacro).SomeProperty && base.BodyEquals((MockStatementWithMacroProperty)OtherMacro);
         }
      }
   }
}
