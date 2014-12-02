using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Macro
{
   public abstract class WithBodyBase : Base
   {
      private Base _body;
      public Base Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }
   }
   
   public abstract class Base : NotifyPropertyChangedBase
   {
      public abstract void Accept(IVisitor Visitor);
   }
}
