using System.Windows.Input;
using RokuTelnet.Views.Shell;

namespace RokuTelnet
{
    public static class GlobalCommands
    {
        public static readonly RoutedUICommand DebuggerStep = new RoutedUICommand("Debugger Step", "DebuggerStep", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F11) });
        public static readonly RoutedUICommand DebuggerContinue = new RoutedUICommand("Debugger Continue", "DebuggerContinue", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F5) });
        public static readonly RoutedUICommand DebuggerDown = new RoutedUICommand("Debugger Down", "DebuggerDown", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F7) });
        public static readonly RoutedUICommand DebuggerUp = new RoutedUICommand("Debugger Up", "DebuggerUp", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F8) });
        public static readonly RoutedUICommand DebuggerStop = new RoutedUICommand("Debugger Stop", "DebuggerStop", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F4) });
        public static readonly RoutedUICommand DebuggerBacktrace = new RoutedUICommand("Debugger Backtrace", "DebuggerBacktrace", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F9) });
        public static readonly RoutedUICommand DebuggerVariables = new RoutedUICommand("Debugger Variables", "DebuggerVariables", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F10) });
        public static readonly RoutedUICommand DebuggerFunction = new RoutedUICommand("Debugger Function", "DebuggerFunction", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F12) });

        public static readonly RoutedUICommand Deploy = new RoutedUICommand("Deploy", "DebuggerContinue", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F6) });
    }
}