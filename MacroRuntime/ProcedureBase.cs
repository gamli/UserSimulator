using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Macro;

namespace MacroRuntime
{
   public abstract class ProcedureBase : Expression
   {
      private ContextBase _definingContext;
      public ContextBase DefiningContext { get { return _definingContext; } set { SetPropertyValue(ref _definingContext, value); } }

      private List _formalArguments;
      public List FormalArguments { get { return _formalArguments; } set { SetPropertyValue(ref _formalArguments, value); } }
      
      // TODO extract superclass from MacroBase to make this dummy implementation unnecessary
      [ExcludeFromCodeCoverage]
      public override void Accept(IVisitor Visitor)
      {
         throw new NotImplementedException();
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProcedure = (ProcedureBase) OtherMacro;
         return 
            DefiningContext.Equals(otherProcedure.DefiningContext) && 
            FormalArguments.Equals(otherProcedure.FormalArguments);
      }
   }
}
