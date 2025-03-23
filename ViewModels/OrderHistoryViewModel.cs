using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;
using QuanAnNhat.Singletons;

namespace QuanAnNhat.ViewModels
{
    partial class OrderHistoryViewModel : ObservableObject
    {
        public ObservableCollection<OrderBill> OrderBills { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        private bool IsDiscount = false;
        private string? StatusText;
        private Brush BaseIconColor;

        [ObservableProperty]
        private string? _PaymentMethodText;
        [ObservableProperty]
        private Discount _DiscountVouncher;
        [ObservableProperty]
        private int _OrderBillId;
        [ObservableProperty]
        private int _OrderTableId;
        [ObservableProperty]
        private string? _OrderBillNotes;
        [ObservableProperty]
        private int? _OrderBillSubtotal;
        [ObservableProperty]
        private int? _OrderBillTotal;
        [ObservableProperty]
        private Brush _OrderingIcon;
        [ObservableProperty]
        private Brush _ServedIcon;
        [ObservableProperty]
        private Brush _DoneIcon;
        [ObservableProperty]
        private Brush _Arrow1;
        [ObservableProperty]
        private Brush _Arrow2;

        public OrderHistoryViewModel()
        {
            Init();
            GetOrderBills("Ordered");
            LiveData();
        }

        public void Init()
        {
            OrderBills = new ObservableCollection<OrderBill>();
            Orders = new ObservableCollection<Order>();
            DiscountVouncher = new Discount()
            {
                DiscountPrice = 0
            };

            OrderBillSubtotal = 0;
            OrderBillTotal = 0;
            OrderBillNotes = null;
            StatusText = "Ordered";

            BaseIconColor = new SolidColorBrush(Color.FromRgb(66, 21, 13));
        }

        public void InitIconColor(int status)
        {
            Arrow1 = BaseIconColor.Clone();
            Arrow2 = BaseIconColor.Clone();
            OrderingIcon = BaseIconColor.Clone();
            ServedIcon = BaseIconColor.Clone();
            DoneIcon = BaseIconColor.Clone();

            switch (status)
            {
                case 1:
                    Arrow1.Opacity = 0.5;
                    Arrow2.Opacity = 0.5;
                    OrderingIcon.Opacity = 0.5;
                    ServedIcon.Opacity = 0.5;
                    DoneIcon.Opacity = 0.5;
                    break;
                case 2:
                    Arrow1.Opacity = 0.5;
                    Arrow2.Opacity = 0.5;
                    ServedIcon.Opacity = 0.5;
                    DoneIcon.Opacity = 0.5;
                    break;
                case 3:
                    Arrow2.Opacity = 0.5;
                    DoneIcon.Opacity = 0.5;
                    break;
            }
        }

        public async void GetOrderBills(string? text)
        {
            OrderBills.Clear();
            using (var context = new QuanannhatContext())
            {
                List<OrderBill> _orderbills = new List<OrderBill>();
                switch (text)
                {
                    case "Ordered":
                        StatusText = text;
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 3).OrderByDescending(o => o.Id).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();
                        break;
                    case "Cancelled":
                        StatusText = text;
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 1).OrderByDescending(o => o.Id).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();
                        break;
                }
                foreach (var bill in _orderbills)
                {
                    OrderBills.Add(bill);
                }
            }

            OrderBillId = 0;
            OrderTableId = 0;
            OrderBillNotes = null;
            GetBillDetails(new OrderBill());
        }

        public void GetBillDetails(OrderBill orderBill)
        {
            OrderBillSubtotal = 0;
            OrderBillTotal = 0;
            OrderBillNotes = orderBill.Note;
            if (orderBill.Id != 0)
            {
                PaymentMethodText = "Checkout";
            } else
            {
                PaymentMethodText = "";
            }

            Orders.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = context.Orders.Where(o => o.OrderbillId == orderBill.Id).Include(d => d.Dish).ToList();
                foreach (var order in res)
                {
                    Orders.Add(order);
                }
            }

            foreach (var order in Orders)
            {
                if (order.OrderbillId == orderBill.Id)
                {
                    if (!IsDiscount)
                    {
                        OrderBillSubtotal += order.TotalPrice;
                        OrderBillTotal = OrderBillSubtotal;
                    }
                    else
                    {
                        DiscountVouncher = orderBill.Discount;
                        OrderBillSubtotal += order.TotalPrice;
                        OrderBillTotal = OrderBillSubtotal - DiscountVouncher.DiscountPrice;
                    }
                }
            }
        }

        public async void LiveData()
        {
            while (true)
            {
                await Task.Delay(100);
                using (var context = new QuanannhatContext())
                {
                    if (OrderBillId != 0 && StatusText.Equals("Ordered"))
                    {
                        var res = await context.OrderBills.Where(b => b.Id == OrderBillId).FirstAsync();
                        InitIconColor(Convert.ToInt32(res.OrderStatus));
                    } else
                    {
                        InitIconColor(1);
                    }
                }
            }
        }

        [RelayCommand]
        public void ExcuteGetOrderBills(object parameter)
        {
            GetOrderBills(parameter.ToString());
        }

        [RelayCommand]
        public void ExcuteGetBillDetails(object parameter)
        {
            foreach (var bill in OrderBills)
            {
                if (bill.Id == Convert.ToInt32(parameter.ToString()))
                {
                    OrderBillId = bill.Id;
                    OrderTableId = Convert.ToInt32(bill.TableId);

                    IsDiscount = (bill.Discount != null) ? true : false;
                    
                    GetBillDetails(bill);
                }
            }
        }
    }
}
