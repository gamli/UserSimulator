using System;
using System.Drawing;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Windowshot_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var windowshot = new Windowshot();
         var testVisitor = new MockVisitor();
         windowshot.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Windowshot));
      }

      [TestMethod]
      public void Equals_TEST()
      {
         var block = new Block();
         var windowshot = 
            new Windowshot 
            { 
               Body = new NoOp(),  
               ImageUrl = ConstantExpressions.Create("nonExistingTestImage"),
               PositionX = ConstantExpressions.Create(13),
               PositionY = ConstantExpressions.Create(17)
            };
         block.Items.Add(windowshot);
         var program = new Program { Body = block };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         windowshot.PositionY = ConstantExpressions.Create(-17);
         Assert.AreNotEqual(program, programClone);
         windowshot.PositionX = ConstantExpressions.Create(-13);
         Assert.AreNotEqual(program, programClone);
         windowshot.ImageUrl = ConstantExpressions.Create("");
         Assert.AreNotEqual(program, programClone);
      }
   }
}
