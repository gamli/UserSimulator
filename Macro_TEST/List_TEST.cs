﻿using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class List_TEST
   {
      [TestMethod]
      public void Constructor_TEST()
      {
         var block = new List();
         Assert.IsNotNull(block.Expressions);
      }

      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var list = new List();
         list.Expressions.Add(new List());
         var clone = MacroCloner.Clone(list);
         Assert.AreEqual(list, clone);
      }

      [TestMethod]
      public void AddAndRemoveItem_TEST()
      {
         var list = new List();
         var constIntValue = 0;
         list.MacroChanged += (Sender, Args) => constIntValue = Sender is Constant && ((Constant)Sender).Value is int ? (int)((Constant)Sender).Value : 0;         
         var constInt = new Constant(-1);

         list.Expressions.Add(constInt);
         constInt.Value = 0;
         Assert.AreEqual(constIntValue, 0);
         
         constInt.Value = -4711;
         Assert.AreEqual(constIntValue, -4711);
         list.Expressions.Remove(constInt);
         
         constInt.Value = 4711;
         Assert.AreEqual(constIntValue, 0);
      }
   }
}
