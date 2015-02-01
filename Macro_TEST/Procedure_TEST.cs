using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Procedure_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var procedure = new FakeProcedure();
         var clone = MacroCloner.Clone(procedure);
         Assert.AreEqual(procedure, clone);
      }
      [ExcludeFromCodeCoverage]
      private class FakeProcedure : ProcedureBase
      {
         protected override int MacroGetHashCode()
         {
            throw new System.NotImplementedException();
         }
      }
   }
}
