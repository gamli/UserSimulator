using System;

namespace Common
{
   public abstract class ViewModelBase<T> : NotifyPropertyChangedBase, IDisposable
      where T : NotifyPropertyChangedBase
   {
      private bool _disposed;

      public T Model { get; private set; }

      protected ViewModelBase(T Model)
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
