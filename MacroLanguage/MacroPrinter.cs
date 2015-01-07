using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroLanguage
{
   public class MacroPrinter : IVisitor
   {
      public static string Print(MacroBase Macro)
      {
         return new MacroPrinter(Macro).Print();
      }

      private StringBuilder _sb;
      private MacroBase _macro;

      private MacroPrinter(MacroBase Macro)
      {
         _sb = new StringBuilder();
         _macro = Macro;
      }

      private string Print()
      {
         _macro.Accept(this);
         return _sb.ToString();
      }

      public void VisitConstant(Constant Constant)
      {
         var value = Constant.Value;

         if (value is string)
            Append("\"" + ((string)value).Replace("\"", "\\\"") + "\"");
         else if (value is double)
            Append(((double)value).ToString(CultureInfo.InvariantCulture));
         else
            Append(value == null ? "null" : value.ToString());
      }

      public void VisitDefinition(Definition Definition)
      {
         VisitList(Definition);
      }

      public void VisitFunctionCall(FunctionCall FunctionCall)
      {
         VisitList(FunctionCall);
      }

      public void VisitIf(If If)
      {
         VisitList(If);
      }

      public void VisitList(List List)
      {
         Append("(");
         foreach (var expression in List.Expressions.Take(List.Expressions.Count - 1))
         {
            expression.Accept(this);
            Append(" ");
         }
         List.Expressions.Last().Accept(this);
         Append(")");
      }

      public void VisitLoop(Loop Loop)
      {
         VisitList(Loop);
      }

      public void VisitQuote(Quote Quote)
      {
         VisitList(Quote);
      }

      public void VisitSymbol(Symbol Symbol)
      {
         Append(Symbol.Value);
      }

      private void Append(string Text)
      {
         _sb.Append(Text);
      }
   }
}
