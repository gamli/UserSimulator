using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
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

      protected bool BodyEquals(MacroWithBodyBase MacroWithBody)
      {
         return Body.Equals(MacroWithBody.Body);
      }
   }

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
