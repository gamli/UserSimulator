using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public class TransformedCollection<TSource, T> : DisposableBase
   {
      private ObservableCollection<TSource> _source;
      private Func<TSource, T> _selector;
      private T _sourceNullValue;
      private Action<T> _release;
      private ObservableCollection<T> _transformed;
      public ReadOnlyObservableCollection<T> Transformed { get; private set; }

      public TransformedCollection(
         ObservableCollection<TSource> SourceCollection, Func<TSource, T> Selector, T SourceNullValue = default(T), Action<T> Release = null)
      {
         _source = SourceCollection;
         _selector = Selector;
         _sourceNullValue = SourceNullValue;
         _release = Release;
         _transformed = new ObservableCollection<T>();
         Transformed = new ReadOnlyObservableCollection<T>(_transformed);
         _source.CollectionChanged += SourceCollectionChanged;
         InitTransformedFromSource();
      }

      private void InitTransformedFromSource()
      {
         ClearTransformed();
         foreach (var sourceItem in _source)
            _transformed.Add(Transform(sourceItem));
      }

      void SourceCollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
      {
         switch (Args.Action)
         {
            case NotifyCollectionChangedAction.Add:
               Contract.Assert(Args.OldItems == null);
               Contract.Assert(Args.OldStartingIndex == -1);
               Contract.Assert(Args.NewItems.Count > -1);
               Contract.Assert(Args.NewStartingIndex > -1);

               for (var i = 0; i < Args.NewItems.Count; i++)
                  _transformed.Insert(Args.NewStartingIndex + i, Transform(Args.NewItems[i]));
               break;
            case NotifyCollectionChangedAction.Move:
               throw new NotImplementedException();
            case NotifyCollectionChangedAction.Remove:
               Contract.Assert(Args.OldItems.Count > -1);
               Contract.Assert(Args.OldStartingIndex > -1);
               Contract.Assert(Args.NewItems == null);
               Contract.Assert(Args.NewStartingIndex == -1);

               for (var i = 0; i < Args.OldItems.Count; i++)
               {
                  Release(_transformed[Args.OldStartingIndex]);
                  _transformed.RemoveAt(Args.OldStartingIndex);
               }
               break;
            case NotifyCollectionChangedAction.Replace:
               Contract.Assert(Args.OldItems.Count > -1);
               Contract.Assert(Args.OldStartingIndex > -1);
               Contract.Assert(Args.NewItems.Count > -1);
               Contract.Assert(Args.NewStartingIndex > -1);
               Contract.Equals(Args.OldItems.Count, Args.NewItems.Count);
               Contract.Equals(Args.OldStartingIndex, Args.NewStartingIndex);

               for (var i = 0; i < Args.NewItems.Count; i++)
               {
                  var newItemIndex = Args.NewStartingIndex + i;
                  Release(_transformed[newItemIndex]);
                  _transformed[newItemIndex] = Transform(Args.NewItems[i]);
               }
               break;
            case NotifyCollectionChangedAction.Reset:
               InitTransformedFromSource();
               break;
            default:
               break;
         }
      }

      private T Transform(object SourceItem)
      {
         return Transform((TSource)SourceItem);
      }

      private T Transform(TSource SourceItem)
      {
         if (SourceItem != null)
            return _selector(SourceItem);
         return _sourceNullValue;
      }

      private void ClearTransformed()
      {
         foreach (var transformedItem in _transformed)
            Release(transformedItem);
         _transformed.Clear();
      }

      private void Release(T TransformedItem)
      {
         if (_release != null)
            _release(TransformedItem);
      }

      protected override void Dispose(bool Disposing)
      {
         if (Disposing)
         {
            _source.CollectionChanged -= SourceCollectionChanged;
            ClearTransformed();
         }
         base.Dispose(Disposing);
      }
   }
}
