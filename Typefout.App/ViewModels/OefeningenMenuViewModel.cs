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
        private readonly IUserRepo _userRepo;

        [ObservableProperty]
        private bool _showWord;

        [ObservableProperty]
        private bool _showSentence;

        [ObservableProperty]
        private bool _showText;
        
        [ObservableProperty]
        private int _score;

        public OefeningenMenuViewModel(
            ISchoolExerciseRepo schoolExerciseRepo,
            IAuthService authService,
            IUserRepo userRepo)
        {
            _schoolExerciseRepo = schoolExerciseRepo;
            _authService = authService;
            _userRepo = userRepo;
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
            Score = _userRepo.SelectScore(currentUser);
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
