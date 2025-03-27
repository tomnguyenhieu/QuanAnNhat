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
        public List<string> Addresses { get; set; }

        [ObservableProperty]
        private Visibility _InfoVisible;
        [ObservableProperty]
        private Visibility _PassVisible;
        [ObservableProperty]
        private Visibility _FeedbackVisible;
        [ObservableProperty]
        private Information _Information;

        public ProfileViewModel(string? userId)
        {
            _UserId = Convert.ToInt32(userId);
            InitAddresses();
            InitInformation();

            InfoVisible = Visibility.Visible;
            PassVisible = Visibility.Hidden;
            FeedbackVisible = Visibility.Hidden;
        }

        public void InitInformation()
        {
            using (var context = new QuanannhatContext())
            {
                var user = context.Users.Where(u => u.Id == _UserId).Include(u => u.Information).First();
                Information = new Information()
                {
                    Name = user.Information.Name,
                    Gender = user.Information.Gender,
                    Phone = user.Information.Phone,
                    Address = user.Information.Address,
                };
            }
            
        }

        public void InitAddresses()
        {
            Addresses = new List<string>();
            string[] provinces =
            {
                "An Giang", "Ba Ria - Vung Tau", "Bac Giang", "Bac Kan", "Bac Lieu", "Bac Ninh",
                "Ben Tre", "Binh Dinh", "Binh Duong", "Binh Phuoc", "Binh Thuan", "Ca Mau", "Can Tho",
                "Cao Bang", "Da Nang", "Dak Lak", "Dak Nong", "Dien Bien", "Dong Nai", "Dong Thap",
                "Gia Lai", "Ha Giang", "Ha Nam", "Ha Noi", "Ha Tinh", "Hai Duong", "Hai Phong",
                "Hau Giang", "Hoa Binh", "Hung Yen", "Khanh Hoa", "Kien Giang", "Kon Tum", "Lai Chau",
                "Lam Dong", "Lang Son", "Lao Cai", "Long An", "Nam Dinh", "Nghe An", "Ninh Binh",
                "Ninh Thuan", "Phu Tho", "Phu Yen", "Quang Binh", "Quang Nam", "Quang Ngai", "Quang Ninh",
                "Quang Tri", "Soc Trang", "Son La", "Tay Ninh", "Thai Binh", "Thai Nguyen", "Thanh Hoa",
                "Thua Thien Hue", "Tien Giang", "TP Ho Chi Minh", "Tra Vinh", "Tuyen Quang", "Vinh Long",
                "Vinh Phuc", "Yen Bai"
            };
            Addresses.AddRange(provinces);
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

        [RelayCommand]
        public void ExecuteSaveChanges()
        {
            if (Information.Phone.Count() != 10)
            {
                MessageBox.Show("Vui lòng kiểm tra lại số điện thoại!");
            } else
            {
                using (var context = new QuanannhatContext())
                {
                    var user = context.Users.Where(u => u.Id == _UserId).Include(u => u.Information).First();

                    context.Informations.Where(i => i.Id == user.InformationId).ExecuteUpdate(u => u.SetProperty(u => u.Name, Information.Name));
                    context.Informations.Where(i => i.Id == user.InformationId).ExecuteUpdate(u => u.SetProperty(u => u.Gender, Information.Gender));
                    context.Informations.Where(i => i.Id == user.InformationId).ExecuteUpdate(u => u.SetProperty(u => u.Phone, Information.Phone));
                    context.Informations.Where(i => i.Id == user.InformationId).ExecuteUpdate(u => u.SetProperty(u => u.Address, Information.Address));

                    context.SaveChanges();

                    MessageBox.Show("Cập nhật thông tin thành công!");
                }
            }
        }
    }
}
