using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro;

namespace Macro_TEST
{
   class FakeMacro : MacroBase
   {
      public int SomeValue { get; set; }
         
      [ExcludeFromCodeCoverage]
      public override void Accept(IVisitor Visitor)
      {
         throw new NotImplementedException();
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         return Equals(SomeValue, ((FakeMacro) OtherMacro).SomeValue);
      }

      protected override int MacroGetHashCode()
      {
         return SomeValue.GetHashCode();
      }
   }
}
