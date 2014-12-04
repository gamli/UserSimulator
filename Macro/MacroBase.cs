using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Macro
{
   public abstract class MacroWithBodyBase : MacroBase
   {
      private MacroBase _body;
      [ExcludeFromCodeCoverage]
      public MacroBase Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }
   }
   
   public abstract class MacroBase : NotifyPropertyChangedBase
   {
      public abstract void Accept(IVisitor Visitor);
      public override bool Equals(object Other)
      {
         if (Other == null || Other.GetType() != GetType())
            return false;
         return MacroEquals((MacroBase)Other);
      }
      protected abstract bool MacroEquals(MacroBase OtherMacro);

      [ExcludeFromCodeCoverage]
      public override int GetHashCode()
      {
         return MacroGetHashCode();
      }
      [ExcludeFromCodeCoverage]
      protected virtual int MacroGetHashCode()
      {
         return 0; // TODO a correct implementation (to use macros in dictionaries implement properly)
      }
   }
}
