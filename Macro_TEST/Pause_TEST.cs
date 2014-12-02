﻿using System;
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
         var testVisitor = new TestVisitor();
         pause.Accept(testVisitor);
         Assert.AreEqual(testVisitor.Macros.Count, 1);
         Assert.AreEqual(testVisitor.Macros[0].Macro.GetType(), typeof(Pause));
      }
   }
}
