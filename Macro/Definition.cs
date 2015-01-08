namespace Macro
{
   public class Definition : SpecialFormBase
   {
      private Symbol _symbol;
      public Symbol Symbol { get { return _symbol; } set { SetPropertyValue(ref _symbol, value); } }

      private ExpressionBase _expression;
      public ExpressionBase Expression { get { return _expression; } set { SetPropertyValue(ref _expression, value); } }

      public Definition()
         : base("define", "Symbol", "Expression")
      {
         // nothing to do
      }

      public override void Accept(IVisitor Visitor)
      {
          Visitor.VisitDefinition(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var other = (Definition)OtherMacro;
         return
            Symbol.Equals(other.Symbol) &&
            Expression.Equals(other.Expression) && base.MacroEquals(other);
      }
   }
}
