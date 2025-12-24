using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(GroupId), "groupId")]
public partial class GroupEditViewModel : ObservableObject
{
    private readonly IGroupRepo _groupRepo;
    private readonly IUserRepo _userRepo;

    private Group? _group;

    [ObservableProperty]
    private int _groupId;

    [ObservableProperty]
    private string _groupName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<User> _teachers = new();

    [ObservableProperty]
    private User? _selectedTeacher;

    public GroupEditViewModel(IGroupRepo groupRepo, IUserRepo userRepo)
    {
        _groupRepo = groupRepo;
        _userRepo = userRepo;
    }

    partial void OnGroupIdChanged(int value)
    {
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadData()
    {
        _group = await _groupRepo.GetByIdAsync(GroupId);
        if (_group == null)
            return;

        GroupName = _group.Name;

        IEnumerable<User> users = await _userRepo.GetAllAsync();

        Teachers.Clear();

        foreach (User user in users)
        {
            if (user.SchoolId == _group.SchoolId && user.UserType == UserType.Docent)
            {
                Teachers.Add(user);

                if (_group.TeacherId.HasValue && user.Id == _group.TeacherId.Value)
                {
                    SelectedTeacher = user;
                }
            }
        }
    }

    [RelayCommand]
    private async Task SaveGroup()
    {
        if (_group == null)
            return;

        if (string.IsNullOrWhiteSpace(GroupName))
            return;

        _group.Name = GroupName;
        _group.TeacherId = SelectedTeacher?.Id;
        _group.TeacherName = SelectedTeacher?.Username ?? string.Empty;

        await _groupRepo.UpdateAsync(_group);
        await Shell.Current.GoToAsync("..");
    }
}
