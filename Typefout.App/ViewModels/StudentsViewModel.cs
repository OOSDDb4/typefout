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
        private readonly IGroupRepo _groupRepo;
        public Dictionary<string, string> Students { get; set; }

        public ICommand CreateStudentCommand { get; }

        public StudentsViewModel()
        {
            _userRepo = IPlatformApplication.Current.Services.GetService<IUserRepo>();
            _groupRepo = IPlatformApplication.Current.Services.GetService<IGroupRepo>();
            Students = new Dictionary<string, string>();
            List<User> users = _userRepo.GetAllUsers().Where(u => u.UserType == UserType.Leerling).ToList();
            foreach (User user in users)
            {
                Group? group = _groupRepo.GetByIdAsync(user.GroupId.Value).GetAwaiter().GetResult();
                Students.Add(user.Username, group?.Name ?? "None");
            }
            CreateStudentCommand = new Command(async () => await OnCreateStudent());
        }
        private async Task OnCreateStudent()
        {
            await Shell.Current.GoToAsync("RegistrationPage");
        }
    }
}
