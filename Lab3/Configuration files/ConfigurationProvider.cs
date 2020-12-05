using System;

namespace Configuration
{
    class ConfigurationProvider
    {

        public void GetFilledOptions(object configModel, string text, IConfigurationParser parser)
        {
            ParsedObject parsedObject = null;

            try
            {
                parsedObject = parser.Parse(text);
            }
            catch
            {
                throw new Exception("Couldn't parse file");
            }

            FillOptions(configModel, parsedObject);
        }

        public void FillOptions(object configModel, ParsedObject parsedObject)
        {
            var fields = configModel.GetType().GetFields();

            foreach (var field in fields)
            {
                ParsedObject subObject = parsedObject.GetSubObjectByKey(field.Name);

                if (subObject == null)
                    continue;

                if (subObject.myType == ParsedObject.Type.SingleValue)
                {
                    try
                    {
                        field.SetValue(configModel, Convert.ChangeType(subObject.GetValue(), field.FieldType));
                    }
                    catch { }

                    continue;
                }

                if (subObject.myType == ParsedObject.Type.NestedType && field.FieldType.IsNested)
                {
                    FillOptions(field.GetValue(configModel), subObject);

                    continue;
                }

                if (subObject.myType == ParsedObject.Type.Array && field.FieldType.IsArray)
                {
                    var array = (Array)field.GetValue(configModel);

                    if (array.GetType().GetElementType().IsNested || array.GetType().GetElementType().IsArray)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            ParsedObject subsubObject = subObject.GetSubObjectByIndex(i);

                            if (subsubObject == null)
                                continue;

                            if (subsubObject.myType != ParsedObject.Type.NestedType && subsubObject.myType != ParsedObject.Type.Array)
                            {
                                continue;
                            }

                            FillOptions(array.GetValue(i), subObject.GetSubObjectByIndex(i));
                        }

                        continue;
                    }

                    if (!array.GetType().IsNested && !array.GetType().GetElementType().IsArray)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            try
                            {
                                array.SetValue(Convert.ChangeType(subObject.GetSubObjectByIndex(i).GetValue(), array.GetType().GetElementType()), i);
                            }
                            catch { }
                        }
                    }

                }

            }
        }

    }
}

