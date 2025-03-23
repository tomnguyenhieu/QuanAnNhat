using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace QuanAnNhat.ViewModels
{
    partial class OrderViewModel : ObservableObject
    {
        private int _UserId = 6;
        public ObservableCollection<Dishlist> Dishlists { get; set; }
        public ObservableCollection<Dish> Dishes { get; set; }
        public Dishlist Category { get; set; }
        public ObservableCollection<Dish> Cart { get; set; }
        public List<Table> Tables { get; set; }

        private string? _SearchText;
        public string? SearchText
        {
            get => _SearchText;
            set
            {
                SetProperty(ref _SearchText, value);
                SearchDishes(SearchText);
            }
        }

        private Table? _SelectedTable;
        public Table? SelectedTable 
        {
            get => _SelectedTable;
            set
            {
                SetProperty(ref _SelectedTable, value);
            }
        }
        private string? _Notes;
        public string? Notes
        {
            get => _Notes;
            set => SetProperty(ref _Notes, value);
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
            orderCode = 0;

            GetDishlists();
            GetMenu(2);
            GetTables();

            LiveData();
        }

        public async void GetDishlists()
        {
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishlists.ToListAsync();
                foreach (var item in res)
                {
                    Dishlists.Add(item);
                }
            }
        }

        public async void GetMenu(int filterStatus)
        {
            Dishes.Clear();
            using (var context = new QuanannhatContext())
            {
                switch (filterStatus)
                {
                    case 1:
                        var res1 = await context.Dishes.OrderBy(d => d.Price).ToListAsync();
                        foreach (var item in res1)
                        {
                            Dishes.Add(item);
                        }
                        break;
                    case 2:
                        var res2 = await context.Dishes.OrderByDescending(d => d.Price).ToListAsync();
                        foreach (var item in res2)
                        {
                            Dishes.Add(item);
                        }
                        break;
                }
            }
        }

        public async void GetMenuByCategory(string? category)
        {
            Dishes.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishes.Include(x => x.Dishlist).Where(x => x.Dishlist.Name == category && x.DishlistId == x.Dishlist.Id).ToListAsync();
                foreach (var item in res)
                {
                    Dishes.Add(item);
                }
            }
        }

        public async void SearchDishes(string? searchText)
        {
            Dishes.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = await context.Dishes.Where(x => x.Name.Contains(searchText)).ToListAsync();
                foreach (var item in res)
                {
                    Dishes.Add(item);
                }
            }
        }

        public void AddToCart(int dishId)
        {
            
            var dish = DataProvider.Ins.Context.Dishes.Where(x => x.Id == dishId).First();
            if (!Cart.Contains(dish))
            {
                dish.Quantity = 1;
                Cart.Add(dish);
            }
            else
            {
                dish.Quantity += 1;
                dish.Price += dish.Price;
            }
        }

        public async void GetTables()
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
            List<Dish> _UnAvailableDishes = new List<Dish>();
            using (var context = new QuanannhatContext())
            {
                foreach (var dish in Cart)
                {
                    foreach (var _dish in context.Dishes.ToList())
                    {
                        if (dish.Id == _dish.Id && _dish.Quantity == 0)
                        {
                            _UnAvailableDishes.Add(dish);
                        }
                    }
                }
            }
            if (_UnAvailableDishes.Count > 0)
            {
                string _names = "";
                foreach (var dish in _UnAvailableDishes)
                {
                    _names += $"{dish.Name}, ";
                }
                MessageBox.Show($"Hết món {_names}!");
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
                    totalPrice += dish.Quantity * dish.Price;
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
                int _orderId = context.Orders.ToList().Count();
                var res = context.Dishes.ToList();
                foreach (var dish in Cart)
                {
                    foreach (var _dish in res)
                    {
                        _orderId += 1;
                        Order order = new Order()
                        {
                            Id = _orderId,
                            DishId = dish.Id,
                            Quantity = dish.Quantity,
                            TotalPrice = dish.Quantity * dish.Price,
                            OrderbillId = GetOrderBillIdByUserId(_UserId)
                        };
                        context.Orders.Add(order);
                    }
                }
                context.SaveChanges();
            }
        }

        public int GetOrderBillIdByUserId(int userId)
        {
            int orderBillId = 0;
            foreach (var orderbill in DataProvider.Ins.Context.OrderBills.ToList())
            {
                if (orderbill.UserId == userId)
                {
                    orderBillId = orderbill.Id;
                }
            }
            return orderBillId;
        }

        public async void HandlePayment()
        {
            int PaymentAmount = 0;
            string text = File.ReadAllText(@"D:\Learning\C#\HAU\QuanAnNhat\.config.json");
            config = JsonSerializer.Deserialize<Config>(text);

            payOS = new PayOS(config.ClientId, config.ApiKey, config.ChecksumKey);

            itemDatas = new List<ItemData>();
            foreach (var dish in Cart)
            {
                itemDatas.Add(new ItemData(dish.Name, dish.Quantity, dish.Price));
            };

            try
            {
                foreach (ItemData item in itemDatas)
                {
                    PaymentAmount += item.price * item.quantity;
                }
                using (var context = new QuanannhatContext())
                {
                    orderCode = GetOrderBillIdByUserId(_UserId);
                    paymentData = new PaymentData(orderCode, PaymentAmount, $"Thanh toan bill #{orderCode}", itemDatas, "http://localhost/", "http://localhost/");

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

        public async void LiveData()
        {
            while (true)
            {
                await Task.Delay(5000);
                using (var context = new QuanannhatContext())
                {
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
                                    context.OrderBills.Where(b => b.Id == orderCode).ExecuteUpdate(u => u.SetProperty(b => b.BillStatus, 1));
                                    MessageBox.Show("Hủy thanh toán thành công!");
                                    break;
                                case "PAID":
                                    context.OrderBills.Where(b => b.Id == orderCode).ExecuteUpdate(u => u.SetProperty(b => b.BillStatus, 3));
                                    MessageBox.Show("Thanh toán thành công!");
                                    break;
                            }
                        }
                    }
                }
            }
        }

        [RelayCommand]
        public void ExecuteFilter(object parameter)
        {
            int filterStatus = Convert.ToBoolean(parameter) ? 1 : 2;
            GetMenu(filterStatus);
        }

        [RelayCommand]
        public void ExcuteGetMenu(object parameter)
        {
            GetMenuByCategory(parameter.ToString());
        }

        [RelayCommand]
        public void ExcuteAddToCart(object parameter)
        {
            AddToCart((int) parameter);
        }

        [RelayCommand]
        public void ExcutePlus(object parameter)
        {
            foreach (var dish in Cart)
            {
                if (dish.Name.Equals(parameter))
                {
                    dish.Quantity += 1;
                    dish.Price += dish.Price;
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
                        dish.Price = res.Price * dish.Quantity;
                        if (dish.Quantity < 1)
                        {
                            Cart.Remove(dish);
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
