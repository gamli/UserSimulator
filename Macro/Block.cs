﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public class Block : Base
   {
      public ObservableCollection<Base> Items { get; private set; }
      public Block()
      {
         Items = new ObservableCollection<Base>();
      }

      public override void Accept(IVisitor Visitor)
      {
         foreach (var item in Items)
            item.Accept(Visitor);
      }
   }
}
