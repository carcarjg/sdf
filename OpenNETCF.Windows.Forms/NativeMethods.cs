using System;
using System.Runtime.InteropServices;

namespace OpenNETCF.Windows.Forms
{
	internal delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    internal static class NativeMethods
    {
        private const string CoreDll = "coredll.dll";
        private const string CommCtrl = "commctrl.dll";
        private const string CommDlg = "commdlg.dll";
        private const string CEShell = "ceshell.dll";

        internal static short LOWORD(int value)
        {
            return (short)(value & 0x0000ffff);
        }

        internal static short HIWORD(int value)
        {
            return (short)(value >> 16);
        }

        #region Clipboard P/Invokes

        [DllImport(CoreDll, EntryPoint = "OpenClipboard", SetLastError = true)]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport(CoreDll, EntryPoint = "CloseClipboard", SetLastError = true)]
        internal static extern bool CloseClipboard();

        [DllImport(CoreDll, EntryPoint = "EmptyClipboard", SetLastError = true)]
        internal static extern bool EmptyClipboard();

        [DllImport(CoreDll, EntryPoint = "IsClipboardFormatAvailable", SetLastError = true)]
        internal static extern bool IsClipboardFormatAvailable(int uFormat);

        #endregion

        #region CE Shell

        [DllImport(CEShell, SetLastError = true)]
        internal static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        [StructLayout(LayoutKind.Sequential)]
        internal struct BROWSEINFO
        {
            internal IntPtr hwndOwner;
            IntPtr pidlRoot;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszDisplayName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string lpszTitle;
            uint ulFlags;
            IntPtr lpfn;
            IntPtr lParam;
            int iImage;
        }
        [DllImport(CEShell, SetLastError = true)]
        internal static extern bool SHGetPathFromIDList(IntPtr pidl, byte[] pszPath);

        #endregion

        #region Common Controls

        [DllImport(CommCtrl, SetLastError = true)]
        internal static extern bool InitCommonControlsEx(ref INITCOMMONCONTROLSEX data);

        #region INITCOMMONCONTROLSEX Struct
        internal struct INITCOMMONCONTROLSEX
        {
            internal int dwSize;
            internal ICC dwICC;
        }
        #endregion

        #region ICC Enum
        internal enum ICC : int
        {
            LISTVIEW_CLASSES =0x00000001, // listview, header
            TREEVIEW_CLASSES =0x00000002, // treeview, tooltips
            BAR_CLASSES      =0x00000004, // toolbar, statusbar, trackbar, tooltips
            TAB_CLASSES      =0x00000008, // tab, tooltips
            UPDOWN_CLASS     =0x00000010, // updown
            PROGRESS_CLASS   =0x00000020, // progress
            ANIMATE_CLASS    =0x00000080, // animate
            WIN95_CLASSES    =0x0000007F, // on WinCE Animate is not part of WIN95 classes
            DATE_CLASSES     =0x00000100, // month picker, date picker, time picker, updown
            COOL_CLASSES     =0x00000400, // rebar (coolbar) control
            INTERNET_CLASSES =0x00000800, // IP Address control
            TOOLTIP_CLASSES  =0x00001000, // Tooltip static & button
            CAPEDIT_CLASS    =0x00002000, // All-caps edit control
            FE_CLASSES       =0x40000000, // FE specific input subclasses
        }
        #endregion

        #endregion

        #region Common Dialog

        [DllImport(CommDlg)]
        internal static extern bool ChooseColor(ColorDialog.CHOOSECOLOR lpcc);

        [DllImport(CommDlg)]
        internal static extern int CommDlgExtendedError();

        #endregion

        #region Windows P/Invokes

        [DllImport(CoreDll, EntryPoint = "PeekMessage", SetLastError = true)]
        internal static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport(CoreDll, EntryPoint = "GetMessageW", SetLastError = true)]
        internal static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport(CoreDll, EntryPoint = "TranslateMessage", SetLastError = true)]
        internal static extern bool TranslateMessage(out MSG lpMsg);

        [DllImport(CoreDll, EntryPoint = "DispatchMessage", SetLastError = true)]
        internal static extern bool DispatchMessage(ref MSG lpMsg);

        [DllImport(CoreDll, EntryPoint = "PostQuitMessage", SetLastError = true)]
        internal static extern void PostQuitMessage(int nExitCode);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool SetActiveWindow(IntPtr hWnd);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName,
			string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight,
			IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string modName);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, Win32.SW nCmdShow);

        [DllImport(CoreDll, EntryPoint = "SetParent", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWChild, IntPtr hwParent);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern IntPtr FindWindow(string className, string wndName);

        [DllImport(CoreDll, SetLastError = true, EntryPoint = "GetWindow")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, Win32.GW nCmd);

        //[DllImport(CoreDll, SetLastError = true)]
        //public extern static int GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, byte[] lpString, int nMaxCount);

        [DllImport(CoreDll, SetLastError = true)]
        internal static extern int SetWindowText(IntPtr hWnd, string lpString);

#endregion

        /*[DllImport("coredll.dll", SetLastError = true)]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName,
            byte[] lpDevMode,
            IntPtr hwnd,
            CDS dwflags,
            IntPtr lParam);*/

        /// <summary>
        /// Specifies an action, or state, that may occur, or should 
        /// occur, in relation to a keyboard key.
        /// </summary>
        public enum KeyActionState : int
        {
            /// <summary>
            /// The key is in the down state.
            /// </summary>
            Down = 0x01,
            /// <summary>
            /// The key is in the up state.
            /// </summary>
            Up = 0x02,
            /// <summary>
            /// The key has been pressed down and then released.
            /// </summary>
            Press = 0x03
        }

        #region System Information
        [DllImport(CoreDll, SetLastError = true)]
        internal static extern int GetSystemMetrics(SM nIndex);

        #region SM
        internal enum SM : int
        {
            CXSCREEN = 0,
            CYSCREEN = 1,
            CXVSCROLL = 2,
            CYHSCROLL = 3,
            CYCAPTION = 4,
            CXBORDER = 5,
            CYBORDER = 6,
            CXDLGFRAME = 7,
            CYDLGFRAME = 8,
            CXICON = 11,
            CYICON = 12,
            // @CESYSGEN IF GWES_ICONCURS
            CXCURSOR = 13,
            CYCURSOR = 14,
            // @CESYSGEN ENDIF
            CYMENU = 15,
            CXFULLSCREEN = 16,
            CYFULLSCREEN = 17,
            MOUSEPRESENT = 19,
            CYVSCROLL = 20,
            CXHSCROLL = 21,
            DEBUG = 22,
            CXDOUBLECLK = 36,
            CYDOUBLECLK = 37,
            CXICONSPACING = 38,
            CYICONSPACING = 39,
            CXEDGE = 45,
            CYEDGE = 46,
            CXSMICON = 49,
            CYSMICON = 50,

            XVIRTUALSCREEN = 76,
            YVIRTUALSCREEN = 77,
            CXVIRTUALSCREEN = 78,
            CYVIRTUALSCREEN = 79,
            CMONITORS = 80,
            SAMEDISPLAYFORMAT = 81,
        }
        #endregion

        //[DllImport("coredll.dll", SetLastError = true)]
        //private static extern bool GetSystemPowerStatusEx(PowerStatus pStatus, bool fUpdate);
        [DllImport(CoreDll, SetLastError = true)]
        internal static extern bool GetSystemPowerStatusEx2(PowerStatus pStatus, int dwLen, bool fUpdate);
        
        #endregion

        #region Keyboard P/Invokes

        [DllImport(CoreDll, EntryPoint="GetKeyState", SetLastError = true)]
        internal static extern short GetKeyState(int vk);

        [DllImport(CoreDll, EntryPoint = "keybd_event", SetLastError = true)]
        internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport(CoreDll, EntryPoint = "PostKeybdMessage", SetLastError = true)]
        internal static extern bool PostKeybdMessage(IntPtr hwnd, uint vKey, KeyStateFlags flags, uint cCharacters, KeyStateFlags[] pShiftStateBuffer, uint[] pCharacterBuffer);

        #endregion

        #region ----------------- Keyboard functions ------------------

        /// <summary>
        /// Send a string to the keyboard
        /// </summary>
        /// <param name="Keys"></param>
        public static void SendKeyboardString(string Keys)
        {
            SendKeyboardString(Keys, KeyStateFlags.Down, IntPtr.Zero);
        }

        /// <summary>
        /// Send a string to the keyboard
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Flags"></param>
        public static void SendKeyboardString(string Keys, KeyStateFlags Flags)
        {
            SendKeyboardString(Keys, Flags, IntPtr.Zero);
        }

        /// <summary>
        /// Send a string to the keyboard
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Flags"></param>
        /// <param name="hWnd"></param>
        public static void SendKeyboardString(string Keys, KeyStateFlags Flags, IntPtr hWnd)
        {
            uint[] keys = new uint[Keys.Length];
            KeyStateFlags[] states = new KeyStateFlags[Keys.Length];
            KeyStateFlags[] dead = { KeyStateFlags.Dead };

            for (int k = 0; k < Keys.Length; k++)
            {
                states[k] = Flags;
                keys[k] = Convert.ToUInt32(Keys[k]);
            }

            PostKeybdMessage(hWnd, 0, Flags, (uint)keys.Length, states, keys);
            PostKeybdMessage(hWnd, 0, dead[0], 1, dead, keys);
        }

        /// <summary>
        /// Send a key to the keyboard
        /// </summary>
        /// <param name="VirtualKey"></param>
        public static void SendKeyboardKey(byte VirtualKey)
        {
            SendKeyboardKey(VirtualKey, true);
        }

        /// <summary>
        /// Send a key to the keyboard
        /// </summary>
        /// <param name="VirtualKey"></param>
        /// <param name="Silent"></param>
        public static void SendKeyboardKey(byte VirtualKey, bool Silent)
        {
            SendKeyboardKey(VirtualKey, Silent, KeyActionState.Press);
        }

        /// <summary>
        /// Simulates a keystroke that the system can use to generate a WM_KEYUP or WM_KEYDOWN message.
        /// </summary>
        /// <param name="VirtualKey">A System.Byte structure that contains a virtual-key code representing the key with which to perform an action.</param>
        /// <param name="Silent">A System.Boolean structure specifying true if a sound should be generated when the keystroke is simulated; otherwise, false.</param>
        /// <param name="State">A KeyActionState enumeration value indicating the action that should be performed with the specified virtual-key code.</param>
        public static void SendKeyboardKey(byte VirtualKey, bool Silent, KeyActionState State)
        {
            int silent = Silent ? (int)KeyEvents.Silent : 0;

            // Note that both of the operations below will be performed if the caller 
            // requested a key press operation (KeyActionState.Press).

            // If requested by the caller, send the virtual-key code as part of a key down operation.
            if ((State & KeyActionState.Down) > 0)
            {
                keybd_event(VirtualKey, 0, 0, silent);
            }

            // If requested by the caller, send the virtual-key code as part of a key up operation.
            if ((State & KeyActionState.Up) > 0)
            {
                keybd_event(VirtualKey, 0, (int)KeyEvents.KeyUp, silent);
            }
        }

        #endregion
	}
}
