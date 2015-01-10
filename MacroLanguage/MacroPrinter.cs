using System.Globalization;
using System.Linq;
using System.Text;
using Macro;

namespace MacroLanguage
{
   public class MacroPrinter : IVisitor
   {
      public static string Print(MacroBase Macro)
      {
         return new MacroPrinter(Macro).Print();
      }

      private readonly StringBuilder _sb;
      private readonly MacroBase _macro;

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
         VisitListExpressionBase(Definition);
      }

      public void VisitIf(If If)
      {
         VisitListExpressionBase(If);
      }

      public void VisitLambda(Lambda Lambda)
      {
         VisitListExpressionBase(Lambda);
      }

      public void VisitLoop(Loop Loop)
      {
         VisitListExpressionBase(Loop);
      }

      public void VisitProcedureCall(ProcedureCall ProcedureCall)
      {
         VisitListExpressionBase(ProcedureCall);
      }

      public void VisitQuote(Quote Quote)
      {
         VisitListExpressionBase(Quote);
      }

      public void VisitSymbol(Symbol Symbol)
      {
         Append(Symbol.Value);
      }

      public void VisitSymbolList(SymbolList SymbolList)
      {
         VisitListExpressionBase(SymbolList);
      }

      private void VisitListExpressionBase<TExpression>(ListExpressionBase<TExpression> List)
         where TExpression : ExpressionBase
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

      private void Append(string Text)
      {
         _sb.Append(Text);
      }
   }
}
