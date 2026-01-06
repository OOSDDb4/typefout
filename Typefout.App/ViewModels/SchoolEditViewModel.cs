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
        _school = await _schoolRepo.GetByIdAsync(SchoolId);
        if (_school == null) return;

        SchoolName = _school.Name;
    }

    [RelayCommand]
    private async Task SaveEdit()
    {
        if (_school == null) return;
        if (string.IsNullOrWhiteSpace(SchoolName)) return;

        _school.Name = SchoolName;

        await _schoolRepo.UpdateAsync(_school);
        await Shell.Current.GoToAsync("..");
    }
}