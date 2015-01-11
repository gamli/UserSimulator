using System.Globalization;
using System.Linq;
using System.Text;
using Macro;

namespace MacroLanguage
{
   public class MacroPrinter : IVisitor
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

      public void VisitConstant(Constant Constant)
      {
         var value = Constant.Value;

         var stringValue = value as string;
         if (stringValue != null)
            Append("\"" + stringValue.Replace("\"", "\\\"") + "\"");
         else if (value is double)
            Append(((double)value).ToString(CultureInfo.InvariantCulture));
         else
            Append(value == null ? "null" : value.ToString());
      }

      public void VisitDefinition(Definition Definition)
      {
         VisitListExpressionBase(Definition, 1);
      }

      public void VisitExpressionList(ExpressionList ExpressionList)
      {
         VisitListExpressionBase(ExpressionList, ExpressionList.Expressions.Count);
      }

      public void VisitIf(If If)
      {
         VisitListExpressionBase(If, 1);
      }

      public void VisitLambda(Lambda Lambda)
      {
         VisitListExpressionBase(Lambda, 1);
      }

      public void VisitLoop(Loop Loop)
      {
         VisitListExpressionBase(Loop, 1);
      }

      public void VisitProcedureCall(ProcedureCall ProcedureCall)
      {
         VisitListExpressionBase(ProcedureCall, 0);
      }

      public void VisitQuote(Quote Quote)
      {
         VisitListExpressionBase(Quote, 1);
      }

      public void VisitSymbol(Symbol Symbol)
      {
         Append(Symbol.Value);
      }

      public void VisitSymbolList(SymbolList SymbolList)
      {
         VisitListExpressionBase(SymbolList, SymbolList.Symbols.Count);
      }

      private void VisitListExpressionBase<TExpression>(ListExpressionBase<TExpression> List, int LinebreakIndex)
         where TExpression : ExpressionBase
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
         if(List.Expressions.Count != 0)
            List.Expressions.Last().Accept(this);
         
         if(_linebreaks && index > LinebreakIndex)
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

      private void Append(string Text)
      {
         _sb.Append(Text);
      }

      private int _indent;
   }
}
