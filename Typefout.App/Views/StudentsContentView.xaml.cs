using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typefout.App.ViewModels;
using Typefout.Core.Interfaces;

namespace Typefout.App.Views
{
    public partial class StudentsContentView : ContentView
    {
        public StudentsContentView(StudentsContentViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
