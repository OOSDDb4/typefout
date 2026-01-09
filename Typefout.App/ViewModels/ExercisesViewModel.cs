using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public partial class ExercisesViewModel : ObservableObject
    {
        private readonly IExerciseRepo _exerciseRepo;

        [ObservableProperty]
        private ObservableCollection<ExerciseRowViewModel> _exercises;

        [ObservableProperty]
        private bool _isLoading;

        public ExercisesViewModel(IExerciseRepo exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
            _exercises = new ObservableCollection<ExerciseRowViewModel>();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsLoading) return;

            IsLoading = true;

            List<Exercise> list = await _exerciseRepo.GetAllAsync();
            Exercises.Clear();

            foreach (Exercise e in list)
            {
                ExerciseRowViewModel row = new ExerciseRowViewModel
                {
                    ExerciseId = e.ExerciseId,
                    ExerciseName = e.ExerciseName,
                    ExerciseActive = e.ExerciseActive
                };

                Exercises.Add(row);
            }

            IsLoading = false;
        }

        [RelayCommand]
        public async Task ToggleGlobalAsync(ExerciseRowViewModel row)
        {
            if (row == null) return;

            bool newValue = !row.ExerciseActive;
            await _exerciseRepo.SetGlobalActiveAsync(row.ExerciseId, newValue);
            row.ExerciseActive = newValue;
        }
    }

    public partial class ExerciseRowViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _exerciseId;

        [ObservableProperty]
        private string _exerciseName = string.Empty;

        [ObservableProperty]
        private bool _exerciseActive;
    }
}