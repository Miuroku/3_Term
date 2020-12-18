using System;
using System.Collections.Generic;

namespace Converter
{

    public interface IParser
    {

        T Map<T>(Dictionary<string, object> dictionary);
	    T DeserializeXML<T>(string xml) where T : new();
        T DeserializeJSON<T>(string json) where T : new();
    }
}