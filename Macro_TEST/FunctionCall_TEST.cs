using System;
using Macro;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class FunctionCall_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var functionCall = new FunctionCall { Function = new Symbol("foo") };
         functionCall.Expressions.Add(new Constant(4711));
         var clone = MacroCloner.Clone(functionCall);
         Assert.AreEqual(functionCall, clone);
         Assert.IsTrue(functionCall.Arguments.SequenceEqual(clone.Arguments));

         functionCall.Function = new Symbol("bar");
         Assert.AreNotEqual(functionCall, clone);
         Assert.IsTrue(functionCall.Arguments.SequenceEqual(clone.Arguments));
         functionCall.Function = new Symbol("foo");
         Assert.AreEqual(functionCall, clone);
         Assert.IsTrue(functionCall.Arguments.SequenceEqual(clone.Arguments));

         functionCall.Expressions.RemoveAt(functionCall.Expressions.Count - 1);
         Assert.AreNotEqual(functionCall, clone);
         Assert.IsFalse(functionCall.Arguments.SequenceEqual(clone.Arguments));
         functionCall.Expressions.Add(new Constant(4711));
         Assert.AreEqual(functionCall, clone);
         Assert.IsTrue(functionCall.Arguments.SequenceEqual(clone.Arguments));
      }
   }
}
