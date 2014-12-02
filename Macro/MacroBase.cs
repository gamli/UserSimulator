using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Macro
{
   public abstract class MacroWithBodyBase : MacroBase
   {
      private MacroBase _body;
      public MacroBase Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }
   }
   
   public abstract class MacroBase : NotifyPropertyChangedBase
   {
      public abstract void Accept(IVisitor Visitor);
   }
}
