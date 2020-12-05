using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    class XMLParser : IConfigurationParser
    {
        private string text;
        private int currentPoint;
        static private readonly string gapSymbols = "\n\r\t ";

        public ParsedObject Parse(string text)
        {
            currentPoint = 0;
            this.text = text;
            // Reading header.
            ReadTag(); 
            text = text.Insert(text.Length, " </endTag>");
            this.text = text;

            return Go();
        }

        private ParsedObject Go()
        {
            ParsedObject newObject;
            if (!isNextSymbolIsBracket())
            {
                newObject = new ParsedObject(ParsedObject.Type.SingleValue);
                newObject.SetValue(ReadTillBracket());
                return newObject;
            }

            newObject = new ParsedObject(ParsedObject.Type.NestedType);
            while (true)
            {
                string tag = ReadTag();
                newObject.AddSubObjectByKey(tag, Go());
                ReadTag(); // closing
                if (isNextTagIsClosing()) break;
            }
            return newObject;
        }

        private bool isNextTagIsClosing()
        {
            while (text[currentPoint] != '<') currentPoint++;
            if (text[currentPoint + 1] == '/') return true;
            return false;
        }

        private bool isNextSymbolIsBracket()
        {
            while (gapSymbols.Contains(Convert.ToString(text[currentPoint]))) currentPoint++;
            return text[currentPoint] == '<';
        }

        private string ReadTag ()
        {
            string tag = "";
            while (text[currentPoint] != '<') currentPoint++;
            currentPoint++;
            while (text[currentPoint] != '>')
            {
                tag += text[currentPoint++];
            }
            currentPoint++;
            return tag;
        }

        private string ReadTillBracket ()
        {
            string response = "";
            while (text[currentPoint] != '<')
            {
                response += text[currentPoint++];
            }
            return response;
        }
    }
}