using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkApp.ViewModels.Account
{
    public class MainViewModel
    {
        public RegisterViewModel RegisterView { get; set; }

        public LoginViewModel LoginView { get; set; }

        public MainViewModel()
        {
            RegisterView = new RegisterViewModel();
            LoginView = new LoginViewModel();
        }
    }
}
