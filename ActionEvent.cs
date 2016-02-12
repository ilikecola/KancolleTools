using System;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;

namespace KancolleMacro
{
    class ActionEvent
    {
        [DllImport("user32.dll", EntryPoint = "PostMessage")]//发送鼠标消息
        private static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "GetDC")]//取设备场景
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("gdi32.dll", EntryPoint = "GetPixel")]//取指定点颜色
        private static extern int GetPixel(IntPtr hdc, Point p);

        //鼠标操作部分
        //常数
        const int WM_MOUSEMOVE = 0x200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x202;
        const int MK_LBUTTON = 0x0001;

        public void MOUSEMOVE(IntPtr hWnd, int xPos, int yPos)
        {
            //y为高16位
            //x为低16位
            yPos = yPos + convarible.poiYcorrect;
            PostMessage(hWnd, WM_MOUSEMOVE, 0, (yPos << 16) | xPos);
            Thread.Sleep(5 + RndTime());
        }

        public void LeftClick(IntPtr hWnd, int xPos, int yPos)
        {
            //y为高16位
            //x为低16位
            yPos = yPos + convarible.poiYcorrect;
            PostMessage(hWnd, WM_LBUTTONDOWN, 0, (yPos << 16) | xPos);
            Thread.Sleep(100 + RndTime());
            PostMessage(hWnd, WM_LBUTTONUP, 0, (yPos << 16) | xPos);
        }

        //后台取色部分
        public string GetPixelColor(IntPtr hWnd, int xPos, int yPos)
        {
            this.MOUSEMOVE(hWnd, 0,0);
            yPos = yPos + convarible.poiYcorrect;
            string PixelColor = "";
            Point p = new Point(xPos, yPos);
            IntPtr hdc = GetDC(hWnd);
            int c = GetPixel(hdc, p);
            int r = (c & 0xFF);//转换R
            int g = (c & 0xFF00) / 256;//转换G
            int b = (c & 0xFF0000) / 65536;//转换B
            PixelColor = b.ToString("X").PadLeft(2, '0') + g.ToString("X").PadLeft(2, '0') + r.ToString("X").PadLeft(2, '0');//输出16进制颜色
            return PixelColor;
        }

        private int RndTime()
        {
            int rnd;
            rnd = (int)(rndint() * 20 - 20 / 2);
            if (rnd < 0) rnd = (-1) * rnd;
            return rnd;
        }

        private double rndint()
        {
            double rnd;
            Thread.Sleep(100);
            Random ra = new Random();
            rnd = ra.NextDouble();
            return rnd;
        }

    }

}
