using System;
using System.Collections.ObjectModel;
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
   }
}
