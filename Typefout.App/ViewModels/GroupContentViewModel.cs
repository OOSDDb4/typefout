using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Typefout.Core.Interfaces;

namespace Typefout.App.ViewModels;

public partial class GroupContentViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<GroupItem> _groups;

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
            var groupsFromDb = await GetGroupsFromDatabase();
            
            Groups.Clear();
            foreach (var group in groupsFromDb)
            {
                Groups.Add(group);
            }
        }
        catch (Exception ex)
        {
            // Log de error of toon een melding aan de gebruiker
            System.Diagnostics.Trace.WriteLine($"Error loading groups: {ex.Message}");
        }
    }

    private async Task<List<GroupItem>> GetGroupsFromDatabase()
    {
        return await Task.Run(() =>
        {
            var groups = new List<GroupItem>();

            try
            {
                // Open database connectie
                _databaseService.Open();

                // Lees data uit de database
                // Pas de tabelnaam en kolommen aan naar jouw database structuur
                DataTable result = _databaseService.Read(
                    table: "groups",  // Pas aan naar jouw tabelnaam
                    columns: new List<string> { "group_name", "teacher", "student_count" },
                    orderBy: "group_name ASC"
                );

                // Converteer DataTable naar List<GroupItem>
                foreach (DataRow row in result.Rows)
                {
                    groups.Add(new GroupItem
                    {
                        GroupName = row["group_name"]?.ToString() ?? "",
                        Teacher = row["teacher"]?.ToString() ?? "",
                        StudentCount = Convert.ToInt32(row["student_count"] ?? 0)
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
                // Sluit altijd de connectie
                _databaseService.Close();
            }

            return groups;
        });
    }

    // Methode om groups te refreshen
    public void RefreshGroups()
    {
        LoadGroups();
    }
}

// Model class voor een groep
public class GroupItem
{
    public string GroupName { get; set; }
    public string Teacher { get; set; }
    public int StudentCount { get; set; }
}