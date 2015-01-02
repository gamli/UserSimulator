using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      public void RaisePropertyChanged(string PropertyName)
      {
         var handler = PropertyChanged;
         if (handler != null)
            handler(this, new PropertyChangedEventArgs(PropertyName));
      }

      public virtual bool SetPropertyValue<TProperty>(ref TProperty BackingField, TProperty Value, [CallerMemberName]string PropertyName = null)
      {
         Contract.Assert(PropertyName != null);
         if (!Equals(BackingField, Value))
         {
            BackingField = Value;
            RaisePropertyChanged(PropertyName);
            return true;
         }
         return false;
      }
   }
}
