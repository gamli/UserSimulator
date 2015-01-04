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
         var windowshot =
            new Windowshot
               {
                  ImageUrl = new Constant("nonExistingTestImage"),
                  PositionX = new Constant(13),
                  PositionY = new Constant(17)
               };
         var ifStatement =
            new If 
            { 
               Expression = windowshot,
               Body = new Block()
            };
         var program = new Program { Body = ifStatement };
         var programClone = new ProgramCloner(program).Clone();
         Assert.AreEqual(program, programClone);
         windowshot.PositionY = new Constant(-17);
         Assert.AreNotEqual(program, programClone);
         windowshot.PositionX = new Constant(-13);
         Assert.AreNotEqual(program, programClone);
         windowshot.ImageUrl = new Constant("");
         Assert.AreNotEqual(program, programClone);
      }
   }
}
