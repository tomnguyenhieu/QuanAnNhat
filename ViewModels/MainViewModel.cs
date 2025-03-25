using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanAnNhat.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanAnNhat.ViewModels
{
    partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _SelectedViewModel;
        [ObservableProperty]
        private string? _UserId;
        private AccessWindow accessWindow;
        private bool IsLogin = false;

        public MainViewModel()
        {
            accessWindow = new AccessWindow();
            accessWindow.DataContext = this;
            accessWindow.ShowDialog();
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
        public void ExecuteClick(object parameter)
        {
            Console.WriteLine($"Login: {UserId}");
            accessWindow.Close();
            IsLogin = true;
            if (IsLogin)
            {
                SelectedViewModel = new HomeViewModel();
            }
        }
    }
}
