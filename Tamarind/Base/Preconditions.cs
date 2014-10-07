using System;
using System.Diagnostics;
using System.Linq;

using Tamarind.Annotations;

namespace Tamarind.Base
{
    /// <summary>
    ///     Simple sattic methods to be called at the start of your own methods to verify correct arugments and state.
    ///     <para>
    ///         This allows constructs such as:
    ///         <code>
    ///   if (count &lt;= 0)
    ///   {
    ///  		throw new ArgumentException("must be positive: " + count);
    ///   }
    ///   </code>
    ///         to be replaced with the more compact
    ///         <c>Preconditions.CheckArgument(count &gt; 0, "count", "must be positive: {0}", count)</c>.
    ///     </para>
    ///     <para>
    ///         Note that the sense of the expression is inverted; with <see cref="Preconditions" /> you declare what you
    ///         expect to be <em>true</em>.
    ///     </para>
    /// </summary>
    public static class Preconditions
    {

        /// <summary>
        ///     Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        [DebuggerStepThrough]
        public static void CheckArgument(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        ///     Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        [DebuggerStepThrough]
        public static void CheckArgument(bool expression, string argumentName)
        {
            if (!expression)
            {
                throw new ArgumentException(argumentName);
            }
        }

        /// <summary>
        ///     Ensures the truth of an expression involving one or more parameters to the calling method.
        /// </summary>
        [DebuggerStepThrough]
        [StringFormatMethod("message")]
        public static void CheckArgument(bool expression, string argumentName, string message, params object[] args)
        {
            if (!expression)
            {
                throw new ArgumentException(string.Format(message, args), argumentName);
            }
        }

        /// <summary>
        ///     Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        [DebuggerStepThrough]
        public static T CheckNotNull<T>(T reference)
            where T : class
        {
            if (reference == null)
            {
                throw new ArgumentNullException();
            }
            return reference;
        }

        /// <summary>
        ///     Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        [DebuggerStepThrough]
        public static T CheckNotNull<T>(T reference, string argumentName)
            where T : class
        {
            if (reference == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            return reference;
        }

        /// <summary>
        ///     Ensures that an object reference passed as a parameter to the calling method is not null.
        /// </summary>
        [DebuggerStepThrough]
        [StringFormatMethod("message")]
        public static T CheckNotNull<T>(T reference, string argumentName, string message, params object[] args)
            where T : class
        {
            if (reference == null)
            {
                throw new ArgumentNullException(argumentName, string.Format(message, args));
            }
            return reference;
        }

        /// <summary>
        ///     Ensures the truth of an expression involving the state of the calling instance, but not involving any parameters to
        ///     the calling method.
        /// </summary>
        [DebuggerStepThrough]
        public static void CheckState(bool expression)
        {
            if (!expression)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Ensures the truth of an expression involving the state of the calling instance, but not involving any parameters to
        ///     the calling method.
        /// </summary>
        [DebuggerStepThrough]
        [StringFormatMethod("message")]
        public static void CheckState(bool expression, string message, params object[] args)
        {
            if (!expression)
            {
                throw new InvalidOperationException(string.Format(message, args));
            }
        }

    }
}
