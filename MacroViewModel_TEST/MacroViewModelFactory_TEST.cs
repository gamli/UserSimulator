using System;
using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class MacroViewModelFactory_TEST
   {
      [TestMethod]
      public void Create_TEST()
      {
         AssertCreatesExpectedViewModelType(typeof(Constant), typeof(ConstantVM));
         AssertCreatesExpectedViewModelType(typeof(Definition), typeof(DefinitionVM));
         AssertCreatesExpectedViewModelType(typeof(FunctionCall), typeof(FunctionCallVM));
         AssertCreatesExpectedViewModelType(typeof(If), typeof(IfVM));
         AssertCreatesExpectedViewModelType(typeof(List), typeof(ListVM));
         AssertCreatesExpectedViewModelType(typeof(Loop), typeof(LoopVM));
         AssertCreatesExpectedViewModelType(typeof(Quote), typeof(QuoteVM));
         AssertCreatesExpectedViewModelType(typeof(Symbol), typeof(SymbolVM));
         Assert.AreEqual(MacroViewModelFactory.Instance.Create(null), null);
      }

      private void AssertCreatesExpectedViewModelType(Type ModelType, Type ExpectedViewModelType)
      {
         var macroType = Type.GetType(ModelType.AssemblyQualifiedName);
         var macro = (MacroBase)macroType.GetConstructor(Type.EmptyTypes).Invoke(null);
         AssertCreatesExpectedViewModelType(macro, ExpectedViewModelType);
      }

      private void AssertCreatesExpectedViewModelType(MacroBase Macro, Type ExpectedMacroViewModelType)
      {
         using (var macroViewModel = MacroViewModelFactory.Instance.Create(Macro))
            Assert.AreEqual(macroViewModel.GetType(), ExpectedMacroViewModelType);
      }
   }
}
