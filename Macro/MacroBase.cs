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
      protected void RaiseMacroChanged(object Sender, EventArgs Args)
      {
         var handler = MacroChanged;
         if (handler != null)
            handler(Sender, Args);
      }

      public override bool SetPropertyValue<TProperty>(ref TProperty BackingField, TProperty Value, [CallerMemberName]string PropertyName = null)
      {
         var oldValue = BackingField;
         // ReSharper disable once ExplicitCallerInfoArgument we are not the caller of the method - our caller is
         var valueChanged = base.SetPropertyValue(ref BackingField, Value, PropertyName);
         if (valueChanged && typeof(MacroBase).IsAssignableFrom(typeof(TProperty)))
         {
            if (oldValue != null)
               ((MacroBase)((object)oldValue)).MacroChanged -= RaiseMacroChanged;
            if (Value != null)
               ((MacroBase)((object)Value)).MacroChanged += RaiseMacroChanged;
         }
         return valueChanged;
      }

      protected MacroBase()
      {
         PropertyChanged += HandlePropertyChanged;
      }
      private void HandlePropertyChanged(object Sender, PropertyChangedEventArgs Args)
      {
         RaiseMacroChanged(this, Args);
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
