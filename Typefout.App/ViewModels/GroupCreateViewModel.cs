using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(SchoolId), "schoolId")]
public partial class GroupCreateViewModel : ObservableObject
{
    private readonly IGroupRepo _groupRepo;

    [ObservableProperty]
    private int _schoolId;

    [ObservableProperty]
    private string _groupName = string.Empty;

    [ObservableProperty]
    private string _teacherName = string.Empty;

    public GroupCreateViewModel(IGroupRepo groupRepo)
    {
        _groupRepo = groupRepo;
    }

    [RelayCommand]
    private async Task CreateGroup()
    {
        if (string.IsNullOrWhiteSpace(GroupName))
        {
            return;
        }

        Group group = new Group
        {
            Name = GroupName,
            SchoolId = SchoolId,
            TeacherId = null,
            TeacherName = TeacherName
        };

        await _groupRepo.CreateAsync(group);
        await Shell.Current.GoToAsync("..");
    }
}