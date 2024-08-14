using PInvoke;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UptimeChecker
{
    internal static class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            KeepAwake();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        // internal const int AttachParentProcess = -1;
        internal static int Delta = 4;

        public static void KeepAwake()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);

            var inp = new User32.INPUT
            {
                type = User32.InputType.INPUT_MOUSE,
                Inputs = new User32.INPUT.InputUnion
                {
                    mi = new User32.MOUSEINPUT
                    {
                        dx = Delta,
                        dy = Delta,
                        mouseData = 0,
                        dwFlags = User32.MOUSEEVENTF.MOUSEEVENTF_MOVE,
                        time = 0,
                        dwExtraInfo_IntPtr = IntPtr.Zero,
                    },
                },
            };

            uint returnValue = User32.SendInput(nInputs: 1, pInputs: new[] { inp, }, cbSize: Marshal.SizeOf<User32.INPUT>());

            if (returnValue != 1)
            {
                int errorCode = Marshal.GetLastWin32Error();

                Debugger.Log(level: 1,
                              category: "Jiggle",
                              message:
                              $"failed to insert event to input stream; retval={returnValue}, errcode=0x{errorCode:x8}\n");
            }

            Delta = -Delta;
        }
    }
}