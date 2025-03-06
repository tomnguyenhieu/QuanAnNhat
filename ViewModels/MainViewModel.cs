using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ObservableObject _SelectedViewModel;

        public MainViewModel()
        {
            //
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
                    SelectedViewModel = new OrderViewModel();
                    break;
            }
        }
    }
}
