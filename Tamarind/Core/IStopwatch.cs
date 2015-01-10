using System;
using System.Linq;

namespace Tamarind.Core
{
    /// <summary>
    ///     An object that can accurately measure elapsed time.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         It is useful to measure time using this interface instead
    ///         of <see cref="System.Diagnostics.Stopwatch" /> because an
    ///         alternate time source can be substituted, for testing or performance reasons.
    ///     </para>
    /// </remarks>
    public interface IStopwatch
    {

        /// <summary>
        ///     Returns the current elapsed time shown on this stopwatch.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        ///     Returns <c>true</c> if <see cref="Start" /> has been called on this stopwatch
        ///     and <see cref="Stop" /> has not been called since the last call to <c>Start()</c>.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Sets the elapsed timer for this stopwatch to zero and places it in a stopped state.
        /// </summary>
        /// <returns>this <c>IStopwatch</c> instance</returns>
        IStopwatch Reset();

        /// <summary>
        ///     Starts the stopwatch.
        /// </summary>
        /// <returns>this <c>IStopwatch</c> instance</returns>
        IStopwatch Start();

        /// <summary>
        ///     Stops the stopwatch. Future reads will return the fixed duration that had elapsed
        ///     up to this point.
        /// </summary>
        /// <returns>this <c>IStopwatch</c> instance</returns>
        IStopwatch Stop();

    }
}
