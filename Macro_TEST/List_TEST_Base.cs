using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   public class List_TEST_Base
   {
      protected void AssertListsAreEqual(List List1, List List2)
      {
         Assert.IsTrue(ListEqual(List1, List2));
      }
      protected void AssertListsAreNotEqual(List List1, List List2)
      {
         Assert.IsFalse(ListEqual(List1, List2));
      }

      private bool ListEqual(List List1, List List2)
      {
         var clone1 = MacroCloner.Clone(List1);
         var clone2 = MacroCloner.Clone(List2);
         clone2.Expressions.Add(null);

         return Equals(List1, List2) && !Equals(clone1, clone2) && List1.Expressions.SequenceEqual(List2.Expressions);
      }
   }
}
