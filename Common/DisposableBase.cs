using System;

namespace Common
{
   public abstract class DisposableBase : IDisposable
   {
      private bool _disposed;

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
