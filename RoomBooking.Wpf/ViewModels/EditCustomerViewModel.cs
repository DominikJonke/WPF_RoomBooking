using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Core.Validations;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerViewModel : BaseViewModel
    {
        private Customer _customer;
        private string _lastName;
        private string _firstName;
        private string _iban;
        private ICommand _cmdSave;
        private ICommand _cmdUndo;

        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                Validate();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string Iban
        {
            get => _iban;
            set
            {
                _iban = value;
                OnPropertyChanged(nameof(Iban));
                Validate();
            }
        }


        public EditCustomerViewModel(IWindowController controller, Customer customer) : base(controller)
        {
            Customer = customer;
            Init();

        }

        private void Init()
        {
            FirstName = Customer.FirstName;
            LastName = Customer.LastName;
            Iban = Customer.Iban;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult(
                    "Lastname is required",
                    new string[] { nameof(LastName) });
            }
            else if (LastName.Length < 2)
            {
                yield return new ValidationResult(
                    "Minimum length of Lastname is 2",
                    new string[] { nameof(LastName) }
                    );
            }
            else if (!IbanChecker.CheckIban(Iban))
            {
                yield return new ValidationResult(
                   "Iban must be valid",
                   new string[] { nameof(Iban) }
                   );
            }
        }

        public ICommand CmdSave
        {
            get
            {
                if (_cmdSave == null)
                {
                    _cmdSave = new RelayCommand(
                      execute: _ =>
                      {
                          using IUnitOfWork uow = new UnitOfWork();
                          _customer.FirstName = FirstName;
                          _customer.LastName = LastName;
                          _customer.Iban = Iban;
                          uow.Customers.Update(_customer);
                          uow.SaveAsync();
                          Controller.CloseWindow(this);

                      },
                      canExecute: _ => _customer != null);

                }
                return _cmdSave;
            }
            set { _cmdSave = value; }
        }

        public ICommand CmdUndo
        {
            get
            {
                if (_cmdUndo == null)
                {
                    _cmdUndo = new RelayCommand(
                      execute: _ =>
                      {
                          Init();

                      },
                      canExecute: _ => true);

                }
                return _cmdUndo;
            }
        }

    }
}
