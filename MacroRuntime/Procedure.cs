using Macro;

namespace MacroRuntime
{
   public class Procedure : ProcedureBase
   {
      private List _lambda;
      public List Lambda { get { return _lambda; } set { SetPropertyValue(ref _lambda, value); } }
      
      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherProcedure = (Procedure) OtherMacro;
         return Equals(Lambda, otherProcedure.Lambda) && base.MacroEquals(otherProcedure);
      }

      protected override int MacroGetHashCode()
      {
         return Lambda.GetHashCode();
      }
   }
}
