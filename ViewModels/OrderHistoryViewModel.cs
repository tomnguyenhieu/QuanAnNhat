using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using QuanAnNhat.DBContext;
using QuanAnNhat.Models;

namespace QuanAnNhat.ViewModels
{
    partial class OrderHistoryViewModel : ObservableObject
    {
        public ObservableCollection<OrderBill> OrderBills { get; set; }
        public ObservableCollection<Order> Orders { get; set; }

        [ObservableProperty]
        private int? _OrderBillId;
        [ObservableProperty]
        private int? _OrderTableId;
        [ObservableProperty]
        private int? _OrderBillSubtotal;
        [ObservableProperty]
        private int? _OrderBillTotal;
        [ObservableProperty]
        private string? _TextButton;

        private bool IsDiscount = false;

        public OrderHistoryViewModel()
        {
            OrderBills = new ObservableCollection<OrderBill>();
            Orders = new ObservableCollection<Order>();

            OrderBillSubtotal = 0;
            OrderBillTotal = 0;
            TextButton = "Cancel";

            GetOrderBills("Ordered");
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
                        TextButton = "Cancel";
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 3 && ob.OrderStatus != 1).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();
                        break;
                    case "Cancelled":
                        TextButton = "Reorder";
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 3 && ob.OrderStatus == 1).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();
                        break;
                }
                foreach (var bill in _orderbills)
                {
                    OrderBills.Add(bill);
                }
            }

            OrderBillId = null;
            OrderTableId = null;
            GetBillDetails(new OrderBill());
        }

        public async void GetBillDetails(OrderBill orderBill)
        {
            OrderBillSubtotal = 0;
            OrderBillTotal = 0;

            Orders.Clear();
            using (var context = new QuanannhatContext())
            {
                var res = await context.Orders.Where(o => o.OrderbillId == orderBill.Id).Include(d => d.Dish).ToListAsync();
                foreach (var order in res)
                {
                    Orders.Add(order);
                }
            }

            if (!IsDiscount)
            {
                foreach (var order in Orders)
                {
                    OrderBillSubtotal = order.Quantity * order.Dish.Price;
                    OrderBillTotal = OrderBillSubtotal;
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
                    OrderTableId = bill.TableId;

                    IsDiscount = (bill.Discount != null) ? true : false;
                    
                    GetBillDetails(bill);
                }
            }
        }
    }
}
