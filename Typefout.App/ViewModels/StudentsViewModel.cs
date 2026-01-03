using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.App.ViewModels
{
    public class StudentsViewModel
    {
        private readonly IUserRepo _userRepo;
        public Dictionary<string, string> Students { get; set; }

        public ICommand CreateStudentCommand { get; }

        public StudentsViewModel()
        {
            _userRepo = IPlatformApplication.Current.Services.GetService<IUserRepo>();
            Students = new Dictionary<string, string>();
            List<User> users = _userRepo.GetAllUsers().Where(u => u.Group != "None").ToList();
            foreach (User user in users)
            {
                Students.Add(user.Username, user.Group);
            }
            CreateStudentCommand = new Command(async () => await OnCreateStudent());
        }

        private async Task OnCreateStudent()
        {
            await Shell.Current.GoToAsync("RegistrationPage");
        }
    }
}
