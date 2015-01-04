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
                  PositionX = new Constant(4711), 
                  PositionY = new Constant(-4711),
                  ImageUrl = new Constant("blubb")
               };
         using (var windowshotVM = new WindowshotVM(windowshot))
         {
            Assert.AreEqual(windowshot.PositionX, windowshotVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, windowshotVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, windowshotVM.ImageUrlVM.Model);
            windowshot.PositionX = new Constant(1147);
            windowshot.PositionY = new Constant(-1147);
            windowshot.ImageUrl = new Constant("blah");
            Assert.AreEqual(windowshot.PositionX, windowshotVM.PositionXVM.Model);
            Assert.AreEqual(windowshot.PositionY, windowshotVM.PositionYVM.Model);
            Assert.AreEqual(windowshot.ImageUrl, windowshotVM.ImageUrlVM.Model);
         }
      }
   }
}
