using System;
using Macro;

namespace MacroRuntime
{
   public class IntrinsicProcedure : ProcedureBase
   {
      private Func<IContext, Expression> _function;
      public Func<IContext, Expression> Function { get { return _function; } set { SetPropertyValue(ref _function, value); } }

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
