using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ViSo.SharedEnums;

namespace GeneralExtensions
{
    public static class ObjectExstentions
    {
        public static bool TryToBool<T>(this T value)
        {
            try
            {
                if (value == null)
                {
                    return false;
                }

                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        public static int ToInt32<T>(this T value)
        {
            return Convert.ToInt32(value);
        }

        public static long ToInt64<T>(this T value)
        {
            return Convert.ToInt64(value);
        }

        public static decimal ToDecimal<T>(this T value)
        {
            StringBuilder resultText = new StringBuilder();

            bool hasDecimal = false;

            string valueString = value.ToString();

            string seperator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            for (int x = valueString.Length - 1; x >= 0; --x)
            {
                char read = valueString[x];

                if (Char.IsNumber(read))
                {
                    resultText.Insert(0, read);

                    continue;
                }
                else if (read == '-' && x == 0)
                {
                    resultText.Insert(0, read);

                    continue;
                }
                else if (hasDecimal)
                {
                    continue;
                }

                resultText.Insert(0, seperator);

                hasDecimal = true;
            }

            return Convert.ToDecimal(resultText.ToString());
        }

        public static double ToDouble<T>(this T value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static string ParseToString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        public static DateTime TryToDate<T>(this T value)
        {
            try
            {
                if (value == null)
                {
                    return DateTime.Now;
                }

                return Convert.ToDateTime(value);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static T To<T>(this object item)
        {
            if (item == null)
            {
                return default(T);
            }

            return (T)item;
        }

        //public static object CloneTo<T>(this T source, Type resultT)
        //{
        //  Type sourceT = source.GetType();

        //  ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);

        //  object result = constructorInfo.Invoke(null);

        //  foreach (PropertyInfo item in sourceT.GetProperties())
        //  {
        //    try
        //    {
        //      PropertyInfo resultP = resultT.GetProperty(item.Name);

        //      if (resultP == null)
        //      {
        //        continue;
        //      }

        //      resultP.SetValue(result, item.GetValue(source, null), null);
        //    }
        //    catch
        //    {
        //      // Do Nothing
        //    }
        //  }

        //  return result;
        //}

        //public static T Clone<T>(this T source)
        //{
        //  Type sourceT = source.GetType();

        //  Type resultT  = source.GetType();

        //  ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);

        //  object result = constructorInfo.Invoke(null);

        //  foreach (PropertyInfo item in sourceT.GetProperties())
        //  {
        //    try
        //    {
        //      PropertyInfo resultP = resultT.GetProperty(item.Name);

        //      if (resultP == null)
        //      {
        //        continue;
        //      }

        //      resultP.SetValue(result, item.GetValue(source, null), null);
        //    }
        //    catch
        //    {
        //      // Do Nothing
        //    }
        //  }

        //  return (T)result;
        //}

        public static byte[] ZipFile<T>(this T source)
        {
            byte[] result = null;

            using (MemoryStream queryStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(queryStream, CompressionMode.Compress))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(zipStream, source);
                }

                result = queryStream.ToArray();
            }

            return result ?? (new byte[] { });
        }

        public static bool? AreEqual<T>(this T source, object compareTo, string[] skipProperties)
        {
            var sourceType = source.GetType();

            var compareType = compareTo.GetType();

            foreach (PropertyInfo item in sourceType.GetProperties())
            {
                if (!item.CanRead || skipProperties.Contains(item.Name))
                {
                    continue;
                }

                try
                {
                    var resultP = compareType.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        return false;
                    }

                    if (item.GetValue(source, null).ParseToString() != resultP.GetValue(compareTo, null).ParseToString())
                    {
                        return false;
                    }
                }
                catch (Exception err)
                {
                    //return null;
                }
            }

            return true;
        }

        public static bool? AreEqual<T>(this T source, object compareTo, string[] skipProperties, out List<string> failedProperties)
        {
            var sourceType = source.GetType();

            var compareType = compareTo.GetType();

            failedProperties = new List<string>();

            bool result = true;

            foreach (PropertyInfo item in sourceType.GetProperties())
            {
                if (!item.CanRead || skipProperties.Contains(item.Name))
                {
                    continue;
                }

                try
                {
                    var resultP = compareType.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        failedProperties.Add(item.Name);

                        result = false;
                    }

                    if (item.GetValue(source, null).ParseToString() != resultP.GetValue(compareTo, null).ParseToString())
                    {
                        failedProperties.Add(item.Name);

                        result = false;
                    }
                }
                catch (Exception err)
                {
                    //return null;
                }
            }

            return result;
        }

        public static T CopyTo<T>(this T source, T result)
        {
            var sourceT = source.GetType();

            var resultT = result.GetType();

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (item.CanRead && !item.CanWrite)
                {
                    continue;
                }

                try
                {
                    var resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return result;
        }

        public static T TryCast<T>(object o)
        {
            return (T)o;
        }

        public static object CopyToObject<T>(this T source, object result)
        {
            var sourceT = source.GetType();

            var resultT = result.GetType();

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (item.CanRead && !item.CanWrite)
                {
                    continue;
                }

                try
                {
                    var resultP = resultT.GetProperty(item.Name);

                    if (resultP == null)
                    {
                        continue;
                    }

                    resultP.SetValue(result, item.GetValue(source, null), null);
                }
                catch
                {
                    // Do Nothing
                }
            }

            return result;
        }

        public static List<object> CopyToObject<T>(this List<T> source, Type resultType)
        {
            List<object> result = new List<object>();

            foreach (T item in source)
            {
                var instance = Activator.CreateInstance(resultType);

                result.Add(item.CopyToObject(instance));
            }

            return result;
        }

        public static object[] CopyToObject<T>(this T[] source, Type resultType)
        {
            List<object> result = new List<object>();

            foreach (T item in source)
            {
                var instance = Activator.CreateInstance(resultType);

                result.Add(item.CopyToObject(instance));
            }

            return result.ToArray();
        }

        public static void SetPropertyValue<T>(this T source, string name, object value) //, bool converter = true)
        {
            Type obj = source.GetType();

            PropertyInfo info = obj.GetProperty(name);

            if (info == null)
            {
                return;
            }

            try
            {
                info.SetValue(source, value, null);
            }
            catch
            {
                // DO NOTHING: This may be because the property was not initialized and is NULL
                //throw;
            }
        }

        public static void SetPropertyValue<T>(this T source, string name, string value) //, bool converter = true)
        {
            Type obj = source.GetType();

            PropertyInfo info = obj.GetProperty(name);

            if (info == null)
            {
                return;
            }

            try
            {
                Type propertyType = info.PropertyType;

                Type targetType = propertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;

                object objectValue = null;

                if (targetType == typeof(Brush))
                {
                    objectValue = (SolidColorBrush)new BrushConverter().ConvertFromString(value);
                }
                else if (targetType == typeof(FontFamily))
                {
                    objectValue = new FontFamily(value);
                }
                else if (targetType.BaseType == typeof(Enum))
                {
                    objectValue = Enum.Parse(targetType, value);
                }
                else if (targetType == typeof(Thickness))
                {
                    string[] thicknessValues = value.Split(',');

                    if (thicknessValues.Length == 1)
                    {
                        objectValue = new Thickness(thicknessValues[0].ToDouble());
                    }
                    else
                    {
                        objectValue = new Thickness
                            (
                                thicknessValues[0].ToDouble(),
                                thicknessValues[1].ToDouble(),
                                thicknessValues[2].ToDouble(),
                                thicknessValues[3].ToDouble()
                            );
                    }
                }
                else if (targetType == typeof(FontWeight))
                {
                    FontWeightConverter converter = new FontWeightConverter();

                    objectValue = (FontWeight)converter.ConvertFromString(value);
                }
                else if (targetType == typeof(System.Windows.Media.Color))
                {
                    objectValue = (Color)ColorConverter.ConvertFromString(value);
                }
                else
                {
                    objectValue = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }

                info.SetValue(source, objectValue, null);
            }
            catch
            {
                // DO NOTHING: This may be because the property was not initialized and is NULL
                throw;
            }
        }

        public static object GetPropertyValue<T>(this T source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type obj = source.GetType();

            PropertyInfo info = obj.GetProperty(name);

            if (info == null)
            {
                return null;
            }

            try
            {
                return info.GetValue(source, null);
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public static object InvokeMethod<T>(this T source, object instance, string method, object[] args, bool throwIfnoMethod = true)
        {
            Type instanceType = instance.GetType();

            if (!args.HasElements())
            {
                MethodInfo inf = instanceType.GetMethod(method);

                if (inf == null)
                {
                    if (throwIfnoMethod)
                    {
                        throw new MissingMethodException($"{method} not found");
                    }

                    return null;
                }

                return inf.Invoke(instance, new object[] { });
            }

            Type[] argumentTypes = new Type[args.Length];

            for (int x = 0; x < args.Count(); ++x)
            {
                if (args[x] == null)
                {
                    argumentTypes[x] = null;

                    continue;
                }

                argumentTypes[x] = args[x].GetType();
            }

            MethodInfo spesificInfo = instanceType.GetMethod(method, argumentTypes);

            if (spesificInfo == null)
            {
                if (throwIfnoMethod)
                {
                    throw new MissingMethodException($"{method} not found");
                }

                return null;
            }

            return spesificInfo.Invoke(instance, args);
        }

        public static object InvokeMethod<T>(this T source, string assembly, string method, object[] args, bool throwIfnoMethod = true)
        {
            Type objectType = Type.GetType(assembly);

            if (objectType == null)
            {
                throw new ApplicationException($"Method {method} or Object not found");
            }

            ConstructorInfo contructor = objectType.GetConstructor(Type.EmptyTypes);

            object controller = contructor.Invoke(null);

            if (!args.HasElements())
            {
                MethodInfo inf = objectType.GetMethod(method);

                if (inf == null)
                {
                    if (throwIfnoMethod)
                    {
                        throw new MissingMethodException($"{method} not found");
                    }

                    return null;
                }

                return inf.Invoke(controller, new object[] { });
            }

            Type[] argumentTypes = new Type[args.Length];

            for (int x = 0; x < args.Count(); ++x)
            {
                argumentTypes[x] = args[x].GetType();
            }

            MethodInfo spesificInfo = objectType.GetMethod(method, argumentTypes);

            if (spesificInfo == null)
            {
                if (throwIfnoMethod)
                {
                    throw new MissingMethodException($"{method} not found");
                }

                return null;
            }

            return spesificInfo.Invoke(controller, args);
        }

        public static bool ContainsValue<T>(this T source, string value, DataComparisonEnum compareType, bool removeSpaces = false)
        {
            if (value.IsNullEmptyOrWhiteSpace() || source == null)
            {
                return false;
            }

            var sourceT = source.GetType();

            value = value;

            if (removeSpaces)
            {
                value = value.Replace(" ", string.Empty);
            }

            foreach (PropertyInfo item in sourceT.GetProperties())
            {
                if (!item.CanRead)
                {
                    continue;
                }

                try
                {
                    string objVal = item.GetValue(source, null).ParseToString();

                    if (objVal.IsNullEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    if (removeSpaces)
                    {
                        objVal = objVal.Replace(" ", string.Empty);
                    }

                    switch (compareType)
                    {
                        case DataComparisonEnum.None:

                            string[] valSplit = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string valItem in valSplit)
                            {
                                if (objVal.Contains(valItem))
                                {
                                    return true;
                                }
                            }

                            break;

                        case DataComparisonEnum.Contains:

                            if (objVal.Contains(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.StartsWith:

                            if (objVal.StartsWith(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.EndsWith:

                            if (objVal.EndsWith(value))
                            {
                                return true;
                            }

                            break;

                        case DataComparisonEnum.Exact:

                            if (objVal == value)
                            {
                                return true;
                            }

                            break;
                    }

                }
                catch
                {
                    // Do Nothing
                }
            }

            return false;
        }
    }
}
