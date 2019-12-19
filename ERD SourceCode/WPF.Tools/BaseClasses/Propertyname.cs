using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WPF.Tools.BaseClasses
{
  public static class Propertyname
  {
    public static MemberInfo GetMember(Expression<Action> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(GetMember(() => expression).Name);
            }

            return GetMemberInfo(expression as LambdaExpression);
        }

        public static MemberInfo GetMember<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(GetMember(() => expression).Name);
            }

            return GetMemberInfo(expression as LambdaExpression);
        }

        public static MethodInfo GetMethod(Expression<Action> expression)
        {
            MethodInfo method = GetMember(expression) as MethodInfo;

            if (method == null)
            {
                throw new ArgumentException("Not a method call expression", GetMember(() => expression).Name);
            }

            return method;
        }

        public static PropertyInfo GetProperty<T>(Expression<Func<T>> expression)
        {
            MemberInfo methodInfo = GetMember(expression);

            PropertyInfo property =  methodInfo as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Not a property expression", GetMember(() => expression).Name);
            }

            return property;
        }

        public static FieldInfo GetField<T>(Expression<Func<T>> expression)
        {
            FieldInfo field = GetMember(expression) as FieldInfo;

            if (field == null)
            {
                throw new ArgumentException("Not a field expression", GetMember(() => expression).Name);
            }

            return field;
        }

        internal static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException(GetMember(() => lambda).Name);
            }

            MemberExpression memberExpression = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression)lambda.Body).Method;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", GetMember(() => lambda).Name);
            }

            return memberExpression.Member;
        }
  }
}
