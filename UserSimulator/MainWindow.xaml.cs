using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IO;
using Macro;

namespace UserSimulator
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : System.Windows.Window
   {
      private ScreenshotModel _model;
      private ScreenshotViewModel _viewModel;

      public string ProgramText { get; private set; }

      public MainWindow()
      {
         InitializeComponent();
         _model = new ScreenshotModel();
         _viewModel = new ScreenshotViewModel(_model);
         DataContext = _viewModel;

         var parser = new ProgramParser();
         var program = parser.Parse("PROGRAM{LEFT_CLICK;}");
         var printer = new ProgramPrinter(program);
         var printedProgram = printer.Print();
         Console.WriteLine(printedProgram);
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         var macro = new Program();
         using (var image = new Bitmap(16, 16))
         {
            using (var graphics = Graphics.FromImage(image))
            {
               const int imagePositionX = 256 * 3 + 75;
               const int imagePositionY = 256 + 275;
               graphics.DrawImage(
                  _model.LastScreenshot,
                  0, 0,
                  new System.Drawing.Rectangle(imagePositionX, imagePositionY, image.Width, image.Height),
                  GraphicsUnit.Pixel
                  );
               var conditionalBody = new Block();
               conditionalBody.Items.Add(new Position { X = imagePositionX + 200, Y = imagePositionY + 230 });
               conditionalBody.Items.Add(new Pause { Duration = TimeSpan.FromSeconds(.5) });
               conditionalBody.Items.Add(new LeftClick());
               conditionalBody.Items.Add(new Pause { Duration = TimeSpan.FromSeconds(.5) });
               conditionalBody.Items.Add(new Position { X = Mouse.X, Y = Mouse.Y });
               var conditional =
                  new ImageEqualsWindowContent
                     {
                        Body = conditionalBody,
                        Image = image,
                        PositionX = imagePositionX,
                        PositionY = imagePositionY,
                        Window = _model.LastScreenshotWindow
                     };
               var loopBody = new Block();
               loopBody.Items.Add(conditional);
               loopBody.Items.Add(new Pause { Duration = TimeSpan.FromSeconds(1) });
               var action = new ForLoop { RepetitionCount = 5, Body = loopBody };
               macro.Block.Items.Add(action);

               using(var overlayImage = new Bitmap(_model.LastScreenshot))
               {
                  using(var overlayGraphics = Graphics.FromImage(overlayImage))
                  {
                     overlayGraphics.FillRectangle(System.Drawing.Brushes.LimeGreen, imagePositionX, imagePositionY, image.Width, image.Height);
                  }
                  var stream = new MemoryStream();
                  overlayImage.Save(stream, ImageFormat.Bmp);
                  stream.Position = 0;
                  var overlayImageSource = new BitmapImage();
                  overlayImageSource.BeginInit();
                  overlayImageSource.StreamSource = stream;
                  overlayImageSource.CacheOption = BitmapCacheOption.OnLoad;
                  overlayImageSource.EndInit();
                  stream.Position = 0;
                  _viewModel.LastScreenshotOverlay = overlayImageSource;
               }
            }
            var macroExecutor = new ProgramInterpreter(macro, _model.LastScreenshotWindow);
            macroExecutor.Execute();
         }
      }

      /*private Action Macro(IEnumerable<Action> Actions)
      {
         var blubber = Actions.ToList();
         Console.WriteLine(blubber);
         return MouseActions.Macro(Actions);
      }

      private IEnumerable<Action> Spiral()
      {
         var screenWidth = (int)(System.Windows.SystemParameters.PrimaryScreenWidth);
         var halfScreenWidth = screenWidth / 2;
         var x = halfScreenWidth;
         var screenHeight = (int)(System.Windows.SystemParameters.PrimaryScreenHeight);
         var halfScreenHeight = screenHeight / 2;
         var y = halfScreenHeight;
         var maxRadius = Math.Min(x, y);

         var radius = .0;
         var angle = 0;
         while (x >= 0 && x < screenWidth && y >= 0 && y < screenWidth)
         {
            yield return MouseActions.Move(x, y);
            yield return MouseActions.Pause(10);

            radius += .1;
            angle = (angle + 1) % 360;
            var angleRad = Deg2Rad(angle);
            x = halfScreenWidth + (int)(radius * Math.Cos(angleRad));
            y = halfScreenHeight + (int)(radius * Math.Sin(angleRad));
         }
      }

      private IEnumerable<Action> Wave()
      {
         var screenWidth = (int)(System.Windows.SystemParameters.PrimaryScreenWidth);
         var halfScreenWidth = screenWidth / 2;
         var x = 0;
         var screenHeight = (int)(System.Windows.SystemParameters.PrimaryScreenHeight);
         var halfScreenHeight = screenHeight / 2;
         var y = halfScreenHeight;

         var radius = y / 4;
         var angle = 0;
         while (x >= 0 && x < screenWidth)
         {
            yield return MouseActions.Move(x, y);
            yield return MouseActions.Pause(10);

            angle = (angle + 1) % 360;
            var angleRad = Deg2Rad(angle);
            x++;
            y = halfScreenHeight + (int)(radius * Math.Sin(angleRad));
         }
      }

      private double Deg2Rad(int Deg)
      {
         return Math.PI * Deg / 180.0;
      }*/
   }
}
