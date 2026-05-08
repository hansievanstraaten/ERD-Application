using System.Reflection;

namespace ERD.Viewer.Shared.Extensions
{
    public static class GenericExtensions
    {
        public static string ParseToString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value) ?? string.Empty;
        }

        public static bool IsNullEmptyOrWhiteSpace<T>(this T value)
        {
            if (value == null)
            {
                return true;
            }

            string parsedValue = value.ParseToString(); 

            if (string.IsNullOrEmpty(parsedValue))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(parsedValue))
            {
                return true;
            }

            return false;
        }

        public static T? To<T>(this object item)
        {
            if (item == null)
            {
                return default(T);
            }

            return (T)item;
        }


        public static bool HasElements<T>(this T[] value)
        {
            if (value == null || value.Length == 0)
            {
                return false;
            }

            return true;
        }

        public static object? InvokeMethod<T>(this T source, string assembly, string method, object[]? args, bool throwIfnoMethod = true)
        {
            Type? objectType = Type.GetType(assembly);

            if (objectType == null)
            {
                throw new ApplicationException($"Method {method} or Object not found");
            }

            ConstructorInfo? contructor = objectType.GetConstructor(Type.EmptyTypes);

            object? controller = contructor?.Invoke(null);

            if (args != null 
             && args.HasElements() == false)
            {
                MethodInfo? inf = objectType.GetMethod(method);

                if (inf == null)
                {
                    if (throwIfnoMethod)
                    {
                        throw new MissingMethodException($"{method} not found");
                    }

                    return null;
                }

                return inf?.Invoke(controller, new object[] { });
            }

            Type[] argumentTypes = new Type[args.Length];

            for (int x = 0; x < args.Count(); ++x)
            {
                argumentTypes[x] = args[x].GetType();
            }

            MethodInfo? spesificInfo = objectType.GetMethod(method, argumentTypes);

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

    }
}
