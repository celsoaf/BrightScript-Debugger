using System;
using BrightScript.Loggger;

namespace BrightScript.Debugger.Exceptions
{
    public static class ExceptionHelper
    {
        public static bool BeforeCatch(Exception currentException, bool reportOnlyCorrupting)
        {
            if (reportOnlyCorrupting && !IsCorruptingException(currentException))
            {
                return true; // ignore non-corrupting exceptions
            }

            try
            {
                LiveLogger.WriteLine("EXCEPTION: " + currentException.GetType());
                LiveLogger.WriteTextBlock("EXCEPTION: ", currentException.StackTrace);
            }
            catch
            {
                // If anything goes wrong, ignore it. We want to report the original exception, not a telemetry problem
            }

            return true;
        }

        public static bool IsCorruptingException(Exception exception)
        {
            if (exception is NullReferenceException)
                return true;
            if (exception is ArgumentNullException)
                return true;
            if (exception is ArithmeticException)
                return true;
            if (exception is ArrayTypeMismatchException)
                return true;
            if (exception is DivideByZeroException)
                return true;
            if (exception is IndexOutOfRangeException)
                return true;
            if (exception is InvalidCastException)
                return true;
            if (exception is System.Runtime.InteropServices.SEHException)
                return true;

            return false;
        }
    }
}