using Typefout.App.Views;
using Typefout.App.Views.Admin;
using Typefout.App.Views.Docent;

namespace Typefout.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(PasswordReset2Page), typeof(PasswordReset2Page));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute("admindashboard", typeof(AdminDashboardPage));
        Routing.RegisterRoute("exercises", typeof(ExercisesPage));
        Routing.RegisterRoute("schools", typeof(SchoolsPage));
        Routing.RegisterRoute("schoolcreate", typeof(SchoolCreatePage));
        Routing.RegisterRoute("schooledit", typeof(SchoolEditPage));
        Routing.RegisterRoute("schoolinfo", typeof(SchoolInfoPage));
        Routing.RegisterRoute("groupcreate", typeof(GroupCreatePage));
        Routing.RegisterRoute("groupedit", typeof(GroupEditPage));
        Routing.RegisterRoute("teachercreate", typeof(TeacherCreatePage));
        Routing.RegisterRoute("teacheredit", typeof(TeacherEditPage));
        Routing.RegisterRoute("studentcreate", typeof(StudentCreatePage));
        Routing.RegisterRoute("studentedit", typeof(StudentEditPage));
        Routing.RegisterRoute("adduser", typeof(AddUserPage));
        Routing.RegisterRoute("oefeningen", typeof(OefeningenMenuPage));
        Routing.RegisterRoute("teacherpage", typeof(TeacherPage));
        Routing.RegisterRoute("registrationpage", typeof(RegistrationPage));
    }
}