using System;
using System.Text;
using Macro;
using MacroLanguage;

namespace MacroLanguage_TEST
{
   class MacroPrintParseTestCase
   {
      public static Tuple<string, MacroBase> TestCase()
      {
         var text = new StringBuilder();

         var macro = new List();
         macro.Expressions.Add(new Symbol("begin"));
         text.Append("(begin");

         macro.Expressions.Add(new Symbol("-"));
         text.Append(" -");

         macro.Expressions.Add(new Symbol("4711ASymbolStartingWithANumber"));
         text.Append(" 4711ASymbolStartingWithANumber");

         const string CRAZY_SYMBOL = IronyLispishGrammar.EXTRA_SYMBOL_CHARACTERS + "ASymbolWithSpecial" +
                                    IronyLispishGrammar.EXTRA_SYMBOL_CHARACTERS + "Characters" +
                                    IronyLispishGrammar.EXTRA_SYMBOL_CHARACTERS;
         macro.Expressions.Add(new Symbol(CRAZY_SYMBOL));
         text.Append(" " + CRAZY_SYMBOL);

         macro.Expressions.Add(new Constant(null));
         text.Append(" null");

         macro.Expressions.Add(new Constant(true));
         text.Append(" true");

         macro.Expressions.Add(new Constant(false));
         text.Append(" false");

         macro.Expressions.Add(new List());
         text.Append(" nil");

         macro.Expressions.Add(new Constant("Some \" String"));
         text.Append(" \"Some \\\" String\"");

         macro.Expressions.Add(new Constant(4711m));
         text.Append(" 4711");

         macro.Expressions.Add(new Constant(4711.1174m));
         text.Append(" 4711.1174");

         macro.Expressions.Add(new Constant(-4711m));
         text.Append(" -4711");

         macro.Expressions.Add(new Constant(-4711.1174m));
         text.Append(" -4711.1174");

         macro.Expressions.Add(new List(new Constant(4711m), new Symbol("some-symbol")));
         text.Append(" (4711 some-symbol)");

         macro.Expressions.Add(SpecialForms.If(new Constant(true), new Constant("Consequent"),new Constant("Alternative")));
         text.Append(" (if true \"Consequent\" \"Alternative\")");

         macro.Expressions.Add(SpecialForms.Loop(new Constant(true), SpecialForms.ProcedureCall(new Symbol("fun"))));
         text.Append(" (loop true (fun))");

         macro.Expressions.Add(SpecialForms.ProcedureCall(new Symbol("fun"), new Constant("arg")));
         text.Append(" (fun \"arg\")");

         macro.Expressions.Add(SpecialForms.Quote(SpecialForms.ProcedureCall(new Symbol("fun"))));
         text.Append(" '(fun)");

         macro.Expressions.Add(SpecialForms.Define(new Symbol("var"), new Constant(4711m)));
         text.Append(" (define var 4711)");

         var symbolX = new Symbol("x");
         macro.Expressions.Add(SpecialForms.Lambda(new List(symbolX), SpecialForms.ProcedureCall(new Symbol("mult"), symbolX, symbolX)));
         text.Append(" (lambda (x) (mult x x))");

         text.Append(")");

         return Tuple.Create(text.ToString(), (MacroBase)macro);
      }
   }
}
