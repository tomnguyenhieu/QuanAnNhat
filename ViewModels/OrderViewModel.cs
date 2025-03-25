using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FontAwesome.WPF;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;
using QuanAnNhat.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.UI.StartScreen;

namespace QuanAnNhat.ViewModels
{
    partial class OrderViewModel : ObservableObject
    {
        public int _UserId { get; set; }
        public ObservableCollection<Dishlist> Dishlists { get; set; }
        public ObservableCollection<Dish> Dishes { get; set; }
        public Dishlist Category { get; set; }
        public ObservableCollection<Dish> Cart { get; set; }
        public List<Table> Tables { get; set; }

        [ObservableProperty]
        private Table? _SelectedTable;
        [ObservableProperty]
        private string? _Notes;
        [ObservableProperty]
        private string? _CategoryText;
        [ObservableProperty]
        private bool _IsFiltered;
        [ObservableProperty]
        private Visibility _IsVisible;

        private string? _SearchText;
        public string? SearchText
        {
            get => _SearchText;
            set
            {
                SetProperty(ref _SearchText, value);
                SearchDishes(_SearchText);
            }
        }

        private PayOS payOS;
        private Config? config;
        private List<ItemData> itemDatas;
        private PaymentData paymentData;
        private int orderCode;
        private string? paymentStatus;

        public OrderViewModel()
        {
            Dishlists = new ObservableCollection<Dishlist>();
            Dishes = new ObservableCollection<Dish>();
            Cart = new ObservableCollection<Dish>();
            Tables = new List<Table>();
            _UserId = 6;
            orderCode = 0;

            _ = Loading(2, false);
            _ = GetDishlists();
            _ = GetTables();

            LiveData();
        }

        private async Task Loading(int filterStatus, bool isCategory)
        {
            IsVisible = Visibility.Visible;

            if (!isCategory)
            {
                await GetMenu(filterStatus);
            } else
            {
                await GetMenuByCategory(CategoryText);
            }

            IsVisible = Visibility.Hidden;
        }

        public async Task GetDishlists()
        {
            Dishlists.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishlists.ToListAsync();
                foreach (var item in res)
                {
                    Dishlists.Add(item);
                }
            }
        }

        public async Task GetMenu(int filterStatus)
        {
            CategoryText = null;
            Dishes.Clear();

            await Task.Delay(1000);
            using (var context = new QuanannhatContext())
            {
                List<Dish> result;
                switch (filterStatus)
                {
                    case 1:
                        result = await context.Dishes.OrderBy(d => d.Price).Include(d => d.Wishlists).ToListAsync();
                        IsFiltered = false;
                        break;
                    case 2:
                        result = await context.Dishes.OrderByDescending(d => d.Price).Include(d => d.Wishlists).ToListAsync();
                        IsFiltered = true;
                        break;
                    default:
                        return;
                }
                foreach (var item in result)
                {
                    Dishes.Add(item);
                }
            }
        }

        public async Task GetMenuByCategory(string? category)
        {
            CategoryText = category;
            Dishes.Clear();
            using (var context = new QuanannhatContext())
            {
                switch (category)
                {
                    case "Wishlist":
                        var res1 = await context.Dishes.Include(d => d.Wishlists).ToListAsync();
                        foreach (var item in res1)
                        {
                            foreach (var wish in item.Wishlists)
                            {
                                if (wish.UserId == _UserId)
                                {
                                    Dishes.Add(item);
                                }
                            }
                        }
                        break;
                    case "Must Try":
                        var res2 = await context.Dishes.Where(d => d.MustTry == 2).Include(d => d.Wishlists).ToListAsync();
                        foreach (var item in res2)
                        {
                            Dishes.Add(item);
                        }
                        break;
                    case "Best Seller":
                        var res3 = await context.Dishes.OrderByDescending(d => d.TotalSold).Include(d => d.Wishlists).ToListAsync();
                        foreach (var item in res3)
                        {
                            Dishes.Add(item);
                        }
                        break;
                    default:
                        var res4 = await context.Dishes.Include(x => x.Dishlist).Include(d => d.Wishlists).Where(x => x.Dishlist.Name == category && x.DishlistId == x.Dishlist.Id).ToListAsync();
                        foreach (var item in res4)
                        {
                            Dishes.Add(item);
                        }
                        break;
                }
                
            }
        }

        public async void SearchDishes(string? searchText)
        {
            Dishes.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishes.Where(x => x.Name.Contains(searchText)).Include(d => d.Wishlists).ToListAsync();
                foreach (var item in res)
                {
                    Dishes.Add(item);
                }
            }
        }

        public void AddToCart(int dishId)
        {
            var dish = DataProvider.Ins.Context.Dishes.FirstOrDefault(x => x.Id == dishId);
            using (var context = new QuanannhatContext())
            {
                var res = context.Dishes.Find(dishId);
                if (res != null && res.Quantity > 0)
                {
                    var cartItem = Cart.FirstOrDefault(x => x.Id == dishId);
                    if (cartItem == null)
                    {
                        dish.Quantity = 1;
                        Cart.Add(dish);
                    }
                    else if (res.Quantity > cartItem.Quantity)
                    {
                        cartItem.Quantity++;
                        dish.Price += res.Price;
                    } else
                    {
                        MessageBox.Show("Số lượng có hạn!");
                    }
                } else
                {
                    MessageBox.Show("Đã hết món!");
                }
            }
        }

        public async Task GetTables()
        {
            using (var context = new QuanannhatContext())
            {
                var res = await context.Tables.Where(t => t.Status == 2).ToListAsync();
                foreach (var table in res)
                {
                    Tables.Add(table);
                }
            }
        }

        public bool OrderValidate()
        {
            if (SelectedTable is null)
            {
                MessageBox.Show("Vui lòng chọn bàn!");
                return false;
            }
            if (Cart.Count() < 1)
            {
                MessageBox.Show("Vui lòng chọn món ăn!");
                return false;
            }
            return true;
        }

        public void CreateOrderBill()
        {
            DateTime dateTime = DateTime.Now;
            using (var context = new QuanannhatContext())
            {
                int _orderId = context.OrderBills.Count() + 1;
                int totalPrice = 0;
                foreach (var dish in Cart)
                {
                    totalPrice += dish.Price;
                }
                OrderBill orderBill = new OrderBill()
                {
                    Id = _orderId,
                    TotalPrice = totalPrice,
                    OrderStatus = 2,
                    BillStatus = 2,
                    Note = Notes,
                    UserId = _UserId,
                    TableId = SelectedTable.Id
                };
                context.OrderBills.Add(orderBill);
                context.SaveChanges();
            }
        }

        public void CreateOrders()
        {
            using (var context = new QuanannhatContext())
            {
                int _orderId = context.Orders.ToList().Count;
                var res = context.Dishes.ToList();
                foreach (var dish in Cart)
                {
                    _orderId += 1;
                    Order order = new Order()
                    {
                        Id = _orderId,
                        DishId = dish.Id,
                        Quantity = dish.Quantity,
                        TotalPrice = dish.Price,
                        OrderbillId = context.OrderBills.Where(b => b.UserId == _UserId).OrderByDescending(o => o.Id).First().Id
                    };
                    context.Orders.Add(order);
                }
                context.SaveChanges();
            }
        }

        public async void HandlePayment()
        {
            string text = File.ReadAllText(@"D:\Learning\C#\HAU\QuanAnNhat\.config.json");
            config = JsonSerializer.Deserialize<Config>(text);
            payOS = new PayOS(config.ClientId, config.ApiKey, config.ChecksumKey);
            itemDatas = new List<ItemData>();

            int PaymentAmount = 0;
            foreach (var dish in Cart)
            {
                itemDatas.Add(new ItemData(dish.Name, dish.Quantity, dish.Price));
                PaymentAmount += dish.Price;
            }
            
            try
            {
                using (var context = new QuanannhatContext())
                {
                    orderCode = context.OrderBills.Where(b => b.UserId == _UserId).OrderByDescending(o => o.Id).First().Id;
                    Console.WriteLine(orderCode);
                    paymentData = new PaymentData(orderCode, PaymentAmount, $"Thanh toan bill #{orderCode}", itemDatas, "http://localhost/", "http://localhost/")
                    {
                        expiredAt = new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds()
                    };

                    CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = createPayment.checkoutUrl
                    };
                    Process.Start(processStartInfo);
                }
            }
            catch (Exception)
            {
                throw;
            }

            SelectedTable = null;
            Notes = null;
            Cart.Clear();
        }

        public async void AddToWishList(int dishId)
        {
            using (var context = new QuanannhatContext())
            {
                var res = context.Wishlists.ToList();
                int count = res.Max(w => w.Id) + 1;
                bool check = true;

                foreach (var item in res)
                {
                    if (item.DishId == dishId && item.UserId == _UserId)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                {
                    context.Wishlists.Add(new Wishlist()
                    {
                        Id = count,
                        DishId = dishId,
                        UserId = _UserId
                    });
                }
                else
                {
                    var wishlist = context.Wishlists.Where(w => w.DishId == dishId && w.UserId == _UserId).First();
                    if (wishlist != null)
                    {
                        context.Wishlists.Remove(wishlist);
                    }
                }
                context.SaveChanges();

                if (!string.IsNullOrEmpty(CategoryText))
                {
                    await Loading(1, true);
                }
                else if (IsFiltered)
                {
                    await Loading(2, false);
                }
                else
                {
                    await Loading(1, false);
                }
            }
        }

        public async void LiveData()
        {
            while (true)
            {
                using (var context = new QuanannhatContext())
                {
                    await Task.Delay(5000);
                    if (orderCode != 0)
                    {
                        PaymentLinkInformation information = await payOS.getPaymentLinkInformation(orderCode);
                        paymentStatus = information.status;

                        var _bill = context.OrderBills.Where(b => b.Id == orderCode).First();
                        if (_bill.BillStatus == 2)
                        {
                            switch (paymentStatus)
                            {
                                case "CANCELLED":
                                    await context.OrderBills.Where(b => b.Id == orderCode).ExecuteUpdateAsync(u => u.SetProperty(b => b.BillStatus, 1));
                                    MessageBox.Show("Hủy thanh toán thành công!");
                                    break;
                                case "PAID":
                                    await context.OrderBills.Where(b => b.Id == orderCode).ExecuteUpdateAsync(u => u.SetProperty(b => b.BillStatus, 3));
                                    MessageBox.Show("Thanh toán thành công!");
                                    break;
                                case "EXPIRED":
                                    await context.OrderBills.Where(b => b.Id == orderCode).ExecuteUpdateAsync(u => u.SetProperty(b => b.BillStatus, 1));
                                    MessageBox.Show("Hết hạn thanh toán!");
                                    break;
                            }
                        }
                    }
                }
            }
        }

        [RelayCommand]
        public async Task ExecuteFilter(object parameter)
        {
            int filterStatus = Convert.ToBoolean(parameter) ? 1 : 2;
            await Loading(filterStatus, false);
        }

        [RelayCommand]
        public void ExecuteAddToWishlist(object parameter)
        {
            AddToWishList((int)parameter);
        }

        [RelayCommand]
        public async Task ExcuteGetMenu(object parameter)
        {
            await GetMenuByCategory(parameter.ToString());
        }

        [RelayCommand]
        public void ExcuteAddToCart(object parameter)
        {
            AddToCart((int) parameter);
        }

        [RelayCommand]
        public void ExcutePlus(object parameter)
        {
            using (var context = new QuanannhatContext())
            {
                foreach (var dish in Cart.ToList())
                {
                    if (dish.Name.Equals(parameter))
                    {
                        var res = context.Dishes.FirstOrDefault(x => x.Id == dish.Id);
                        if (res != null && res.Quantity > dish.Quantity)
                        {
                            dish.Quantity++;
                            dish.Price += res.Price;
                        } else
                        {
                            MessageBox.Show("Số lượng có giới hạn!");
                        }
                    }
                }
            }
        }

        [RelayCommand]
        public void ExcuteMinus(object parameter)
        {
            using (var context = new QuanannhatContext())
            {
                foreach (var dish in Cart.ToList())
                {
                    if (dish.Name.Equals(parameter))
                    {
                        var res = context.Dishes.Where(d => d.Id == dish.Id).First();
                        dish.Quantity -= 1;
                        dish.Price -= res.Price;
                        if (dish.Quantity < 1)
                        {
                            Cart.Remove(dish);
                            dish.Price = res.Price;
                        }
                    }
                }
            }
        }

        [RelayCommand]
        public void ExcutePay()
        {
            if (OrderValidate())
            {
                CreateOrderBill();
                CreateOrders();
                HandlePayment();
            }
        }
    }
}
