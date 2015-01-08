using System.Collections.ObjectModel;
using Common;
using Macro;

namespace MacroViewModel
{
   public class ListVM : ExpressionBaseVM
   {
      private readonly TransformedCollection<ExpressionBase, ExpressionBaseVM> _expressionsVM;
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
