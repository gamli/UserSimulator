using System;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows;
using Common;
using IO;
using MacroLanguage;
using MacroRuntime;
using Expression = Macro.Expression;
using Window = IO.Window;

namespace UserSimulator
{
   class UserSimulatorModel : NotifyPropertyChangedBase, IDisposable
   {
      public UserSimulatorModel()
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
         _timer = new Timer(1000);
         _timer.Elapsed += (Sender, Args) => UpdateWindowshot();
         _timer.Start();

         REPL = new REPL(true);
      }

      private bool _disposed;
      private Timer _timer;
      private Image _lastWindowshot;
      private IntPtr _lastWindowHandle;
      private string _lastWindowTitle;
      private string _expressionText;

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

      public IntPtr LastWindowHandle { get { return _lastWindowHandle; } set { SetPropertyValue(ref _lastWindowHandle, value); } }

      public string LastWindowTitle { get { return _lastWindowTitle; } set { SetPropertyValue(ref _lastWindowTitle, value); } }


      #region Windowshot update

      private void UpdateWindowshot()
      {
         if (Keyboard.IsControlKeyDown())
         {
            var mousePosition = Mouse.Position;
            LastWindowHandle = Desktop.WindowHandle(mousePosition.MouseX, mousePosition.MouseY);
            if (IsLastWindowHandleValid())
               LastWindowTitle = Window.Text(LastWindowHandle);
         }
         if (IsLastWindowHandleValid())
            LastWindowshot = Window.Capture(LastWindowHandle);
      }

      private bool IsLastWindowHandleValid()
      {
         return LastWindowHandle != IntPtr.Zero;
      }

      #endregion

      #region REPL + Expression

      public string ExpressionText
      {
         get { return _expressionText; }
         set
         {
            if (SetPropertyValue(ref _expressionText, value))
               ParsePreview();
         }
      }

      public REPL REPL { get; private set; }

      public void Format(string ExprTxt)
      {
         ExpressionText = ExprTxt;

         ParsePreview();

         if (REPL.LastParsedExpression != null)
            ExpressionText = MacroPrinter.Print(REPL.LastParsedExpression, true);
      }

      public void ParsePreview()
      {
         REPL.ParsePreview(ExpressionText);
      }

      #endregion

      #region IDisposable implementation

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
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
            LastWindowshot = null;
         }
      }

      #endregion
   }
}
