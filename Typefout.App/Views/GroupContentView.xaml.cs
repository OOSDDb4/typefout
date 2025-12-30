using Typefout.App.ViewModels;
namespace Typefout.App.Views
{
    public partial class GroupContentView
    {
        public GroupContentView()
        {
            InitializeComponent();
            BindingContext = new GroupContentViewModel();
        }
    }
}