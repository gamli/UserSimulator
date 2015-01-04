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
      [ExcludeFromCodeCoverage]
      public ExpressionBase Condition { get { return _condition; } set { SetPropertyValue(ref _condition, value, 0); } }

      private ExpressionBase _consequent;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Consequent { get { return _consequent; } set { SetPropertyValue(ref _consequent, value, 1); } }

      private ExpressionBase _alternative;
      [ExcludeFromCodeCoverage]
      public ExpressionBase Alternative { get { return _alternative; } set { SetPropertyValue(ref _alternative, value, 2); } }

      public If()
      {
         Expressions.CollectionChanged +=
            (Sender, Arsg) => 
               {
                  Condition = Expressions[0];
                  Consequent = Expressions[1];
                  Alternative = Expressions[2]; 
               };
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
