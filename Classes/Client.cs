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
        public DateTime DateBirth { get; set; }

        public Client(string fullName, string passportNumber, DateTime dateBirth)
        {
            FullName = fullName;
            PassportNumber = passportNumber;
            DateBirth = dateBirth;
        }
    }
}
