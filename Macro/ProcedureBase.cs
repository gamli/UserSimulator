using System;

namespace Macro
{
   public abstract class ProcedureBase : Expression
   {
      private IContext _definingContext;
      public IContext DefiningContext { get { return _definingContext; } set { SetPropertyValue(ref _definingContext, value); } }

      private Expression _formalArguments;
      public Expression FormalArguments { get { return _formalArguments; } set { SetPropertyValue(ref _formalArguments, value); } }
      
      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitProcedure(this);
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
