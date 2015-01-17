using System;
using Macro;

namespace MacroRuntime
{
   public class IntrinsicProcedure : ProcedureBase
   {
      private Func<ContextBase, Expression> _function;
      public Func<ContextBase, Expression> Function { get { return _function; } set { SetPropertyValue(ref _function, value); } }

      protected override Expression ExecuteCall(ContextBase Context)
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

      protected override int MacroGetHashCode()
      {
         return Function.GetHashCode();
      }
   }
}
