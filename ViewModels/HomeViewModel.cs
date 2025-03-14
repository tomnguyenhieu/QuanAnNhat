using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;
using QuanAnNhat.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public async void GetMustTryDishes()
        {
            using (var context = new QuanannhatContext())
            {
                var tmpDishes =  await context.Dishes.Where(x => x.MustTry == 2).Where(x => x.Status == 2).ToListAsync();
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
        }

        public async void GetBestSellerDishes()
        {
            using (var context = new QuanannhatContext())
            {
                var tmpDishes = await context.Dishes.OrderByDescending(x => x.TotalSold).ToListAsync();
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
}
