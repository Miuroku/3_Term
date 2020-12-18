using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PersonInfo
    {
        public Person Person { get; set; }
        public Address Address { get; set; }
        public Password Password { get; set; }
        public BusinessEntityAddress BusinessEntityAddress { get; set; }
        public Phone PersonPhone { get; set; }    

        public PersonInfo(Person person, Password password, Phone personPhone, Address address, BusinessEntityAddress businessEntityAddress)
        {
            Person = person;
            Address = address;
            Password = password;
            BusinessEntityAddress = businessEntityAddress;
            PersonPhone = personPhone;
        }

        public PersonInfo()
        {

        }

    }
}