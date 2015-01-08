using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Common;

namespace MacroRuntime
{
   [ExcludeFromCodeCoverage]
   public class Logger : NotifyPropertyChangedBase
   {
      private static Logger _instance;
      public static Logger Instance { get { return _instance ?? (_instance = new Logger()); }
      }
      private Logger()
      {
         Messages = new ObservableCollection<string>();
      }

      public ObservableCollection<string> Messages { get; private set; }

      private string _lastMessage;
      public string LastMessage { get { return _lastMessage; } set { SetPropertyValue(ref _lastMessage, value); } }

      public void Log(string Message)
      {
         Messages.Add(Message);
         LastMessage = Message;
      }
   }
}
