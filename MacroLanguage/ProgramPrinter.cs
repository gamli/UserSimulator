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
         IncreaseIndent();
         AppendNewLine();
         ForLoop.Body.Accept(this);
         DecreaseIndent();
      }

      public void VisitMove(Move Move)
      {
         AppendStatement("MOVE(" + Move.TranslationX + ", " + Move.TranslationY + ")");
      }

      public void VisitPosition(Position Position)
      {
         AppendStatement("POSITION(" + Position.X + ", " + Position.Y + ")");
      }

      public void VisitPause(Pause Pause)
      {
         AppendStatement("PAUSE(" + Pause.Duration.TotalMilliseconds + ")");
      }

      public void VisitWindowshot(Windowshot Windowshot)
      {
         throw new NotImplementedException();
      }

      public void VisitLeftClick(LeftClick LeftClick)
      {
         AppendStatement("LEFT_CLICK()");
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
   }
}
