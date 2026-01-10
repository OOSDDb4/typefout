using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class OefeningenMenuViewModel : ObservableObject
    {
        private readonly ISchoolExerciseRepo _schoolExerciseRepo;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private bool _showWord;

        [ObservableProperty]
        private bool _showSentence;

        [ObservableProperty]
        private bool _showText;

        public OefeningenMenuViewModel(
            ISchoolExerciseRepo schoolExerciseRepo,
            IAuthService authService)
        {
            _schoolExerciseRepo = schoolExerciseRepo;
            _authService = authService;

            ShowWord = false;
            ShowSentence = false;
            ShowText = false;
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            User currentUser = _authService.CurrentUser;
            int schoolId = currentUser.SchoolId;

            List<Exercise> availableExercises =
                await _schoolExerciseRepo.GetAvailableExercisesForSchoolAsync(schoolId);

            bool word = false;
            bool sentence = false;
            bool text = false;

            foreach (Exercise exercise in availableExercises)
            {
                if (exercise.ExerciseName.Equals("word", StringComparison.OrdinalIgnoreCase))
                {
                    word = true;
                }
                else if (exercise.ExerciseName.Equals("sentence", StringComparison.OrdinalIgnoreCase))
                {
                    sentence = true;
                }
                else if (exercise.ExerciseName.Equals("text", StringComparison.OrdinalIgnoreCase))
                {
                    text = true;
                }
            }

            ShowWord = word;
            ShowSentence = sentence;
            ShowText = text;
        }
    }
}
