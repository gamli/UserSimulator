﻿using System.Diagnostics.CodeAnalysis;
using Macro;

namespace MacroViewModel
{
   public class SymbolVM : ExpressionVM
   {
      [ExcludeFromCodeCoverage]
      public SymbolVM(Symbol Model)
         : base (Model)
      {
      }
   }
}
