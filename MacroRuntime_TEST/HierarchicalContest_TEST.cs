using Macro;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class HierarchicalContest_TEST
   {
      [TestMethod]
      public void SymbolNotFoundSetValue_TEST()
      {
         var mockContext = new MockContext();
         var hierarchicalContext = new HierarchicalContext(mockContext);
         var someValue = new Constant(true);
         var undefinedVar = new Symbol("undefinedVar");
         hierarchicalContext.SetValue(undefinedVar, someValue);
         Assert.AreSame(mockContext.SymbolNotFundSetValueValue, someValue);
      }

      [TestMethod]
      public void SymbolNotFoundGetValue_TEST()
      {
         var mockContext = new MockContext();
         var hierarchicalContext = new HierarchicalContext(mockContext);
         Assert.AreSame(MockContext.DefautlValue, hierarchicalContext.GetValue(new Symbol("undefinedVar")));
      }
   }
}
