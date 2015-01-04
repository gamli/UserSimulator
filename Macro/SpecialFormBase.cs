using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
   public abstract class SpecialFormBase : List
   {
      private Symbol _specialFormSymbol;
      public Symbol SpecialFormSymbol { get { return _specialFormSymbol; } set { SetPropertyValue(ref _specialFormSymbol, value); } }

      protected SpecialFormBase(string SpecialFormSymbol, params string[] SpecialProperties)
      {
         Expressions.Add(new Symbol { Value = SpecialFormSymbol });
         for (var i = 0; i < SpecialProperties.Length; i++)
            Expressions.Add(null);

         Expressions.CollectionChanged +=
            (Sender, Args) =>
               {
                  this.SpecialFormSymbol = (Symbol)Expressions[0];
                  var index = 1;
                  foreach (var specialProperty in SpecialProperties)
                  {
                     GetType().GetProperty(specialProperty).SetValue(this, Expressions[index]);
                     index++;
                  }
               };

         PropertyChanged += (Sender, Args) => UpdateSpecialProperties(SpecialProperties);
         UpdateSpecialProperties(SpecialProperties);
      }

      private void UpdateSpecialProperties(string[] SpecialProperties)
      {
         Expressions[0] = this.SpecialFormSymbol;
         var index = 1;
         foreach (var specialProperty in SpecialProperties)
         {
            Expressions[index] = (ExpressionBase)GetType().GetProperty(specialProperty).GetValue(this);
            index++;
         }
      }
   }
}
