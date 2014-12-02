﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class ForLoop : WithBodyBase
   {
      private int _repetitionCount;
      public int RepetitionCount { get { return _repetitionCount; } set { SetPropertyValue(ref _repetitionCount, value); } }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitForLoop(this);
      }
   }
}
