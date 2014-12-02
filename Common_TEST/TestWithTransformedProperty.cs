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
      public static TestWithTransformedProperty Create(TestNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         return new TestWithTransformedProperty(TestNotifyPropertyChanged);
      }

      private TransformedProperty<bool> _isYourNameHugo;
      public bool IsYourNameHugo { get { return _isYourNameHugo.Value; } }

      public TestWithTransformedProperty(TestNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         _isYourNameHugo =
            new TransformedProperty<bool>(
               new[] { "SomeProperty" }, "IsYourNameHugo",
               TestNotifyPropertyChanged,
               () => TestNotifyPropertyChanged.SomeProperty == "Hugo");
      }

      public void Dispose()
      {
         _isYourNameHugo.Dispose();
      }
   }
}
