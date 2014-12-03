using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{
   public class TestNotifyPropertyChanged : NotifyPropertyChangedBase
   {
      public static TestNotifyPropertyChanged Create()
      {
         return new TestNotifyPropertyChanged { SomeProperty = "Hugo" };
      }

      private string _someProperty;
      public string SomeProperty
      {
         get
         {
            return _someProperty;
         }
         set
         {
            SetPropertyValue(ref _someProperty, value);
         }
      }

      private string _someOtherProperty;
      public string SomeOtherProperty
      {
         set
         {
            SetPropertyValue(ref _someOtherProperty, value);
         }
      }

      public void RaisePropertyChanged_TEST(string PropertyName)
      {
         RaisePropertyChanged(PropertyName);
      }
   }
}
