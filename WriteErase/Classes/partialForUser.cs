using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteErase
{
    partial class User
    {
        public string userFullName
        {
            get
            {
                return $"{UserSurname} {UserName} {UserPatronymic}";
            }
        }
    }
}
