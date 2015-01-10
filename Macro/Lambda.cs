using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Macro
{

   public class Lambda : SpecialFormBase
   {
      private SymbolList _argumentSymbols;
      public SymbolList ArgumentSymbols { get { return _argumentSymbols; } set { SetPropertyValue(ref _argumentSymbols, value); } }

      private ExpressionBase _body;
      public ExpressionBase Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }

      public Lambda()
         : base("lambda", "ArgumentSymbols", "Body")
      {
         // nothing to do
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitLambda(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherLambda = (Lambda)OtherMacro;
         return
            ArgumentSymbols.Equals(otherLambda.ArgumentSymbols) &&
            Body.Equals(otherLambda.Body) && base.MacroEquals(otherLambda);
      }
   }
}
