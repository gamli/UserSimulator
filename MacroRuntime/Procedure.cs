using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroRuntime
{
   public class Procedure : ProcedureBase
   {
      private Lambda _lambda;
      public Lambda Lambda { get { return _lambda; } set { SetPropertyValue(ref _lambda, value); } }

      protected override ExpressionBase ExecuteCall(ContextBase Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(Lambda.Body);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProcedure = (Procedure) OtherMacro;
         return Lambda.Equals(otherProcedure.Lambda) && base.MacroEquals(otherProcedure);
      }
   }
}
