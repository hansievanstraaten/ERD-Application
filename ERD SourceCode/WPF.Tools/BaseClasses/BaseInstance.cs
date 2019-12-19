using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF.Tools.BaseClasses
{
  public class BaseInstance
  {
    public BaseInstance()
    {
    }

    public Type CreateType(string assemblyName, string moduleName)
    {
      AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(
        new AssemblyName(assemblyName),
        AssemblyBuilderAccess.RunAndCollect);

      ModuleBuilder module = asm.DefineDynamicModule(string.Format("{0}.dll", assemblyName));

      TypeBuilder typeBuild = module.DefineType(moduleName);

      return typeBuild.CreateType();
    }

    public object CreateAsObject(Type type)
    {
      return this.CreateAsObject(type.GetConstructor(Type.EmptyTypes));
    }

    public UserControlBase CreateControlInstance(Type type, Guid moduleId, object[] para = null)
    {
      if (para == null || para.Length == 0)
      {
        return this.CreateControlInstance(type.GetConstructor(Type.EmptyTypes), moduleId);
      }
      else
      {
        if (type.GetConstructor(new Type[] {typeof(object[])}) == null)
        {
          List<Type> constructorTypes = new List<Type>();

          foreach (object o in para)
          {
            constructorTypes.Add(o.GetType());
          }

          ConstructorInfo constructor = type.GetConstructor(constructorTypes.ToArray());

          if (constructor != null)
          {
            return this.CreateControlInstanceTypeSpecific(constructor, moduleId, para);
          }
        }
      }

      return this.CreateControlInstance(type.GetConstructor(new[] {typeof(object[])}), moduleId, para);
    }

    public Page CreatePageInstance(Type type, Guid moduleID, object[] para = null)
    {
      if (para == null || para.Length == 0)
      {
        return this.CreatePageInstance(type.GetConstructor(Type.EmptyTypes), moduleID);
      }
      else
      {
        List<Type> constructorTypes = new List<Type>();

        foreach (object o in para)
        {
          constructorTypes.Add(o.GetType());
        }

        ConstructorInfo constructor = type.GetConstructor(constructorTypes.ToArray());

        if (constructor != null)
        {
          return this.CreatePageInstanceWithParameters(constructor, moduleID, para);
        }
      }

      return this.CreatePageInstance(type.GetConstructor(new[] {typeof(object[])}), moduleID, para);
    }

    public WindowBase CreateWindowInstance(Type type, Guid moduleId, object[] para = null)
    {
      if (para == null || para.Length == 0)
      {
        return this.CreateWindowInstance(type.GetConstructor(Type.EmptyTypes), moduleId);
      }
      else
      {
        return this.CreateWindowInstance(type.GetConstructor(new[] {typeof(object[])}), moduleId, para);
      }
    }

    public UserControlBase CreateControlInstanceTypeSpecific(ConstructorInfo target, Guid moduleId, object[] para = null)
    {
      UserControlBase result = null;

      if (para == null)
      {
        result = (UserControlBase) target.Invoke(null);
      }
      else
      {
        result = (UserControlBase) target.Invoke(para);
      }

      return result;
    }

    private UserControlBase CreateControlInstance(ConstructorInfo target, Guid moduleId, object[] para = null)
    {
      UserControlBase result = null;

      if (para == null)
      {
        result = (UserControlBase) target.Invoke(null);
      }
      else
      {
        result = (UserControlBase) target.Invoke(new object[] {para});
      }

      return result;
    }

    private Page CreatePageInstance(ConstructorInfo target, Guid moduleId, object[] para = null)
    {
      Page result = null;

      if (para == null)
      {
        result = (Page) target.Invoke(null);
      }
      else
      {
        result = (Page) target.Invoke(new object[] {para});
      }

      return result;
    }

    private Page CreatePageInstanceWithParameters(ConstructorInfo target, Guid moduleId, object[] para)
    {
      Page result = null;

      result = (Page) target.Invoke(para);

      return result;
    }

    private WindowBase CreateWindowInstance(ConstructorInfo target, Guid moduleId, object[] para = null)
    {
      WindowBase result = null;

      if (para == null)
      {
        result = (WindowBase) target.Invoke(null);
      }
      else
      {
        result = (WindowBase) target.Invoke(new object[] {para});
      }

      return result;
    }

    private object CreateAsObject(ConstructorInfo target)
    {
      return (object) target.Invoke(null);
    }
  }
}
