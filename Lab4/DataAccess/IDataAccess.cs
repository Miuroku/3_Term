using Converter;
using DataAccessLayer.Models;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public interface IDataAccessLayer
    {
        Person GetPerson(int id);
        T GetPersonOpts<T>(int id) where T : new();
    }
}