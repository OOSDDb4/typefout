using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(SchoolId), "schoolId")]
public partial class SchoolEditViewModel : ObservableObject
{
    private readonly ISchoolRepo _schoolRepo;
    private School? _school;

    [ObservableProperty]
    private int _schoolId;

    [ObservableProperty]
    private string _schoolName = string.Empty;

    public SchoolEditViewModel(ISchoolRepo schoolRepo)
    {
        _schoolRepo = schoolRepo;
    }

    partial void OnSchoolIdChanged(int value)
    {
        LoadSchoolCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSchool()
    {
        try
        {
            _school = await _schoolRepo.GetByIdAsync(SchoolId);
            if (_school == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "School niet gevonden.", "OK");
                return;
            }

            SchoolName = _school.Name;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SaveEdit()
    {
        if (_school == null)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "School niet gevonden.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(SchoolName))
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Schoolnaam is verplicht.", "OK");
            return;
        }

        try
        {
            _school.Name = SchoolName.Trim();

            await _schoolRepo.UpdateAsync(_school);

            await Application.Current.MainPage.DisplayAlert("Gelukt", "School is bijgewerkt.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }
}
