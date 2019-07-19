using LikeWater.SpyClass.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LikeWater.SpyClass.WinApi
{
    //Develpment Author Paulo Santos 
    //www.likewatercs.com
    //pjdsant@gmail.com
    public class WinApiX
    {
        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder buf, int nMaxCount);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageClick(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false, EntryPoint = "SendMessage")]
        public static extern IntPtr SendRefMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, string lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        const int CB_SETCURSEL = 0x014E;
        private static bool EnumWindowEx(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
            {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }
        public static List<IntPtr> GetAllChildHandles(string windowTitle)
        {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            var windowHWnd = FindWindowByCaption(IntPtr.Zero, windowTitle);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindowEx);
                EnumChildWindows(windowHWnd, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        private static bool EnumChildWindowsCallback(IntPtr handle, IntPtr pointer)
        {
            var gcHandle = GCHandle.FromIntPtr(pointer);
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }

            list.Add(handle);

            return true;
        }
        private static IEnumerable<IntPtr> GetChildWindows(IntPtr parent)
        {
            var result = new List<IntPtr>();
            var listHandle = GCHandle.Alloc(result);

            try
            {
                EnumChildWindows(parent, EnumChildWindowsCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            return result;
        }

        private static string GetText(IntPtr handle)
        {
            const uint WM_GETTEXTLENGTH = 0x000E;
            const uint WM_GETTEXT = 0x000D;

            var sbi = new StringBuilder(100);
            var length = (int)SendMessage(handle, WM_GETTEXTLENGTH, IntPtr.Zero, null);
            var sb = new StringBuilder(length + 1);
            GetClassName(handle, sbi, sbi.Capacity);
            SendMessage(handle, WM_GETTEXT, (IntPtr)sb.Capacity, sb);

            return sb.ToString();
        }

        public static string GetAllFromWindowByTitle(string windowTitle)
        {
            var sb = new StringBuilder();

            StringBuilder lpClassName = new StringBuilder();

            try
            {
                var windowHWnd = FindWindowByCaption(IntPtr.Zero, windowTitle);

                var childWindows = GetChildWindows(windowHWnd);

                int nRet = GetClassName(windowHWnd, lpClassName, lpClassName.Capacity);

                List<IntPtr> handlers = new List<IntPtr>();

                handlers = GetAllChildHandles(windowTitle);

                int i = 0;

                Logger.Log(String.Format($"{"Log criado em "} : {DateTime.Now}"), "SpyLog");

                foreach (var item in handlers)
                {
                    StringBuilder ClassName = new StringBuilder(256);
                    GetClassName(item, ClassName, ClassName.Capacity);
                    
                    Console.WriteLine(ClassName.ToString() + ": " + item + ": " + GetText(item) + ": " + i);

                    Logger.Log(ClassName.ToString() + ": " + item + ": " + GetText(item) + ": " + i);

                    i++;
                }

                foreach (var childWindowText in childWindows.Select(GetText))
                {
                    sb.Append(childWindowText);
                }

                return sb.ToString();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }


}
