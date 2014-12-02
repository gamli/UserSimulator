﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Common;
using IO;

namespace UserSimulator
{
   // to remove sealed, implement Disposable-Pattern properly
   sealed class ScreenshotModel : NotifyPropertyChangedBase, IDisposable
   {
      private bool _disposed;

      private Timer _timer;

      private object _lastScreenshotLocker = new object();
      private Image _lastScreenshot;
      public Image LastScreenshot 
      { 
         get 
         {
            return _lastScreenshot;
         }
         private set
         {
            if (LastScreenshot != null)
               LastScreenshot.Dispose();
            SetPropertyValue(ref _lastScreenshot, value);
         }
      }

      private IntPtr _lastScreenshotWindow;
      public IntPtr LastScreenshotWindow
      {
         get
         {
            return _lastScreenshotWindow;
         }
         set
         {
            SetPropertyValue(ref _lastScreenshotWindow, value);
         }
      }

      public ScreenshotModel(int ScreenshotInterval = 100)
      {
         UpdateScreenshot();
         _timer = new Timer(ScreenshotInterval);
         _timer.Elapsed += (Sender, Args) => UpdateScreenshot();
         _timer.Start();
      }

      private void UpdateScreenshot()
      {
         if (!Keyboard.IsControlKeyDown())
            return;
         var mousePosition = Mouse.Position;
         LastScreenshotWindow = Desktop.WindowHandle(mousePosition.X, mousePosition.Y);
         LastScreenshot = Window.Capture(LastScreenshotWindow);
      }

      public void Dispose()
      {
         if (_disposed)
            return;
         _disposed = true;
         _timer.Dispose();
         if (_lastScreenshot != null)
            _lastScreenshot.Dispose();
      }
   }
}
