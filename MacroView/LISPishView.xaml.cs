﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MacroRuntime;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for LISPishView.xaml
   /// </summary>
   public partial class LISPishView
   {
      private readonly LispishSyntaxHighlighter _syntaxHighlighter = new LispishSyntaxHighlighter();

      public LISPishView()
      {
         InitializeComponent();

         _editor.PreviewKeyDown +=
            (Sender, Args) =>
            {
               if (Formatter != null &&
                     (Args.Key == Key.F) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                  Formatter(_editor.Text);
            };

         _editor.LostFocus +=
            (Sender, Args) =>
            {
               Text = _editor.Text;
            };

         _editor.TextArea.Caret.PositionChanged += 
            (Sender, Args) => CursorPosition = _editor.CaretOffset;

         var textView = _editor.TextArea.TextView;
         textView.LineTransformers.Add(_syntaxHighlighter);
         textView.ElementGenerators.Add(new ImageConstant());
      }

      public Action<string> Formatter { get; set; }

      private void ButtonEvaluate_OnClick(object Sender, RoutedEventArgs E)
      {
         REPL.Reset();
         REPL.ConsumeInput(Text);
      }
      private void ButtonParsePreview_OnClick(object Sender, RoutedEventArgs E)
      {
         REPL.ParsePreview(Text);
      }

      public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
         "Text", typeof(string), typeof(LISPishView),
         new FrameworkPropertyMetadata(
            default(string),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            Text_PropertyCallback));
      public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }
      private static void Text_PropertyCallback(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
      {
         var editor = ((LISPishView)Sender)._editor;
         var newValue = (string)Args.NewValue;
         if (!Equals(editor.Text, newValue))
         {
            editor.Text = newValue;
         }
      }

      // TODO bad name - something like CaretPosition seems more appropriate 
      public static readonly DependencyProperty CursorPositionProperty = DependencyProperty.Register(
         "CursorPosition", typeof(int), typeof(LISPishView), 
         new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
            CursorPosition_PropertyCallback));
      public int CursorPosition { get { return (int) GetValue(CursorPositionProperty); } set { SetValue(CursorPositionProperty, value); } }
      private static void CursorPosition_PropertyCallback(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
      {
         var lispishView = ((LISPishView)Sender);
         lispishView._editor.CaretOffset = lispishView.CursorPosition;
      }

      public static readonly DependencyProperty REPLProperty = DependencyProperty.Register(
         "REPL", typeof(REPL), typeof(LISPishView), new PropertyMetadata(default(REPL), REPL_PropertyCallback));
      public REPL REPL { get { return (REPL)GetValue(REPLProperty); } set { SetValue(REPLProperty, value); } }

      private static void REPL_PropertyCallback(DependencyObject Sender, DependencyPropertyChangedEventArgs Args)
      {
         var lispishView = ((LISPishView)Sender);

         var oldValue = Args.OldValue as REPL;
         if (oldValue != null)
            oldValue.PropertyChanged -= lispishView.REPL_OnPropertyChanged;

         var newValue = Args.NewValue as REPL;
         if (newValue != null)
            newValue.PropertyChanged += lispishView.REPL_OnPropertyChanged;

         lispishView.UpdateSyntaxHighlighterError();
      }

      private void REPL_OnPropertyChanged(object Sender, PropertyChangedEventArgs Args)
      {
         if (Args.PropertyName == "LastErrorPosition" || Args.PropertyName == "LastErrorLength")
            UpdateSyntaxHighlighterError();
      }

      private void UpdateSyntaxHighlighterError()
      {
         _syntaxHighlighter.LastErrorPosition = REPL.LastErrorPosition;
         _syntaxHighlighter.LastErrorLength = REPL.LastErrorLength;
         RepaintEditor();
      }

      private void RepaintEditor()
      {
         var text = _editor.Text;
         _editor.Text = "";
         _editor.Text = text;
      }
   }
}
