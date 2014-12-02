using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class NoOpVM : ViewModelBase<NoOp>
   {
      public NoOpVM(NoOp Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
