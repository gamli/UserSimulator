namespace Macro
{
   public class Loop : SpecialFormBase
   {
      private ExpressionBase _condition;
      public ExpressionBase Condition { get { return _condition; } set { SetPropertyValue(ref _condition, value); } }

      private ExpressionBase _body;
      public ExpressionBase Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }

      public Loop()
         : base("loop", "Condition", "Body")
      {
         // nothing to do
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitLoop(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherLoop = (Loop)OtherMacro;
         return 
            Condition.Equals(otherLoop.Condition) &&
            Body.Equals(otherLoop.Body) && base.MacroEquals(otherLoop);
      }
   }
}
