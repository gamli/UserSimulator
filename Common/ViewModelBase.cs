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
   public abstract class ViewModelBase<T> : NotifyPropertyChangedBase, IDisposable
      where T : NotifyPropertyChangedBase
   {
      private bool _disposed;

      public T Model { get; private set; }

      public ViewModelBase(T Model)
      {
         this.Model = Model;
      }

      public void Dispose()
      {
         if (_disposed)
            return;
         Dispose(true);
         _disposed = true;
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool Disposing)
      {
         // nothing to do
      }
   }
}
