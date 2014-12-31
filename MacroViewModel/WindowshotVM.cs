using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Macro;

namespace MacroViewModel
{
   public class WindowshotVM : MacroWithBodyBaseVM<Windowshot>
   {
      private NotifyingTransformedProperty<ExpressionBaseVM> _positionXVM;
      public ExpressionBaseVM PositionXVM
      {
         get
         {
            return _positionXVM.Value;
         }
      }

      private NotifyingTransformedProperty<ExpressionBaseVM> _positionYVM;
      public ExpressionBaseVM PositionYVM
      {
         get
         {
            return _positionYVM.Value;
         }
      }

      private NotifyingTransformedProperty<ExpressionBaseVM> _imageUrlYVM;
      public ExpressionBaseVM ImageUrlVM
      {
         get
         {
            return _imageUrlYVM.Value;
         }
      }

      public WindowshotVM(Windowshot Model)
         : base(Model)
      {
         _positionXVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "PositionX" }, "PositionXVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.PositionX),
               VM => VM.Dispose());

         _positionYVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "PositionY" }, "PositionYVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.PositionY),
               VM => VM.Dispose());

         _imageUrlYVM =
            new NotifyingTransformedProperty<ExpressionBaseVM>(
               new[] { "ImageUrl" }, "ImageUrlVM",
               Model, this,
               () => (ExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.ImageUrl),
               VM => VM.Dispose());
      }
   }
}
