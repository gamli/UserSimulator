using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Block : StatementBase
   {
      public ObservableCollection<MacroBase> Items { get; private set; }

      public Block()
      {
         Items = new ObservableCollection<MacroBase>();
         Items.CollectionChanged += HandleItemsCollecionChanged;
      }
      private void HandleItemsCollecionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
      {
         if(Args.OldItems != null)
            foreach (MacroBase oldItem in Args.OldItems)
               oldItem.MacroChanged -= HandleItemMacroChanged;
         if(Args.NewItems != null)
            foreach (MacroBase newItem in Args.NewItems)
               newItem.MacroChanged += HandleItemMacroChanged;
         RaiseMacroChanged(this, new EventArgs());
      }
      private void HandleItemMacroChanged(object Sender, EventArgs Args)
      {
         RaiseMacroChanged(Sender, Args);
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitBlock(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherBlock = (Block)OtherMacro;
         return Items.SequenceEqual(otherBlock.Items);
      }
   }
}
