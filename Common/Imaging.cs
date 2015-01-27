using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Common
{
   [ExcludeFromCodeCoverage]
   public static class Imaging
   {
      public static string Image2PngHexString(Image Image)
      {
         return Image2HexString(Image, ImageFormat.Png);
      }

      public static string Image2HexString(Image Image, ImageFormat ImageFormat)
      {
         using (var memStream = new MemoryStream())
         {
            Image.Save(memStream, ImageFormat);
            var binaryValue = memStream.ToArray();
            return BitConverter.ToString(binaryValue).Replace("-", String.Empty).ToUpper();
         }
      }

      public static Image HexString2Image(string HexValue)
      {
         return Image.FromStream(HexString2Stream(HexValue));
      }

      public static MemoryStream HexString2Stream(string HexValue)
      {
         var binaryValue = new byte[HexValue.Length / 2];
         for (var i = 0; i < HexValue.Length; i += 2)
            binaryValue[i / 2] = byte.Parse(HexValue.Substring(i, 2), NumberStyles.AllowHexSpecifier);

         var stream = new MemoryStream();
         stream.Write(binaryValue, 0, binaryValue.Length);
         stream.Position = 0;
         return stream;
      }

      public static Bitmap CropImage(Image Image, int X, int Y, int Width, int Height)
      {
         var croppedImage = new Bitmap(Width, Height);
         using (var imageGraphics = Graphics.FromImage(croppedImage))
         {
            imageGraphics.DrawImage(
               Image,
               0, 0,
               new Rectangle(X, Y, Width, Height),
               GraphicsUnit.Pixel
               );
         }
         return croppedImage;
      }

      public static object ReadText(Image Image)
      {
         using (var bitmap = ToBitmap(Image))
         {
            using (var engine = new TesseractEngine("./Tessdata", "eng"))
            {
               using (var pix = PixConverter.ToPix(bitmap))
               {
                  using (var page = engine.Process(pix))
                  {
                     var text = page.GetText();
                     decimal textDecimal;
                     if (decimal.TryParse(text, out textDecimal))
                        return textDecimal;
                     return text;
                  }
               }
            }
         }
      }

      private static Bitmap ToBitmap(Image Image)
      {
         return ((Bitmap) Image).Clone(new Rectangle(0, 0, Image.Width, Image.Height), PixelFormat.Format24bppRgb);
      }
   }
}
