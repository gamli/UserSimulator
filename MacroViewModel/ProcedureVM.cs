using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;
using MacroRuntime;

namespace MacroViewModel
{
   public class ProcedureVM : ExpressionVM
   {
      public ProcedureVM(ProcedureBase Model) 
         : base(Model)
      {
      }
   }
}
