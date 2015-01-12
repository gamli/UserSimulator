using System;
using System.Text;
using Macro;

namespace MacroLanguage_TEST
{
   class MacroPrintParseTestCase
   {
      public static Tuple<string, MacroBase> TestCase()
      {
         var text = new StringBuilder();

         var macro = new ProcedureCall { Procedure = new Symbol("funTEST") };
         text.Append("(funTEST");

         macro.Expressions.Add(new Constant(true));
         text.Append(" True");

         macro.Expressions.Add(new Constant(false));
         text.Append(" False");

         macro.Expressions.Add(new ExpressionList());
         text.Append(" null");

         macro.Expressions.Add(new Constant("Some \" String"));
         text.Append(" \"Some \\\" String\"");

         macro.Expressions.Add(new Constant(4711));
         text.Append(" 4711");

         macro.Expressions.Add(new Constant(4711.1174));
         text.Append(" 4711.1174");

         macro.Expressions.Add(new If { Condition = new Constant(true), Consequent = new Constant("Consequent"), Alternative = new Constant("Alternative") });
         text.Append(" (if True \"Consequent\" \"Alternative\")");

         macro.Expressions.Add(new Loop { Condition = new Constant(true), Body = new ProcedureCall { Procedure = new Symbol("fun") } });
         text.Append(" (loop True (fun))");

         var procedureCall = new ProcedureCall { Procedure = new Symbol("fun") };
         procedureCall.Expressions.Add(new Constant("arg"));
         macro.Expressions.Add(procedureCall);
         text.Append(" (fun \"arg\")");

         macro.Expressions.Add(new Quote { Expression = new ProcedureCall { Procedure = new Symbol("fun") } });
         text.Append(" (quote (fun))");

         macro.Expressions.Add(new Definition { Symbol = new Symbol("var"), Expression = new Constant(4711) });
         text.Append(" (define var 4711)");

         var lambdaBody = new ProcedureCall { Procedure = new Symbol("mult") };
         lambdaBody.Expressions.Add(new Symbol("x"));
         lambdaBody.Expressions.Add(new Symbol("x"));
         var lambda = new Lambda { ArgumentSymbols = new SymbolList(), Body = lambdaBody };
         lambda.ArgumentSymbols.Expressions.Add(new Symbol("x"));
         macro.Expressions.Add(lambda);
         text.Append(" (lambda (x) (mult x x))");

         text.Append(")");

         return Tuple.Create(text.ToString(), (MacroBase)macro);
      }
   }
}
