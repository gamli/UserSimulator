using System.Collections.ObjectModel;
using Common;
using Macro;

namespace MacroViewModel
{
   public class ListVM : ExpressionVM
   {
      private readonly TransformedCollection<Expression, ExpressionVM> _expressionsVM;
      public ReadOnlyObservableCollection<ExpressionVM> ExpressionsVM { get { return _expressionsVM.Transformed; } }

      public ListVM(List Model)
         : base(Model)
      {
         _expressionsVM =
            new TransformedCollection<Expression, ExpressionVM>(
               Model.Expressions,
               Expression => (ExpressionVM)MacroViewModelFactory.Instance.Create(Expression),
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
