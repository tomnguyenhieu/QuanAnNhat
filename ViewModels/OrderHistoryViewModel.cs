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
        public OrderHistoryViewModel()
        {
            OrderBills = new ObservableCollection<OrderBill>();
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
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 3 && ob.OrderStatus != 1).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();
                        break;
                    case "Cancelled":
                        _orderbills = await context.OrderBills.Where(ob => ob.BillStatus == 3 && ob.OrderStatus == 1).Include(ob => ob.Orders).ThenInclude(o => o.Dish).ToListAsync();

                        break;
                }
                foreach (var bill in _orderbills)
                {
                    OrderBills.Add(bill);
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

        }
    }
}
