using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

public partial class SchoolCreateViewModel : ObservableObject
{
    private readonly ISchoolRepo _schoolRepo;

    [ObservableProperty]
    private string _schoolName;

    public SchoolCreateViewModel(ISchoolRepo schoolRepo)
    {
        _schoolRepo = schoolRepo;
        _schoolName = string.Empty;
    }

    [RelayCommand]
    private async Task CreateSchool()
    {
        if (string.IsNullOrWhiteSpace(SchoolName))
        {
            return;
        }

        School school = new School
        {
            Name = SchoolName
        };

        await _schoolRepo.CreateAsync(school);
        await Shell.Current.GoToAsync("..");
    }
}
