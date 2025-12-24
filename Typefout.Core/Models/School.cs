namespace Typefout.Core.Models
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int GroupCount { get; set; }
    }
}