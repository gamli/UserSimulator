using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class NoOp_TEST
   {
      [TestMethod]
      public void Accept_TEST()
      {
         var noOp = new NoOp();
         var testVisitor = new TestVisitor();
         noOp.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(NoOp));
      }
   }
}
