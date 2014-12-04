using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Pause_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var pause = new Pause();
         var testVisitor = new MockVisitor();
         pause.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Pause));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var pause = new Pause { Duration = TimeSpan.FromMilliseconds(4711) };
         block.Items.Add(pause);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
      }
   }
}
