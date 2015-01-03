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
   public class WindowshotVM : BooleanExpressionBaseVM
   {
      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _positionXVM;
      public IntegerExpressionBaseVM PositionXVM
      {
         get
         {
            return _positionXVM.Value;
         }
      }

      private NotifyingTransformedProperty<IntegerExpressionBaseVM> _positionYVM;
      public IntegerExpressionBaseVM PositionYVM
      {
         get
         {
            return _positionYVM.Value;
         }
      }

      private NotifyingTransformedProperty<StringExpressionBaseVM> _imageUrlYVM;
      public StringExpressionBaseVM ImageUrlVM
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
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "PositionX" }, "PositionXVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.PositionX),
               VM => VM.Dispose());

         _positionYVM =
            new NotifyingTransformedProperty<IntegerExpressionBaseVM>(
               new[] { "PositionY" }, "PositionYVM",
               Model, this,
               () => (IntegerExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.PositionY),
               VM => VM.Dispose());

         _imageUrlYVM =
            new NotifyingTransformedProperty<StringExpressionBaseVM>(
               new[] { "ImageUrl" }, "ImageUrlVM",
               Model, this,
               () => (StringExpressionBaseVM)MacroViewModelFactory.Instance.Create(Model.ImageUrl),
               VM => VM.Dispose());
      }
   }
}
