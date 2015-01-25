using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public static class Imaging
   {
      [ExcludeFromCodeCoverage]
      public static string Image2PngHexString(Image Image)
      {
         return Image2HexString(Image, ImageFormat.Png);
      }

      [ExcludeFromCodeCoverage]
      public static string Image2HexString(Image Image, ImageFormat ImageFormat)
      {
         using (var memStream = new MemoryStream())
         {
            Image.Save(memStream, ImageFormat);
            var binaryValue = memStream.ToArray();
            return BitConverter.ToString(binaryValue).Replace("-", string.Empty).ToUpper();
         }
      }
   }
}
