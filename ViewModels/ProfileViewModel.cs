using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanAnNhat.ViewModels
{
    partial class ProfileViewModel : ObservableObject
    {
        public int _UserId { get; set; }

        [ObservableProperty]
        private Visibility _InfoVisible;
        [ObservableProperty]
        private Visibility _PassVisible;
        [ObservableProperty]
        private Visibility _FeedbackVisible;
        [ObservableProperty]
        private string? _UserName;

        public ProfileViewModel(string? userId)
        {
            _UserId = Convert.ToInt32(userId);
            InfoVisible = Visibility.Visible;
            PassVisible = Visibility.Hidden;
            FeedbackVisible = Visibility.Hidden;

            using (var context = new QuanannhatContext())
            {
                var user = context.Users.Where(u => u.Id == _UserId).Include(u => u.Information).First();
                UserName = user.Information.Name;
            }
        }

        [RelayCommand]
        public void ExecuteChangeTab(object parameter)
        {
            switch (parameter.ToString()) 
            {
                case "InfoBtn":
                    InfoVisible = Visibility.Visible;
                    PassVisible = Visibility.Hidden;
                    FeedbackVisible = Visibility.Hidden;
                    break;
                case "PassBtn":
                    InfoVisible = Visibility.Hidden;
                    PassVisible = Visibility.Visible;
                    FeedbackVisible = Visibility.Hidden;
                    break;
                case "FeedbackBtn":
                    InfoVisible = Visibility.Hidden;
                    PassVisible = Visibility.Hidden;
                    FeedbackVisible = Visibility.Visible;
                    break;
            }
        }
    }
}
