using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace MacroView
{
   /// <summary>
   /// Interaction logic for BlockView.xaml
   /// </summary>
   public partial class ListView
   {
      public ListView()
      {
         InitializeComponent();
      }

      private void FilterTail(object Sender, FilterEventArgs E)
      {
         E.Accepted = !ReferenceEquals(E.Item, ((IEnumerable<object>)((CollectionViewSource) Sender).Source).First());
      }
   }
}
