using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Options
{
    public class ConnectionOptions
    {

        public string DataSource { get; set; } = @"DESKTOP-SC2U7J5\SQLEXPRESS";
        public string Database { get; set; } = "AdventureWorks";
        public string User { get; set; } = "UserName1";
        public string Password { get; set; } = "asdf5678";
    }
}