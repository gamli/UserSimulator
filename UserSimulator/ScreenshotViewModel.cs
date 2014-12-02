using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using IO;

namespace UserSimulator
{
   class ScreenshotViewModel : ViewModelBase<ScreenshotModel>
   {
      private ImageSource _lastScreenshot;
      public ImageSource LastScreenshot
      {
         get
         {
            return _lastScreenshot;
         }
         private set
         {
            SetPropertyValue(ref _lastScreenshot, value);
         }
      }

      private string _lastScreenshotWindow;
      public string LastScreenshotWindow
      {
         get
         {
            return _lastScreenshotWindow;
         }
         private set
         {
            SetPropertyValue(ref _lastScreenshotWindow, value);
         }
      }

      private ImageSource _lastScreenshotOverlay;
      public ImageSource LastScreenshotOverlay
      {
         get
         {
            return _lastScreenshotOverlay;
         }
         set
         {
            SetPropertyValue(ref _lastScreenshotOverlay, value);
         }
      }

      public ScreenshotViewModel(ScreenshotModel Model)
         : base(Model)
      {
         Model.PropertyChanged +=
            (Sender, Args) =>
            {
               switch(Args.PropertyName)
               {
                  case "LastScreenshot":
                     System.Windows.Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                           try
                           {
                              var lastScreenshot = Model.LastScreenshot;
                              if (lastScreenshot != null)
                              {
                                 var stream = new MemoryStream();
                                 Model.LastScreenshot.Save(stream, ImageFormat.Bmp);
                                 stream.Position = 0;
                                 var imageSource = new BitmapImage();
                                 imageSource.BeginInit();
                                 imageSource.StreamSource = stream;
                                 imageSource.CacheOption = BitmapCacheOption.OnLoad;
                                 imageSource.EndInit();
                                 stream.Position = 0;
                                 LastScreenshot = imageSource;
                              }
                           }
                           catch (Exception)
                           {
                              // do nothing and keep old screenshot
                           }
                        });
                     break;
                  case "LastScreenshotWindow":
                     LastScreenshotWindow = Window.Text(Model.LastScreenshotWindow);
                     break;
               }
            };
      }
   }
}
