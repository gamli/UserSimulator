using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class PositionVM_TEST
   {
      [TestMethod]
      public void PositionVM_Property_TEST()
      {
         var position = new Position { X = ConstantExpressions.Create(4711), Y = ConstantExpressions.Create(-4711) };
         using (var positionVM = new PositionVM(position))
         {
            Assert.AreEqual(position.X, positionVM.XVM.Model);
            Assert.AreEqual(position.Y, positionVM.YVM.Model);
            position.X = ConstantExpressions.Create(1147);
            position.Y = ConstantExpressions.Create(-1147);
            Assert.AreEqual(position.X, positionVM.XVM.Model);
            Assert.AreEqual(position.Y, positionVM.YVM.Model);
         }
      }
   }
}
