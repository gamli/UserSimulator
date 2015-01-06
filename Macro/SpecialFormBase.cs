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
         this.SpecialFormSymbol = new Symbol { Value = SpecialFormSymbol };
         Expressions.Add(this.SpecialFormSymbol);
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
         if (!Equals(Expressions[0], SpecialFormSymbol))
            Expressions[0] = SpecialFormSymbol;
         var index = 1;
         foreach (var specialProperty in SpecialProperties)
         {
            var propertyValue = (ExpressionBase)GetType().GetProperty(specialProperty).GetValue(this);
            if(!Equals(Expressions[index], propertyValue))
               Expressions[index] = propertyValue;
            index++;
         }
      }
   }
}
