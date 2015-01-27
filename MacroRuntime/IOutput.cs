namespace MacroRuntime
{
   public interface IOutput
   {
      void Print(string Text);
      void PrintLine(string Text = "");
   }
}