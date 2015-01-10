using System.Linq;
using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class ProcedureCall_TEST
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var procedureCall = new ProcedureCall { Procedure = new Symbol("foo") };
         procedureCall.Expressions.Add(new Constant(4711));
         var clone = MacroCloner.Clone(procedureCall);
         Assert.AreEqual(procedureCall, clone);
         Assert.IsTrue(procedureCall.Arguments.SequenceEqual(clone.Arguments));

         procedureCall.Procedure = new Symbol("bar");
         Assert.AreNotEqual(procedureCall, clone);
         Assert.IsTrue(procedureCall.Arguments.SequenceEqual(clone.Arguments));
         procedureCall.Procedure = new Symbol("foo");
         Assert.AreEqual(procedureCall, clone);
         Assert.IsTrue(procedureCall.Arguments.SequenceEqual(clone.Arguments));

         procedureCall.Expressions.RemoveAt(procedureCall.Expressions.Count - 1);
         Assert.AreNotEqual(procedureCall, clone);
         Assert.IsFalse(procedureCall.Arguments.SequenceEqual(clone.Arguments));
         procedureCall.Expressions.Add(new Constant(4711));
         Assert.AreEqual(procedureCall, clone);
         Assert.IsTrue(procedureCall.Arguments.SequenceEqual(clone.Arguments));
      }
   }
}
