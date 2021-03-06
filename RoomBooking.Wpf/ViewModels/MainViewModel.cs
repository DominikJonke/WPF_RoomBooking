﻿using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Room> _rooms;
        private ObservableCollection<Booking> _bookings;
        private Booking _selectedBooking;
        private Room _selectedRoom;
        private Booking _selectedCurrentBooking;
        private ICommand _cmdEditCustomers;

        public Booking SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBooking));
            }
        }
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));

            }
        }
        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }
        public ObservableCollection<Room> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }
        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        private async Task LoadDataAsync()
        {
            using IUnitOfWork uow = new UnitOfWork();

            var rooms = await uow.Rooms
                .GetAllAsync();
            Rooms = new ObservableCollection<Room>(rooms);
            SelectedRoom = Rooms.First();
            await LoadBookingsAsync();
        }

        private async Task LoadBookingsAsync()
        {
            _selectedCurrentBooking = SelectedBooking;
            using IUnitOfWork uow = new UnitOfWork();

            var bookings = await uow.Bookings
                .GetByRoomWithCustomerAsync(SelectedRoom.Id);
            Bookings = new ObservableCollection<Booking>(bookings);

            if(_selectedCurrentBooking == null)
            {
                SelectedBooking = Bookings.First();
            }
            else
            {
                SelectedBooking = _selectedCurrentBooking;
            }
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        public ICommand CmdEditCustomers
        {
            get
            {
                if(_cmdEditCustomers == null)
                {
                    _cmdEditCustomers = new RelayCommand(
                        execute: _ =>
                        {
                            Controller.ShowWindow(new EditCustomerViewModel(Controller, SelectedBooking.Customer), true);
                            LoadDataAsync();
                        },
                        canExecute: _ => SelectedBooking != null);
                }

                return _cmdEditCustomers;
            }
        }
    }
}
