using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public static class SpecialForms
   {
      public static List Define(Symbol Symbol, Expression Expression)
      {
         return new List(new Symbol("define"), Symbol, Expression);
      }

      public static List If(Expression Condition, Expression Consequent, Expression Alternative)
      {
         return new List(new Symbol("if"), Condition, Consequent, Alternative);
      }

      public static List Lambda(Expression FormalArguments, Expression Body)
      {
         return new List(new Symbol("lambda"), FormalArguments, Body);
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
         return new List(new Symbol("quote"), Expression);
      }

      public static List Loop(Expression Condition, Expression Body)
      {
         return new List(new Symbol("loop"), Condition, Body);
      }
   }
}
