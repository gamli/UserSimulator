﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

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

      private readonly HashSet<string> _sourcePropertyNames;
      protected string _targetPropertyName;
      private readonly INotifyPropertyChanged _sourcePropertyOwner;
      private readonly Func<T> _transform;
      private readonly Action<T> _release;
      private void SourcePropertyOwnerPropertyChanged(object Sender, PropertyChangedEventArgs Args)
      {
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
            ReleaseValueIfItCanBeReleased();
            _value = value;
         }
      }

      private void ReleaseValueIfItCanBeReleased()
      {
         if (CanValueBeReleased())
            _release(_value);
      }

      [DebuggerNonUserCode]
      private bool CanValueBeReleased()
      {
         return _value != null && _release != null;
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
         {
            _sourcePropertyOwner.PropertyChanged -= SourcePropertyOwnerPropertyChanged;
            ReleaseValueIfItCanBeReleased();
         }
      }
   }
}
