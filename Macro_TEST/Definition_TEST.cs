using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Definition_TEST : List_TEST_Base
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var definition = new Definition { Symbol = new Symbol("varName"), Expression = new Constant(true) };
         var clone = MacroCloner.Clone(definition);
         Assert.AreEqual(definition, clone);

         definition.Expressions.Add(new Constant("not allowed"));
         AssertListsAreNotEqual(definition, clone);
         definition.Expressions.RemoveAt(definition.Expressions.Count - 1);
         AssertListsAreEqual(definition, clone);

         definition.Expression = new Constant(false);
         AssertListsAreNotEqual(definition, clone);
         definition.Expression = new Constant(true);
         AssertListsAreEqual(definition, clone);

         definition.Symbol.Value = "otherVarName";
         AssertListsAreNotEqual(definition, clone);
         definition.Symbol.Value = "varName";
         AssertListsAreEqual(definition, clone);
      }
   }
}
