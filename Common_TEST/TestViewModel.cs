using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Common_TEST
{
   public class TestViewModel : ViewModelBase<TestNotifyPropertyChanged>
   {
      public TestViewModel(TestNotifyPropertyChanged Model)
         : base(Model)
      {
         // nothing to do
      }

      public static TestViewModel Create(TestNotifyPropertyChanged Model)
      {
         return new TestViewModel(Model);
      }
   }
}
