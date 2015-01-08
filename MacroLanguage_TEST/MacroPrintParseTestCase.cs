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

         var macro = new FunctionCall { Function = new Symbol("funTEST") };
         text.Append("(funTEST");

         macro.Expressions.Add(new Constant(true));
         text.Append(" True");

         macro.Expressions.Add(new Constant(false));
         text.Append(" False");

         macro.Expressions.Add(new Constant(null));
         text.Append(" null");

         macro.Expressions.Add(new Constant("Some \" String"));
         text.Append(" \"Some \\\" String\"");

         macro.Expressions.Add(new Constant(4711));
         text.Append(" 4711");

         macro.Expressions.Add(new Constant(4711.1174));
         text.Append(" 4711.1174");

         macro.Expressions.Add(new Definition { Symbol = new Symbol("var"), Expression = new Constant(4711) });
         text.Append(" (define var 4711)");

         var functionCall = new FunctionCall { Function = new Symbol("fun") };
         functionCall.Expressions.Add(new Constant("arg"));
         macro.Expressions.Add(functionCall);
         text.Append(" (fun \"arg\")");

         macro.Expressions.Add(new If { Condition = new Constant(true), Consequent = new Constant("Consequent"), Alternative = new Constant("Alternative") });
         text.Append(" (if True \"Consequent\" \"Alternative\")");

         macro.Expressions.Add(new Loop { Condition = new Constant(true), Body = new FunctionCall { Function = new Symbol("fun") } });
         text.Append(" (loop True (fun))");

         macro.Expressions.Add(new Quote { Expression = new FunctionCall { Function = new Symbol("fun") } });
         text.Append(" (quote (fun))");

         text.Append(")");

         return Tuple.Create(text.ToString(), (MacroBase)macro);
      }
   }
}
