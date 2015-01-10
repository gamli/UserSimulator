using Macro;
using MacroViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MacroViewModel_TEST
{
   [TestClass]
   public class FunctionCallVM_TEST
   {
      [TestMethod]
      public void ProcedureVM_Property_TEST()
      {
         var procedureCall = new ProcedureCall { Procedure = new Symbol("foo") };
         procedureCall.Expressions.Add(new Constant(true));
         procedureCall.Expressions.Add(new Constant("arg"));
         using (var procedureCallVM = new ProcedureCallVM(procedureCall))
         {
            Assert.AreEqual(procedureCall.Procedure, procedureCallVM.ProcedureVM.Model);
            Assert.AreEqual(procedureCall.Arguments.Count, procedureCallVM.ArgumentsVM.Count);
            for (var i = 0; i < procedureCall.Arguments.Count; i++)
               Assert.AreSame(procedureCall.Arguments[i], procedureCallVM.ArgumentsVM[i].Model);
            procedureCall.Procedure = new Symbol("bar");
            procedureCall.Expressions[1] = new Constant(-1147);
            procedureCall.Expressions[2] = new Constant("gra");
            Assert.AreEqual(procedureCall.Procedure, procedureCallVM.ProcedureVM.Model);
            Assert.AreEqual(procedureCall.Arguments.Count, procedureCallVM.ArgumentsVM.Count);
            for (var i = 0; i < procedureCall.Arguments.Count; i++)
               Assert.AreSame(procedureCall.Arguments[i], procedureCallVM.ArgumentsVM[i].Model);
         }
      }
   }
}
