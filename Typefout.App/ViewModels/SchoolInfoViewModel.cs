using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

[QueryProperty(nameof(SchoolId), "schoolId")]
public partial class SchoolInfoViewModel : ObservableObject
{
    private readonly IUserRepo _userRepo;
    private readonly IGroupRepo _groupRepo;
    private readonly ISchoolRepo _schoolRepo;

    private const int _pageSize = 10;

    private List<User> _allUsers = new();
    private List<Group> _allGroups = new();

    [ObservableProperty] private int _schoolId;
    [ObservableProperty] private string _schoolName = string.Empty;

    [ObservableProperty] private ObservableCollection<User> _users = new();
    [ObservableProperty] private ObservableCollection<Group> _groups = new();

    [ObservableProperty] private string _searchText = string.Empty;

    [ObservableProperty] private int _userPage = 1;
    [ObservableProperty] private int _userTotalPages = 1;

    [ObservableProperty] private int _groupPage = 1;
    [ObservableProperty] private int _groupTotalPages = 1;
    
    public string UserPageText => $"{UserPage} / {UserTotalPages}";
    public string GroupPageText => $"{GroupPage} / {GroupTotalPages}";

    public SchoolInfoViewModel(
        IUserRepo userRepo,
        IGroupRepo groupRepo,
        ISchoolRepo schoolRepo)
    {
        _userRepo = userRepo;
        _groupRepo = groupRepo;
        _schoolRepo = schoolRepo;
    }

    partial void OnSchoolIdChanged(int value)
    {
        LoadDataCommand.Execute(null);
    }
    partial void OnUserPageChanged(int value)
    {
        OnPropertyChanged(nameof(UserPageText));
    }

    partial void OnUserTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(UserPageText));
    }

    partial void OnGroupPageChanged(int value)
    {
        OnPropertyChanged(nameof(GroupPageText));
    }

    partial void OnGroupTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(GroupPageText));
    }
    
    [RelayCommand]
    private async Task LoadData()
    {
        School? school = await _schoolRepo.GetByIdAsync(SchoolId);
        SchoolName = school?.Name ?? string.Empty;

        IEnumerable<User> users = await _userRepo.GetAllAsync();
        _allUsers = users.Where(u => u.SchoolId == SchoolId).ToList();

        IEnumerable<Group> groups = await _groupRepo.GetBySchoolIdAsync(SchoolId);
        _allGroups = groups.ToList();

        Dictionary<int, Group> groupById =
            _allGroups.ToDictionary(g => g.Id);

        Dictionary<int, User> teacherById =
            _allUsers
                .Where(u => u.UserType == UserType.Docent)
                .ToDictionary(u => u.Id);

        foreach (User user in _allUsers)
        {
            if (user.GroupId.HasValue &&
                groupById.TryGetValue(user.GroupId.Value, out Group group))
            {
                user.GroupName = group.Name;
            }
            else
            {
                user.GroupName = string.Empty;
            }
        }

        foreach (Group group in _allGroups)
        {
            if (group.TeacherId.HasValue &&
                teacherById.TryGetValue(group.TeacherId.Value, out User teacher))
            {
                group.TeacherName = teacher.Username;
            }
        }

        ApplyUserPaging();
        ApplyGroupPaging();
    }


    partial void OnSearchTextChanged(string value)
    {
        UserPage = 1;
        ApplyUserPaging();
    }

    private void ApplyUserPaging()
    {
        IEnumerable<User> filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allUsers
            : _allUsers.Where(u =>
                u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        UserTotalPages = Math.Max(1, (int)Math.Ceiling(filtered.Count() / (double)_pageSize));

        Users.Clear();
        foreach (User user in filtered
                     .Skip((UserPage - 1) * _pageSize)
                     .Take(_pageSize))
        {
            Users.Add(user);
        }
    }

    private void ApplyGroupPaging()
    {
        GroupTotalPages = Math.Max(1, (int)Math.Ceiling(_allGroups.Count / (double)_pageSize));

        Groups.Clear();
        foreach (Group group in _allGroups
                     .Skip((GroupPage - 1) * _pageSize)
                     .Take(_pageSize))
        {
            Groups.Add(group);
        }
    }

    [RelayCommand]
    private void NextUserPage()
    {
        if (UserPage < UserTotalPages)
        {
            UserPage++;
            ApplyUserPaging();
        }
    }

    [RelayCommand]
    private void PreviousUserPage()
    {
        if (UserPage > 1)
        {
            UserPage--;
            ApplyUserPaging();
        }
    }

    [RelayCommand]
    private void NextGroupPage()
    {
        if (GroupPage < GroupTotalPages)
        {
            GroupPage++;
            ApplyGroupPaging();
        }
    }

    [RelayCommand]
    private void PreviousGroupPage()
    {
        if (GroupPage > 1)
        {
            GroupPage--;
            ApplyGroupPaging();
        }
    }

    [RelayCommand]
    private async Task AddUser()
    {
        await Shell.Current.GoToAsync($"adduser?schoolId={SchoolId}");
    }

    [RelayCommand]
    private async Task AddGroup()
    {
        await Shell.Current.GoToAsync($"groupcreate?schoolId={SchoolId}");
    }

    [RelayCommand]
    private async Task EditUser(User user)
    {
        string route = user.UserType == UserType.Docent
            ? $"teacheredit?userId={user.Id}"
            : $"studentedit?userId={user.Id}";

        await Shell.Current.GoToAsync(route);
    }

    [RelayCommand]
    private async Task EditGroup(Group group)
    {
        await Shell.Current.GoToAsync($"groupedit?groupId={group.Id}");
    }
}
