using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(SchoolId), "schoolId")]
public partial class AddUserViewModel : ObservableObject
{
    private readonly IUserRepo _userRepo;
    private readonly IGroupRepo _groupRepo;

    [ObservableProperty]
    private int _schoolId;

    [ObservableProperty]
    private bool _isStudentSelected;

    [ObservableProperty]
    private bool _isTeacherSelected;

    [ObservableProperty]
    private ObservableCollection<Group> _groups;

    [ObservableProperty]
    private Group? _selectedGroup;

    [ObservableProperty]
    private string _studentUsername;

    [ObservableProperty]
    private string _studentPassword;

    [ObservableProperty]
    private string _studentPasswordRepeat;

    [ObservableProperty]
    private string _teacherUsername;

    [ObservableProperty]
    private string _teacherEmail;

    [ObservableProperty]
    private string _teacherPassword;

    [ObservableProperty]
    private string _teacherPasswordRepeat;

    public AddUserViewModel(IUserRepo userRepo, IGroupRepo groupRepo)
    {
        _userRepo = userRepo;
        _groupRepo = groupRepo;

        _groups = new ObservableCollection<Group>();

        _studentUsername = string.Empty;
        _studentPassword = string.Empty;
        _studentPasswordRepeat = string.Empty;

        _teacherUsername = string.Empty;
        _teacherEmail = string.Empty;
        _teacherPassword = string.Empty;
        _teacherPasswordRepeat = string.Empty;
    }

    partial void OnSchoolIdChanged(int value)
    {
        LoadGroupsCommand.Execute(null);
    }

    partial void OnIsStudentSelectedChanged(bool value)
    {
        if (value)
        {
            IsTeacherSelected = false;
        }
    }

    partial void OnIsTeacherSelectedChanged(bool value)
    {
        if (value)
        {
            IsStudentSelected = false;
        }
    }

    [RelayCommand]
    private async Task LoadGroups()
    {
        IEnumerable<Group> groups = await _groupRepo.GetBySchoolIdAsync(SchoolId);

        Groups.Clear();

        foreach (Group group in groups)
        {
            Groups.Add(group);
        }

        if (Groups.Count == 0)
        {
            SelectedGroup = null;
        }
    }

    [RelayCommand]
    private async Task CreateStudent()
    {
        if (!IsStudentSelected)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(StudentUsername))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(StudentPassword) || StudentPassword != StudentPasswordRepeat)
        {
            return;
        }

        User user = new User
        {
            Username = StudentUsername,
            Password = PasswordHelper.HashPassword(StudentPassword),
            UserType = UserType.Leerling,
            SchoolId = SchoolId,
            GroupId = SelectedGroup != null ? SelectedGroup.Id : 0,
            IsActive = true
        };

        await _userRepo.CreateAsync(user);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CreateTeacher()
    {
        if (!IsTeacherSelected)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(TeacherUsername))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(TeacherEmail))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(TeacherPassword) || TeacherPassword != TeacherPasswordRepeat)
        {
            return;
        }

        User user = new User
        {
            Username = TeacherUsername.Trim(),
            Email = TeacherEmail.Trim(),
            Password = PasswordHelper.HashPassword(TeacherPassword),
            UserType = UserType.Docent,
            SchoolId = SchoolId,
            GroupId = 0,
            IsActive = true
        };

        await _userRepo.CreateAsync(user);
        await Shell.Current.GoToAsync("..");
    }
}
