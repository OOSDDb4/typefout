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
            await Application.Current.MainPage.DisplayAlert(
                "Fout",
                "Schoolnaam is verplicht.",
                "OK");
            return;
        }

        try
        {
            School school = new School
            {
                Name = SchoolName.Trim()
            };

            await _schoolRepo.CreateAsync(school);

            await Application.Current.MainPage.DisplayAlert(
                "Gelukt",
                "School is aangemaakt.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout",
                ex.Message,
                "OK");
        }
    }
}