using System;
using System.Drawing;
using System.Timers;
using System.Windows;
using Common;
using IO;
using Macro;
using MacroLanguage;
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

      private ExpressionBase _expression;
      public ExpressionBase Expression
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
                  Expression = (ExpressionBase)_parser.Parse(_expressionText);
                  ParserError = "Parsing successfull";
               }
               catch (ParseException e)
               {
                  ParserError = "(LINE: " + e.LineNumber + ") " + e.Message;
               }
            }
         }
      }

      readonly MacroParser _parser = new MacroParser();

      private string _parserError;
      public string ParserError { get { return _parserError; } set { SetPropertyValue(ref _parserError, value); } }


      public UserSimulatorModel(int ScreenshotInterval = 100)
      {
         var screenshotErrorImage =
            Application.GetResourceStream(
               new Uri("pack://application:,,,/UserSimulator;component/Resources/ErrorScreenshot.png"));
         if (screenshotErrorImage != null)
            LastWindowshot =
               Image.FromStream(screenshotErrorImage.Stream);
         var initialFunction = new ProcedureCall { Procedure = new Symbol("print") };
         initialFunction.Expressions.Add(new Constant("Hello World"));
         Expression = initialFunction;         
         UpdateWindowshot();
         _timer = new Timer(ScreenshotInterval);
         _timer.Elapsed += (Sender, Args) => UpdateWindowshot();
         _timer.Start();
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
