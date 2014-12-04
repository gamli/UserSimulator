using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class ProgramVM_TEST
   {
      [TestMethod]
      public void Block_Property_TEST()
      {
         var program = new Program { Block = new Block() };
         using(var programVM = new ProgramVM(program))
         {
            Assert.AreSame(program.Block, programVM.BlockVM.Model);
         }
      }
   }
}
