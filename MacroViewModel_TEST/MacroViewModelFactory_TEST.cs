using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using MacroRuntime;
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
         AssertCreatesExpectedViewModelType(typeof(Symbol), typeof(SymbolVM));
         AssertCreatesExpectedViewModelType(typeof(List), typeof(ListVM));
         AssertCreatesExpectedViewModelType(typeof(Procedure), typeof(ProcedureVM));
         AssertCreatesExpectedViewModelType(typeof(IntrinsicProcedure), typeof(ProcedureVM));
         Assert.AreEqual(MacroViewModelFactory.Instance.Create(null), null);
      }

      [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
      [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
      private static void AssertCreatesExpectedViewModelType(Type ModelType, Type ExpectedViewModelType)
      {
         var macroType = Type.GetType(ModelType.AssemblyQualifiedName);
         var macro = (MacroBase)macroType.GetConstructor(Type.EmptyTypes).Invoke(null);
         AssertCreatesExpectedViewModelType(macro, ExpectedViewModelType);
      }

      private static void AssertCreatesExpectedViewModelType(MacroBase Macro, Type ExpectedMacroViewModelType)
      {
         using (var macroViewModel = MacroViewModelFactory.Instance.Create(Macro))
            Assert.AreEqual(macroViewModel.GetType(), ExpectedMacroViewModelType);
      }
   }
}
