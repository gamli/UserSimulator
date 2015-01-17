using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common;

namespace Macro
{
   public abstract class MacroBase : NotifyPropertyChangedBase
   {
      protected MacroBase()
      {
         Data = new Dictionary<string, object>();
      }

      public Dictionary<string, object> Data { get; private set; }

      public event EventHandler MacroChanged;
      protected void OnMacroChanged()
      {
         var handler = MacroChanged;
         if (handler != null)
            handler(this, new EventArgs());
      }

      public override bool SetPropertyValue<TProperty>(ref TProperty BackingField, TProperty Value, [CallerMemberName]string PropertyName = null)
      {
         // ReSharper disable once ExplicitCallerInfoArgument we are not the caller of the method - our caller is
         var valueChanged = base.SetPropertyValue(ref BackingField, Value, PropertyName);
       
         if (valueChanged)
            OnMacroChanged();

         return valueChanged;
      }

      public abstract void Accept(IVisitor Visitor);
      public override bool Equals(object Other)
      {
         if (Other == this) // TODO necessary?
            return true;
         if (Other == null || Other.GetType() != GetType()) // TODO null check necessary?
            return false;
         return MacroEquals((MacroBase)Other);
      }
      protected abstract bool MacroEquals(MacroBase OtherMacro);

      public override int GetHashCode()
      {
         return MacroGetHashCode();
      }

      protected abstract int MacroGetHashCode();
   }
}
