using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Numerics;

namespace Macro
{
   public class Constant : AtomicExpression
   {
      private object _value;
      public object Value { get { return _value; } set { SetPropertyValue(ref _value, value); } }

      public Constant()
      {
         // nothing to do
      }

      public Constant(object Value)
      {
         this.Value = Value;
      }

      public override void Accept(IVisitor Visitor)
      {
         Visitor.VisitConstant(this);
      }

      protected override bool MacroEquals(MacroBase OtherMacro)
      {
         var otherConstantExpression = (Constant)OtherMacro;
         return Equals(Value, otherConstantExpression.Value);
      }

      protected override int MacroGetHashCode()
      {
         return Value == null ? 0 : Value.GetHashCode();
      }

      [ExcludeFromCodeCoverage]
      public override string ToString() // TODO - move logic to printer?
      {
         return
            Value == null
               ? "null"
               : Value is string
                  ? "\"" + ((string)Value).Replace(@"\", @"\\").Replace(@"""", @"\""") + "\""
                  : Value is BigRational
                     ? PrintRational((BigRational)Value)
                     : Value is bool
                        ? (bool)Value ? "true" : "false"
                        : Value.ToString();
      }

      private static string PrintRational(BigRational Rational)
      {
         try
         {
            return ((decimal)Rational).ToString(CultureInfo.InvariantCulture);
         }
         catch (OverflowException)
         {
            return Rational.ToString();
         }
      }

      public static Constant Number(decimal Value)
      {
         return new Constant(new BigRational(Value));
      }

      public static Expression Number(double Value)
      {
         return Number((decimal)Value);
      }

      public static Expression Number(int Value)
      {
         return Number((decimal)Value);
      }

      public static Expression Number(long Value)
      {
         return Number((decimal)Value);
      }
   }
}
