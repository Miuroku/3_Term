using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Converter
{
    public class Converter : IParser
    {
        public T DeserializeXML<T>(string xml) where T : new()
        {
            List<string> list = SplitXML(xml);
            T res = Deserialize<T>(list);

            return res;
        }

        public T DeserializeJSON<T>(string json) where T : new()
        {
            List<string> list = SplitJSON(json);
            T res = Deserialize<T>(list);

            return res;
        }        

        T Deserialize<T>(List<string> list) where T : new()
        {
            Type type = typeof(T);
            T res = new T();
            string key;
            string value;
            Regex complexRgx = new Regex(@"""(\w+)""\s*:\s*{(.*)}", RegexOptions.Singleline);
            Regex simpleRgx = new Regex(@"""(\w+)""\s*:\s*(.*)", RegexOptions.Singleline);
            Match match;

            foreach (string prop in list)
            {
                match = complexRgx.Match(prop);

                if (match.Success)
                {
                    key = match.Groups[1].ToString();
                    value = match.Groups[2].ToString();
                    PropertyInfo info = type.GetProperty(key);
                    info.SetValue(res, typeof(Converter).GetMethod("DeserializeJSON").MakeGenericMethod(new Type[] { info.PropertyType }).Invoke(null, new object[] { value }));
                }
                else
                {
                    match = simpleRgx.Match(prop);

                    if (match.Success)
                    {
                        key = match.Groups[1].Value;
                        value = match.Groups[2].Value.Trim(new char[] { '"' });
                        PropertyInfo info = type.GetProperty(key);
                        info.SetValue(res, Convert.ChangeType(value, info.PropertyType));
                    }
                }
            }
            return res;
        }

        static List<string> SplitXML(string xml)
        {
            List<string> list = new List<string>();
            StringBuilder line = new StringBuilder();
            StringBuilder tag = new StringBuilder();
            StringBuilder value = new StringBuilder();
            List<string> tags = new List<string>();
            string baseTag = "";
            bool isTag = false;

            if (GetNextTag(xml, 0) == "root")
            {
                xml = new Regex(@"<root>(.*)</root>", RegexOptions.Singleline)
                    .Match(xml)
                    .Groups[1].ToString();
            }

            foreach (char ch in xml)
                if (!char.IsWhiteSpace(ch))
                {
                    if (ch == '<')
                    {
                        isTag = true;
                        continue;
                    }
                    if (ch == '>')
                    {
                        isTag = false;
                        if (tag[0] == '/')
                        {
                            string curTag = tag.ToString().Trim(new char[] { '/' });
                            if (value.ToString() != "")
                            {
                                if (tags.Contains(baseTag))
                                {
                                    line.Append($",\"{curTag}\":{value}");
                                    tags.Remove(curTag);
                                    value.Clear();
                                }
                                else
                                {
                                    for (int i = 0; tags[i] != curTag; i++)
                                    {
                                        line.Append($"\"{tags[i]}\":{{");
                                        baseTag = tags[i];
                                    }
                                    line.Append($"\"{curTag}\":{value}");
                                    tags.Remove(curTag);
                                    value.Clear();
                                }
                            }
                            else
                            {
                                tags.Remove(curTag);
                                line.Append("}");
                            }
                            if (tags.Count == 0)
                            {
                                list.Add(line.ToString());
                                line.Clear();
                            }
                        }
                        else
                        {
                            tags.Add(tag.ToString());
                        }
                        tag.Clear();
                        continue;
                    }
                    if (isTag)
                    {
                        tag.Append(ch);
                    }
                    else
                    {
                        value.Append(ch);
                    }
                }
            return list;
        }

        static string GetNextTag(string str, int i)
        {
            bool isTag = false;
            char ch;
            StringBuilder tag = new StringBuilder();

            for (; i < str.Length; i++)
            {
                ch = str[i];
                if (ch == '<')
                {
                    isTag = true;
                }
                else
                {
                    if (ch == '>')
                    {
                        break;
                    }
                    if (isTag)
                    {
                        tag.Append(ch);
                    }
                }
            }
            return tag.ToString();
        }

        static List<string> SplitJSON(string json)
        {
            var list = new List<string>();
            int bracketsCount = 0;
            var line = new StringBuilder();

            json = json.Trim(new char[] { '{', ' ', '}' });

            foreach (char ch in json)
            {
                if (char.IsLetterOrDigit(ch) || char.IsPunctuation(ch) || ch == '"')
                {
                    if ((ch == ',') && (bracketsCount == 0))
                    {

                        list.Add(line.ToString());
                        line.Clear();
                    }
                    else
                    {
                        if (ch == '{')
                        {
                            line.Append(ch);
                            bracketsCount++;
                        }
                        else
                        {
                            if (ch == '}')
                            {
                                line.Append(ch);
                                bracketsCount--;
                            }
                            else
                            {
                                line.Append(ch);
                            }
                        }
                    }
                }
            }
            list.Add(line.ToString());

            return list;
        }

        public T Map<T>(Dictionary<string, object> dictionary)
        {
            T ans = (T)Activator.CreateInstance(typeof(T));

            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value.GetType() != typeof(DBNull))
                {
                    SetMemberValue(ans, pair.Key, pair.Value);
                }
                else
                {
                    SetMemberValue(ans, pair.Key, null);
                }
            }

            return ans;
        }

        private void SetMemberValue<T>(T obj, string key, object value)
        {
            Type type = typeof(T);
            
            if (type.GetProperty(key) != null)
            {
                PropertyInfo info = type.GetProperty(key);
                info.SetValue(obj, value);
            }
            else
            {
                if (type.GetField(key) != null)
                {
                    FieldInfo info = type.GetField(key);
                    info.SetValue(obj, value);
                }
                else
                {
                    throw new Exception($"{obj.GetType()} type does not contain member with {key} key");
                }
            }
        }
    }
}