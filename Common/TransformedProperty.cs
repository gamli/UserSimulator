using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class TransformedProperty<T> : DisposableBase
   {
      public TransformedProperty(
         string[] SourcePropertyNames, string TargetPropertyName,
         INotifyPropertyChanged SourcePropertyOwner,
         Func<T> Transform, 
         Action<T> Release = null)
      {
         _sourcePropertyNames = new HashSet<string>(SourcePropertyNames);
         _targetPropertyName = TargetPropertyName;
         _sourcePropertyOwner = SourcePropertyOwner;
         _transform = Transform;
         _release = Release;

         _sourcePropertyOwner.PropertyChanged += SourcePropertyOwnerPropertyChanged;
         _value = SourceValue(); // don't use the property here to avoid virtual setter before subclass is initialized
      }
      protected T SourceValue()
      {
         return _transform();
      }

      private HashSet<string> _sourcePropertyNames;
      protected string _targetPropertyName;
      private readonly INotifyPropertyChanged _sourcePropertyOwner;
      private readonly Func<T> _transform;
      private readonly Action<T> _release;
      private void SourcePropertyOwnerPropertyChanged(object Sender, PropertyChangedEventArgs Args)
      {
         Contract.Equals(Sender, _sourcePropertyOwner);
         if (!_sourcePropertyNames.Contains(Args.PropertyName))
            return;
         UpdateValueFromSource();
      }
      protected void UpdateValueFromSource()
      {
         Value = SourceValue();
      }

      protected T _value;
      public virtual T Value
      {
         get
         {
            return _value;
         }
         protected set
         {
            if (_value != null && _release != null)
               _release(_value);
            _value = value;
         }
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
            _sourcePropertyOwner.PropertyChanged -= SourcePropertyOwnerPropertyChanged;
      }
   }
}
