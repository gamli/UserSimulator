using System.ComponentModel;
using System.Linq;
using System.Text;
using Macro;

namespace MacroLanguage
{
   public class MacroPrinter : SpecialFormAwareVisitor
   {
      public static string Print(MacroBase Macro, bool Linebreaks)
      {
         return new MacroPrinter(Macro, Linebreaks).Print();
      }

      private readonly StringBuilder _sb;
      private readonly MacroBase _macro;
      private readonly bool _linebreaks;

      private MacroPrinter(MacroBase Macro, bool Linebreaks)
      {
         _sb = new StringBuilder();
         _macro = Macro;
         _linebreaks = Linebreaks;
      }

      private string Print()
      {
         _macro.Accept(this);
         return _sb.ToString();
      }

      public override void VisitConstant(Constant Constant)
      {
         Append(Constant.ToString());
      }

      public override void VisitNil(List Nil)
      {
         Append(Nil.ToString());
      }

      public override void VisitDefinition(List Definition)
      {
         PrintSpecialForm(Definition);
      }

      public override void VisitIf(List If)
      {
         PrintSpecialForm(If);
      }

      public override void VisitLambda(List Lambda)
      {
         PrintSpecialForm(Lambda);
      }

      public override void VisitProcedureCall(List ProcedureCall)
      {
         var procedureSymol = ProcedureCall.Expressions[0] as Symbol;
         if (procedureSymol != null && procedureSymol.Value == "begin")
            PrintList(ProcedureCall, 0);
         else
            PrintList(ProcedureCall, ProcedureCall.Expressions.Count);
      }

      public override void VisitQuote(List Quote)
      {
         Append("'"); Quote.Expressions[1].Accept(this);
      }

      public override void VisitLoop(List Loop)
      {
         PrintSpecialForm(Loop);
      }

      public override void VisitSetValue(List SetValue)
      {
         PrintSpecialForm(SetValue, SetValue.Expressions.Count);
      }

      public void PrintSpecialForm(List SpecialForm, int LinebreakIndex = 1)
      {
         PrintList(SpecialForm, LinebreakIndex);
      }

      public void PrintList(List List, int LinebreakIndex)
      {
         Append("(");

         var index = 0;
         foreach (var expression in List.Expressions.Take(List.Expressions.Count - 1))
         {
            expression.Accept(this);

            if (_linebreaks && index == LinebreakIndex)
               IncreaseIndent();

            if (_linebreaks && index >= LinebreakIndex)
               AppendNewLine();
            else
               Append(" ");

            index++;
         }
         if (List.Expressions.Count != 0)
            List.Expressions.Last().Accept(this);

         if (_linebreaks && index > LinebreakIndex)
            DecreaseIndent();

         Append(")");
      }

      private void AppendNewLine()
      {
         Append("\n");
         Append(new string(Enumerable.Repeat('\t', _indent).ToArray()));
      }

      private void IncreaseIndent()
      {
         _indent++;
      }

      private void DecreaseIndent()
      {
         _indent--;
      }

      public override void VisitSymbol(Symbol Symbol)
      {
         Append(Symbol.ToString());
      }

      private void Append(string Text)
      {
         _sb.Append(Text);
      }

      private int _indent;
   }
}
