using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroLanguage
{
   public class ProgramPrinter : IVisitor
   {
      private StringBuilder _sb;
      private Program _program;

      public ProgramPrinter(Program Program)
      {
         _sb = new StringBuilder();
         _program = Program;
      }

      public string Print()
      {
         _program.Accept(this);
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
         Append("FOR(" + ForLoop.RepetitionCount + ")");
         AppendBody(ForLoop);
      }

      public void VisitWindowshot(Windowshot Windowshot)
      {
         Append(FunctionCall("IF_WINDOWSHOT", Windowshot.PositionX, Windowshot.PositionY, StringParameter(Windowshot.ImageUrl)));
         AppendBody(Windowshot);
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
         AppendFunctionCallStatement("PAUSE", Pause.Duration.TotalMilliseconds);
      }

      public void VisitLeftClick(LeftClick LeftClick)
      {
         AppendFunctionCallStatement("LEFT_CLICK");
      }

      private string StringParameter(string ParameterValue)
      {
         return ParameterValue == null ? null : "\"" + ParameterValue + "\"";
      }

      private void AppendFunctionCallStatement(string FunctionName, params object[] FunctionParameters)
      {
         AppendStatement(FunctionCall(FunctionName, FunctionParameters));
      }

      private static string FunctionCall(string FunctionName, params object[] FunctionParameters)
      {
         return FunctionName + "(" + string.Join(ParameterSeperator(), FunctionParameters.Select(Param => Param == null ? "null" : Param)) + ")";
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
