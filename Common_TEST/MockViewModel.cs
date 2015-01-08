using Common;

namespace Common_TEST
{
   public class MockViewModel : ViewModelBase<MockNotifyPropertyChanged>
   {
      public MockViewModel(MockNotifyPropertyChanged Model)
         : base(Model)
      {
         // nothing to do
      }

      public static MockViewModel Create(MockNotifyPropertyChanged Model)
      {
         return new MockViewModel(Model);
      }
   }
}
