using System.Linq;
using System.Linq.Expressions;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class ListVM_TEST
   {
      [TestMethod]
      public void Expressions_Property_TEST()
      {
         var list = new List();
         foreach(var childExpression in Enumerable.Repeat(0, 8).Select(ConstantValue => new Symbol(ConstantValue.ToString())))
            list.Expressions.Add(childExpression);
         list.Expressions.RemoveAt(3);
         list.Expressions.RemoveAt(0);
         list.Expressions.Insert(5, new List());
         using(var listVM = new ListVM(list))
         {
            Assert.AreEqual(list.Expressions.Count, listVM.ExpressionsVM.Count);
            for (var i = 0; i < list.Expressions.Count; i++)
               Assert.AreSame(list.Expressions[i], listVM.ExpressionsVM[i].Model);
         }
      }
   }
}
