using System;
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
         context.DefineValue(symbol, this);
         Assert.AreSame(this, context.GetValue(symbol));
         
         try
         {
            context.DefineValue(symbol, this);
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
         context.SetValue(symbol, this);
         Assert.AreSame(this, context.GetValue(symbol));

         try
         {
            context.SetValue(new Symbol("undefinedVar"), this);
            Assert.Fail();
         }
         catch (RuntimeException)
         {
            // everything ok
         }
      }

      [TestMethod]
      public void GetValue_TEST()
      {
         var context = new MockContext();
         Assert.AreSame(MockContext.DEFAULT_VALUE, context.GetValue(new Symbol("undefinedVar")));
      }
   }
}
