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
using MimeKit;
using MailKit.Net.Smtp;

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
        private string? GenerateCode;

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

        private string? _AuthCode;
        public string? AuthCode
        {
            get => _AuthCode;
            set => SetProperty(ref _AuthCode, value);
        }

        private AccessWindow AccessWindow;
        private AuthWindow AuthWindow;

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

        public bool CheckAuthenticate()
        {
            using (var context = new QuanannhatContext())
            {
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    if (user.Email.Contains(Email) && user.Status == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void HandleLogin()
        {
            if (CheckLogin())
            {
                if (!CheckAuthenticate())
                {
                    MessageBox.Show("Tài khoản chưa xác thực, vui lòng xác thực!");
                    AccessWindow.Close();
                    AuthWindow = new AuthWindow();
                    AuthWindow.DataContext = this;
                    AuthWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thành công!");
                    AccessWindow.Close();
                    SelectedViewModel = new HomeViewModel();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin đăng nhập!");
            }
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
                var info = new Information()
                {
                    Id = context.Informations.ToList().Count + 1
                };
                context.Informations.Add(info);
                var user = new User()
                {
                    Id = context.Users.ToList().Count + 1,
                    Email = Email,
                    Password = Password,
                    Status = 1,
                    Role = 3,
                    Score = 0,
                    TotalScore = 0,
                    InformationId = info.Id
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public async Task SendGenerateCode()
        {
            await Task.Delay(3000);
            MailboxAddress MailAddressFrom = new MailboxAddress("Sakuramen", "tomnguyenhieu2004@gmail.com");
            MailboxAddress MailAddressTo = new MailboxAddress("User", $"{Email}");

            var message = new MimeMessage();
            message.From.Add(MailAddressFrom);
            message.To.Add(MailAddressTo);
            message.Subject = "Test sending mail from C#";
            message.Body = new TextPart("plain")
            {
                Text = $"Ma xac thuc cua ban la: {GenerateCode}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("tomnguyenhieu2004@gmail.com", "hryd ptlm ryey rgim");
                client.Send(message);
                client.Disconnect(true);
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
            HandleLogin();
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

        [RelayCommand]
        public async Task ExecuteSendCode()
        {
            Random random = new Random();
            GenerateCode = $"{random.Next(9)}{random.Next(9)}{random.Next(9)}{random.Next(9)}";

            Console.WriteLine(GenerateCode);

            await SendGenerateCode();
            MessageBox.Show("Vui lòng kiểm tra Gmail!");
        }

        [RelayCommand]
        public void ExecuteAuth()
        {
            if (AuthCode.Equals(GenerateCode))
            {
                using (var context = new QuanannhatContext())
                {
                    var res = context.Users.Where(u => u.Id == (Convert.ToInt32(UserId))).First();
                    res.Status = 2;

                    context.SaveChanges();
                }
                MessageBox.Show("Xác thực thành công!");
                AuthWindow.Close();
                SelectedViewModel = new HomeViewModel();
            }
            else
            {
                MessageBox.Show("Vui lòng kiểm tra lại!");
            }
        }
    }
}
