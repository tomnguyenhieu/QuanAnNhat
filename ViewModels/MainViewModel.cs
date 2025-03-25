using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanAnNhat.DBContext;
using QuanAnNhat.Singletons;
using QuanAnNhat.Views;
using QuanAnNhat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuanAnNhat.ViewModels
{
    partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _SelectedViewModel;
        [ObservableProperty]
        private string? _UserId;
        [ObservableProperty]
        private Visibility _IsVisible;

        private string? _Email;
        public string? Email
        {
            get => _Email;
            set => SetProperty(ref _Email, value);
        }

        private string? _Password;
        public string? Password
        {
            get => _Password;
            set => SetProperty(ref _Password, value);
        }

        private string? _RePassword;
        public string? RePassword
        {
            get => _RePassword;
            set => SetProperty(ref _RePassword, value);
        }

        private AccessWindow AccessWindow;

        public MainViewModel()
        {
            AccessWindow = new AccessWindow();
            AccessWindow.DataContext = this;
            AccessWindow.ShowDialog();
        }

        public void ChangeForm(string visibleStatus)
        {
            switch (visibleStatus.ToString())
            {
                case "Visible":
                    IsVisible = Visibility.Hidden;
                    break;
                case "Hidden":
                    IsVisible = Visibility.Visible;
                    break;
            }
        }

        public bool CheckLogin()
        {
            using (var context = new QuanannhatContext())
            {
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    if (user.Email.Equals(Email) && user.Password.Equals(Password))
                    {
                        UserId = user.Id.ToString();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RegisterValidate()
        {
            using (var context = new QuanannhatContext())
            {
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    if (Email.Equals(user.Email) || !Email.Contains("@") || Email.Contains(" ") || Email is null)
                    {
                        return false;
                    }
                }
                if (!RePassword.Equals(Password) || Password.Length < 8)
                {
                    return false;
                }
                return true;
            }
        }

        public void ProcessRegister()
        {
            using (var context = new QuanannhatContext())
            {
                var user = new User()
                {
                    Id = context.Users.ToList().Count + 1,
                    Email = Email,
                    Password = Password,
                    Status = 1,
                    Role = 3,
                    Score = 0,
                    TotalScore = 0
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        [RelayCommand]
        public void ExcuteUpdateView(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Home":
                    SelectedViewModel = new HomeViewModel();
                    break;
                case "Order":
                    SelectedViewModel = new OrderViewModel(UserId);
                    break;
                case "Order History":
                    SelectedViewModel = new OrderHistoryViewModel(UserId);
                    break;
            }
        }

        [RelayCommand]
        public void ExecuteChangeForm(object parameter)
        {
            ChangeForm(parameter.ToString());
        }

        [RelayCommand]
        public void ExecuteLogin()
        {
            if (CheckLogin())
            {
                MessageBox.Show("Đăng nhập thành công!");
                AccessWindow.Close();
                
            }
            else
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đăng nhập!");
            }
        }

        [RelayCommand]
        public void ExecuteRegister(object parameter)
        {
            if (RegisterValidate())
            {
                MessageBox.Show("Đăng ký thành công!");
                ProcessRegister();
                Password = RePassword = null;
                ChangeForm(parameter.ToString());
            }
            else
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đăng ký!");
            }
        }
    }
}
