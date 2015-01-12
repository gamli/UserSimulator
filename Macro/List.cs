using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class List : Expression
   {
      public ObservableCollection<Expression> Expressions { get; private set; }

      public List()
      {
         Expressions = new ObservableCollection<Expression>();
         Expressions.CollectionChanged += HandleItemsCollecionChanged;
      }

      public List(params Expression[] Expressions) 
         : this()
      {
         foreach (var expression in Expressions)
            this.Expressions.Add(expression);
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

         OnMacroChanged();
      }
      private void HandleItemMacroChanged(object Sender, EventArgs Args)
      {
         OnMacroChanged();
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

      protected override int MacroGetHashCode()
      {
         return Expressions.GetHashCode();
      }



      public override string ToString()
      {
         return "(" + string.Join(" ", Expressions) + ")";
      }
   }
}
