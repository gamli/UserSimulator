﻿using Common;

namespace Common_TEST
{
   public class MockNotifyPropertyChanged : NotifyPropertyChangedBase
   {
      public static MockNotifyPropertyChanged Create()
      {
         return new MockNotifyPropertyChanged { SomeProperty = "Hugo" };
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
