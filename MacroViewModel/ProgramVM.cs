using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class ProgramVM : MacroBaseVM<Program>
   {
      private NotifyingTransformedProperty<BlockVM> _blockVM;
      public BlockVM BlockVM
      {
         get
         {
            return _blockVM.Value;
         }
      }

      public ProgramVM(Program Model)
         : base(Model)
      {
         _blockVM =
            new NotifyingTransformedProperty<BlockVM>(
               new[] { "Block" }, "BlockVM",
               Model, this,
               () => new BlockVM(Model.Block),
               VM => VM.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         _blockVM.Dispose();
         base.Dispose(Disposing);
      }
   }
}
