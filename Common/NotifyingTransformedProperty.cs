using System;
using System.ComponentModel;

namespace Common
{
   public class NotifyingTransformedProperty<TTargetProperty> : TransformedProperty<TTargetProperty>
   {
      private readonly NotifyPropertyChangedBase _targetPropertyOwner;
      public NotifyingTransformedProperty(
         string[] SourcePropertyNames, string TargetPropertyName,
         INotifyPropertyChanged SourcePropertyOwner, NotifyPropertyChangedBase TargetPropertyOwner,
         Func<TTargetProperty> Transform,
         Action<TTargetProperty> Release = null)
         : base(
            SourcePropertyNames, TargetPropertyName,
            SourcePropertyOwner,
            Transform,
            Release)
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
