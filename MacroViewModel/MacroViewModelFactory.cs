using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Macro;

namespace MacroViewModel
{
   public class MacroViewModelFactory
   {
      private static readonly MacroViewModelFactory INSTANCE = new MacroViewModelFactory();
      public static MacroViewModelFactory Instance
      {
         get
         {
            return INSTANCE;
         }
      }
      
      public MacroBaseVM Create(MacroBase Macro)
      {
         if (Macro == null)
            return null;
         var macroType = Macro.GetType();
         Contract.Assert(_supportedTypes.ContainsKey(macroType));
         return _supportedTypes[macroType](Macro);
      }

      private readonly Dictionary<Type, Func<MacroBase, MacroBaseVM>> _supportedTypes;

      private MacroViewModelFactory()
      {
         _supportedTypes =
            new Dictionary<Type, Func<MacroBase, MacroBaseVM>>
            {
               { typeof(Constant), Macro => 
                  new ConstantVM((Constant)Macro) },

               { typeof(Definition), Macro => 
                  new DefinitionVM((Definition)Macro) },

               { typeof(If), Macro => 
                  new IfVM((If)Macro) },

               { typeof(Lambda), Macro => 
                  new LambdaVM((Lambda)Macro) },

               { typeof(Loop), Macro => 
                  new LoopVM((Loop)Macro) },

               { typeof(ProcedureCall), Macro => 
                  new ProcedureCallVM((ProcedureCall)Macro) },

               { typeof(Quote), Macro => 
                  new QuoteVM((Quote)Macro) },

               { typeof(Symbol), Macro => 
                  new SymbolVM((Symbol)Macro) },

               { typeof(SymbolList), Macro => 
                  new SymbolListVM((SymbolList)Macro) },
            };
      }
   }
}
