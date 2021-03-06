﻿using System;
using Common;

namespace Common_TEST
{
   public sealed class MockWithTransformedProperty : IDisposable
   {
      public static MockWithTransformedProperty Create(MockNotifyPropertyChanged TestNotifyPropertyChanged, Action<bool> Release)
      {
         return new MockWithTransformedProperty(TestNotifyPropertyChanged, Release);
      }

      private readonly TransformedProperty<bool> _isYourNameHugo;
      public bool IsYourNameHugo { get { return _isYourNameHugo.Value; } }

      public MockWithTransformedProperty(MockNotifyPropertyChanged TestNotifyPropertyChanged, Action<bool> Release)
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
