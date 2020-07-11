using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Oasis
{
    //鼠标钩子程序
    public class OasisMouseHook : IDisposable
    {
        public delegate int HookProcedure(int nCode, int wParam, IntPtr lParam);

        static int s_hMouseHook = 0;
        //private HookProcedure m_HookProcedure;
        
        //声明一个Point的封送类型  
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
            public override string ToString()
            {
                return string.Format("x={0},y={1}", x, y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public const int WH_MOUSE_LL = 14;
            public POINT pos;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
            public override string ToString()
            {
                return string.Format(": pos={0},hWnd={1},wHitTestCode={2},dwExtraInfo={3}", pos, hWnd, wHitTestCode, dwExtraInfo);
            }
        }

        public void Start()
        {
            IntPtr handle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            if (s_hMouseHook == 0)
            {
                //m_HookProcedure = new HookProcedure(MouseHookProc);
                s_hMouseHook = SetWindowsHookEx(MouseHookStruct.WH_MOUSE_LL, MouseHookProc, handle, 0);
                if (s_hMouseHook == 0) UnhookWindowsHookEx(s_hMouseHook);
            }
        }

        public void Clear()
        {
            if (s_hMouseHook != 0)
            {
                UnhookWindowsHookEx(s_hMouseHook);
                s_hMouseHook = 0;
            }
        }

        public void Dispose()
        {
            Clear();
        }

        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var kbh = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));


                UnityEngine.Debug.LogFormat("MouseHookProc() ：{0}", kbh);
            }
            return CallNextHookEx(s_hMouseHook, nCode, wParam, lParam);
        }

        #region [DllImport("user32.dll")]

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(int idHook, HookProcedure lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        #endregion
    }
}