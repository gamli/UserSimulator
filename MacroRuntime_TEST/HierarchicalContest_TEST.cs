using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class HierarchicalContest_TEST
   {
      [TestMethod]
      public void SymbolNotFoundGetValue_TEST()
      {
         var mockContext = new MockContext();
         var hierarchicalContext = new HierarchicalContext(mockContext);
         Assert.AreSame(MockContext.DEFAULT_VALUE, hierarchicalContext.GetValue(new Symbol("undefinedVar")));
      }
   }
}
