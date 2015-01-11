using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace MacroViewModel
{
   public class ExpressionListVM : ListExpressionBaseVM<ExpressionBase, ExpressionBaseVM>
   {
      public ExpressionListVM(ListExpressionBase<ExpressionBase> Model) 
         : base(Model)
      {
      }
   }
}
