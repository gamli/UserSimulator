﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace IO
{
   [ExcludeFromCodeCoverage]
   public static class Keyboard
   {
      public static bool IsControlKeyDown()
      {
         return (GetKeyState(VK_CONTROL) & KEY_PRESSED) != 0;
      }

      public static bool IsF12KeyDown()
      {
         return (GetKeyState(VK_F12) & KEY_PRESSED) != 0;
      }

      private const int KEY_PRESSED = 0x8000;
      private const int VK_CONTROL = 0x11;
      private const int VK_F12 = 0x7B;
      [DllImport("user32.dll")]
      static extern short GetKeyState(int Key);
   }
}
