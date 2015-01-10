using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public abstract class ListExpressionBase<TExpression> : ExpressionBase
      where TExpression : ExpressionBase
   {
      public ObservableCollection<TExpression> Expressions { get; private set; }

      protected ListExpressionBase()
      {
         Expressions = new ObservableCollection<TExpression>();
         Expressions.CollectionChanged += HandleItemsCollecionChanged;
      }
      private void HandleItemsCollecionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
      {
         if (Args.OldItems != null)
            foreach (MacroBase oldItem in Args.OldItems)
               if (oldItem != null)
                  oldItem.MacroChanged -= HandleItemMacroChanged;
         if (Args.NewItems != null)
            foreach (MacroBase newItem in Args.NewItems)
               if (newItem != null)
                  newItem.MacroChanged += HandleItemMacroChanged;
         RaiseMacroChanged(this, new EventArgs());
      }
      private void HandleItemMacroChanged(object Sender, EventArgs Args)
      {
         RaiseMacroChanged(Sender, Args);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherList = (ListExpressionBase<TExpression>)OtherMacro;
         return Expressions.SequenceEqual(otherList.Expressions);
      }

      public override string ToString()
      {
         return "(" + string.Join(" ", Expressions) + ")";
      }
   }
}
