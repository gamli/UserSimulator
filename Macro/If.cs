using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class If : SpecialFormBase
   {
      private ExpressionBase _condition;
      public ExpressionBase Condition { get { return _condition; } set { SetPropertyValue(ref _condition, value); } }

      private ExpressionBase _consequent;
      public ExpressionBase Consequent { get { return _consequent; } set { SetPropertyValue(ref _consequent, value); } }

      private ExpressionBase _alternative;
      public ExpressionBase Alternative { get { return _alternative; } set { SetPropertyValue(ref _alternative, value); } }

      public If() 
         : base("if", "Condition", "Consequent", "Alternative")
      {
         // nothing to do
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitIf(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherIf = (If)OtherMacro;
         return 
            Condition.Equals(otherIf.Condition) &&
            Consequent.Equals(otherIf.Consequent) &&
            Alternative.Equals(otherIf.Alternative);
      }
   }
}
