using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Configuration
{
    class JsonParser : IConfigurationParser
    {
        static private readonly string gapSymbols = "\n\r\t ";

        public ParsedObject Parse (string text)
        {
            return ParseJsonEntity(text).Item1;
        }

        private Tuple<ParsedObject, int> ParseJsonEntity (string text, int startPoint = 0)
        {
            if (text[startPoint] == '{')
                return ParseJsonObject(text, startPoint + 1);
            else if (text[startPoint] == '[')
                return ParseJsonArray(text, startPoint + 1);

            throw new Exception("Text can not be parsed");
        }

        private Tuple<ParsedObject, int> ParseJsonObject (string text, int currentPoint)
        {
            ParsedObject parsedObject = new ParsedObject(ParsedObject.Type.NestedType);

            while (true)
            {
                Tuple<string, int> nextKey = GetJsonKey(text, currentPoint);
                currentPoint = nextKey.Item2 + 1;
                Tuple<ParsedObject, int> nextValue = GetJsonValue(text, currentPoint);
                currentPoint = nextValue.Item2;
                parsedObject.AddSubObjectByKey(nextKey.Item1, nextValue.Item1);
                while (gapSymbols.Contains(Convert.ToString(text[currentPoint]))) currentPoint++;
                if (text[currentPoint] == '}') break;
                currentPoint++;
            }

            return new Tuple<ParsedObject, int>(parsedObject, currentPoint);
        }

        private Tuple<ParsedObject, int> ParseJsonArray (string text, int currentPoint)
        {
            ParsedObject parsedObject = new ParsedObject(ParsedObject.Type.Array);
            int i = 0;
            while (true)
            {
                Tuple<ParsedObject, int> nextValue = GetJsonValue(text, currentPoint);
                currentPoint = nextValue.Item2;

                parsedObject.AddSubObjectByIndex(i++, nextValue.Item1);
                while (gapSymbols.Contains(Convert.ToString(text[currentPoint]))) currentPoint++;
                if (text[currentPoint] == ']') break;
                currentPoint++;
            }
            return new Tuple<ParsedObject, int>(parsedObject, currentPoint);
        }

        private string myTrim (string text)
        {
            while (gapSymbols.Contains(Convert.ToString(text[0]))) text = text.Remove(0, 1);
            while (gapSymbols.Contains(Convert.ToString(text[text.Length - 1]))) text = text.Remove(text.Length - 1, 1);
            if (text[0] == '\"') text = text.Remove(0, 1);
            if (text[text.Length - 1] == '\"') text = text.Remove(text.Length - 1, 1);
            return text;
        }

        private Tuple<string, int> GetJsonKey (string text, int currentPoint)
        {
            string jsonKey = "";
            while (text[currentPoint] != ':') jsonKey += text[currentPoint++];

            jsonKey = myTrim(jsonKey);
            return new Tuple<string, int>(jsonKey, currentPoint);
        }

        private Tuple<ParsedObject, int> GetJsonValue (string text, int currentPoint)
        {
            string jsonValue = "";
            string symbols = ",[]{}";
            bool quotationMarkWasClosed = true;
            while (!symbols.Contains(Convert.ToString(text[currentPoint])) || (symbols.Contains(Convert.ToString(text[currentPoint])) && !quotationMarkWasClosed))
            {
                if (text[currentPoint] == '\"')
                    quotationMarkWasClosed ^= true;
                jsonValue += text[currentPoint++];
            }
            if (text[currentPoint] == '{' || text[currentPoint] == '[')
            {
                Tuple<ParsedObject, int> nextItem = ParseJsonEntity(text, currentPoint);
                return new Tuple<ParsedObject, int>(nextItem.Item1, nextItem.Item2 + 1);
            }
            ParsedObject response = new ParsedObject(ParsedObject.Type.SingleValue);
            response.SetValue(myTrim(jsonValue));
            return new Tuple<ParsedObject, int>(response, currentPoint);
        }
    }
}