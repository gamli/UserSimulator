using System;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Definition_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var definition = new Definition { Symbol = new Symbol("varName"), Expression = new Constant(true) };
         var clone = MacroCloner.Clone(definition);
         Assert.AreEqual(definition, clone);
         
         definition.Expression = new Constant(false);
         Assert.AreNotEqual(definition, clone);
         definition.Expression = new Constant(true);
         Assert.AreEqual(definition, clone);
         

         definition.Symbol.Value = "otherVarName";
         Assert.AreNotEqual(definition, clone);
         definition.Symbol.Value = "varName";
         Assert.AreEqual(definition, clone);
      }
   }
}
