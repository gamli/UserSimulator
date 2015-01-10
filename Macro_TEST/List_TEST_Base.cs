using System.Linq;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   public class List_TEST_Base<TExpression>
      where TExpression : ExpressionBase
   {
      protected void AssertListsAreEqual(ListExpressionBase<TExpression> List1, ListExpressionBase<TExpression> List2)
      {
         Assert.IsTrue(ListEqual(List1, List2));
      }
      protected void AssertListsAreNotEqual(ListExpressionBase<TExpression> List1, ListExpressionBase<TExpression> List2)
      {
         Assert.IsFalse(ListEqual(List1, List2));
      }

      private bool ListEqual(ListExpressionBase<TExpression> List1, ListExpressionBase<TExpression> List2)
      {
         var clone1 = MacroCloner.Clone(List1);
         var clone2 = MacroCloner.Clone(List2);
         clone2.Expressions.Add(null);

         return Equals(List1, List2) && !Equals(clone1, clone2) && List1.Expressions.SequenceEqual(List2.Expressions);
      }
   }
}
