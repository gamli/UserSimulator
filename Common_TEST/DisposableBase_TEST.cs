using System;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class DisposableBase_TEST
   {
      [TestMethod]
      public void Dispose_TEST()
      {
         var testObject = new TestDisposableBase();
         Assert.AreEqual(testObject.DisposeCallCounter, 0);
         testObject.Dispose();
         Assert.AreEqual(testObject.DisposeCallCounter, 1);
         testObject.Dispose();
         Assert.AreEqual(testObject.DisposeCallCounter, 1);
      }

      private class TestDisposableBase : DisposableBase
      {
         public int DisposeCallCounter { get; private set; }

         protected override void Dispose(bool Disposing)
         {
            if (Disposing)
               DisposeCallCounter++;
            base.Dispose(Disposing);
         }
      }
   }
}
