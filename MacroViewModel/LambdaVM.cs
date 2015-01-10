using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Macro;

namespace MacroViewModel
{
   public class LambdaVM : ExpressionBaseVM
   {
      private readonly NotifyingTransformedProperty<SymbolVM> _argumentSymbolsVM;
      public SymbolVM ArgumentSymbolsVM
      {
         get
         {
            return _argumentSymbolsVM.Value;
         }
      }

      private readonly NotifyingTransformedProperty<ExpressionBaseVM> _bodyVM;
      public ExpressionBaseVM BodyVM
      {
         get
         {
            return _bodyVM.Value;
         }
      }

      public LambdaVM(Lambda Model)
         : base(Model)
      {
         _argumentSymbolsVM =
            new NotifyingTransformedProperty<SymbolVM>(
               new[] { "ArgumentSymbols" }, "ArgumentSymbolsVM",
               Model, this,
               () => (SymbolVM)MacroViewModelFactory.Instance.Create(Model.ArgumentSymbols),
               VM => VM.Dispose());

         _bodyVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "Body" }, "BodyVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.Body),
               VM => VM.Dispose());
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
         {
            _argumentSymbolsVM.Dispose();
            _bodyVM.Dispose();
         }
         base.Dispose(Disposing);
      }
   }
}
