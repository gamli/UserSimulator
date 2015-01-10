using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class SymbolList : ListExpressionBase<Symbol>
   {
      public ObservableCollection<Symbol> Symbols { get { return Expressions; } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitSymbolList(this);
      }
   }
}
