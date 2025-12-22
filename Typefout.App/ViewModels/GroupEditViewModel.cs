using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(GroupId), "groupId")]
public partial class GroupEditViewModel : ObservableObject
{
    private readonly IGroupRepo _groupRepo;

    private Group? _group;

    [ObservableProperty] private int _groupId;
    [ObservableProperty] private string _groupName = string.Empty;
    [ObservableProperty] private string _selectedTeacherName = string.Empty;

    public GroupEditViewModel(IGroupRepo groupRepo)
    {
        _groupRepo = groupRepo;
    }

    partial void OnGroupIdChanged(int value)
    {
        LoadGroupCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadGroup()
    {
        _group = await _groupRepo.GetByIdAsync(GroupId);
        if (_group == null) return;

        GroupName = _group.Name;
        SelectedTeacherName = _group.TeacherName;
    }

    [RelayCommand]
    private async Task SaveGroup()
    {
        if (_group == null) return;
        if (string.IsNullOrWhiteSpace(GroupName)) return;

        _group.Name = GroupName;
        _group.TeacherName = SelectedTeacherName;

        await _groupRepo.UpdateAsync(_group);
        await Shell.Current.GoToAsync("..");
    }
}