using System;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace ServiceLayer
{
    public interface IServiceLayer
    {        
        List<PersonInfo> GetPersonInfoList(int count);
        
    }
}