using System;
using System.Linq;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class BlockVM_TEST
   {
      [TestMethod]
      public void Items_Property_TEST()
      {
         var block = new Block();
         foreach(var childBlock in Enumerable.Repeat(0, 8).Select(DummyParameter => new Block()))
            block.Items.Add(childBlock);
         using(var blockVM = new BlockVM(block))
         {
            Assert.AreEqual(block.Items.Count, blockVM.ItemsVM.Count);
            for (var i = 0; i < block.Items.Count; i++)
               Assert.AreSame(block.Items[i], blockVM.ItemsVM[i].Model);
         }
      }
   }
}
