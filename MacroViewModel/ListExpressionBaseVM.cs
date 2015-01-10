using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Common;
using Macro;

namespace MacroViewModel
{
   public abstract class ListExpressionBaseVM<TExpression, TExpressionVM> : ExpressionBaseVM
      where TExpression : ExpressionBase
      where TExpressionVM : ExpressionBaseVM
   {
      private readonly TransformedCollection<TExpression, TExpressionVM> _expressionsVM;
      public ReadOnlyObservableCollection<TExpressionVM> ExpressionsVM { get { return _expressionsVM.Transformed; } }

      protected ListExpressionBaseVM(ListExpressionBase<TExpression> Model)
         : base(Model)
      {
         _expressionsVM =
            new TransformedCollection<TExpression, TExpressionVM>(
               Model.Expressions,
               Expression => (TExpressionVM)MacroViewModelFactory.Instance.Create(Expression),
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
