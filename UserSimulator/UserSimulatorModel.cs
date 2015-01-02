﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Common;
using IO;
using Macro;
using MacroLanguage;

namespace UserSimulator
{
   class UserSimulatorModel : NotifyPropertyChangedBase, IDisposable
   {
      private bool _disposed;

      private Timer _timer;

      private object _lastScreenshotLocker = new object();
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

      private Program _program;
      public Program Program
      {
         get { return _program; }
         set
         {
            var oldProgram = _program;
            if (SetPropertyValue(ref _program, value))
            {
               if (oldProgram != null)
                  oldProgram.MacroChanged -= HandleProgramChanged;
               if (value != null)
                  value.MacroChanged += HandleProgramChanged;
               HandleProgramChanged();
            }
         }
      }
      private void HandleProgramChanged(object Sender, EventArgs Args)
      {
         HandleProgramChanged();
      }
      private void HandleProgramChanged()
      {
         SetPropertyValue(ref _programText, new MacroPrinter(_program).Print(), "ProgramText");
      }


      private string _programText;
      public string ProgramText
      {
         get { return _programText; }
         set 
         {
            if (SetPropertyValue(ref _programText, value))
            { 
               try
               {
                  Program = _parser.Parse(_programText);
                  ParserError = "Parsing successfull";
               }
               catch (ParseException E)
               {
                  ParserError = "(LINE: " + E.LineNumber + ") " + E.Message;
               }
            }
         }
      }
      ProgramParser _parser = new ProgramParser();

      private string _parserError;
      public string ParserError { get { return _parserError; } set { SetPropertyValue(ref _parserError, value); } }


      public UserSimulatorModel(int ScreenshotInterval = 100)
      {
         LastWindowshot = 
            Bitmap.FromStream(Application.GetResourceStream(new Uri("pack://application:,,,/UserSimulator;component/Resources/ErrorScreenshot.png")).Stream);
         Program = new Program { Body = new Block() };
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
         LastWindow = Desktop.WindowHandle(mousePosition.X, mousePosition.Y);
         LastWindowshot = IO.Window.Capture(LastWindow);
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