using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public ProfileViewModel(string? userId)
        {
            _UserId = Convert.ToInt32(userId);
            InfoVisible = Visibility.Visible;
            PassVisible = Visibility.Hidden;
            FeedbackVisible = Visibility.Hidden;
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
