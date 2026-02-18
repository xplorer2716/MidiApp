/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2026 Pascal Schmitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ThirdParty.Win32
{
    internal class Win32Api
    {
        public const uint WS_EX_STATICEDGE = 0x00020000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_HSCROLL = 0x00100000;
        public const uint WS_VSCROLL = 0x00200000;

        public const int GWL_WNDPROC = -4;

        public const int WM_ACTIVATEAPP = 0x1C;
        public const int WM_COMMAND = 0x111;
        public const int WM_DESTROY = 2;
        public const int WM_DROPFILES = 0x233;
        public const int WM_ERASEBKGND = 0x14;
        public const int WM_KEYDOWN = 256;
        public const int WM_LBUTTONDBLCLK = 515;
        public const int WM_LBUTTONDOWN = 513;
        public const int WM_LBUTTONUP = 514;
        public const int WM_MOVE = 0x3;
        public const int WM_MOUSEMOVE = 512;
        public const int WM_MOVING = 0x216;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_NCLBUTTONDOWN = 161;
        public const int WM_NCLBUTTONDBLCLK = 163;
        public const int WM_PAINT = 0xF;
        public const int WM_RBUTTONDOWN = 516;
        public const int WM_TIMER = 0x113;
        public const int WM_PRINT = 0x317;
        public const int WM_PRINTCLIENT = 0x318;
        public const int WM_SETREDRAW = 0xB;
        public const int WM_SIZING = 0x214;
        public const int WM_SIZE = 0x5;

        public const int VK_HOME = 36;
        public const int VK_END = 35;
        public const int VK_PRIOR = 33;
        public const int VK_NEXT = 34;
        public const int VK_LEFT = 37;
        public const int VK_UP = 38;
        public const int VK_RIGHT = 39;
        public const int VK_DOWN = 40;
        public const int VK_PGUP = 0x21;
        public const int VK_PGDN = 0x22;

        public const int PRF_CLIENT = 0x00000004;
        public const int PRF_CHILDREN = 0x00000010;

        public const int HTNOWHERE = 0;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const int HTSYSMENU = 3;
        public const int HTGROWBOX = 4;
        public const int HTMENU = 5;
        public const int HTHSCROLL = 6;
        public const int HTVSCROLL = 7;
        public const int HTMINBUTTON = 8;
        public const int HTMAXBUTTON = 9;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTTOP = 12;
        public const int HTTOPLEFT = 13;
        public const int HTTOPRIGHT = 14;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOMRIGHT = 17;
        public const int HTBORDER = 18;
        public const int HTOBJECT = 19;
        public const int HTCLOSE = 20;
        public const int HTHELP = 21;
        public const int MAX_PATH = 260;
        public const int SRCCOPY = 0x00CC0020;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOOWNERZORDER = 0x0200;  // Don't do owner Z ordering
        public const uint SWP_NOSENDCHANGING = 0x0400; // Don't send WM_WINDOWPOSCHANGING

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public const int RGN_AND = 1;
        public const int RGN_OR = 2;
        public const int RGN_XOR = 3;
        public const int RGN_DIFF = 4;
        public const int RGN_COPY = 5;
        public const int RGN_MIN = RGN_AND;
        public const int RGN_MAX = RGN_COPY;

        public const int FW_BOLD = 700;

        private const string USER32 = "User32.DLL";
        private const string GDI32 = "GDI32.DLL";
        private const string KERNEL32 = "kernel32.Dll";
        private const string SHELL32 = "SHELL32.DLL";

        //public struct OSVERSIONINFOEX
        //{
        //    public int dwOSVersionInfoSize;
        //    public int dwMajorVersion;
        //    public int dwMinorVersion;
        //    public int dwBuildNumber;
        //    public int dwPlatformId;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        //    public string szCSDVersion;
        //    public ushort wServicePackMajor;
        //    public ushort wServicePackMinor;
        //    public ushort wSuiteMask;
        //    public byte wProductType;
        //    public byte wReserved;
        //}

        [StructLayout(LayoutKind.Explicit)]
        public struct RECT
        {
            [FieldOffset(0)]
            public int left;

            [FieldOffset(4)]
            public int top;

            [FieldOffset(8)]
            public int right;

            [FieldOffset(12)]
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        public static int RGB(byte R, byte G, byte B)
        {
            return ((B * 256) + G) * 256 + R;
        }

        public static int ARGB(byte A, byte R, byte G, byte B)
        {
            return (((A * 256) + R) * 256 + G) * 256 + B;
        }

        public static byte RGBRed(int colrRGB)
        {
            return (byte)(colrRGB & 0x000000FF);
        }

        public static byte RGBGreen(int colrRGB)
        {
            return (byte)((colrRGB >> 8) & 0x000000FF);
        }

        public static byte RGBBlue(int colrRGB)
        {
            return (byte)((colrRGB >> 16) & 0x000000FF);
        }

        public static int LoWrd(uint value)
        {
            return (int)(value & 0xFFFF);
        }

        public static int HiWrd(uint value)
        {
            return (int)(value >> 16);
        }

        [DllImport(USER32, EntryPoint = "CreateWindowExA")]
        public static extern IntPtr CreateWindowEx(
            uint ExStyle,
            string ClassName,
            string Name,
            uint Style,
            int x,
            int y,
            int w,
            int h,
            IntPtr parent,
            int Menu,
            IntPtr Instance,
            int lpParam);

        public static string ExePath()
        {
            return Application.StartupPath.TrimEnd('\\') + @"\";
        }

        [DllImport(USER32)] // Win32 encapsulation
        public static extern bool PtInRect(ref RECT r, POINT p);

        [DllImport(USER32)] // Win32 encapsulation
        public static extern int GetWindowDC(IntPtr hWnd);

        [DllImport(GDI32)]  // Win32 encapsulation
        public static extern int BitBlt(
        int hDCDest, int xDest, int yDest, int Width, int Heifght,
        int hDCSrce, int xSrce, int ySrce, int CopyMode
        );

        [DllImport(KERNEL32, EntryPoint = "GetModuleHandleA")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(USER32)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int wCmd);

        [DllImport(USER32)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport(SHELL32)]
        public static extern void DragAcceptFiles(IntPtr hWnd, bool fAccept);

        [DllImport(SHELL32)]
        public static extern void DragFinish(uint hDrop);

        [DllImport(SHELL32, EntryPoint = "DragQueryFileA")]
        public static extern int DragQueryFile(uint hDrop, int uiFile, StringBuilder lpStr, int cch);

        [DllImport(USER32)]
        public static extern int GetDC(IntPtr hWnd);

        [DllImport(USER32)]
        public static extern bool ReleaseDC(IntPtr hWnd, int hDC);

        public delegate void TimerProc(IntPtr hWnd, uint nMsg, int nIDEvent, int dwTime);

        [DllImport(USER32)]
        public static extern int SetTimer(IntPtr hWnd, int nIDEvent, int uElapse, TimerProc callback);

        [DllImport(USER32)]
        public static extern bool KillTimer(IntPtr hWnd, int nIDEvent);

        [DllImport(USER32)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rw);

        [DllImport(USER32)]
        public static extern bool ValidateRect(IntPtr hWnd, ref RECT rw);

        [DllImport(USER32)]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT rc);

        [DllImport(USER32, EntryPoint = "PostMessageA")] // Win32 encapsulation
        public static extern int PostMessage(IntPtr hWnd, uint dwMsg, uint wParam, int lParam);

        [DllImport(USER32)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, int dwFlags);

        [DllImport(USER32)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(USER32)]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(USER32)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,               // window handle
            IntPtr hWndInsertAfter,    // placement-order handle
            int X,                     // horizontal position
            int Y,                     // vertical position
            int cx,                    // width
            int cy,                    // height
            uint uFlags);              // window positioning flags

        [DllImport(USER32)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(USER32, EntryPoint = "GetClassLongA")]
        public static extern int GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport(USER32, EntryPoint = "SetClassLongA")]
        public static extern int SetClassLong(IntPtr hWnd, int nIndex, int lNewLong);

        [DllImport(USER32)] // Win32 encapsulation
        public static extern bool GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport(GDI32)] // Win32 encapsulation
        public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

        [DllImport(USER32)] // Win32 encapsulation
        public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport(GDI32)]  // Win32 encapsulation
        public static extern bool OffsetRgn(IntPtr hRgn, int x, int y);

        [DllImport(GDI32)]  // Win32 encapsulation
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(GDI32)]  // Win32 encapsulation
        public static extern bool CombineRgn(IntPtr hDestRgn, IntPtr hSrcRgn1, IntPtr hSrcRgn2, int nCombineMode);

        //[DllImport(KERNEL32)]
        //public static extern bool GetVersionEx(ref OSVERSIONINFOEX o);

        //unsafe public static int GetOsVersion()
        //{
        //    OSVERSIONINFOEX os = new OSVERSIONINFOEX();
        //    int nRet = 0;
        //    os.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)); // SIZEOF
        //    if (GetVersionEx(ref os))
        //    {
        //        switch (os.dwPlatformId)
        //        {
        //            case 1:
        //                nRet = os.dwPlatformId;
        //                switch (os.dwMinorVersion)
        //                {
        //                    case 0: nRet = 95;   // 95
        //                        break;
        //                    case 10: nRet = 98;  // 98
        //                        break;
        //                    case 90: nRet = 100; // ME
        //                        break;
        //                }
        //                break;
        //            case 2:
        //                nRet = os.dwPlatformId;
        //                switch (os.dwMajorVersion)
        //                {
        //                    case 3: nRet = 351; // NT 3.51
        //                        break;
        //                    case 4: nRet = 400; // NT 4.0
        //                        break;
        //                    case 5:
        //                        nRet = 500;     // 2000
        //                        if (os.dwMinorVersion == 1)
        //                        {
        //                            nRet = 501; // XP
        //                        }
        //                        break;
        //                    case 6:
        //                        nRet = 600;     // VISTA
        //                        break;
        //                }
        //                break;
        //            default:
        //                nRet = -1;
        //                break;
        //        }
        //    }
        //    return nRet;
        //}

        //// Using Drop Shadow slows down display when using large window.
        //public static void UseDropShadow(IntPtr hWnd)
        //{
        //    // Get the Operating System
        //    if (GetOsVersion() > 499)
        //    { // OS NT only
        //        if (IsWindow(hWnd))
        //        {
        //            int GCL_STYLE = -26;
        //            int CS_DROPSHADOW = 131072;
        //            int ClassLong = GetClassLong(hWnd, GCL_STYLE);
        //            if ((ClassLong & CS_DROPSHADOW) == 0)
        //            {
        //                ClassLong += CS_DROPSHADOW;
        //                SetClassLong(hWnd, GCL_STYLE, ClassLong);
        //            }
        //        }

        //        //int WS_EX_COMPOSITED = 0x02000000;
        //        //int GWL_EXSTYLE = -20;
        //        //int WindowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
        //        //SetWindowLong(hWnd, GWL_EXSTYLE, WindowLong | WS_EX_COMPOSITED);
        //    }
        //}

        public static void ButtonClick(IntPtr hButton)
        {
            PostMessage(hButton, WM_LBUTTONDOWN, 0, 0);
            PostMessage(hButton, WM_LBUTTONUP, 0, 0);
        }

        [DllImport(USER32, EntryPoint = "GetWindowLongA")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(USER32, EntryPoint = "SetWindowLongA")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int lNewLong);

        [DllImport(USER32, EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hWnd, int dwMsg, uint wParam, int lParam);

        [DllImport(USER32, EntryPoint = "CallWindowProcA")]
        public static extern int CallWindowProc(int lpPrevWndFunc, IntPtr hWnd, int uMsg, uint wParam, uint lParam);

        [DllImport(USER32)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(USER32)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(USER32)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(USER32)]
        public static extern IntPtr GetFocus();

        [DllImport(USER32)]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [DllImport(USER32)]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport(USER32)]
        public static extern bool DeferWindowPos(
            IntPtr hWinPosInfo,
            IntPtr hWnd,               // window handle
            IntPtr hWndInsertAfter,    // placement-order handle
            int X,                     // horizontal position
            int Y,                     // vertical position
            int cx,                    // width
            int cy,                    // height
            uint uFlags);              // window positioning flags

        [DllImport(USER32)]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        public static void ControlDrawingDisable(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SETREDRAW, 0, 0);
        }

        public static void ControlDrawingEnable(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SETREDRAW, 1, 0);
            Control.FromHandle(hWnd).Refresh();
        }
    }
}