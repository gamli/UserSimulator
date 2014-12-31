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

               { typeof(ForLoop), Macro => 
                  new ForLoopVM((ForLoop)Macro) },

               { typeof(Windowshot), Macro => 
                  new WindowshotVM((Windowshot)Macro) },

               { typeof(Move), Macro => 
                  new MoveVM((Move)Macro) },

               { typeof(NoOp), Macro => 
                  new NoOpVM((NoOp)Macro) },

               { typeof(Pause), Macro => 
                  new PauseVM((Pause)Macro) },

               { typeof(Position), Macro => 
                  new PositionVM((Position)Macro) },

               { typeof(ConstantExpression<bool>), Macro => 
                  new ConstantBooleanExpressionVM((ConstantExpression<bool>)Macro) },

               { typeof(ConstantExpression<string>), Macro => 
                  new ConstantStringExpressionVM((ConstantExpression<string>)Macro) },

               { typeof(ConstantExpression<int>), Macro => 
                  new ConstantIntegerExpressionVM((ConstantExpression<int>)Macro) },

               { typeof(IfStatement), Macro => 
                  new IfStatementVM((IfStatement)Macro) },

               { typeof(Program), Macro => 
                  new ProgramVM((Program)Macro) },
            };
      }
   }
}
