﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroRuntime
{
   public class Procedure : ProcedureBase
   {
      private List _lambda;
      public List Lambda { get { return _lambda; } set { SetPropertyValue(ref _lambda, value); } }

      protected override Expression ExecuteCall(ContextBase Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(Lambda.Expressions[2]);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProcedure = (Procedure) OtherMacro;
         return Lambda.Equals(otherProcedure.Lambda) && base.MacroEquals(otherProcedure);
      }

      protected override int MacroGetHashCode()
      {
         return Lambda.GetHashCode();
      }
   }
}
