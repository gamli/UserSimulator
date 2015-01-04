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

      public void VisitForLoop(ForLoop ForLoop)
      {
         Append("FOR(");
         ForLoop.RepetitionCount.Accept(this);
         Append(")");
         AppendBody(ForLoop);
      }

      public void VisitWindowshot(Windowshot Windowshot)
      {
         Append(FunctionCall("WINDOWSHOT", Windowshot.PositionX, Windowshot.PositionY, Windowshot.ImageUrl));
      }

      private void AppendBody(StatementWithBodyBase StatementWithBody)
      {
         IncreaseIndent();
         AppendNewLine();
         StatementWithBody.Body.Accept(this);
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
      
      public void VisitIf(If If)
      {
         Append("IF(");
         If.Expression.Accept(this);
         Append(")");
         AppendBody(If);
      }
      
      public void VisitVariableAssignment(VariableAssignment VariableAssignment)
      {
         Append(VariableAssignment.Symbol);
         Append(AssignmentSymbol());
         VariableAssignment.Expression.Accept(this);
      }

      private static string AssignmentSymbol()
      {
         return " = ";
      }

      private void AppendFunctionCallStatement(string FunctionName, params MacroBase[] FunctionParameters)
      {
         Append(FunctionCall(FunctionName, FunctionParameters));
      }

      private static string FunctionCall(string FunctionName, params MacroBase[] FunctionParameters)
      {
         var printedFunctionParameters =
            FunctionParameters.Select(Param => new MacroPrinter(Param).Print());
         return FunctionName + "(" + string.Join(ParameterSeperator(), printedFunctionParameters) + ")";
      }

      private static string ParameterSeperator()
      {
         return ", ";
      }

      private void Append(string Text)
      {
         _sb.Append(Text);
      }
   }
}
