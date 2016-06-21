using System;
using System.Diagnostics;
using System.Globalization;

namespace RokuTelnet.Telnet
{
    /// <summary>
    /// Provides guard clauses.
    /// 
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Guards against a null argument.
        /// 
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam><param name="parameterName">Name of the parameter.</param><param name="argument">The argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="argument"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <typeparamref name="TArgument"/> is restricted to reference types to avoid boxing of value type objects.
        /// </remarks>
        [DebuggerStepThrough]
        public static void AgainstNullArgument<TArgument>(string parameterName, [Guard.ValidatedNotNull] TArgument argument) where TArgument : class
        {
            if ((object)argument == null)
                throw new ArgumentNullException(parameterName, string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0} is null.", new object[1]
        {
          (object) parameterName
        }));
        }

        /// <summary>
        /// Guards against a null argument if <typeparamref name="TArgument"/> can be <c>null</c>.
        /// 
        /// </summary>
        /// <typeparam name="TArgument">The type of the argument.</typeparam><param name="parameterName">Name of the parameter.</param><param name="argument">The argument.</param><exception cref="T:System.ArgumentNullException"><paramref name="argument"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Performs a type check to avoid boxing of value type objects.
        /// 
        /// </remarks>
        [DebuggerStepThrough]
        public static void AgainstNullArgumentIfNullable<TArgument>(string parameterName, [Guard.ValidatedNotNull] TArgument argument)
        {
            if (Guard.IsNullableType(typeof(TArgument)) && (object)argument == null)
                throw new ArgumentNullException(parameterName, string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0} is null.", new object[1]
        {
          (object) parameterName
        }));
        }

        /// <summary>
        /// Guards against a null argument property value.
        /// 
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam><param name="parameterName">Name of the parameter.</param><param name="propertyName">Name of the property.</param><param name="argumentProperty">The argument property.</param><exception cref="T:System.ArgumentException"><paramref name="argumentProperty"/> is <c>null</c>.</exception>
        /// <remarks>
        /// <typeparamref name="TProperty"/> is restricted to reference types to avoid boxing of value type objects.
        /// </remarks>
        [DebuggerStepThrough]
        public static void AgainstNullArgumentProperty<TProperty>(string parameterName, string propertyName, [Guard.ValidatedNotNull] TProperty argumentProperty) where TProperty : class
        {
            if ((object)argumentProperty == null)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1} is null.", new object[2]
        {
          (object) parameterName,
          (object) propertyName
        }), parameterName);
        }

        /// <summary>
        /// Guards against a null argument property value if <typeparamref name="TProperty"/> can be <c>null</c>.
        /// 
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam><param name="parameterName">Name of the parameter.</param><param name="propertyName">Name of the property.</param><param name="argumentProperty">The argument property.</param><exception cref="T:System.ArgumentException"><paramref name="argumentProperty"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Performs a type check to avoid boxing of value type objects.
        /// 
        /// </remarks>
        [DebuggerStepThrough]
        public static void AgainstNullArgumentPropertyIfNullable<TProperty>(string parameterName, string propertyName, [Guard.ValidatedNotNull] TProperty argumentProperty)
        {
            if (Guard.IsNullableType(typeof(TProperty)) && (object)argumentProperty == null)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1} is null.", new object[2]
        {
          (object) parameterName,
          (object) propertyName
        }), parameterName);
        }

        /// <summary>
        /// Determines whether the specified type is a nullable type.
        /// 
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if the specified type is a nullable type; otherwise, <c>false</c>.
        /// 
        /// </returns>
        private static bool IsNullableType(this Type type)
        {
            if (!type.IsValueType)
                return true;
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return false;
        }

        /// <summary>
        /// When applied to a parameter, this attribute provides an indication to code analysis that the argument has been null checked.
        /// 
        /// </summary>
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}