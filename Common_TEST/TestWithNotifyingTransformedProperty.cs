using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{

   class TestWithNotifyingTransformedProperty : NotifyPropertyChangedBase, IDisposable
   {
      private bool _disposed;

      public static TestWithNotifyingTransformedProperty Create(TestNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         return new TestWithNotifyingTransformedProperty(TestNotifyPropertyChanged);
      }

      private NotifyingTransformedProperty<bool> _isYourNameHugo;
      public bool IsYourNameHugo { get { return _isYourNameHugo.Value; } }

      public TestWithNotifyingTransformedProperty(TestNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         _isYourNameHugo =
            new NotifyingTransformedProperty<bool>(
               new[] { "SomeProperty" }, "IsYourNameHugo",
               TestNotifyPropertyChanged, this,
               () => TestNotifyPropertyChanged.SomeProperty == "Hugo");
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
