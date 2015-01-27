using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroRuntime_TEST
{
   [TestClass]
   public class ContextBase_TEST
   {
      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void DefineValue_TEST()
      {
         var context = new MockContext();
         var symbol = new Symbol("var");
         Assert.IsTrue(context.IsValueDefined(symbol));
         context.DefineValue(symbol, new Constant(this));
         Assert.IsTrue(context.IsValueDefined(symbol));
         Assert.AreEqual(new Constant(this), context.GetValue(symbol));
         
         try
         {
            context.DefineValue(symbol, new Constant(this));
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void SetValue_TEST()
      {
         var context = new MockContext();
         var symbol = new Symbol("var");

         context.DefineValue(symbol, null);
         Assert.AreSame(null, context.GetValue(symbol));

         context.SetValue(symbol, new Constant(this));
         Assert.AreEqual(new Constant(this), context.GetValue(symbol));
         Assert.AreNotEqual(new Constant(this), context.SymbolNotFundSetValueValue);

         context.SetValue(new Symbol("undefinedVar"), new Constant(this));
         Assert.AreEqual(new Constant(this), context.SymbolNotFundSetValueValue);
      }

      [TestMethod]
      public void GetValue_TEST()
      {
         var context = new MockContext();
         Assert.AreSame(MockContext.DefautlValue, context.GetValue(new Symbol("undefinedVar")));
      }
   }
}
