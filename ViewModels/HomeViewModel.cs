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

            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await GetMustTryDishes();
            await GetBestSellerDishes();
        }

        public async Task GetMustTryDishes()
        {
            using (var context = new QuanannhatContext())
            {
                var tmpDishes =  await context.Dishes.Where(x => x.MustTry == 2 && x.Status == 2).Include(x => x.Wishlists).ToListAsync();
                int count = 0;
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

        public async Task GetBestSellerDishes()
        {
            using (var context = new QuanannhatContext())
            {
                var tmpDishes = await context.Dishes.OrderByDescending(x => x.TotalSold).Include(x => x.Wishlists).ToListAsync();
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
