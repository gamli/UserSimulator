using System;
using System.Diagnostics.CodeAnalysis;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class SpecialForms_TEST
   {
      [TestMethod]
      public void CreateFunctions_TEST() // TODO implement tests
      {
         var symbol = new Symbol("mooo");
         var condition = new Constant(true);
         var one = new Constant(1);
         var two = new Constant(2);

         var defineForm = SpecialForms.Define(symbol, one);

         var ifForm = SpecialForms.If(condition, one, two);
         
         var lambdaForm = SpecialForms.Lambda(new List(symbol), one);
         
         var procedureCallForm = SpecialForms.ProcedureCall(symbol, one, two);
         
         var quoteForm = SpecialForms.Quote(symbol);
         
         var loopForm = SpecialForms.Loop(condition, symbol);
      }
   }
}
