namespace Typefout.Core.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public int SchoolId { get; set; }
    public int? TeacherId { get; set; }
    public string TeacherName { get; set; } = String.Empty;
}