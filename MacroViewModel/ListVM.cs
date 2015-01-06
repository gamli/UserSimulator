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
   public class ListVM : ExpressionBaseVM
   {
      private TransformedCollection<ExpressionBase, ExpressionBaseVM> _expressionsVM;
      public ReadOnlyObservableCollection<ExpressionBaseVM> ExpressionsVM { get { return _expressionsVM.Transformed; } }

      public ListVM(List Model)
         : base(Model)
      {
         _expressionsVM =
            new TransformedCollection<ExpressionBase, ExpressionBaseVM>(
               Model.Expressions,
               Expression => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Expression),
               null,
               ExpressionVM => { if (ExpressionVM != null) ExpressionVM.Dispose(); });
      }

      protected override void Dispose(bool Disposing)
      {
         _expressionsVM.Dispose();
         base.Dispose(Disposing);
      }
   }
}
