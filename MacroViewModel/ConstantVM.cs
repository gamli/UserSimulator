﻿using System.Diagnostics.CodeAnalysis;
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
