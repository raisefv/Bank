using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Classes
{
    public class Client
    {
        public string FullName { get; set; }
        public string PassportNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Client(string fullName, string passportNumber, DateTime dateOfBirth)
        {
            FullName = fullName;
            PassportNumber = passportNumber;
            DateOfBirth = dateOfBirth;
        }
    }
}
