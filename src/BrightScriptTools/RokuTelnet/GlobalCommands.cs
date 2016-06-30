using System.Windows.Input;
using RokuTelnet.Views.Shell;

namespace RokuTelnet
{
    public static class GlobalCommands
    {
        public static readonly RoutedUICommand DebuggerStep = new RoutedUICommand("Debugger Step", "DebuggerStep", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F11) });
        public static readonly RoutedUICommand DebuggerContinue = new RoutedUICommand("Debugger Continue", "DebuggerContinue", typeof(ShellView), new InputGestureCollection() { new KeyGesture(Key.F5) });
    }
}