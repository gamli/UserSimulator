﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;
using Common;
using IO;
using Macro;
using MacroLanguage;
using Numerics;

namespace MacroRuntime
{
   public class RuntimeContext : ContextBase
   {
      private readonly IntPtr _windowHandle;
      private readonly IOutput _output;
      private readonly MacroParser _parser = new MacroParser();

      public RuntimeContext(IntPtr WindowHandle, IOutput Output = null)
      {
         _windowHandle = WindowHandle;
         _output = Output;

         AddIntrinsicProcedure("eval", Eval, _evalExpression);

         AddIntrinsicProcedure("=", Equal, _equalLeft, _equalRight);
         AddIntrinsicProcedure("not", Not, _notBooleanExpression);

         AddIntrinsicProcedure("constant?", IsConstant, _isConstantExpression);
         AddIntrinsicProcedure("list?", IsList, _isListExpression);
         AddIntrinsicProcedure("symbol?", IsSymbol, _isSymbolExpression);

         AddIntrinsicProcedure("<", Less, _lessLeft, _lessRight);
         AddIntrinsicProcedure(">", Greater, _greaterLeft, _greaterRight);

         AddIntrinsicProcedure("+", Add, _addLeft, _addRight);
         AddIntrinsicProcedure("-", Sub, _subLeft, _subRight);
         AddIntrinsicProcedure("*", Mul, _mulLeft, _mulRight);
         AddIntrinsicProcedure("/", Div, _divLeft, _divRight);
         AddIntrinsicProcedure("%", Mod, _modLeft, _modRight);
         AddIntrinsicProcedure("abs", Abs, _absValue);

         AddIntrinsicProcedure("car", Car, _carList);
         AddIntrinsicProcedure("cdr", Cdr, _cdrList);
         AddIntrinsicProcedure("append", Append, _appendListLeft, _appendListRight);

         AddIntrinsicVarArgProcedure("print", Print, _printExpressions);
         AddDerivedVarArgProcedure("log", "(print (append (list (time-str (time))) args))", "args");
         AddIntrinsicProcedure("time", Time);
         AddIntrinsicProcedure("time-str", TimeString, _timeStringTime);
         AddIntrinsicProcedure("ocr", Ocr, _ocrImage);
         AddIntrinsicProcedure("edit-distance", EditDistance, _editDistanceLeft, _editDistanceRight);
         AddIntrinsicProcedure("substr", Substring, _substringStartIndex, _substringLength);
         AddIntrinsicProcedure("strlen", StringLength, _stringLengthString);
         AddIntrinsicProcedure("regex-replace", RegexReplace, _regexReplaceString, _regexReplaceRegexToReplace, _regexReplaceReplacement);
         AddIntrinsicProcedure("regex-match", RegexMatch, _regexMatchString, _regexMatchRegex);

         AddIntrinsicProcedure("move", MouseMove, _mouseMoveDeltaX, _mouseMoveDeltaY);
         AddIntrinsicProcedure("position", MousePosition, _mousePositionX, _mousePositionY);
         AddIntrinsicProcedure("pause", Pause, _pauseDuration);
         AddIntrinsicProcedure("click", LeftClick);
         AddIntrinsicProcedure("wheel", Wheel, _wheelDelta);
         AddIntrinsicProcedure("windowshot", Windowshot, _windowshotX, _windowshotY, _windowshotWidth, _windowshotHeight);

         AddDerivedProcedure("suffix", "(substr str start-index (- (strlen str) start-index))", "str", "start-index");
         AddDerivedProcedure("prefix", "(substr str 0 length)", "str", "length");

         AddDerivedProcedure("first", "(car lst)", "lst");
         AddDerivedProcedure("rest", "(cdr lst)", "lst");
         AddDerivedProcedure("last", "(if (cdr lst) (last (cdr lst)) (car lst))", "lst");
         AddDerivedVarArgProcedure("list", "args", "args");

         AddDerivedVarArgProcedure("begin", "(last args)", "args");
         
         AddDerivedProcedure("!=", "(not (= left right))", "left", "right");
         AddDerivedVarArgProcedure("or", "(if args (if (first args) true (or (rest args))) true)", "args");
         AddDerivedVarArgProcedure("and", "(if args (if (first args) (and (rest args)) false) true)", "args");
         AddDerivedProcedure("<=", "(or (< left right) (= left right))", "left", "right");
         AddDerivedProcedure(">=", "(or (> left right) (= left right))", "left", "right");

         AddDerivedProcedure("map", "(begin (define mapped nil) (loop lst (begin (set! mapped (append mapped (list (fun (first lst))))) (set! lst (rest lst)))) mapped)", "lst", "fun");

         AddDerivedVarArgProcedure("max", "(begin (define max-value (first lst)) (loop lst (begin (if (> (first lst) max-value) (set! max-value (first lst)) nil) (set! lst (rest lst)))) max-value)", "lst");
         AddDerivedVarArgProcedure("min", "(begin (define min-value (first lst)) (loop lst (begin (if (< (first lst) min-value) (set! min-value (first lst)) nil) (set! lst (rest lst)))) min-value)", "lst");

         AddDerivedProcedure("arg-max", "(first (args-max lst fun))", "lst", "fun");
         AddDerivedProcedure("args-max", "(begin (define mapped (map lst fun)) (define max-val (max mapped)) (filter lst (lambda (arg) (= (fun arg) max-val))))", "lst", "fun");

         AddDerivedProcedure("filter", "(begin (define filtered nil) (loop lst (begin (if (predicate (first lst)) (set! filtered (append filtered (list (first lst)))) nil) (set! lst (rest lst)))) filtered)", "lst", "predicate");
      }

      private void AddIntrinsicProcedure(string Symbol, Func<IContext, Expression> Procedure, params Symbol[] FormalArguments)
      {
         var formalArguments = new List();
         foreach (var argument in FormalArguments)
            formalArguments.Expressions.Add(argument);
         DefineValue(new Symbol(Symbol), new IntrinsicProcedure { DefiningContext = this, FormalArguments = formalArguments, Function = Procedure });
      }

      private void AddIntrinsicVarArgProcedure(string Symbol, Func<IContext, Expression> Procedure, Symbol VarArgArgument)
      {
         DefineValue(new Symbol(Symbol), new IntrinsicProcedure { DefiningContext = this, FormalArguments = VarArgArgument, Function = Procedure });
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

      private void AddDerivedVarArgProcedure(string Symbol, string Body, string VarArgArgument)
      {
         var body = (Expression)_parser.Parse(Body);
         Contract.Assert(body != null);
         var varArgArgument = new Symbol(VarArgArgument);
         var lambda = SpecialForms.Lambda(varArgArgument, body);
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
      private Expression IsConstant(IContext Context)
      {
         return new Constant(GetGenericValue<Expression>(Context, _isSymbolExpression).GetType() == typeof(Constant));
      }

      private readonly Symbol _isListExpression = new Symbol("Expression");
      private Expression IsList(IContext Context)
      {
         return new Constant(GetGenericValue<Expression>(Context, _isSymbolExpression).GetType() == typeof(List));
      }

      private readonly Symbol _isSymbolExpression = new Symbol("Expression");
      private Expression IsSymbol(IContext Context)
      {
         var symbolExpression = GetGenericValue<Expression>(Context, _isSymbolExpression);
         return new Constant(symbolExpression.GetType() == typeof(Symbol));
      }

      private readonly Symbol _addLeft = new Symbol("Left");
      private readonly Symbol _addRight = new Symbol("Right");
      private Expression Add(IContext Context)
      {
         var left = GetNumber(Context, _addLeft);
         var right = GetNumber(Context, _addRight);
         return new Constant(left + right);
      }

      private readonly Symbol _subLeft = new Symbol("Left");
      private readonly Symbol _subRight = new Symbol("Right");
      private Expression Sub(IContext Context)
      {
         var left = GetNumber(Context, _subLeft);
         var right = GetNumber(Context, _subRight);
         return new Constant(left - right);
      }

      private readonly Symbol _mulLeft = new Symbol("Left");
      private readonly Symbol _mulRight = new Symbol("Right");
      private Expression Mul(IContext Context)
      {
         var left = GetNumber(Context, _mulLeft);
         var right = GetNumber(Context, _mulRight);
         return new Constant(left * right);
      }

      private readonly Symbol _divLeft = new Symbol("Left");
      private readonly Symbol _divRight = new Symbol("Right");
      private Expression Div(IContext Context)
      {
         var left = GetNumber(Context, _divLeft);
         var right = GetNumber(Context, _divRight);
         return new Constant(left / right);
      }

      private readonly Symbol _modLeft = new Symbol("Left");
      private readonly Symbol _modRight = new Symbol("Right");
      private Expression Mod(IContext Context)
      {
         var left = GetNumber(Context, _modLeft);
         var right = GetNumber(Context, _modRight);
         return new Constant(left % right);
      }

      private readonly Symbol _lessLeft = new Symbol("Left");
      private readonly Symbol _lessRight = new Symbol("Right");
      private Expression Less(IContext Context)
      {
         var left = GetNumber(Context, _lessLeft);
         var right = GetNumber(Context, _lessRight);
         return new Constant(left < right);
      }

      private readonly Symbol _greaterLeft = new Symbol("Left");
      private readonly Symbol _greaterRight = new Symbol("Right");
      private Expression Greater(IContext Context)
      {
         var left = GetNumber(Context, _greaterLeft);
         var right = GetNumber(Context, _greaterRight);
         return new Constant(left > right);
      }

      private readonly Symbol _equalLeft = new Symbol("Left");
      private readonly Symbol _equalRight = new Symbol("Right");
      private Expression Equal(IContext Context)
      {
         var left = GetGenericValue<Expression>(Context, _equalLeft);
         var right = GetGenericValue<Expression>(Context, _equalRight);
         var leftEqualsRight = Equals(left, right);
         return new Constant(leftEqualsRight);
      }

      private readonly Symbol _notBooleanExpression = new Symbol("BooleanExpression");
      private Expression Not(IContext Context)
      {
         var booleanExpression = GetBoolean(Context, _notBooleanExpression);
         return new Constant(!booleanExpression);
      }

      private readonly Symbol _evalExpression = new Symbol("Expression");
      private Expression Eval(IContext Context)
      {
         return new ExpressionEvaluator(Context).Evaluate(GetGenericValue<Expression>(Context, _evalExpression));
      }

      private readonly Symbol _absValue = new Symbol("Value");
      private Expression Abs(IContext Context)
      {
         var number = GetNumber(Context, _absValue);
         return new Constant(BigRational.Abs(number));
      }

      private readonly Symbol _carList = new Symbol("List");
      private Expression Car(IContext Context)
      {
         var list = GetGenericValue<List>(Context, _carList);
         if (list.Expressions.Count == 0)
            throw new RuntimeException("Cannot get car of empty list", list, this);
         return list.Expressions.First();
      }

      private readonly Symbol _cdrList = new Symbol("List");
      private Expression Cdr(IContext Context)
      {
         var list = GetGenericValue<List>(Context, _cdrList);
         if (list.Expressions.Count == 0)
            throw new RuntimeException("Cannot get cdr of empty list", list, this);
         return new List(list.Expressions.Skip(1));
      }

      private readonly Symbol _appendListLeft = new Symbol("ListLeft");
      private readonly Symbol _appendListRight = new Symbol("ListRight");
      private Expression Append(IContext Context)
      {
         var listLeft = GetGenericValue<List>(Context, _appendListLeft);
         var listRight = GetGenericValue<List>(Context, _appendListRight);
         return new List(listLeft.Expressions.Concat(listRight.Expressions));
      }

      private readonly Symbol _printExpressions = new Symbol("Expressions");
      [ExcludeFromCodeCoverage]
      private Expression Print(IContext Context)
      {
         var expressions = GetGenericValue<List>(Context, _printExpressions);
         var printedExpressions = MacroPrinter.Print(expressions, true);
         if (_output != null)
            _output.PrintLine(printedExpressions);
         return new Constant(printedExpressions);
      }

      [ExcludeFromCodeCoverage]
      private Expression Time(IContext Context)
      {
         return Constant.Number(DateTime.Now.Ticks);
      }

      private readonly Symbol _timeStringTime = new Symbol("Time");
      [ExcludeFromCodeCoverage]
      private Expression TimeString(IContext Context)
      {
         var time = GetNumber(Context, _timeStringTime);
         return new Constant(new DateTime((long) time).ToString(CultureInfo.InvariantCulture));
      }

      private readonly Symbol _ocrImage = new Symbol("Image");
      private Expression Ocr(IContext Context)
      {
         var imageHexString = GetString(Context, _ocrImage);
         var image = Imaging.HexString2Image(imageHexString);
         var recognizedText = Imaging.ReadText(image);
         return new Constant(recognizedText);
      }

      private readonly Symbol _editDistanceLeft = new Symbol("Left");
      private readonly Symbol _editDistanceRight = new Symbol("Right");
      private Expression EditDistance(IContext Context)
      {
         var left = GetString(Context, _editDistanceLeft);
         var right = GetString(Context, _editDistanceRight);
         var editDistance = ComputeEditDistance(left, right);
         return Constant.Number(editDistance);
      }

      private readonly Symbol _substringString = new Symbol("String");
      private readonly Symbol _substringStartIndex = new Symbol("StartIndex");
      private readonly Symbol _substringLength = new Symbol("Length");
      [ExcludeFromCodeCoverage]
      private Expression Substring(IContext Context)
      {
         var str = GetString(Context, _substringString);
         var startIndex = (int)GetNumber(Context, _substringStartIndex);
         var length = (int)GetNumber(Context, _substringLength);
         var substring = str.Substring(startIndex, length);
         return new Constant(substring);
      }

      private readonly Symbol _stringLengthString = new Symbol("String");
      [ExcludeFromCodeCoverage]
      private Expression StringLength(IContext Context)
      {
         var str = GetString(Context, _stringLengthString);
         var strLength = str.Length;
         return new Constant(strLength);
      }

      private readonly Symbol _regexReplaceString = new Symbol("String");
      private readonly Symbol _regexReplaceRegexToReplace = new Symbol("RegexToReplace");
      private readonly Symbol _regexReplaceReplacement = new Symbol("Replacement");
      [ExcludeFromCodeCoverage]
      private Expression RegexReplace(IContext Context)
      {
         var str = GetString(Context, _regexReplaceString);
         var regexToReplace = GetString(Context, _regexReplaceRegexToReplace);
         var replacement = GetString(Context, _regexReplaceReplacement);
         var replaced = Regex.Replace(str, regexToReplace, replacement);
         return new Constant(replaced);
      }

      private readonly Symbol _regexMatchString = new Symbol("String");
      private readonly Symbol _regexMatchRegex = new Symbol("Regex");
      [ExcludeFromCodeCoverage]
      private Expression RegexMatch(IContext Context)
      {
         var str = GetString(Context, _regexMatchString);
         var regex = GetString(Context, _regexMatchRegex);

         var match = Regex.Match(str, regex);

         return match.Captures.Count > 0 ? new Constant(match.Captures[0].Value) : new Constant("");
      }

      private readonly Symbol _mouseMoveDeltaX = new Symbol("DeltaX");
      private readonly Symbol _mouseMoveDeltaY = new Symbol("DeltaY");
      private Constant MouseMove(IContext Context)
      {
         Mouse.X += (int)GetNumber(Context, _mouseMoveDeltaX);
         Mouse.Y += (int)GetNumber(Context, _mouseMoveDeltaY);

         return new Constant(true);
      }


      private readonly Symbol _mousePositionX = new Symbol("X");
      private readonly Symbol _mousePositionY = new Symbol("Y");
      private Constant MousePosition(IContext Context)
      {
         int screenX, screenY;
         Window.ClientToScreen(
            _windowHandle,
            (int)GetNumber(Context, _mousePositionX), (int)GetNumber(Context, _mousePositionY),
            out screenX, out screenY);
         Mouse.Position = new Mouse.MousePoint(screenX, screenY);

         return new Constant(true);
      }


      private readonly Symbol _pauseDuration = new Symbol("Duration");
      private Constant Pause(IContext Context)
      {
         var milliseconds = GetNumber(Context, _pauseDuration);
         Thread.Sleep((int)milliseconds);

         return new Constant(milliseconds);
      }

      [ExcludeFromCodeCoverage]
      private Constant LeftClick(IContext Context)
      {
         Mouse.LeftClick();

         return new Constant(true);
      }


      private readonly Symbol _wheelDelta = new Symbol("Delta");
      private Constant Wheel(IContext Context)
      {
         var delta = GetNumber(Context, _wheelDelta);
         Mouse.Wheel((int) delta);

         return new Constant(delta);
      }


      private readonly Symbol _windowshotX = new Symbol("X");
      private readonly Symbol _windowshotY = new Symbol("Y");
      private readonly Symbol _windowshotWidth = new Symbol("Width");
      private readonly Symbol _windowshotHeight = new Symbol("Height");
      [ExcludeFromCodeCoverage]
      private Constant Windowshot(IContext Context)
      {
         var x = (int)GetNumber(Context, _windowshotX);
         var y = (int)GetNumber(Context, _windowshotY);
         var width = (int)GetNumber(Context, _windowshotWidth);
         var height = (int)GetNumber(Context, _windowshotHeight);

         using (var windowCapture = Window.Capture(_windowHandle))
         {
            using (var croppedImage = Imaging.CropImage(windowCapture, x, y, width, height))
            {
               return new Constant(Imaging.Image2PngHexString(croppedImage));
            }
         }
      }

      private bool GetBoolean(IContext Context, Symbol Symbol)
      {
         try
         {
            return GetConstantValue<bool>(Context, Symbol);
         }
         catch (InvalidCastException)
         {
            var constantValue = GetGenericValue<Constant>(Context, Symbol).Value;
            var stringValue = constantValue as string;
            if (stringValue != null)
            {
               bool parsedValue;
               if (bool.TryParse(stringValue, out parsedValue))
                  return parsedValue;
            }
            throw;
         }
      }

      private string GetString(IContext Context, Symbol Symbol)
      {
         try
         {
            return GetConstantValue<string>(Context, Symbol);
         }
         catch (InvalidCastException)
         {
            var toStringValue = GetGenericValue<Constant>(Context, Symbol).Value.ToString();
            return toStringValue;
         }
      }

      private BigRational GetNumber(IContext Context, Symbol Symbol)
      {
         try
         {
            return GetConstantValue<BigRational>(Context, Symbol);
         }
         catch (RuntimeException)
         {
            var constantValue = GetGenericValue<Constant>(Context, Symbol).Value;
            var stringValue = constantValue as string;
            
            if (stringValue == null) 
               throw;

            try
            {
               return (BigRational)((Constant)_parser.Parse(stringValue)).Value;
            }
            catch (Exception parseException)
            {
               throw new RuntimeException(
                  string.Format("Symbol >> {0} <<: could not parse {1} to BigRational", Symbol, constantValue),
                  Symbol,
                  Context,
                  parseException);
            }
         }
      }

      private T GetConstantValue<T>(IContext Context, Symbol Symbol)
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

      private T GetGenericValue<T>(IContext Context, Symbol Symbol)
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


      // TODO - do not commit this code - testing purpuse only
      public static int ComputeEditDistance(string s, string t)
      {
         int n = s.Length;
         int m = t.Length;
         int[,] d = new int[n + 1, m + 1];

         // Step 1
         if (n == 0)
         {
            return m;
         }

         if (m == 0)
         {
            return n;
         }

         // Step 2
         for (int i = 0; i <= n; d[i, 0] = i++)
         {
         }

         for (int j = 0; j <= m; d[0, j] = j++)
         {
         }

         // Step 3
         for (int i = 1; i <= n; i++)
         {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
               // Step 5
               int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

               // Step 6
               d[i, j] = Math.Min(
                   Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                   d[i - 1, j - 1] + cost);
            }
         }
         // Step 7
         return d[n, m];
      }
   }
}
