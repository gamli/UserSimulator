﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading;
using IO;
using Macro;
using MacroLanguage;

namespace MacroRuntime
{
   public class RuntimeContext : ContextBase
   {
      private readonly IntPtr _targetWindow;
      private readonly MacroParser _parser = new MacroParser();

      public RuntimeContext(IntPtr TargetWindow)
      {
         _targetWindow = TargetWindow;
         
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
         AddIntrinsicProcedure("abs", Abs, _absValue);

         AddIntrinsicProcedure("car", Car, _carList);
         AddIntrinsicProcedure("cdr", Cdr, _cdrList);

         AddIntrinsicProcedure("move", MouseMove, _mouseMoveDeltaX, _mouseMoveDeltaY);
         AddIntrinsicProcedure("position", MousePosition, _mousePositionX, _mousePositionY);
         AddIntrinsicProcedure("pause", Pause, _pauseDuration);
         AddIntrinsicProcedure("click", LeftClick);
         AddIntrinsicProcedure("windowshot", Windowshot, _windowshotX, _windowshotY, _windowshotImageUrl);

         AddDerivedProcedure("last", "(if (and list (cdr list)) (last (cdr list)) (car list))", "list");
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
         Expression[] formalArguments = FormalArguments.Select(FormArg => new Symbol(FormArg)).ToArray();
         var lambda = SpecialForms.Lambda(new List(formalArguments), body);
         DefineValue(new Symbol(Symbol), ExpressionEvaluator.Evaluate(lambda, this));
      }

      [ExcludeFromCodeCoverage]
      protected override void SymbolNotFoundSetValue(Symbol Symbol, Expression Value)
      {
         var exceptionMessage = "SetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to 'define' first?)";
         throw new RuntimeException(exceptionMessage, Symbol, this);
      }

      [ExcludeFromCodeCoverage]
      protected override Expression SymbolNotFoundGetValue(Symbol Symbol)
      {
         var exceptionMessage = "GetValue: Symbol >>" + Symbol.Value + "<< is not defined (did you forget to 'define' first?)";
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
         return new Constant(GetGenericValue<Expression>(Context, _isSymbolExpression).GetType() == typeof(Symbol));
      }

      private readonly Symbol _addLeft = new Symbol("Left");
      private readonly Symbol _addRight = new Symbol("Right");
      private Expression Add(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _addLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _addRight).Value;
         return new Constant(left + right);
      }

      private readonly Symbol _subLeft = new Symbol("Left");
      private readonly Symbol _subRight = new Symbol("Right");
      private Expression Sub(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _subLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _subRight).Value;
         return new Constant(left - right);
      }

      private readonly Symbol _mulLeft = new Symbol("Left");
      private readonly Symbol _mulRight = new Symbol("Right");
      private Expression Mul(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _mulLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _mulRight).Value;
         return new Constant(left * right);
      }

      private readonly Symbol _divLeft = new Symbol("Left");
      private readonly Symbol _divRight = new Symbol("Right");
      private Expression Div(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _divLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _divRight).Value;
         return new Constant(left / right);
      }

      private readonly Symbol _lessLeft = new Symbol("Left");
      private readonly Symbol _lessRight = new Symbol("Right");
      private Expression Less(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _lessLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _lessRight).Value;
         return new Constant(left < right);
      }

      private readonly Symbol _greaterLeft = new Symbol("Left");
      private readonly Symbol _greaterRight = new Symbol("Right");
      private Expression Greater(ContextBase Context)
      {
         dynamic left = GetGenericValue<Constant>(Context, _greaterLeft).Value;
         dynamic right = GetGenericValue<Constant>(Context, _greaterRight).Value;
         return new Constant(left > right);
      }

      private readonly Symbol _orLeft = new Symbol("Left");
      private readonly Symbol _orRight = new Symbol("Right");
      private Expression Or(ContextBase Context)
      {
         dynamic left = TypeConversion.ConvertToBoolean(GetGenericValue<Constant>(Context, _orLeft), Context);
         dynamic right = TypeConversion.ConvertToBoolean(GetGenericValue<Constant>(Context, _orRight), Context);
         return new Constant(left || right);
      }

      private readonly Symbol _andLeft = new Symbol("Left");
      private readonly Symbol _andRight = new Symbol("Right");
      private Expression And(ContextBase Context)
      {
         dynamic left = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _andLeft), Context);
         dynamic right = TypeConversion.ConvertToBoolean(GetGenericValue<Expression>(Context, _andRight), Context);
         return new Constant(left && right);
      }

      private readonly Symbol _equalLeft = new Symbol("Left");
      private readonly Symbol _equalRight = new Symbol("Right");
      private Expression Equal(ContextBase Context)
      {
         return
            new Constant(
               Equals(
                  GetGenericValue<Expression>(Context, _equalLeft),
                  GetGenericValue<Expression>(Context, _equalRight)));
      }

      private readonly Symbol _absValue = new Symbol("Value");
      private Expression Abs(ContextBase Context)
      {
         dynamic numeric = GetGenericValue<Constant>(Context, _absValue);
         return Math.Abs(numeric.Value);
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
         return new List(list.Expressions.Skip(1).ToArray());
      }


      private readonly Symbol _mouseMoveDeltaX = new Symbol("DeltaX");
      private readonly Symbol _mouseMoveDeltaY = new Symbol("DeltaY");
      private Constant MouseMove(ContextBase Context)
      {
         Mouse.X += GetConstantValue<int>(Context, _mouseMoveDeltaX);
         Mouse.Y += GetConstantValue<int>(Context, _mouseMoveDeltaY);

         return new Constant(true);
      }


      private readonly Symbol _mousePositionX = new Symbol("X");
      private readonly Symbol _mousePositionY = new Symbol("Y");
      private Constant MousePosition(ContextBase Context)
      {
         int screenX, screenY;
         Window.ClientToScreen(
            _targetWindow,
            GetConstantValue<int>(Context, _mousePositionX), GetConstantValue<int>(Context, _mousePositionY),
            out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);

         return new Constant(true);
      }


      private readonly Symbol _pauseDuration = new Symbol("Duration");
      private Constant Pause(ContextBase Context)
      {
         var milliseconds = GetConstantValue<int>(Context, _pauseDuration);
         Thread.Sleep(milliseconds);

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
      private readonly Symbol _windowshotImageUrl = new Symbol("ImageUrl");
      [ExcludeFromCodeCoverage]
      private Constant Windowshot(ContextBase Context)
      {
         using (var image = new Bitmap(GetConstantValue<string>(Context, _windowshotImageUrl)))
         using (var windowContent = Window.Capture(_targetWindow))
         using (var clippedWindowContent = new Bitmap(image.Width, image.Height))
         using (var clippedWindowContentGraphics = Graphics.FromImage(clippedWindowContent))
         {
            var windowshotX = GetConstantValue<int>(Context, _windowshotX);
            var windowshotY = GetConstantValue<int>(Context, _windowshotY);

            clippedWindowContentGraphics.DrawImage(
               windowContent,
               0, 0,
               new Rectangle(
                  windowshotX - image.Width / 2, windowshotY - image.Width / 2,
                  image.Width, image.Height),
               GraphicsUnit.Pixel
               );

            for (var x = 0; x < image.Width; x++)
               for (var y = 0; y < image.Height; y++)
                  if (image.GetPixel(x, y) != clippedWindowContent.GetPixel(x, y))
                  {
                     return new Constant(false);
                  }
         }
         return new Constant(true);
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
