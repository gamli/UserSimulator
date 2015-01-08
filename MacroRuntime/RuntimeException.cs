using System;
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
   }
}
