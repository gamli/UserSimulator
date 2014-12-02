using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Program : MacroBase
   {
      private Block _block;
      public Block Block { get { return _block; } set { SetPropertyValue(ref _block, value); } }

      public Program()
      {
         Block = new Block();
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.BeginVisitProgram(this);
         Block.Accept(Visitor);
         Visitor.EndVisitProgram(this);
      }
   }
}
