using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels;

public partial class SchoolsViewModel : ObservableObject
{
    private readonly ISchoolRepo _schoolRepo;
    private readonly IGroupRepo _groupRepo;
    private readonly IUserRepo _userRepo;

    private const int _pageSize = 10;

    private List<School> _allSchools = new();

    [ObservableProperty]
    private ObservableCollection<School> _schools = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private int _page = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    public string PageText => $"{Page} / {TotalPages}";

    public SchoolsViewModel(
        ISchoolRepo schoolRepo,
        IGroupRepo groupRepo,
        IUserRepo userRepo)
    {
        _schoolRepo = schoolRepo;
        _groupRepo = groupRepo;
        _userRepo = userRepo;
    }

    partial void OnPageChanged(int value)
    {
        OnPropertyChanged(nameof(PageText));
    }

    partial void OnTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(PageText));
    }

    partial void OnSearchTextChanged(string value)
    {
        Page = 1;
        ApplyPaging();
    }

    [RelayCommand]
    private async Task LoadSchools()
    {
        IsLoading = true;

        IEnumerable<School> schools = await _schoolRepo.GetAllAsync();
        IEnumerable<Group> allGroups = await _groupRepo.GetAllAsync();
        IEnumerable<User> allUsers = await _userRepo.GetAllAsync();

        _allSchools = schools.ToList();

        foreach (School school in _allSchools)
        {
            school.GroupCount =
                allGroups.Count(g => g.SchoolId == school.Id);

            school.StudentCount =
                allUsers.Count(u =>
                    u.SchoolId == school.Id &&
                    u.UserType == UserType.Leerling);
        }

        ApplyPaging();

        IsLoading = false;
    }

    private void ApplyPaging()
    {
        IEnumerable<School> filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allSchools
            : _allSchools.Where(s =>
                s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        TotalPages = Math.Max(1,
            (int)Math.Ceiling(filtered.Count() / (double)_pageSize));

        Schools.Clear();

        foreach (School school in filtered
                     .Skip((Page - 1) * _pageSize)
                     .Take(_pageSize))
        {
            Schools.Add(school);
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        if (Page < TotalPages)
        {
            Page++;
            ApplyPaging();
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (Page > 1)
        {
            Page--;
            ApplyPaging();
        }
    }

    [RelayCommand]
    private async Task AddSchool()
    {
        await Shell.Current.GoToAsync("schoolcreate");
    }

    [RelayCommand]
    private async Task EditSchool(School school)
    {
        await Shell.Current.GoToAsync($"schooledit?schoolId={school.Id}");
    }

    [RelayCommand]
    private async Task ViewInfo(School school)
    {
        await Shell.Current.GoToAsync($"schoolinfo?schoolId={school.Id}");
    }
}
