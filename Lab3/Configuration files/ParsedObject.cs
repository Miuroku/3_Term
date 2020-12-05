using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    class ParsedObject
    {
        private Dictionary<string, ParsedObject> subObjects = new Dictionary<string, ParsedObject>();
        private object myValue;

        public Type myType;
        public enum Type
        {
            //unordered complex type
            NestedType, 
            //ordered complex type
            Array,  
            SingleValue
        }

        public ParsedObject(Type type)
        {
            myType = type;
        }

        public void AddSubObjectByKey(string key, ParsedObject subObject)
        {
            if (subObjects.ContainsKey(key))
            {
                throw new Exception("aaa");
            }

            subObjects.Add(key, subObject);
        }

        public void AddSubObjectByIndex(int index, ParsedObject subObject)
        {
            subObjects.Add(index.ToString(), subObject);
        }

        public void SetValue(string text)
        {
            myValue = text;

        }

        public object GetValue()
        {
            if (subObjects.Count != 0)
                return null;

            return myValue;
        }

        public ParsedObject GetSubObjectByKey(string key)
        {
            if (!subObjects.ContainsKey(key))
                return null;

            return subObjects[key];
        }

        public ParsedObject GetSubObjectByIndex(int index)
        {
            if (!subObjects.ContainsKey(index.ToString()))
                return null;

            return subObjects[index.ToString()];
        }
    }
}
