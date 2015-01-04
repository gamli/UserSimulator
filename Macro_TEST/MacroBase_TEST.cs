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

      private class MockStatementWithMacroProperty : StatementWithBodyBase
      {
         // nothing to add - base class's  Body is enough

         [ExcludeFromCodeCoverage]
         public override void Accept(IVisitor Visitor)
         {
            throw new NotImplementedException();
         }

         [ExcludeFromCodeCoverage]
         protected override bool MacroEquals(MacroBase OtherMacro)
         {
            throw new NotImplementedException();
         }
      }
   }
}
