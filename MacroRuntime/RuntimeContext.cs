using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading;
using Common;
using IO;
using Macro;
using MacroLanguage;

namespace MacroRuntime
{
   public class RuntimeContext : ContextBase
   {
      private readonly IntPtr _windowHandle;
      private readonly MacroParser _parser = new MacroParser();

      public RuntimeContext(IntPtr WindowHandle)
      {
         _windowHandle = WindowHandle;

         AddIntrinsicProcedure("eval", Eval, _evalExpression);

         AddIntrinsicProcedure("=", Equal, _equalLeft, _equalRight);

         AddIntrinsicProcedure("constant?", IsConstant, _isConstantExpression);
         AddIntrinsicProcedure("list?", IsList, _isListExpression);
         AddIntrinsicProcedure("symbol?", IsSymbol, _isSymbolExpression);

         AddIntrinsicProcedure("<", Less, _lessLeft, _lessRight);
         AddIntrinsicProcedure(">", Greater, _greaterLeft, _greaterRight);
         AddIntrinsicProcedure("or", Or, _orLeft, _orRight);
         AddIntrinsicProcedure("and", And, _andLeft, _andRight);

         AddIntrinsicProcedure("+", Add, _addLeft, _addRight);
         AddIntrinsicProcedure("-", Sub, _subLeft, _subRight);
         AddIntrinsicProcedure("*", Mul, _mulLeft, _mulRight);
         AddIntrinsicProcedure("/", Div, _divLeft, _divRight);
         AddIntrinsicProcedure("%", Mod, _modLeft, _modRight);
         AddIntrinsicProcedure("abs", Abs, _absValue);

         AddIntrinsicProcedure("car", Car, _carList);
         AddIntrinsicProcedure("cdr", Cdr, _cdrList);
         AddIntrinsicProcedure("append", Append, _appendListLeft, _appendListRight);

         AddIntrinsicProcedure("move", MouseMove, _mouseMoveDeltaX, _mouseMoveDeltaY);
         AddIntrinsicProcedure("position", MousePosition, _mousePositionX, _mousePositionY);
         AddIntrinsicProcedure("pause", Pause, _pauseDuration);
         AddIntrinsicProcedure("click", LeftClick);
         AddIntrinsicProcedure("windowshot", Windowshot, _windowshotX, _windowshotY, _windowshotWidth, _windowshotHeight);

         AddDerivedProcedure("first", "lst", "(car lst)");
         AddDerivedProcedure("rest", "lst", "(cdr lst)");
         AddDerivedProcedure("last", "(if (and list (cdr list)) (last (cdr list)) (car list))", "list");
         AddDerivedProcedure("list", ".", ".");

         AddDerivedProcedure("begin", "(last .)", ".");
         
         AddDerivedProcedure("<=", "(or (< left right) (= left right))", "left", "right");
         AddDerivedProcedure(">=", "(or (> left right) (= left right))", "left", "right");
      }

      private void AddIntrinsicProcedure(string Symbol, Func<ContextBase, Expression> Procedure, params Symbol[] FormalArguments)
      {
         var formalArguments = new List();
         foreach (var argument in FormalArguments)
            formalArguments.Expressions.Add(argument);
         DefineValue(new Symbol(Symbol), new IntrinsicProcedure { DefiningContext = this, FormalArguments = formalArguments, Function = Procedure });
      }

      private void AddDerivedProcedure(string Symbol, string Body, params string[] FormalArguments)
      {
         var body = (Expression)_parser.Parse(Body);
         Contract.Assert(body != null);
         var formalArguments = FormalArguments.Select(FormArg => new Symbol(FormArg)).ToArray();
         // ReSharper disable once CoVariantArrayConversion - this is ok, since the constructor uses the array read only
         var lambda = SpecialForms.Lambda(new List(formalArguments), body);
         DefineValue(new Symbol(Symbol), ExpressionEvaluator.Evaluate(lambda, this));
      }

      protected override bool SymbolNotFoundIsValueDefined(Symbol Symbol) // TODO BUG?
      {
         return false;
      }

      protected override void SymbolNotFoundSetValue(Symbol Symbol, Expression Value) // TODO BUG?
      {
         var exceptionMessage = "SetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to define first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      protected override Expression SymbolNotFoundGetValue(Symbol Symbol) // TODO BUG?
      {
         var exceptionMessage = "GetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to define first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      // TODO currently not needed - remove? private readonly Symbol _varArg = new Symbol(".");

      private readonly Symbol _isConstantExpression = new Symbol("Expression");
      private Expression IsConstant(ContextBase Context)
      {
         return new Constant(GetGenericValue<Expression>(Context, _isSymbolExpression).GetType() == typeof(Constant));
      }

      private readonly Symbol _isListExpression = new Symbol("Expression");
      private Expression IsList(ContextBase Context)
      {
         return new Constant(GetGenericValue<Expression>(Context, _isSymbolExpression).GetType() == typeof(List));
      }

      private readonly Symbol _isSymbolExpression = new Symbol("Expression");
      private Expression IsSymbol(ContextBase Context)
      {
         var symbolExpression = GetGenericValue<Expression>(Context, _isSymbolExpression);
         return new Constant(symbolExpression.GetType() == typeof(Symbol));
      }

      private readonly Symbol _addLeft = new Symbol("Left");
      private readonly Symbol _addRight = new Symbol("Right");
      private Expression Add(ContextBase Context)
      {
         var left = (decimal) GetGenericValue<Constant>(Context, _addLeft).Value;
         var right = (decimal) GetGenericValue<Constant>(Context, _addRight).Value;
         return new Constant(left + right);
      }

      private readonly Symbol _subLeft = new Symbol("Left");
      private readonly Symbol _subRight = new Symbol("Right");
      private Expression Sub(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _subLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _subRight).Value;
         return new Constant(left - right);
      }

      private readonly Symbol _mulLeft = new Symbol("Left");
      private readonly Symbol _mulRight = new Symbol("Right");
      private Expression Mul(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _mulLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _mulRight).Value;
         return new Constant(left * right);
      }

      private readonly Symbol _divLeft = new Symbol("Left");
      private readonly Symbol _divRight = new Symbol("Right");
      private Expression Div(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _divLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _divRight).Value;
         return new Constant(left / right);
      }

      private readonly Symbol _modLeft = new Symbol("Left");
      private readonly Symbol _modRight = new Symbol("Right");
      private Expression Mod(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _modLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _modRight).Value;
         return new Constant(left % right);
      }

      private readonly Symbol _lessLeft = new Symbol("Left");
      private readonly Symbol _lessRight = new Symbol("Right");
      private Expression Less(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _lessLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _lessRight).Value;
         return new Constant(left < right);
      }

      private readonly Symbol _greaterLeft = new Symbol("Left");
      private readonly Symbol _greaterRight = new Symbol("Right");
      private Expression Greater(ContextBase Context)
      {
         var left = (decimal)GetGenericValue<Constant>(Context, _greaterLeft).Value;
         var right = (decimal)GetGenericValue<Constant>(Context, _greaterRight).Value;
         return new Constant(left > right);
      }

      private readonly Symbol _orLeft = new Symbol("Left");
      private readonly Symbol _orRight = new Symbol("Right");
      private Expression Or(ContextBase Context)
      {
         var left = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _orLeft), Context);
         var right = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _orRight), Context);
         return new Constant(left || right);
      }

      private readonly Symbol _andLeft = new Symbol("Left");
      private readonly Symbol _andRight = new Symbol("Right");
      private Expression And(ContextBase Context)
      {
         var left = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _andLeft), Context);
         var right = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _andRight), Context);
         return new Constant(left && right);
      }

      private readonly Symbol _equalLeft = new Symbol("Left");
      private readonly Symbol _equalRight = new Symbol("Right");
      private Expression Equal(ContextBase Context)
      {
         var left = GetGenericValue<Expression>(Context, _equalLeft);
         var right = GetGenericValue<Expression>(Context, _equalRight);
         var leftEqualsRight = Equals( left, right);
         return new Constant(leftEqualsRight);
      }

      private readonly Symbol _evalExpression = new Symbol("Expression");
      private Expression Eval(ContextBase Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(GetGenericValue<Expression>(Context, _evalExpression));
      }

      private readonly Symbol _absValue = new Symbol("Value");
      private Expression Abs(ContextBase Context)
      {
         var numeric = (decimal)GetGenericValue<Constant>(Context, _absValue).Value;
         return new Constant(Math.Abs(numeric));
      }

      private readonly Symbol _carList = new Symbol("List");
      private Expression Car(ContextBase Context)
      {
         var list = GetGenericValue<List>(Context, _carList);
         if (list.Expressions.Count == 0)
            throw new RuntimeException("Cannot get car of empty list", list, this);
         return list.Expressions.First();
      }

      private readonly Symbol _cdrList = new Symbol("List");
      private Expression Cdr(ContextBase Context)
      {
         var list = GetGenericValue<List>(Context, _cdrList);
         if (list.Expressions.Count == 0)
            throw new RuntimeException("Cannot get cdr of empty list", list, this);
         return new List(list.Expressions.Skip(1));
      }

      private readonly Symbol _appendListLeft = new Symbol("ListLeft");
      private readonly Symbol _appendListRight = new Symbol("ListRight");
      private Expression Append(ContextBase Context)
      {
         var listLeft = GetGenericValue<List>(Context, _appendListLeft);
         var listRight = GetGenericValue<List>(Context, _appendListRight);
         return new List(listLeft.Expressions.Concat(listRight.Expressions));
      }


      private readonly Symbol _mouseMoveDeltaX = new Symbol("DeltaX");
      private readonly Symbol _mouseMoveDeltaY = new Symbol("DeltaY");
      private Constant MouseMove(ContextBase Context)
      {
         Mouse.X += (int)GetConstantValue<decimal>(Context, _mouseMoveDeltaX);
         Mouse.Y += (int)GetConstantValue<decimal>(Context, _mouseMoveDeltaY);

         return new Constant(true);
      }


      private readonly Symbol _mousePositionX = new Symbol("X");
      private readonly Symbol _mousePositionY = new Symbol("Y");
      private Constant MousePosition(ContextBase Context)
      {
         int screenX, screenY;
         Window.ClientToScreen(
            _windowHandle,
            (int)GetConstantValue<decimal>(Context, _mousePositionX), (int)GetConstantValue<decimal>(Context, _mousePositionY),
            out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);

         return new Constant(true);
      }


      private readonly Symbol _pauseDuration = new Symbol("Duration");
      private Constant Pause(ContextBase Context)
      {
         var milliseconds = GetConstantValue<decimal>(Context, _pauseDuration);
         Thread.Sleep((int) milliseconds);

         return new Constant(milliseconds);
      }

      [ExcludeFromCodeCoverage]
      private Constant LeftClick(ContextBase Context)
      {
         Mouse.LeftClick();

         return new Constant(true);
      }


      private readonly Symbol _windowshotX = new Symbol("X");
      private readonly Symbol _windowshotY = new Symbol("Y");
      private readonly Symbol _windowshotWidth = new Symbol("Width");
      private readonly Symbol _windowshotHeight = new Symbol("Height");
      [ExcludeFromCodeCoverage]
      private Constant Windowshot(ContextBase Context)
      {
         var x = (int)GetConstantValue<decimal>(Context, _windowshotX);
         var y = (int)GetConstantValue<decimal>(Context, _windowshotY);
         var width = (int)GetConstantValue<decimal>(Context, _windowshotWidth);
         var height = (int)GetConstantValue<decimal>(Context, _windowshotHeight);

         using (var windowCapture = Window.Capture(_windowHandle))
         {
            using (var croppedImage = Imaging.CropImage(windowCapture, x, y, width, height))
            {
               return new Constant(Imaging.Image2PngHexString(croppedImage));  
            }
         }
      }

      private T GetConstantValue<T>(ContextBase Context, Symbol Symbol)
      {
         var constantValue = GetGenericValue<Constant>(Context, Symbol).Value;
         try
         {
            return (T)constantValue;
         }
         catch (InvalidCastException e)
         {
            throw new RuntimeException(
               string.Format("Symbol >> {0} <<: expected type was >> {1} << but got >> {2} <<", Symbol, typeof(T), constantValue.GetType()),
               Symbol,
               Context,
               e);
         }
      }

      private T GetGenericValue<T>(ContextBase Context, Symbol Symbol)
         where T : Expression
      {
         try
         {
            return (T)Context.GetValue(Symbol);
         }
         catch (InvalidCastException e)
         {
            throw new RuntimeException(
               string.Format(
                  "Symbol >> {0} <<: expected [{1}] but got >> {2} [{3}] <<",
                  Symbol,
                  typeof(T),
                  Context.GetValue(Symbol),
                  Context.GetValue(Symbol).GetType()),
               Symbol,
               Context,
               e);
         }
      }
   }
}
