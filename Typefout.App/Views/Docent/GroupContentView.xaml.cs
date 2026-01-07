using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views
{
    public partial class GroupContentView
    {
        public GroupContentView(GroupContentViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}