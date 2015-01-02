using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public abstract class StatementWithBodyBase : StatementBase
   {
      private StatementBase _body;
      [ExcludeFromCodeCoverage]
      public StatementBase Body { get { return _body; } set { SetPropertyValue(ref _body, value); } }

      protected bool BodyEquals(StatementWithBodyBase MacroWithBody)
      {
         return Body.Equals(MacroWithBody.Body);
      }
   }

   public abstract class StatementBase : MacroBase
   {
   }
}
