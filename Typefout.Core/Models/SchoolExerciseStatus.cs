namespace Typefout.Core.Models
{
    public class SchoolExerciseStatus
    {
        public int SchoolId { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;

        public bool GlobalActive { get; set; }
        public bool Linked { get; set; }
        public bool SchoolActive { get; set; }
        public bool ShowLinkButton
        {
            get { return !Linked; }
        }

        public bool ShowUnlinkButton
        {
            get { return Linked; }
        }

        public bool CanToggleSchool
        {
            get { return Linked && GlobalActive; }
        }
    }
}