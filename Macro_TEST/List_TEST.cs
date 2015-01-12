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
   class List_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var list = new List(new Symbol("moo"), new Constant(true), new List());
         var clone = MacroCloner.Clone(list);
         Assert.AreEqual(list, clone);

         list.Expressions.Add(new Constant("foo"));
         Assert.AreNotEqual(list, clone);

         list.Expressions.Remove(new Constant("foo"));
         Assert.AreEqual(list, clone);
      }

      [TestMethod]
      public void MacroChangedEvent_TEST()
      {
         var list = new List(new Symbol("moo"), new Constant(true), new List());
         var test = false;
         list.MacroChanged += (Sender, Args) => test = true;

         list.Expressions.Add(new Constant("foo"));
         Assert.IsTrue(test);
         test = false;

         list.Expressions.Remove(new Constant("foo"));
         Assert.IsTrue(test);
         test = false;

         ((List)list.Expressions.Last()).Expressions.Add(new Constant("elementInNestedList"));
         Assert.IsTrue(test);
         test = false;
      }
   }
}
