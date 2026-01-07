using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Typefout.App.ViewModels;

public partial class GroupContentInfoViewModel : ObservableObject
{
    [ObservableProperty]
    private GroupItem _selectedGroup;

    public GroupContentInfoViewModel()
    {
    }

    public void SetGroup(GroupItem group)
    {
        SelectedGroup = group;
    }
}