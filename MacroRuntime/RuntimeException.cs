using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Macro;
using MacroLanguage;

namespace MacroRuntime
{
   [ExcludeFromCodeCoverage]
   public class RuntimeException : Exception
   {
      public MacroBase Macro { get; set; }

      public IContext Context { get; set; }

      public RuntimeException(string Message, MacroBase Macro, IContext Context, Exception InnerException = null)
         : base(Message, InnerException)
      {
         this.Macro = Macro;
         this.Context = Context;
      }

      public MacroBase InnermostMacro()
      {
         var currentException = this;
         while (true)
         {
            var innerRuntimeException = currentException.InnerException as RuntimeException;
            if (innerRuntimeException != null)
               currentException = innerRuntimeException;
            else
               break;
         }
         return currentException.Macro;
      }

      public int InnermostTextLine()
      {
         return GetMacroData(InnermostMacro(), MacroParser.TEXTLINE_DATA, -1);
      }

      public int InnermostTextColumn()
      {
         return GetMacroData(InnermostMacro(), MacroParser.TEXTCOLUMN_DATA, -1);
      }

      public int InnermostTextPosition()
      {
         return GetMacroData(InnermostMacro(), MacroParser.TEXTPOSITION_DATA, -1);
      }

      public int InnermostTextLength()
      {
         return GetMacroData(InnermostMacro(), MacroParser.TEXTLINE_DATA, -1);
      }

      private T GetMacroData<T>(MacroBase Macro, string DataKey, T DefaultValue)
      {
         return (T) (Macro != null && Macro.Data.ContainsKey(DataKey) ? Macro.Data[DataKey] : DefaultValue);
      }

      public string MacroStackTrace()
      {
         var sb = new StringBuilder();
         Action<Exception> errorGenerator = null;
         errorGenerator =
            Exception =>
            {
               if (Exception.InnerException != null)
                  errorGenerator(Exception.InnerException);
               sb.Append(Exception.Message);
               var runtimeException = Exception as RuntimeException;
               if (runtimeException != null)
               {
                  var macro = runtimeException.Macro;
                  sb.Append(" at expression >> ").Append(macro != null ? macro.ToString() : "null").Append(" <<");
               }
               sb.Append("\n");
            };
         errorGenerator(this);
         return sb.ToString();
      }
   }
}
