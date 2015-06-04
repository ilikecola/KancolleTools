using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace KancolleMacro
{
    class imageCapture
    {
        [DllImport("user32.dll")]
        internal extern static IntPtr GetWindowDC(IntPtr ptr);
        [DllImport("gdi32.dll")]
        internal extern static IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        internal extern static IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        internal extern static void SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        internal extern static void PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);
        [DllImport("gdi32.dll")]
        internal extern static void DeleteDC(IntPtr hDc);


        public Bitmap GetWindowImage(IntPtr hWnd)
        {
            IntPtr hscrdc = GetWindowDC(hWnd);
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, 800, 480);
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(hWnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);//删除用过的对象
            DeleteDC(hmemdc);//删除用过的对象
            return bmp;
        }  
    }
}
