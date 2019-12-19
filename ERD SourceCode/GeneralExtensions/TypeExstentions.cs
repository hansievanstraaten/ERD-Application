using System;
using System.Reflection;

namespace GeneralExtensions
{
  public static class TypeExstentions
  {
    private delegate object DelegateInvoke();

    public static object Invoke(this Type objectType, string methodName)
    {
      MethodInfo inf = objectType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

      if (inf == null)
      {
        throw new ApplicationException($"Method {methodName} or Object {objectType.Name} not found");
      }
      
      DelegateInvoke delegateInvoke = (DelegateInvoke) Delegate.CreateDelegate(typeof(DelegateInvoke), inf);

      return delegateInvoke;
    }

    public static bool IsNullableType(this Type source)
    {
      return source.IsGenericType && source.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
  }
}
