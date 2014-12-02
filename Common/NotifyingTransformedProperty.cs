using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class NotifyingTransformedProperty<TTargetProperty> : TransformedProperty<TTargetProperty>
   {
      private NotifyPropertyChangedBase _targetPropertyOwner;
      public NotifyingTransformedProperty(
         string[] SourcePropertyNames, string TargetPropertyName,
         INotifyPropertyChanged SourcePropertyOwner, NotifyPropertyChangedBase TargetPropertyOwner,
         Func<TTargetProperty> Transform)
         : base(
            SourcePropertyNames, TargetPropertyName,
            SourcePropertyOwner,
            Transform)
      {
         _targetPropertyOwner = TargetPropertyOwner;
      }

      public override TTargetProperty Value
      {
         protected set
         {
            base.Value = value;
            _targetPropertyOwner.RaisePropertyChanged(_targetPropertyName);
         }
      }
   }
}
