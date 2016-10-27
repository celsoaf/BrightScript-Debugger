namespace BrightScript.Debugger.Enums
{
    public enum DebuggerCommandEnum
    {
        // Summary:
        //     Print current BrightScript Component instances
        bsc,
        // Summary:
        //     Print a summary of BrightScript component instance counts by component type.
        bscs,
        // Summary:
        //     Toggle whether BrightScript should break into the debugger after non-fatal diagnostic messages
        brkd,
        // Summary:
        //     Print backtrace of call function context frames
        bt,
        // Summary:
        //     Print Brightscript Component classes
        classes,
        // Summary:
        //     Continue Script Execution
        c,
        // Summary:
        //     Move down the function context chain one
        d,
        // Summary:
        //     Exit shell
        exit,
        // Summary:
        //     Run garbage collector
        gc,
        // Summary:
        //     Print the list of debugger commands
        help,
        // Summary:
        //     Print the last line that executed
        last,
        // Summary:
        //     List current function
        list,
        // Summary:
        //     Print the next line to execute
        next,
        // Summary:
        //     Print a variable or expression
        p,
        // Summary:
        //     Step one program statement
        s,
        // Summary:
        //     Step over function
        over,
        // Summary:
        //     Step out of a function
        @out,
        // Summary:
        //     Move up the function context chain one
        u,
        // Summary:
        //     Print local variables and their types/values
        var
    }
}