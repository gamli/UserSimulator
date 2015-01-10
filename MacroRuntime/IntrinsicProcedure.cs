using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroRuntime
{
   public class IntrinsicProcedure : ProcedureBase
   {
      private Func<ContextBase, ExpressionBase> _function;
      public Func<ContextBase, ExpressionBase> Function { get { return _function; } set { SetPropertyValue(ref _function, value); } }

      protected override ExpressionBase ExecuteCall(ContextBase Context)
      {
         return Function(Context);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherIntrinsicProcedure = (IntrinsicProcedure)OtherMacro;
         var equals = Function.Equals(otherIntrinsicProcedure.Function);
         if(equals)
            equals &= base.MacroEquals(otherIntrinsicProcedure);
         return equals;
      }
   }
}
