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
    private ObservableCollection<User> _teachers = new ObservableCollection<User>();

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
        try
        {
            _group = await _groupRepo.GetByIdAsync(GroupId);
            if (_group == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Groep niet gevonden.", "OK");
                return;
            }

            GroupName = _group.Name;

            IEnumerable<User> users = await _userRepo.GetAllAsync();

            Teachers.Clear();
            SelectedTeacher = null;

            foreach (User user in users)
            {
                if (user.SchoolId == _group.SchoolId && user.UserType == UserType.Docent)
                {
                    Teachers.Add(user);
                    int groupTeacherId = 0;

                    if (_group.TeacherId != null)
                    {
                        groupTeacherId = Convert.ToInt32(_group.TeacherId);
                    }

                    if (groupTeacherId > 0 && user.Id == groupTeacherId)
                    {
                        SelectedTeacher = user;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SaveGroup()
    {
        if (_group == null)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Groep niet gevonden.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(GroupName))
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Groepsnaam is verplicht.", "OK");
            return;
        }

        try
        {
            _group.Name = GroupName.Trim();
            if (SelectedTeacher == null)
            {
                _group.TeacherId = null;
                _group.TeacherName = string.Empty;
            }
            else
            {
                _group.TeacherId = SelectedTeacher.Id;
                _group.TeacherName = SelectedTeacher.Username ?? string.Empty;
            }

            await _groupRepo.UpdateAsync(_group);

            await Application.Current.MainPage.DisplayAlert("Gelukt", "Groep is bijgewerkt.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
        }
    }
}
