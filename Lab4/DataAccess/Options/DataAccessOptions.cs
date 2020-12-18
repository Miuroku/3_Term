using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Options
{
    public class DataAccessOptions
    {        
        public SendingOptions SendingOptions { get; set; } = new SendingOptions();
        public ConnectionOptions ConnectionOptions { get; set; } = new ConnectionOptions();

    }
}