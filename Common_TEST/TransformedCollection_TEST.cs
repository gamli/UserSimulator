using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common_TEST
{
   [TestClass]
   public class TransformedCollection_TEST
   {
      [TestMethod]
      public void Transformed_Property_TEST()
      {
         var testCollection = new ObservableCollection<object> { 0 };
         var releaseCounter = 0;
         using (var transformedCollection =
            new TransformedCollection<object, object>(
               testCollection,
               BoxedIntValue => -((int)BoxedIntValue),
               12345678,
               BoxedIntValue => releaseCounter++))
         {
            Assert.AreEqual(0, releaseCounter);
            Assert.AreEqual(1, transformedCollection.Transformed.Count);
            testCollection.Clear();
            Assert.AreEqual(1, releaseCounter);
            Assert.AreEqual(0, transformedCollection.Transformed.Count);
            testCollection.Add(4711);
            Assert.AreEqual(1, transformedCollection.Transformed.Count);
            Assert.AreEqual(-4711, transformedCollection.Transformed[0]);
            testCollection.Add(1147);
            Assert.AreEqual(2, transformedCollection.Transformed.Count);
            Assert.AreEqual(-4711, transformedCollection.Transformed[0]);
            Assert.AreEqual(-1147, transformedCollection.Transformed[1]);
            testCollection.Remove(4711);
            Assert.AreEqual(2, releaseCounter);
            Assert.AreEqual(1, transformedCollection.Transformed.Count);
            Assert.AreEqual(-1147, transformedCollection.Transformed[0]);
            testCollection[0] = 13;
            Assert.AreEqual(3, releaseCounter);
            Assert.AreEqual(1, transformedCollection.Transformed.Count);
            Assert.AreEqual(-13, transformedCollection.Transformed[0]);
            testCollection.Add(null);
            Assert.AreEqual(2, transformedCollection.Transformed.Count);
            Assert.AreEqual(12345678, transformedCollection.Transformed[1]);
         }
         Assert.AreEqual(5, releaseCounter);
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void MoveNotSupportedTest_TEST()
      {
         var testCollection = new ObservableCollection<object> { 0, 1, 2 };
         var releaseCounter = 0;
         using (var transformedCollection =
            new TransformedCollection<object, object>(
               testCollection,
               BoxedIntValue => -((int)BoxedIntValue),
               12345678,
               BoxedIntValue => releaseCounter++))
         {
            try
            {
               testCollection.Move(0, 1);
               Assert.Fail();
            }
            catch (NotImplementedException)
            {
               // everything ok
            }
         }
      }

      [TestMethod]
      [ExcludeFromCodeCoverage]
      public void IllegalCollectionChangedEventArgs_TEST()
      {
         var testCollection = new ObservableCollection<object> { 0 };
         var releaseCounter = 0;
         using (var transformedCollection =
            new TransformedCollection<object, object>(
               testCollection,
               BoxedIntValue => -((int)BoxedIntValue),
               12345678,
               BoxedIntValue => releaseCounter++))
         {
            var eventName = "CollectionChanged";
            var eventInfo = testCollection.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var eventDelegate =
               (MulticastDelegate)testCollection.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(testCollection);

            Assert.IsNotNull(eventDelegate);

            var illegalEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            var fields = illegalEventArgs.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            Console.Write(fields);
            illegalEventArgs.GetType().GetField("_action", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(illegalEventArgs, 5);
            try
            {
               foreach (var handler in eventDelegate.GetInvocationList())
                  handler.Method.Invoke(handler.Target, new object[] { testCollection, illegalEventArgs });
               Assert.Fail();
            }
            catch (TargetInvocationException Ex)
            {
               // everything ok
               Assert.AreEqual(Ex.InnerException.GetType(), typeof(ArgumentException));
            }
         }
      }
   }
}
