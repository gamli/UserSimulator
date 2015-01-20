using System;
using System.Collections.ObjectModel;
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

      public ObservableCollection<REPLOutput> Output { get; private set; }

      public REPL(bool FormatOutput)
      {
         _formatOutput = FormatOutput;
         Output = new ObservableCollection<REPLOutput>();
         Reset();
      }

      public void Reset()
      {
         Output.Clear();
         ResetIndent();
         _context = new RuntimeContext(IntPtr.Zero);
         OutputEmptyInputEcho();
      }

      public void ConsumeInput(string Input)
      {
         try
         {
            var inputEcho = Output[Output.Count - 1];
            if (!string.IsNullOrEmpty(inputEcho.Text))
            {
               Indent();
               inputEcho.Text += "\n" + Indentation();
            }
            inputEcho.Text += Input;

            var parsed = _parser.Parse(inputEcho.Text);
            if (parsed != null)
            {
               var evaluated = new ExpressionEvaluator(_context).Evaluate((Expression) parsed);

               var printed = MacroPrinter.Print(evaluated, _formatOutput);

               ResetIndent();
               OutputEvaluatedExpression(printed);
               OutputEmptyInputEcho(); 
            }
         }
         catch (ParseException e)
         {
            OutputParseError(e.DisplayMessage());
            OutputEmptyInputEcho(); 
         }
         catch (RuntimeException e)
         {
            OutputRuntimeError(e.MacroStackTrace());
            OutputEmptyInputEcho(); 
         }
      }
      
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

      private void OutputEmptyInputEcho()
      {
         AddOutput("", REPLOutputType.InputEcho);
      }

      private void OutputEvaluatedExpression(string Text)
      {
         AddOutput(Text, REPLOutputType.EvaluatedExpression);
      }

      private void OutputParseError(string Text)
      {
         AddOutput(Text, REPLOutputType.ParseError);
      }

      private void OutputRuntimeError(string Text)
      {
         AddOutput(Text, REPLOutputType.RuntimeError);
      }

      private void AddOutput(string Text, REPLOutputType Type)
      {
         Output.Add(new REPLOutput { Text = Text, Type = Type });
      }
   }

   public class REPLOutput : NotifyPropertyChangedBase
   {
      private string _text;
      private REPLOutputType _type;

      public string Text
      {
         get { return _text; }
         set { SetPropertyValue(ref _text, value); }
      }

      public REPLOutputType Type
      {
         get { return _type; }
         set { SetPropertyValue(ref _type, value); }
      }
   }

   public enum REPLOutputType
   {
      InputEcho, EvaluatedExpression, ParseError, RuntimeError
   }
}
