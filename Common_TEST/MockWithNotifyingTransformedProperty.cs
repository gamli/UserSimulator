using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{

   sealed class MockWithNotifyingTransformedProperty : NotifyPropertyChangedBase
   {
      public static MockWithNotifyingTransformedProperty Create(MockNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         return new MockWithNotifyingTransformedProperty(TestNotifyPropertyChanged);
      }

      private NotifyingTransformedProperty<bool> _isYourNameHugo;
      public bool IsYourNameHugo { get { return _isYourNameHugo.Value; } }

      public MockWithNotifyingTransformedProperty(MockNotifyPropertyChanged TestNotifyPropertyChanged)
      {
         _isYourNameHugo =
            new NotifyingTransformedProperty<bool>(
               new[] { "SomeProperty" }, "IsYourNameHugo",
               TestNotifyPropertyChanged, this,
               () => TestNotifyPropertyChanged.SomeProperty == "Hugo");
      }
   }
}
