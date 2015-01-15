using System;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows;
using Common;
using IO;
using Macro;
using MacroLanguage;
using MacroRuntime;
using Expression = Macro.Expression;
using Window = IO.Window;

namespace UserSimulator
{
   class UserSimulatorModel : NotifyPropertyChangedBase, IDisposable
   {
      private bool _disposed;

      private readonly Timer _timer;

      private Image _lastWindowshot;
      public Image LastWindowshot
      {
         get { return _lastWindowshot; }
         private set
         {
            if (LastWindowshot != null)
               LastWindowshot.Dispose();
            SetPropertyValue(ref _lastWindowshot, value);
         }
      }

      private IntPtr _lastWindow;
      public IntPtr LastWindow { get { return _lastWindow; } set { SetPropertyValue(ref _lastWindow, value); } }

      private Expression _expression;
      public Expression Expression
      {
         get { return _expression; }
         set
         {
            var oldExpression = _expression;
            if (SetPropertyValue(ref _expression, value))
            {
               if (oldExpression != null)
                  oldExpression.MacroChanged -= HandleExpressionChanged;
               if (value != null)
                  value.MacroChanged += HandleExpressionChanged;
               HandleExpressionChanged();
            }
         }
      }
      private void HandleExpressionChanged(object Sender, EventArgs Args)
      {
         HandleExpressionChanged();
      }
      private void HandleExpressionChanged()
      {
         // ReSharper disable once ExplicitCallerInfoArgument
         SetPropertyValue(ref _expressionText, MacroPrinter.Print(_expression, true), "ExpressionText");
      }


      private string _expressionText;
      public string ExpressionText
      {
         get { return _expressionText; }
         set 
         {
            if (SetPropertyValue(ref _expressionText, value))
            { 
               try
               {
                  ParserErrorPosition = -1;
                  ParserError = "Parsing successfull";
                  Expression = (Expression)_parser.Parse(_expressionText);
               }
               catch (ParseException e)
               {
                  ParserError = "(LINE: " + (e.Line + 1) + ", COLOUMN: " + (e.Column + 1) + ") " + e.Message;
                  ParserErrorPosition = e.Position;
               }
            }
         }
      }
      readonly MacroParser _parser = new MacroParser();

      private string _parserError;
      public string ParserError { get { return _parserError; } set { SetPropertyValue(ref _parserError, value); } }

      private int _parserErrorPosition;
      public int ParserErrorPosition { get { return _parserErrorPosition; } set { SetPropertyValue(ref _parserErrorPosition, value); } }
      
      public void EvaluateExpression()
      {
         try
         {
            EvaluatedExpressionText = "evaluating ...";
            EvaluatedExpression = new ExpressionEvaluator(new RuntimeContext(LastWindow)).Evaluate(Expression);
            EvaluatedExpressionText = EvaluatedExpression.ToString();
         }
         catch (RuntimeException e)
         {
            var sb = new StringBuilder();
            Action<Exception> errorGenerator = null;
            errorGenerator = 
               Exception =>
                  {
                     if(Exception.InnerException != null)
                        errorGenerator(Exception.InnerException);
                     sb.Append(Exception.Message);
                     var runtimeException = Exception as RuntimeException;
                     if(runtimeException != null)
                        sb.Append(" at expression >> ").Append(runtimeException.Macro.ToString()).Append(" <<");
                     sb.Append("\n");
                  };
            errorGenerator(e);
            EvaluatedExpressionText = sb.ToString();
         }
      }

      private Expression _evaluatedExpression;
      public Expression EvaluatedExpression { get { return _evaluatedExpression; } private set { SetPropertyValue(ref _evaluatedExpression, value); } }

      private string _evaluatedExpressionText;
      public string EvaluatedExpressionText { get { return _evaluatedExpressionText; } set { SetPropertyValue(ref _evaluatedExpressionText, value); } }

      public UserSimulatorModel(int ScreenshotInterval = 100)
      {
         var screenshotErrorImage =
            Application.GetResourceStream(
               new Uri("pack://application:,,,/UserSimulator;component/Resources/ErrorScreenshot.png"));
         if (screenshotErrorImage != null)
            using (var screenshotImageStream = screenshotErrorImage.Stream)
            {
               LastWindowshot = Image.FromStream(screenshotImageStream);
               screenshotErrorImage.Stream.Dispose();
            }
         UpdateWindowshot();
         _timer = new Timer(ScreenshotInterval);
         _timer.Elapsed += (Sender, Args) => UpdateWindowshot();
         _timer.Start();
         ExpressionText = "(+ 4700 11)";
         EvaluateExpression();
      }

      private void UpdateWindowshot()
      {
         if (!Keyboard.IsControlKeyDown())
            return;
         var mousePosition = Mouse.Position;
         LastWindow = Desktop.WindowHandle(mousePosition.MouseX, mousePosition.MouseY);
         LastWindowshot = Window.Capture(LastWindow);
      }

      public void Dispose()
      {
         if (_disposed)
            return;
         _disposed = true;
         Dispose(true);
      }

      protected virtual void Dispose(bool Disposing)
      {
         if (Disposing)
         {
            _timer.Dispose();
            if (_lastWindowshot != null)
               _lastWindowshot.Dispose();
         }
      }
   }
}
