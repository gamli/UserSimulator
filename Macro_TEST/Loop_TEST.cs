using Macro;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Macro_TEST
{
   [TestClass]
   public class Loop_TEST : List_TEST_Base<ExpressionBase>
   {
      [TestMethod]
      public void CloneAndEqualsAndAccept_TEST()
      {
         var forLoop = new Loop { Condition = new Constant(true), Body = new Constant(4711) };
         var clone = MacroCloner.Clone(forLoop);
         AssertListsAreEqual(forLoop, clone);


         forLoop.Body = new Constant(-4711);
         AssertListsAreNotEqual(forLoop, clone);
         forLoop.Body = new Constant(4711);
         AssertListsAreEqual(forLoop, clone);
         
         forLoop.Condition = new Constant(false);
         AssertListsAreNotEqual(forLoop, clone);
         forLoop.Condition = new Constant(true);
         AssertListsAreEqual(forLoop, clone);
      }
   }
}
