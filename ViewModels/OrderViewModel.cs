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
        public string? Notes { get; set; }

        private PayOS payOS;
        private Config? config;
        private List<ItemData> itemDatas;
        private PaymentData paymentData;
        public OrderViewModel()
        {
            Dishlists = new ObservableCollection<Dishlist>();
            Dishes = new ObservableCollection<Dish>();
            Cart = new ObservableCollection<Dish>();
            Tables = new List<Table>();

            GetMenu();
            GetDishlists();
            GetTables();
        }

        public void GetDishlists()
        {
            foreach (var item in DataProvider.Ins.Context.Dishlists.ToList())
            {
                Dishlists.Add(item);
            }
        }

        public void GetMenu()
        {
            Dishes.Clear();
            foreach (var item in DataProvider.Ins.Context.Dishes.ToList())
            {
                Dishes.Add(item);
            }
        }

        public void GetMenuByCategory(string? category)
        {
            Dishes.Clear();
            var _dishes = DataProvider.Ins.Context.Dishes.Include(x => x.Dishlist).Where(x => x.Dishlist.Name == category && x.DishlistId == x.Dishlist.Id).ToList();
            foreach (var item in _dishes)
            {
                Dishes.Add(item);
            }
        }

        public void SearchDishes(string? searchText)
        {
            Dishes.Clear();
            var _dishes = DataProvider.Ins.Context.Dishes.Where(x => x.Name.Contains(searchText)).ToList();
            foreach (var item in _dishes)
            {
                Dishes.Add(item);
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
            }
        }

        public void GetTables()
        {
            foreach (var table in DataProvider.Ins.Context.Tables.ToList())
            {
                Tables.Add(table);
            }
        }

        public bool OrderValidate()
        {
            if (SelectedTable is null)
            {
                MessageBox.Show("Vui long chon ban!");
                return false;
            }
            if (Cart.Count() < 1)
            {
                MessageBox.Show("Vui long chon mon an!");
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

        public void CreateOrders()
        {
            using (var context = new QuanannhatContext())
            {
                int _orderId = context.Orders.ToList().Count();
                foreach (var dish in Cart)
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
                context.SaveChanges();
            }
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
                    int orderCode = GetOrderBillIdByUserId(_UserId);
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
                }
            }
        }
        [RelayCommand]
        public void ExcuteMinus(object parameter)
        {
            foreach (var dish in Cart.ToList())
            {
                if (dish.Name.Equals(parameter))
                {
                    dish.Quantity -= 1;
                    if (dish.Quantity < 1)
                    {
                        Cart.Remove(dish);
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
