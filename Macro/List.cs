using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class List : ExpressionBase
   {
      public ObservableCollection<ExpressionBase> Expressions { get; private set; }

      public List()
      {
         Expressions = new ObservableCollection<ExpressionBase>();
         Expressions.CollectionChanged += HandleItemsCollecionChanged;
      }
      private void HandleItemsCollecionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
      {
         if (Args.OldItems != null)
            foreach (MacroBase oldItem in Args.OldItems)
               oldItem.MacroChanged -= HandleItemMacroChanged;
         if (Args.NewItems != null)
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
         Visitor.VisitList(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherList = (List)OtherMacro;
         return Expressions.SequenceEqual(otherList.Expressions);
      }
   }
}
