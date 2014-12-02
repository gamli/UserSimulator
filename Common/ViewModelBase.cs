using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public abstract class ViewModelBase<T> : NotifyPropertyChangedBase
      where T : NotifyPropertyChangedBase
   {
      public T Model { get; private set; }

      public ViewModelBase(T Model)
      {
         this.Model = Model;
      }
   }
}
