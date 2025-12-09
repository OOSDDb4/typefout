namespace Typefout.Core.Models
{
    public class KeyStat
    {
        public char Key { get; set; }
        public int Attempts { get; set; }
        public int Mistakes { get; set; }

        public double ErrorRate =>
            Attempts == 0 ? 0 : (double)Mistakes / Attempts;

        public string ErrorPercentage =>
            $"{(ErrorRate * 100):0.0}%";
    }
}