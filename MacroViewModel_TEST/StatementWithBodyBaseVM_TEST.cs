using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class StatementWithBodyBaseVM_TEST
   {

      [TestMethod]
      public void Body_Property_TEST()
      {
         var mockMacro = new MockMacro();
         using(var mockMacroVM = new MockMacroVM(mockMacro))
         {
            Assert.AreSame(mockMacro.Body, mockMacroVM.BodyVM.Model);
         }
      }

      [ExcludeFromCodeCoverage]
      private class MockMacroVM : StatementWithBodyBaseVM<MockMacro>
      {
         public MockMacroVM(MockMacro Model)
            : base(Model)
         {
            // nothing to do
         }
      }

      [ExcludeFromCodeCoverage]
      private class MockMacro : StatementWithBodyBase
      {
         public MockMacro()
         {
            Body = new Block();
         }

         public override void Accept(IVisitor Visitor)
         {
            throw new NotImplementedException();
         }

         protected override bool MacroEquals(MacroBase OtherMacro)
         {
            return true;
         }
      }
   }
}
