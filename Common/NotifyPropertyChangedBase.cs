using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
