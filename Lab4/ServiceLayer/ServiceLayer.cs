using Converter;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace ServiceLayer
{
    public class ServiceLayer : IServiceLayer
    {
        public DataAccessLayer.DataAccessLayer dataAccessLayer;

        public ServiceLayer(DataAccessLayer.Options.ConnectionOptions options, IParser parser)
        {
            dataAccessLayer = new DataAccessLayer.DataAccessLayer(options, parser);
        }

        public PersonInfo GetPersonInfo(int id) 
        {
            Person person = dal.GetPerson(id);
            PersonInfo personInfo = GetInfo(person);

            return personInfo;
        }

        // Returns PersonInfo list with id in range (1,count).
        public List<PersonInfo> GetPersonInfoList(int count)
        {
            List<PersonInfo> list = new List<PersonInfo>();
            PersonInfo person;

            for (int id = 1; id < count; id++)
            {
                person = GetPersonInfo(id);
                list.Add(person);
            }

            return list;
        }

        public List<PersonInfo> GetPersonInfoList(int startIndex, int count)
        {
            List<PersonInfo> list = new List<PersonInfo>();
            PersonInfo person;

            for (int id = startIndex; id < startIndex + count; id++)
            {
                person = GetPersonInfo(id);
                list.Add(person);
            }

            return list;
        }
        
        // Fills PersonInof.
        PersonInfo GetInfo(Person person)
        {
            PersonInfo personInfo = new PersonInfo();
            int id = person.BusinessEntityID;

            personInfo.PersonPhone = dataAccessLayer.GetPersonOpts<PersonPhone>(id);
            personInfo.Address = dataAccessLayer.GetPersonOpts<Address>(id);
            personInfo.Person = dataAccessLayer.GetPersonOpts<Person>(id);
            personInfo.BusinessEntityAddress = dataAccessLayer.GetPersonOpts<BusinessEntityAddress>(id);
            personInfo.Password = dataAccessLayer.GetPersonOpts<Password>(id);

            return personInfo;
        }
    }
}