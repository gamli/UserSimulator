using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class WindowshotVM_TEST
   {
      [TestMethod]
      public void PositionVM_Property_TEST()
      {
         var windowshot = 
            new WindowshotExpression 
               { 
                  PositionX = ConstantExpressions.Create(4711), 
                  PositionY = ConstantExpressions.Create(-4711),
                  ImageUrl = ConstantExpressions.Create("blubb")
               };
         using (var moveVM = new WindowshotExpressionVM(windowshot))
         {
            Assert.AreEqual(windowshot.PositionX, moveVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, moveVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, moveVM.ImageUrlVM.Model);
            windowshot.PositionX = ConstantExpressions.Create(1147);
            windowshot.PositionY = ConstantExpressions.Create(-1147);
            windowshot.ImageUrl = ConstantExpressions.Create("blah");
            Assert.AreEqual(windowshot.PositionX, moveVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, moveVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, moveVM.ImageUrlVM.Model);
         }
      }
   }
}
