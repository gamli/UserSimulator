﻿using System;
using System.Text;
using Macro;

namespace MacroRuntime
{
   public class RuntimeException : Exception
   {
      public MacroBase Macro { get; set; }

      public ContextBase Context { get; set; }

      public RuntimeException(string Message, MacroBase Macro, ContextBase Context, Exception InnerException = null)
         : base(Message, InnerException)
      {
         this.Macro = Macro;
         this.Context = Context;
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
