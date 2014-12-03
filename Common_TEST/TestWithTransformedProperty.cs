using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{
   public sealed class TestWithTransformedProperty : IDisposable
   {
      public static TestWithTransformedProperty Create(TestNotifyPropertyChanged TestNotifyPropertyChanged, Action<bool> Release)
      {
         return new TestWithTransformedProperty(TestNotifyPropertyChanged, Release);
      }

      private TransformedProperty<bool> _isYourNameHugo;
      public bool IsYourNameHugo { get { return _isYourNameHugo.Value; } }

      public TestWithTransformedProperty(TestNotifyPropertyChanged TestNotifyPropertyChanged, Action<bool> Release)
      {
         _isYourNameHugo =
            new TransformedProperty<bool>(
               new[] { "SomeProperty" }, "IsYourNameHugo",
               TestNotifyPropertyChanged,
               () => TestNotifyPropertyChanged.SomeProperty == "Hugo",
               Release);
      }

      public void Dispose()
      {
         _isYourNameHugo.Dispose();
      }
   }
}
