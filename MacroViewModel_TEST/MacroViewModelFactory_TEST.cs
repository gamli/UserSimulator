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
         AssertCreatesExpectedViewModelType(typeof(Block), typeof(BlockVM));
         AssertCreatesExpectedViewModelType(typeof(LeftClick), typeof(LeftClickVM));
         AssertCreatesExpectedViewModelType(typeof(Loop), typeof(ForLoopVM));
         AssertCreatesExpectedViewModelType(typeof(Windowshot), typeof(WindowshotVM));
         AssertCreatesExpectedViewModelType(typeof(Move), typeof(MoveVM));
         AssertCreatesExpectedViewModelType(typeof(Pause), typeof(PauseVM));
         AssertCreatesExpectedViewModelType(typeof(Position), typeof(PositionVM));
         AssertCreatesExpectedViewModelType(typeof(Constant), typeof(ConstantVM));
         AssertCreatesExpectedViewModelType(typeof(If), typeof(IfVM));
         AssertCreatesExpectedViewModelType(typeof(Definition), typeof(VariableAssignmentVM));
         AssertCreatesExpectedViewModelType(typeof(Program), typeof(ProgramVM));
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
