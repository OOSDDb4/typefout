using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typefout.App.ViewModels;

namespace Typefout.App.Views.Admin;

public partial class SchoolEditPage : ContentPage
{
    public SchoolEditPage(SchoolEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}