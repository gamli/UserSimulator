using System.Drawing;

namespace Common
{
   public static class Geometry
   {
      public static Rectangle NormalizedRectangle(int X, int Y, int Width, int Height)// TODO duplicated code for x and y
      {
         if (Width < 0)
         {
            X = X + Width;
            Width = -Width;
         }
       
         if (Height < 0) 
         {
            Y = Y + Height;
            Height = -Height;
         }

         return new Rectangle(X, Y, Width, Height);
      }
   }
}
