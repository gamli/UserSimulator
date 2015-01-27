using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class SpecialForms_TEST
   {
      [TestMethod]
      public void CreateFunctions_TEST()
      {
         var symbol = new Symbol("mooo");
         var condition = new Constant(true);
         var one = new Constant(1);
         var two = new Constant(2);

         var defineForm = SpecialForms.Define(symbol, one);
         Assert.AreEqual(defineForm.Expressions[0], SpecialForms.DefineSymbol);
         Assert.AreEqual(defineForm.Expressions[1], symbol);
         Assert.AreEqual(defineForm.Expressions[2], one);

         var ifForm = SpecialForms.If(condition, one, two);
         Assert.AreEqual(ifForm.Expressions[0], SpecialForms.IfSymbol);
         Assert.AreEqual(ifForm.Expressions[1], condition);
         Assert.AreEqual(ifForm.Expressions[2], one);
         Assert.AreEqual(ifForm.Expressions[3], two);

         var lambdaForm = SpecialForms.Lambda(new List(symbol), one);
         Assert.AreEqual(lambdaForm.Expressions[0], SpecialForms.LambdaSymbol);
         Assert.AreEqual(lambdaForm.Expressions[1], new List(symbol));
         Assert.AreEqual(lambdaForm.Expressions[2], one);

         var procedureCallForm = SpecialForms.ProcedureCall(symbol, one, two);
         Assert.AreEqual(procedureCallForm.Expressions[0], symbol);
         Assert.AreEqual(procedureCallForm.Expressions[1], one);
         Assert.AreEqual(procedureCallForm.Expressions[2], two);

         var quoteForm = SpecialForms.Quote(symbol);
         Assert.AreEqual(quoteForm.Expressions[0], SpecialForms.QuoteSymbol);
         Assert.AreEqual(quoteForm.Expressions[1], symbol);

         var loopForm = SpecialForms.Loop(condition, symbol);
         Assert.AreEqual(loopForm.Expressions[0], SpecialForms.LoopSymbol);
         Assert.AreEqual(loopForm.Expressions[1], condition);
         Assert.AreEqual(loopForm.Expressions[2], symbol);

         var setValueForm = SpecialForms.SetValue(symbol, one);
         Assert.AreEqual(setValueForm.Expressions[0], SpecialForms.SetValueSymbol);
         Assert.AreEqual(setValueForm.Expressions[1], symbol);
         Assert.AreEqual(setValueForm.Expressions[2], one);
      }
   }
}
