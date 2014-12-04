using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Block : MacroBase
   {
      public ObservableCollection<MacroBase> Items { get; private set; }
      public Block()
      {
         Items = new ObservableCollection<MacroBase>();
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitBlock(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherBlock = (Block)OtherMacro;
         return Items.SequenceEqual(otherBlock.Items);
      }
   }
}
