using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class ProgramVM : StatementWithBodyBaseVM<Program>
   {
      [ExcludeFromCodeCoverage]
      public ProgramVM(Program Model)
         : base(Model)
      {
      }
   }
}
