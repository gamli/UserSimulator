using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class FunctionCallVM_TEST
   {
      [TestMethod]
      public void FunctionVM_Property_TEST()
      {
         var functionCall = new FunctionCall { Function = new Symbol("foo") };
         functionCall.Expressions.Add(new Constant(true));
         functionCall.Expressions.Add(new Constant("arg"));
         using (var functionCallVM = new FunctionCallVM(functionCall))
         {
            Assert.AreEqual(functionCall.Function, functionCallVM.FunctionVM.Model);
            Assert.AreEqual(functionCall.Arguments.Count, functionCallVM.ArgumentsVM.Count);
            for (var i = 0; i < functionCall.Expressions.Count; i++)
               Assert.AreSame(functionCall.Expressions[i], functionCallVM.ExpressionsVM[i].Model);
            functionCall.Function = new Symbol("bar");
            functionCall.Expressions[1] = new Constant(-1147);
            functionCall.Expressions[2] = new Constant("gra");
            Assert.AreEqual(functionCall.Function, functionCallVM.FunctionVM.Model);
            Assert.AreEqual(functionCall.Arguments.Count, functionCallVM.ArgumentsVM.Count);
            for (var i = 0; i < functionCall.Expressions.Count; i++)
               Assert.AreSame(functionCall.Expressions[i], functionCallVM.ExpressionsVM[i].Model);
         }
      }
   }
}
