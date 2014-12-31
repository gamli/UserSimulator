using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroLanguage
{
   public class MacroPrinter : IVisitor
   {
      private StringBuilder _sb;
      private MacroBase _macro;

      public MacroPrinter(MacroBase Macro)
      {
         _sb = new StringBuilder();
         _macro = Macro;
      }

      public string Print()
      {
         _macro.Accept(this);
         return _sb.ToString();
      }

      public void VisitProgram(Program Program)
      {
         Append("PROGRAM");
         AppendNewLine();
         Program.Body.Accept(this);
      }

      public void VisitBlock(Block Block)
      {
         Append("{");
         IncreaseIndent();
         foreach (var item in Block.Items)
         {
            AppendNewLine();
            item.Accept(this);
         }
         DecreaseIndent();
         AppendNewLine();
         Append("}");
      }

      public void VisitNoOp(NoOp NoOp)
      {
         AppendStatement("");
      }

      public void VisitForLoop(ForLoop ForLoop)
      {
         Append("FOR(");
         ForLoop.RepetitionCount.Accept(this);
         Append(")");
         AppendBody(ForLoop);
      }

      public void VisitWindowshotExpression(WindowshotExpression Windowshot)
      {
         Append(FunctionCall("WINDOWSHOT", Windowshot.PositionX, Windowshot.PositionY, Windowshot.ImageUrl));
      }

      private void AppendBody(MacroWithBodyBase MacroWithBody)
      {
         IncreaseIndent();
         AppendNewLine();
         MacroWithBody.Body.Accept(this);
         DecreaseIndent();
      }

      private void AppendNewLine()
      {
         _sb.Append("\r\n");
         Indent();
      }

      private void Indent()
      {
         for (var i = 0; i < _currentIndent; i++)
            _sb.Append("   ");
      }

      private void IncreaseIndent()
      {
         _currentIndent++;
      }
      private void DecreaseIndent()
      {
         _currentIndent--;
      }
      private int _currentIndent;

      public void VisitMove(Move Move)
      {
         AppendFunctionCallStatement("MOVE", Move.TranslationX, Move.TranslationY);
      }

      public void VisitPosition(Position Position)
      {
         AppendFunctionCallStatement("POSITION", Position.X, Position.Y);
      }

      public void VisitPause(Pause Pause)
      {
         AppendFunctionCallStatement("PAUSE", Pause.Duration);
      }

      public void VisitLeftClick(LeftClick LeftClick)
      {
         AppendFunctionCallStatement("LEFT_CLICK");
      }

      public void VisitConstantExpression<T>(ConstantExpression<T> ConstantExpression)
      {
         var expressionValue = ConstantExpression.Value;

         if (expressionValue is string)
            Append("\"" + expressionValue + "\"");
         else
            Append(expressionValue == null ? "null" : expressionValue.ToString());         
      }
      
      public void VisitIfStatement(IfStatement IfStatement)
      {
         Append("IF(");
         IfStatement.Expression.Accept(this);
         Append(")");
         AppendBody(IfStatement);
      }

      private void AppendFunctionCallStatement(string FunctionName, params ExpressionBase[] FunctionParameters)
      {
         AppendStatement(FunctionCall(FunctionName, FunctionParameters));
      }

      private static string FunctionCall(string FunctionName, params ExpressionBase[] FunctionParameters)
      {
         var printedFunctionParameters =
            FunctionParameters.Select(Param => Param == null ? "null" : new MacroPrinter(Param).Print());
         return FunctionName + "(" + string.Join(ParameterSeperator(), printedFunctionParameters) + ")";
      }

      private static string ParameterSeperator()
      {
         return ", ";
      }

      private void AppendStatement(string StatementText)
      {
         Append(StatementText);
         Append(";");
      }

      private void Append(string Text)
      {
         _sb.Append(Text);
      }
   }
}
