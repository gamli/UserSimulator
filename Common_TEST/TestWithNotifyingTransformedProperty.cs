using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{

   sealed class TestWithNotifyingTransformedProperty : NotifyPropertyChangedBase
   {
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
   }
}
