using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Common;

namespace Macro
{
   public abstract class MacroBase : NotifyPropertyChangedBase
   {
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
         if (Other == this)
            return true;
         if (Other == null || Other.GetType() != GetType())
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
