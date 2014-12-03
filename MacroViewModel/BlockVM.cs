using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class BlockVM : MacroBaseVM<Block>
   {
      private TransformedCollection<MacroBase, MacroBaseVM> _itemsVM;
      public ReadOnlyObservableCollection<MacroBaseVM> ItemsVM { get; private set; }

      public BlockVM(Block Model)
         : base(Model)
      {
         _itemsVM =
            new TransformedCollection<MacroBase, MacroBaseVM>(
               Model.Items,
               Item => MacroViewModelFactory.Instance.Create(Item),
               null,
               Item => Item.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         _itemsVM.Dispose();
         base.Dispose(Disposing);
      }
   }
}
