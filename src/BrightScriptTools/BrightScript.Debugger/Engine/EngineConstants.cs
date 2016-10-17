namespace BrightScript.Debugger.Engine
{
    public static class EngineConstants
    {
        /// <summary>
        /// This is the engine GUID of the sample engine. It needs to be changed here and in EngineRegistration.pkgdef
        /// when turning the sample into a real engine.
        /// </summary>
        public const string EngineId = "{FE72992A-F091-45CA-B9C6-B653D374AA61}";

        public const string DebugEngine = "B4E29A42-7843-41C0-B10D-A7FFE3A40532";
        public const string DebugProgramProvider = "AD786763-0784-4AB6-9A3D-882784BCA391";

        public const string DebuggerPort = "DebuggerPort";
    }
}