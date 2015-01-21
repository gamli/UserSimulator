using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using Macro;
using MacroLanguage;

namespace MacroRuntime
{
   public class REPL : NotifyPropertyChangedBase
   {
      private readonly MacroParser _parser = new MacroParser();
      private RuntimeContext _context;
      private readonly bool _formatOutput;
      private int _indentation;
      private Expression _lastEvaluatedExpression;
      private Expression _lastParsedExpression;
      private int _lastErrorLine;
      private int _lastErrorColumn;
      private int _lastErrorPosition;
      private int _lastErrorLength;

      public ObservableCollection<REPLOutput> Output { get; private set; }

      public Expression LastEvaluatedExpression { get { return _lastEvaluatedExpression; } private set { SetPropertyValue(ref _lastEvaluatedExpression, value); } }

      public Expression LastParsedExpression { get { return _lastParsedExpression; } private set { SetPropertyValue(ref _lastParsedExpression, value); } }

      public int LastErrorLine { get { return _lastErrorLine; } private set { SetPropertyValue(ref _lastErrorLine, value); } }

      public int LastErrorColumn { get { return _lastErrorColumn; } private set { SetPropertyValue(ref _lastErrorColumn, value); } }

      public int LastErrorPosition { get { return _lastErrorPosition; } private set { SetPropertyValue(ref _lastErrorPosition, value); } }

      public int LastErrorLength { get { return _lastErrorLength; } private set { SetPropertyValue(ref _lastErrorLength, value); } }

      public REPL(bool FormatOutput)
      {
         _formatOutput = FormatOutput;
         Output = new ObservableCollection<REPLOutput>();
         Reset();
      }

      public void Reset()
      {
         ResetError();
         Output.Clear();
         ResetIndent();
         _context = new RuntimeContext(IntPtr.Zero);
         OutputEmptyInputEcho();
      }

      public void ConsumeInput(string Input)
      {
         AppendInputEcho(Input);

         ParseLastInput();
         
         EvaluateLastParsedExpression();
      }

      public void ParsePreview(string Input)
      {
         if (IsInWaitingForMoreInputState())
            AppendInputEcho("...parse preview requested - canceling input...");

         OutputEmptyInputEcho();

         AppendInputEcho(Input);

         ParseLastInput();
      }

      private void ResetError()
      {
         LastErrorLine = -1;
         LastErrorColumn = -1;
         LastErrorPosition = -1;
         LastErrorLength = -1;
      }

      private void AppendInputEcho(string Input)
      {
         if (IsInWaitingForMoreInputState())
         {
            Indent();
            GetLastInputEcho().AppendText("\n" + Indentation());
         }
         GetLastInputEcho().AppendText(Input);
      }

      private bool IsInWaitingForMoreInputState()
      {
         return !string.IsNullOrEmpty(GetLastInputEcho().Text);
      }

      private REPLOutput GetLastInputEcho()
      {
         return Output[Output.Count - 1];
      }

      #region parsing + evaluation

      private void ParseLastInput()
      {
         ResetError();

         try
         {
            LastParsedExpression = (Expression)_parser.Parse(GetLastInputEcho().Text);
            OutputInfo("Parsing successfull");
            OutputEmptyInputEcho();
         }
         catch (ParseException e)
         {
            LastErrorLine = e.Line;
            LastErrorColumn = e.Column;
            LastErrorPosition = e.Position;
            LastParsedExpression = null;
            OutputParseError(e);
            OutputEmptyInputEcho();
         }
      }

      private void EvaluateLastParsedExpression()
      {
         if (LastParsedExpression == null)
         {
            LastEvaluatedExpression = null;
            return;
         }

         ResetError();

         try
         {
            LastEvaluatedExpression = new ExpressionEvaluator(_context).Evaluate(LastParsedExpression);

            var printed = MacroPrinter.Print(LastEvaluatedExpression, _formatOutput);
            ResetIndent();
            OutputEvaluatedExpression(printed);
            OutputEmptyInputEcho();
         }
         catch (RuntimeException e)
         {
            LastErrorLine = e.InnermostTextLine();
            LastErrorColumn = e.InnermostTextColumn();
            LastErrorPosition = e.InnermostTextPosition();
            LastErrorLength = e.InnermostTextLength();
            LastParsedExpression = null;
            LastEvaluatedExpression = null;
            OutputRuntimeError(e);
            OutputEmptyInputEcho();
         }
      }

      #endregion

      #region Output

      private string Indentation()
      {
         return new string('\t', _indentation);
      }

      private void Indent()
      {
         _indentation++;
      }

      private void ResetIndent()
      {
         _indentation = 0;
      }

      private void OutputInfo(string Text)
      {
         AddOutput(Text, REPLOutputType.Info);
      }

      private void OutputEmptyInputEcho()
      {
         AddOutput("", REPLOutputType.InputEcho);
      }

      private void OutputEvaluatedExpression(string Text)
      {
         AddOutput(Text, REPLOutputType.EvaluatedExpression);
      }

      private void OutputParseError(ParseException Error)
      {
         AddOutput(Error.DisplayMessage(), REPLOutputType.ParseError);
      }

      private void OutputRuntimeError(RuntimeException Error)
      {
         AddOutput(Error.MacroStackTrace(), REPLOutputType.RuntimeError);
      }

      private void AddOutput(string Text, REPLOutputType Type)
      {
         Output.Add(new REPLOutput(Text, Type));
      }

      #endregion
   }

   public class REPLOutput : NotifyPropertyChangedBase
   {
      public REPLOutput(string Text, REPLOutputType Type)
      {
         this.Text = Text;
         this.Type = Type;
      }

      public string Text { get; private set; }
      public REPLOutputType Type { get; private set; }

      internal void AppendText(string Txt)
      {
         Text += Txt;
      }
   }

   public enum REPLOutputType
   {
      Info,
      InputEcho, 
      EvaluatedExpression, 
      ParseError, 
      RuntimeError
   }
}
