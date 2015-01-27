using System.Collections.Generic;
using System.Linq;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class List_TEST
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
      public void MacroGetHashCode_TEST()
      {
         var list1 = new List(new Constant("moo"));
         var list2 = new List(new Constant("moo"));
         var list3 = new List(new Constant("foo"));
         var list4 = new List(new Constant("moo"), new Constant("foo"));
         var list5 = new List();

         var set = new HashSet<List> { list1 };
         Assert.IsTrue(set.Contains(list1));
         Assert.IsTrue(set.Contains(list2));
         Assert.IsFalse(set.Contains(list3));
         Assert.IsFalse(set.Contains(list4));
         Assert.IsFalse(set.Contains(list5));

         set.Add(list2);
         Assert.AreEqual(set.Count, 1);

         set.Add(list3);
         Assert.IsTrue(set.Contains(list1));
         Assert.IsTrue(set.Contains(list2));
         Assert.IsTrue(set.Contains(list3));
         Assert.IsFalse(set.Contains(list4));
         Assert.IsFalse(set.Contains(list5));

         set.Add(list4);
         Assert.IsTrue(set.Contains(list1));
         Assert.IsTrue(set.Contains(list2));
         Assert.IsTrue(set.Contains(list3));
         Assert.IsTrue(set.Contains(list4));
         Assert.IsFalse(set.Contains(list5));

         set.Add(list5);
         Assert.IsTrue(set.Contains(list1));
         Assert.IsTrue(set.Contains(list2));
         Assert.IsTrue(set.Contains(list3));
         Assert.IsTrue(set.Contains(list4));
         Assert.IsTrue(set.Contains(list5));
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
         Assert.IsFalse(test); // changed events are NOT bubbled up
         test = false;
      }
   }
}
