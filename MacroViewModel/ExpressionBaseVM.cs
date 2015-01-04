using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public abstract class ExpressionBaseVM : MacroBaseVM<ExpressionBase>
   {
      [ExcludeFromCodeCoverage]
      protected ExpressionBaseVM(ExpressionBase Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
