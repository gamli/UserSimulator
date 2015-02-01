namespace Macro
{
   public interface IContext
   {
      void DefineValue(Symbol Symbol, Expression Value);
      bool IsValueDefined(Symbol Symbol);
      void SetValue(Symbol Symbol, Expression Value);
      Expression GetValue(Symbol Symbol);
   }
}