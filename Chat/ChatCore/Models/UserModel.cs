using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ChatCore.Models
{
    public class UserModel : BaseUserModel
    {
        public string Id { get; set; }
        private string _firstName;
        [RegularExpression("^[A-Za-z]+(((\'|-|.)?([A-Za-z])+))?$", ErrorMessage = "Wrong format")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (String.Equals(_firstName, value))
                    return;

                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        [EmailAddress]
        public string Email
        {
            get => _email;
            set
            {
                if (String.Equals(_email, value))
                    return;

                _email = value;
                OnPropertyChanged();
            }
        }

        private string _phone;
        [Phone]
        public string Phone
        {
            get => _phone;
            set
            {
                if (String.Equals(_phone, value))
                    return;

                _phone = value;
                OnPropertyChanged();
            }
        }

        private string _lastName;
        [RegularExpression("^[A-Za-z]+((s)?((\'|-|.)?([A-Za-z])+))*$", ErrorMessage = "Wrong format")]
        public string LastName
        {
            get => _lastName;
            set
            {
                if (String.Equals(_lastName, value))
                    return;

                _lastName = value;
                OnPropertyChanged();
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (String.Equals(_status, value))
                    return;

                _status = value;
                OnPropertyChanged();
            }
        }

        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (String.Equals(_city, value))
                    return;

                _city = value;
                OnPropertyChanged();
            }
        }

        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                if (String.Equals(_country, value))
                    return;

                _country = value;
                OnPropertyChanged();
            }
        }

        private string _job;
        public string Job
        {
            get => _job;
            set
            {
                if (String.Equals(_job, value))
                    return;

                _job = value;
                OnPropertyChanged();
            }
        }

        private string _company;
        public string Company
        {
            get => _company;
            set
            {
                if (String.Equals(_company, value))
                    return;

                _company = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _birthday;
        public DateTime? Birthday
        {
            get => _birthday;
            set
            {
                if (String.Equals(_birthday, value))
                    return;

                _birthday = value;
                OnPropertyChanged();
            }
        }

        private string _postAddress;
        public string PostAddress
        {
            get => _postAddress;
            set
            {
                if (String.Equals(_postAddress, value))
                    return;

                _postAddress = value;
                OnPropertyChanged();
            }
        }
    }
}
