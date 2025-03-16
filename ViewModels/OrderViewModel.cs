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
using System.Windows;

namespace QuanAnNhat.ViewModels
{
    partial class OrderViewModel : ObservableObject
    {
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

        public void CreateOrders()
        {
            DateTime dateTime = DateTime.Now;
            int _orderId = DataProvider.Ins.Context.Orders.Count() + 1;
            foreach (var dish in Cart)
            {
                Order order = new Order()
                {
                    Id = _orderId++,
                    //UserId = 1,
                    //Status = 2, //cho xac nhan
                    DishId = dish.Id,
                    Quantity = dish.Quantity,
                    //TableId = SelectedTable?.Id,
                    //Time = dateTime,
                    //Note = Notes,
                    TotalPrice = dish.Quantity * dish.Price
                };
                DataProvider.Ins.Context.Orders.Add(order);
            }
            DataProvider.Ins.Context.SaveChanges();

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
                CreateOrders();
                MessageBox.Show("Dat mon thanh cong!");
            }
        }
    }
}
