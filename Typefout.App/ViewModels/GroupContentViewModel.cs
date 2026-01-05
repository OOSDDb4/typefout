using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;

namespace Typefout.App.ViewModels;

public partial class GroupContentViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<GroupItem> _groups;

    [RelayCommand]
    private void InfoGroups(GroupItem group)
    {
        if (group == null)
            return;

        try
        {
            // Start from the current view
            Page currentView = Application.Current?.MainPage;

            System.Diagnostics.Trace.WriteLine($"MainPage type: {currentView?.GetType().Name}");

            // Check common navigation patterns
            TeacherPage teacherPage = null;

            // Pattern 1: Direct TeacherPage
            if (currentView is TeacherPage tp1)
            {
                teacherPage = tp1;
            }
            // Pattern 2: NavigationPage -> TeacherPage
            else if (currentView is NavigationPage navPage)
            {
                teacherPage = navPage.CurrentPage as TeacherPage;
                System.Diagnostics.Trace.WriteLine($"NavigationPage.CurrentPage type: {navPage.CurrentPage?.GetType().Name}");
            }
            // Pattern 3: Shell with TeacherPage
            else if (currentView is Shell shell)
            {
                teacherPage = shell.CurrentPage as TeacherPage;
                System.Diagnostics.Trace.WriteLine($"Shell.CurrentPage type: {shell.CurrentPage?.GetType().Name}");
            }
            // Pattern 4: FlyoutPage or TabbedPage
            else if (currentView is FlyoutPage flyoutPage)
            {
                teacherPage = flyoutPage.Detail as TeacherPage;
            }

            if (teacherPage != null)
            {
                System.Diagnostics.Trace.WriteLine("TeacherPage found!");
                teacherPage.LoadContentView("InformatieGroepen", group);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("TeacherPage NOT found - check your navigation structure");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"InfoGroups error: {ex.Message}");
        }
    }

    private TeacherPage FindTeacherPage(Element element)
    {
        if (element == null)
            return null;

        if (element is TeacherPage teacherPage)
            return teacherPage;

        // Zoek in de parent hierarchy
        if (element.Parent != null)
            return FindTeacherPage(element.Parent);

        // Zoek in de navigation stack
        if (Application.Current.MainPage is NavigationPage navPage && navPage.CurrentPage is TeacherPage tp)
            return tp;

        return null;
    }

    public GroupContentViewModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        Groups = new ObservableCollection<GroupItem>();
        LoadGroups();
    }

    private async void LoadGroups()
    {
        try
        {
            List<GroupItem> groupsFromDb = await GetGroupsFromDatabase();

            Groups.Clear();
            foreach (GroupItem group in groupsFromDb)
            {
                Groups.Add(group);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Error loading groups: {ex.Message}");
        }
    }

    private async Task<List<GroupItem>> GetGroupsFromDatabase()
    {
        return await Task.Run(() =>
        {
            List<GroupItem> groups = new List<GroupItem>();

            try
            {
                _databaseService.Connect();
                _databaseService.Open();

                // Lees data uit de database
                // Pas de tabelnaam en kolommen aan naar jouw database structuur
                DataTable groupInfo = _databaseService.Read(
                    table: "SchoolGroup",
                    where: "`SchoolId` = 1"
                );

                foreach (DataRow row in groupInfo.Rows)
                {
                    DataTable groupStudentAmount = _databaseService.Read(
                        table: "StudentGroup",
                        where: $"`GroupId` = {row["GroupId"]}"
                    );
                    int studentCount = groupStudentAmount.Rows.Count;

                    groups.Add(new GroupItem
                    {
                        GroupId = Convert.ToInt32(row["GroupId"]),
                        GroupName = row["GroupName"]?.ToString() ?? "",
                        Teacher = row["TeacherId"]?.ToString() ?? "",
                        StudentCount = studentCount.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Database error: {ex.Message}");
                throw;
            }
            finally
            {
                _databaseService.Close();
            }

            return groups;
        });
    }

    public void RefreshGroups()
    {
        LoadGroups();
    }

}

public class GroupItem
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public string Teacher { get; set; }
    public string StudentCount { get; set; }
}