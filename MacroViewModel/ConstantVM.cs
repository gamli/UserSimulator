using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Macro;

namespace MacroViewModel
{
   public class ConstantVM : ExpressionBaseVM
   {
      [ExcludeFromCodeCoverage]
      public ConstantVM(Constant Model)
         : base(Model)
      {
         // nothing to do
      }
   }
}
