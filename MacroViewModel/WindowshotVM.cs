using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Macro;

namespace MacroViewModel
{
   [ExcludeFromCodeCoverage]
   public class WindowshotVM : MacroWithBodyBaseVM<Windowshot>
   {
      public WindowshotVM(Windowshot Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
