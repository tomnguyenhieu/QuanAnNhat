using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;
using QuanAnNhat.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Formats.Asn1;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.UI.Composition;

namespace QuanAnNhat.ViewModels
{
    partial class LiveDataViewModel : ObservableObject
    {
        public ObservableCollection<Dish> AvailableDishes { get; set; }
        public ObservableCollection<Dish> UnAvailableDishes { get; set; }

        public LiveDataViewModel()
        {
            AvailableDishes = new ObservableCollection<Dish>();
            UnAvailableDishes = new ObservableCollection<Dish>();

            GetDishes(1);
            GetDishes(2);

            NotificationDelay();

            LiveData();
        }

        public async void NotificationDelay()
        {
            await Task.Delay(5000);
            AvailableDishes.CollectionChanged += change;
            UnAvailableDishes.CollectionChanged += change;
        }

        public async void GetDishes(int status)
        {
            using (var context = new QuanannhatContext())
            {
                var result = await context.Dishes.ToListAsync();
                switch (status)
                {
                    case 1:
                        foreach (var dish in result)
                        {
                            if (dish.Status == status && !UnAvailableDishes.Any(x => x.Id == dish.Id))
                            {
                                UnAvailableDishes.Add(dish);
                            }
                        }
                        break;
                    case 2:
                        foreach (var dish in result)
                        {
                            if (dish.Status == status && !AvailableDishes.Any(x => x.Id == dish.Id))
                            {
                                AvailableDishes.Add(dish);
                            }
                        }
                        break;
                }
            }
        }

        public async void RemoveDishes(int status)
        {
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishes.ToListAsync();
                switch (status)
                {
                    case 1:
                        for (int i = 0; i < UnAvailableDishes.Count; i++)
                        {
                            for (int j = 0; j < res.Count; j++)
                            {
                                if (UnAvailableDishes[i].Id == res[j].Id && UnAvailableDishes[i].Status != res[j].Status)
                                {
                                    UnAvailableDishes.RemoveAt(i);
                                }
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < AvailableDishes.Count; i++)
                        {
                            for (int j = 0; j < res.Count; j++)
                            {
                                if (AvailableDishes[i].Id == res[j].Id && AvailableDishes[i].Status != res[j].Status)
                                {
                                    AvailableDishes.RemoveAt(i);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public async void LiveData()
        {
            while (true)
            {
                await Task.Delay(1000);
                GetDishes(1);
                GetDishes(2);

                RemoveDishes(1);
                RemoveDishes(2);
            }
        }

        private static void change(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine($"Added!");
                    SendToastNotification();
                    break;
            }
        }

        public static void SendToastNotification()
        {
            new ToastContentBuilder().AddText("Bạn có 1 thông báo mới")
            .AddText("Đơn món của bạn đã được chuẩn bị xong!")
            .Show();
        }

        [RelayCommand]
        public void ExcuteSendNotification()
        {
            SendToastNotification();
        }

        [RelayCommand]
        public void ExcuteCheck()
        {
            foreach (var dish in UnAvailableDishes)
            {
                Console.WriteLine($"Id: {dish.Id}, name: {dish.Name}, status: {dish.Status}");
            }
            Console.WriteLine("---------------------");
        }
    }
}
