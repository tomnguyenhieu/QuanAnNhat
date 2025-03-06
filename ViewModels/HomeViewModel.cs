using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.Models;
using QuanAnNhat.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanAnNhat.ViewModels
{
    partial class HomeViewModel : ObservableObject
    {
        public ObservableCollection<Dish> MustTryDishes { get; set; }
        public ObservableCollection<Dish> BestSellerDishes { get; set; }
        public HomeViewModel()
        {
            MustTryDishes = new ObservableCollection<Dish>();
            BestSellerDishes = new ObservableCollection<Dish>();

            GetMustTryDishes();
            GetBestSellerDishes();
        }

        public void GetMustTryDishes()
        {
            var tmpDishes = DataProvider.Ins.Context.Dishes.Where(x => x.MustTry == 2).Where(x => x.Status == 2).ToList();
            int count = 1;
            foreach (var _dish in tmpDishes)
            {
                count++;
                if (count <= 5)
                {
                    MustTryDishes.Add(_dish);
                }
            }
        }
        public void GetBestSellerDishes()
        {
            var tmpDishes = DataProvider.Ins.Context.Dishes.OrderByDescending(x => x.TotalSold).ToList();
            int count = 0;
            foreach (var _dish in tmpDishes)
            {
                count++;
                if (count <= 5)
                {
                    BestSellerDishes.Add(_dish);
                }
            }
        }
    }
}
