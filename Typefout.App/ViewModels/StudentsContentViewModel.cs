using System.Collections.ObjectModel;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Typefout.App.Views;
using Typefout.Core.Interfaces;

namespace Typefout.App.ViewModels;

public partial class StudentsContentViewModel : ObservableObject
{
    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<StudentItem> _student;

    [RelayCommand]
    private async Task EditStudent(StudentItem student)
    {
        if (student == null)
            return;

        try
        {
            // Vraag om nieuwe gebruikersnaam
            string newUsername = await Application.Current.MainPage.DisplayPromptAsync(
                "Gebruikersnaam wijzigen",
                "Voer nieuwe gebruikersnaam in:",
                initialValue: student.Username,
                maxLength: 50,
                keyboard: Keyboard.Text
            );

            // Check of er iets is ingevuld
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Gebruikersnaam mag niet leeg zijn", "OK");
                return;
            }

            // Update in database
            bool success = await UpdateStudentUsername(student, newUsername);

            if (success)
            {
                student.Username = newUsername;

                RefreshGroups();

                await Application.Current.MainPage.DisplayAlert("Gelukt", "Gebruikersnaam is gewijzigd", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Kon gebruikersnaam niet wijzigen", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"EditStudent error: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
        }
    }

    private async Task<bool> UpdateStudentUsername(StudentItem student, string newUsername)
    {
        return await Task.Run(() =>
        {
            try
            {
                _databaseService.Connect();
                _databaseService.Open();

                // Haal UserId op basis van de huidige username
                string getUserIdQuery = "SELECT UserId FROM User WHERE Username = @username";

                DataTable userTable = _databaseService.ReadQuery(
                    getUserIdQuery,
                    new Dictionary<string, object>
                    {
                        { "@username", student.Username }
                    }
                );

                if (userTable.Rows.Count == 0)
                {
                    System.Diagnostics.Trace.WriteLine("Gebruiker niet gevonden");
                    return false;
                }

                int userId = Convert.ToInt32(userTable.Rows[0]["UserId"]);

                // Update username
                int result = _databaseService.Update(
                    table: "User",
                    whereName: "UserId",
                    whereValue: userId.ToString(),
                    data: new Dictionary<string, object>
                    {
                        { "Username", newUsername }
                    }
                );

                return result == 202;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Database update error: {ex.Message}");
                return false;
            }
            finally
            {
                _databaseService.Close();
            }
        });
    }

    public StudentsContentViewModel(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        _student = new ObservableCollection<StudentItem>();
        System.Diagnostics.Trace.WriteLine("StudentsContentViewModel initialized");
        LoadStudents();
    }

    private async void LoadStudents()
    {
        try
        {
            System.Diagnostics.Trace.WriteLine("LoadStudents started");

            List<StudentItem> studentsFromDb = await GetStudentsFromDatabase();

            Student.Clear();
            foreach (StudentItem student in studentsFromDb)
            {
                Student.Add(student);
            }

            System.Diagnostics.Trace.WriteLine($"Totaal studenten in UI: {Student.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Error loading students: {ex.Message}");
        }
    }

    private async Task<List<StudentItem>> GetStudentsFromDatabase()
    {
        return await Task.Run(() =>
        {
            List<StudentItem> students = new List<StudentItem>();

            try
            {
                _databaseService.Connect();
                _databaseService.Open();

                string query = @"
                SELECT 
                    u.UserId,
                    u.Username, 
                    sgroup.GroupName 
                FROM SchoolUser su
                LEFT JOIN User u ON su.UserId = u.UserId
                LEFT JOIN StudentGroup sg ON su.UserId = sg.UserId
                LEFT JOIN SchoolGroup sgroup ON sg.GroupId = sgroup.GroupId
                WHERE u.RoleId = 1";

                DataTable studentInfo = _databaseService.ReadQuery(query);

                System.Diagnostics.Trace.WriteLine($"Aantal studenten gevonden: {studentInfo.Rows.Count}");

                foreach (DataRow row in studentInfo.Rows)
                {
                    students.Add(new StudentItem()
                    {
                        UserId = row["UserId"] != DBNull.Value ? Convert.ToInt32(row["UserId"]) : 0,
                        Username = row["Username"]?.ToString() ?? "Onbekend",
                        GroupName = row["GroupName"] != DBNull.Value
                            ? row["GroupName"]?.ToString()
                            : "Geen groep"
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

            return students;
        });
    }

    public void RefreshGroups()
    {
        LoadStudents();
    }
}

public partial class StudentItem : ObservableObject
{
    [ObservableProperty]
    private int _userId;

    [ObservableProperty]
    private int _groupId;

    [ObservableProperty]
    private string _groupName;

    [ObservableProperty]
    private string _teacher;

    [ObservableProperty]
    private string _studentCount;

    [ObservableProperty]
    private string _username;
}