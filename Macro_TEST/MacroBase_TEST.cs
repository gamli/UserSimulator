using System;
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
         var mockMacro = new MockMacroWithMacroProperty();
         var macroChangedFired = false;
         mockMacro.MacroChanged += (Sender, Args) => macroChangedFired = true;
         Assert.IsFalse(macroChangedFired);
         var position = new Position { X = ConstantExpressions.Create(4711), Y = ConstantExpressions.Create(-4711) };
         mockMacro.Body = position;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         ((ConstantExpression<int>)position.X).Value = 0;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         position.X = ConstantExpressions.Create(4711);
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         mockMacro.Body = null;
         Assert.IsTrue(macroChangedFired);
         macroChangedFired = false;
         position.X = ConstantExpressions.Create(0);
         Assert.IsFalse(macroChangedFired);
      }

      private class MockMacroWithMacroProperty : MacroWithBodyBase
      {
         // nothing to add - base class's  Body is enough

         public override void Accept(IVisitor Visitor)
         {
            throw new NotImplementedException();
         }

         protected override bool MacroEquals(MacroBase OtherMacro)
         {
            throw new NotImplementedException();
         }
      }
   }
}
