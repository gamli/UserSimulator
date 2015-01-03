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
            new Windowshot 
               { 
                  PositionX = ConstantExpressions.Create(4711), 
                  PositionY = ConstantExpressions.Create(-4711),
                  ImageUrl = ConstantExpressions.Create("blubb")
               };
         using (var windowshotVM = new WindowshotVM(windowshot))
         {
            Assert.AreEqual(windowshot.PositionX, windowshotVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, windowshotVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, windowshotVM.ImageUrlVM.Model);
            windowshot.PositionX = ConstantExpressions.Create(1147);
            windowshot.PositionY = ConstantExpressions.Create(-1147);
            windowshot.ImageUrl = ConstantExpressions.Create("blah");
            Assert.AreEqual(windowshot.PositionX, windowshotVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, windowshotVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, windowshotVM.ImageUrlVM.Model);
         }
      }
   }
}
