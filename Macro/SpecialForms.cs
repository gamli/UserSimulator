namespace Macro
{
   public static class SpecialForms
   {
      public static readonly Symbol DefineSymbol = new Symbol("define");
      public static readonly Symbol IfSymbol = new Symbol("if");
      public static readonly Symbol LambdaSymbol = new Symbol("lambda");
      public static readonly Symbol QuoteSymbol = new Symbol("quote");
      public static readonly Symbol LoopSymbol = new Symbol("loop");
      public static readonly Symbol SetValueSymbol = new Symbol("set!");


      public static List Define(Symbol Symbol, Expression Expression)
      {
         return new List(DefineSymbol, Symbol, Expression);
      }

      public static List If(Expression Condition, Expression Consequent, Expression Alternative)
      {
         return new List(IfSymbol, Condition, Consequent, Alternative);
      }

      public static List Lambda(Expression FormalArguments, Expression Body)
      {
         return new List(LambdaSymbol, FormalArguments, Body);
      }

      public static List ProcedureCall(Expression Procedure, params Expression[] Arguments)
      {
         var procedureCall = new List(Procedure);

         foreach (var argument in Arguments)
            procedureCall.Expressions.Add(argument);

         return procedureCall;
      }

      public static List Quote(Expression Expression)
      {
         return new List(QuoteSymbol, Expression);
      }

      public static List Loop(Expression Condition, Expression Body)
      {
         return new List(LoopSymbol, Condition, Body);
      }

      public static List SetValue(Symbol Symbol, Expression Value)
      {
         return new List(SetValueSymbol, Symbol, Value);
      }
   }
}
