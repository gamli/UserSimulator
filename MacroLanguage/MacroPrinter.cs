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

      public void VisitSymbol(Symbol Symbol)
      {
         Append(Symbol.Value);
      }

      public void VisitList(List List)
      {
         if (List.Expressions.Count == 0)
         {
            Append("nil");
            return;
         }

         Append("(");

         var index = 0;
         foreach (var expression in List.Expressions.Take(List.Expressions.Count - 1))
         {
            expression.Accept(this);

            if (_linebreaks && index == 1)
               IncreaseIndent();

            if (_linebreaks && index >= 1)
               AppendNewLine();
            else
               Append(" ");

            index++;
         }
         if(List.Expressions.Count != 0)
            List.Expressions.Last().Accept(this);
         
         if(_linebreaks && index > 1)
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
