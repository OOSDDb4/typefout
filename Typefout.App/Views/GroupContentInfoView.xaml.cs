using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views
{
    public partial class GroupContentInfoView : ContentView
    {
        private GroupItem _group;

        public GroupContentInfoView(GroupContentInfoViewModel vm, GroupItem group)
        {
            InitializeComponent();
            BindingContext = vm;
            _group = group;

            // Pass the group to the viewmodel
            vm.SetGroup(group);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            // Find the TeacherPage and navigate back to Groups
            TeacherPage teacherPage = FindTeacherPage(this);
            if (teacherPage != null)
            {
                teacherPage.LoadContentView("Groepen");
            }
        }

        private TeacherPage FindTeacherPage(Element element)
        {
            if (element == null)
                return null;

            if (element is TeacherPage teacherPage)
                return teacherPage;

            if (element.Parent != null)
                return FindTeacherPage(element.Parent);

            return null;
        }
    }
}