using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class MacroViewModelFactory
   {
      private static MacroViewModelFactory _instance = new MacroViewModelFactory();
      public static MacroViewModelFactory Instance
      {
         get
         {
            return _instance;
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
               { typeof(Block), Macro => 
                  new BlockVM((Block)Macro) },

               { typeof(LeftClick), Macro => 
                  new LeftClickVM((LeftClick)Macro) },

               { typeof(Loop), Macro => 
                  new ForLoopVM((Loop)Macro) },

               { typeof(Windowshot), Macro => 
                  new WindowshotVM((Windowshot)Macro) },

               { typeof(Move), Macro => 
                  new MoveVM((Move)Macro) },

               { typeof(Pause), Macro => 
                  new PauseVM((Pause)Macro) },

               { typeof(Position), Macro => 
                  new PositionVM((Position)Macro) },

               { typeof(Constant), Macro => 
                  new ConstantVM((Constant)Macro) },

               { typeof(If), Macro => 
                  new IfVM((If)Macro) },

               { typeof(Definition), Macro => 
                  new VariableAssignmentVM((Definition)Macro) },

               { typeof(Program), Macro => 
                  new ProgramVM((Program)Macro) },
            };
      }
   }
}
