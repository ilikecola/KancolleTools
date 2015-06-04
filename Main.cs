using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;

namespace KancolleMacro
{
    public partial class Main : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "GetClassName")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(Point point);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        /*定义一代理
        * 说明:其实例作为Invoke参数,以实现后台线程调用主线的函数
        * MessageEventArgs传递显示的信息.
        * */
        private delegate void MessageHandler(MessageEventArgs e);
        public ThreadManager[] threadmanager = new ThreadManager[3];

        //窗口相关
        IntPtr GamehWnd= new IntPtr(0);
        IntPtr KCVhWnd = new IntPtr(0);
        int Gamewidth = 0; //窗口的宽度
        int Gameheight = 0; //窗口的高度
        int KCVposX;
        int KCVposY;
        //系统时间
        DateTime currentTime = new DateTime();
        string currentTimestr;
        //启动开关
        int _switch = 0;
        //时间设置
        int DelayStart;
        int StopTime;
        int OverTime = 20;
        int fleet2 = 59;
        int fleet3 = 59;
        int fleet4 = 59;
        //统计信息
        int fleet2count = 0;
        int fleet3count = 0;
        int fleet4count = 0;

        public Main()
        {        
            InitializeComponent();
            currentTime = System.DateTime.Now;
            currentTimestr = currentTime.ToString("MM-dd HH:mm:ss");
            listBox1.Items.Insert(0,currentTimestr + " 系统启动");
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //停止计时器
            fleet2timer.Enabled = false;
            fleet3timer.Enabled = false;
            fleet4timer.Enabled = false;
            manual.Enabled = false;
            SysTime.Enabled = true;
            Overtimer.Enabled = false;
            delaytimer.Enabled = false;
            autostop.Enabled = false;
            //计时器时间设定
            fleet2timer.Interval = 1000;//1s
            fleet3timer.Interval = 1000;//1s
            fleet4timer.Interval = 1000;//1s
            manual.Interval = 500;//手动找句柄
            SysTime.Interval = 1000;//       
            Overtimer.Interval = 1000;//
            delaytimer.Interval = 1000;//1s
            autostop.Interval = 1000;//1s

            listBox1.SelectionMode = SelectionMode.One;

            for (int i = 2; i < 40; i++ )
            {
                comboBox1.Items.Add(i);
                comboBox2.Items.Add(i);
                comboBox3.Items.Add(i);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 2;
            comboBox1.Items.Remove(24);
            comboBox2.Items.Remove(24);
            comboBox3.Items.Remove(24);

            for (int i = 0; i <= 59; i++)
            {
                comboBox4.Items.Add(i);
                comboBox5.Items.Add(i);
                comboBox6.Items.Add(i);
                comboBox7.Items.Add(i);
                comboBox8.Items.Add(i);
                comboBox9.Items.Add(i);
                comboBox10.Items.Add(i);
                comboBox11.Items.Add(i);
                comboBox12.Items.Add(i);
            }

            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox11.SelectedIndex = 0;
            comboBox12.SelectedIndex = 0;

            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;

            convarible.inital();
            for (int i = 0; i < 3; i++)
            {
                threadmanager[i] = new ThreadManager();
                threadmanager[i].listboxMessageSend += new ThreadManager.MessageEventHandler(_listbox_MessageSend);//log
                threadmanager[i].labelMessageSend += new ThreadManager.MessageEventHandler(_label_MessageSend);//即时log
                threadmanager[i].timeMessageSend += new ThreadManager.MessageEventHandler(_time_MessageSend);//timer交互
            }

            listBox1.Items.Insert(0,currentTimestr + " 组件初始化完成");
        }

        private void FindKCV()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr childHwnd = new IntPtr(0);
            ParenthWnd = FindWindow(null, "提督業も忙しい！");            
            //判断这个窗体是否有效 
            if (ParenthWnd != IntPtr.Zero)
            {
                KCVhWnd = ParenthWnd;
                childHwnd = FindWindowEx(ParenthWnd, IntPtr.Zero, "Shell Embedding", null);
                if(childHwnd != IntPtr.Zero){
                    childHwnd = FindWindowEx(childHwnd, IntPtr.Zero, "Shell DocObject View", null);                    
                    if (childHwnd != IntPtr.Zero)
                    {
                        childHwnd = FindWindowEx(childHwnd, IntPtr.Zero, "Internet Explorer_Server", null);                      
                        RECT rc = new RECT();
                        GetWindowRect(childHwnd, ref rc);
                        Gamewidth = rc.Right - rc.Left; //窗口的宽度
                        Gameheight = rc.Bottom - rc.Top; //窗口的高度
                        //int x = rc.Left;
                        //int y = rc.Top;
                        MessageBox.Show("已自动找到KCV窗口 窗口大小" + Gamewidth + "x" + Gameheight);
                        if (Gamewidth == 800 && Gameheight == 480)
                        {
                            GamehWnd = childHwnd;
                            label22.Text = GamehWnd.ToString();
                            label62.Text = GamehWnd.ToString();
                            listBox1.Items.Insert(0,currentTimestr + " 自动动设置句柄成功 句柄为:" + GamehWnd.ToString());
                        }
                        else
                        {
                            Gamewidth = 0;
                            Gameheight = 0;
                            GamehWnd = IntPtr.Zero;
                            MessageBox.Show("已自动找到KCV窗口 窗口大小不正确 请调整为800x480");
                            listBox1.Items.Insert(0,currentTimestr + " 已自动找到KCV窗口 窗口大小不正确");
                        }
                    }                    
                }
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FindKCV();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(_switch == 0){
                if (GamehWnd != IntPtr.Zero)
                {
                    listBox1.Items.Insert(0,currentTimestr + " 开始启用");
                    label21.Text = "已启动";
                    Startinital(); 
                }
                else
                {
                    FindKCV();
                    if (GamehWnd != IntPtr.Zero)
                    {
                        listBox1.Items.Insert(0,currentTimestr + " 开始启用");
                        label21.Text = "已启动";
                        Startinital();                       
                    }
                    else
                    {
                        MessageBox.Show("句柄未找到，请手动设置");
                        listBox1.Items.Insert(0,currentTimestr + " 句柄未找到，请手动设置");
                    }
                }
            }              
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            manual.Enabled = true;
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            manual.Enabled = false;
            RECT rc = new RECT();
            GetWindowRect(GamehWnd, ref rc);
            Gamewidth = rc.Right - rc.Left; //窗口的宽度
            Gameheight = rc.Bottom - rc.Top; //窗口的高度
            MessageBox.Show("句柄为" + GamehWnd + " " + "窗口大小为" + Gamewidth + "x" + Gameheight);
            if (Gamewidth != 800 || Gameheight != 480)
            {
                MessageBox.Show("窗口大小不正确 请重新自动/手动获取句柄");
                GamehWnd = IntPtr.Zero;
                Gamewidth = 0;
                Gameheight = 0;
                label22.Text = "未设定";
                label62.Text = "未设定";
                listBox1.Items.Insert(0,currentTimestr + " 窗口大小不正确 请重新自动/手动获取句柄");
            }
            else
            {
                label22.Text = GamehWnd.ToString();
                label62.Text = GamehWnd.ToString();
                listBox1.Items.Insert(0,currentTimestr + " 手动设置句柄成功 句柄为:" + GamehWnd.ToString());
            }
        }       

        private void button2_Click(object sender, EventArgs e)
        {
            if (_switch == 1)
            {
                checkBox5.Checked = false;
                StopThread();
                label21.Text = "已停止";
                listBox1.Items.Insert(0, currentTimestr + " 手动停止");            
            }            
        }

        private void _listbox_MessageSend(object sender, MessageEventArgs e)
        {
            //实例化代理
            MessageHandler handler = new MessageHandler(updatelistbox);
            //调用Invoke
            this.Invoke(handler, new object[] { e });
        }

        private void _label_MessageSend(object sender, MessageEventArgs e)
        {
            //实例化代理
            MessageHandler handler = new MessageHandler(updatelabel);
            //调用Invoke
            this.Invoke(handler, new object[] { e });
        }

        private void _time_MessageSend(object sender, MessageEventArgs e)
        {
            //实例化代理
            MessageHandler handler = new MessageHandler(timehandle);
            //调用Invoke
            this.Invoke(handler, new object[] { e });
        }

        private void updatelistbox(MessageEventArgs e)
        {
            listBox1.Items.Insert(0, currentTimestr + " " + e.Message.ToString());
        }

        private void updatelabel(MessageEventArgs e)
        {
            label21.Text = e.Message.ToString();
            //"第二舰队已远征"
            if (e.Message.ToString() == "第2舰队已远征")
            {
                label5.Text = "正在远征";
            }
            else if (e.Message.ToString() == "第3舰队已远征")
            {
                label6.Text = "正在远征";
            }
            else if (e.Message.ToString() == "第4舰队已远征")
            {
                label7.Text = "正在远征";
            }

        }

        private void timehandle(MessageEventArgs e)
        {
            if(e.Message.ToString() == "overstart"){
                OverTime = int.Parse(textBox6.Text);
                Overtimer.Enabled = true;
            }
            if (e.Message.ToString() == "overstop")
            {
                OverTime = int.Parse(textBox6.Text);
                Overtimer.Enabled = false;
            }
            if (e.Message.ToString() == "fleet2finish")
            {
                fleet2 = convarible.Expeditiontime[int.Parse(comboBox1.Text.ToString()) - 1] * 60 + this.RndTime();
                comboBox4.SelectedIndex = ((fleet2 - fleet2 % 60) / 60 - ((fleet2 - fleet2 % 60) / 60) % 60) / 60;
                comboBox9.SelectedIndex = ((fleet2 - fleet2 % 60) / 60) % 60;
                comboBox12.SelectedIndex = fleet2 % 60;
                fleet2timer.Enabled = true;
            }
            if (e.Message.ToString() == "fleet3finish")
            {
                fleet3 = convarible.Expeditiontime[int.Parse(comboBox2.Text.ToString()) - 1] * 60 + this.RndTime();
                comboBox5.SelectedIndex = ((fleet3 - fleet3 % 60) / 60 - ((fleet3 - fleet3 % 60) / 60) % 60) / 60;
                comboBox8.SelectedIndex = ((fleet3 - fleet3 % 60) / 60) % 60;
                comboBox11.SelectedIndex = fleet3 % 60;
                fleet3timer.Enabled = true;
            }
            if (e.Message.ToString() == "fleet4finish")
            {
                fleet4 = convarible.Expeditiontime[int.Parse(comboBox3.Text.ToString()) - 1] * 60 + this.RndTime();
                comboBox6.SelectedIndex = ((fleet4 - fleet4 % 60) / 60 - ((fleet3 - fleet4 % 60) / 60) % 60) / 60;
                comboBox7.SelectedIndex = ((fleet4 - fleet4 % 60) / 60) % 60;
                comboBox10.SelectedIndex = fleet4 % 60;
                fleet4timer.Enabled = true;
            }

        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // e.KeyChar == (Char)48 ~ 57 -----> 0~9
            // e.KeyChar == (Char)8 -----------> Backpace
            // e.KeyChar == (Char)13-----------> Enter
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
               e.KeyChar == (Char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void manual_Tick(object sender, EventArgs e)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            Point p = new Point(x, y);
            GamehWnd = WindowFromPoint(p);
            label62.Text = GamehWnd.ToString();

        }

        private void SysTime_Tick(object sender, EventArgs e)
        {
            currentTime = System.DateTime.Now;
            currentTimestr = currentTime.ToString("MM-dd HH:mm:ss");
            if (_switch == 1 && checkBox7.Checked == true && currentTime.Hour == int.Parse(textBox8.Text) && currentTime.Minute == int.Parse(textBox7.Text))
            {
                label21.Text = "自动停止";
                listBox1.Items.Insert(0, currentTimestr + " 到达自动停止时间");
                StopThread();
            }
        }

        private void Overtimer_Tick(object sender, EventArgs e)
        {
            OverTime = OverTime - 1;
            if (OverTime <= 0)
            {
                _switch = 0;
                label21.Text = "已停止";
                listBox1.Items.Insert(0, currentTimestr + " 超时停止");
                StopThread();

            }
        }

        private void fleet2timer_Tick(object sender, EventArgs e)
        {
            fleet2 = fleet2 - 1;
            if (fleet2>-1)
            {
                comboBox4.SelectedIndex = ((fleet2 - fleet2 % 60) / 60 - ((fleet2 - fleet2 % 60) / 60) % 60) / 60;
                comboBox9.SelectedIndex = ((fleet2 - fleet2 % 60) / 60) % 60;
                comboBox12.SelectedIndex = fleet2 % 60; 
            }
            if (fleet2 <= 0)
            {
                fleet2timer.Enabled = false;
                convarible.TeamSupply[0] = true;
                fleet2count++;
                label18.Text = fleet2count.ToString();
                threadmanager[0].StartThread(GamehWnd, threadmanager, 2);
            }
        }

        private void fleet3timer_Tick(object sender, EventArgs e)
        {
            fleet3 = fleet3 - 1;
            if (fleet3>-1)
            {
                comboBox5.SelectedIndex = ((fleet3 - fleet3 % 60) / 60 - ((fleet3 - fleet3 % 60) / 60) % 60) / 60;
                comboBox8.SelectedIndex = ((fleet3 - fleet3 % 60) / 60) % 60;
                comboBox11.SelectedIndex = fleet3 % 60; 
            }
            if (fleet3 <= 0)
            {
                fleet3timer.Enabled = false;
                convarible.TeamSupply[1] = true;
                fleet3count++;
                label17.Text = fleet3count.ToString();
                threadmanager[1].StartThread(GamehWnd, threadmanager, 3);
            }
        }

        private void fleet4timer_Tick(object sender, EventArgs e)
        {
            fleet4 = fleet4 - 1;
            if (fleet4>-1)
            {
                comboBox6.SelectedIndex = ((fleet4 - fleet4 % 60) / 60 - ((fleet4 - fleet4 % 60) / 60) % 60) / 60;
                comboBox7.SelectedIndex = ((fleet4 - fleet4 % 60) / 60) % 60;
                comboBox10.SelectedIndex = fleet4 % 60; 
            }
            if (fleet4 <= 0)
            {
                fleet4timer.Enabled = false;
                convarible.TeamSupply[2] = true;
                fleet4count++;
                label16.Text = fleet4count.ToString();
                threadmanager[2].StartThread(GamehWnd, threadmanager, 4);
            }
        }

        private void delaytimer_Tick(object sender, EventArgs e)
        {
            DelayStart = DelayStart - 1;
            label70.Text = ((DelayStart - DelayStart % 60) / 60).ToString() + "分钟" + DelayStart % 60 + "秒";
            if (DelayStart <= 0)
            {               
                label21.Text = "延迟脚本自动开始";
                listBox1.Items.Insert(0, currentTimestr + " 到达自动停止时间");
                StopThread();
            }           
        }

        private void autostop_Tick(object sender, EventArgs e)
        {
            StopTime = StopTime - 1;
            label45.Text = (((StopTime - StopTime % 60) / 60) - ((StopTime - StopTime % 60) / 60) % 60) / 60 + "小时"
                + ((StopTime - StopTime % 60) / 60) % 60 + "分钟" 
                + StopTime % 60 + "秒";
            if (StopTime <= 0)
            {
                label21.Text = "自动停止";
                listBox1.Items.Insert(0, currentTimestr + " 到达自动停止时间");
                autostop.Enabled = false;
                StopThread();
            }
        }

        private void Startinital()
        {
            if (checkBox1.Checked == true||checkBox2.Checked == true||checkBox3.Checked == true)
            {
                if ((checkBox4.Checked == true && int.Parse(textBox2.Text) * 60 * 60 + int.Parse(textBox3.Text) * 60 >= DelayStart)
                        ||
                        (checkBox4.Checked == false)
                            )
                {
                    //convarible.Team2_index = int.Parse(comboBox1.Text.ToString());
                    //convarible.Team3_index = int.Parse(comboBox2.Text.ToString());
                    //convarible.Team4_index = int.Parse(comboBox3.Text.ToString());
                    //convarible.Team2Go = checkBox1.Checked;
                    //convarible.Team3Go = checkBox2.Checked;
                    //convarible.Team4Go = checkBox3.Checked;
                    convarible.RndTimeRange = int.Parse(textBox5.Text);
                    convarible.RndPixelRange = int.Parse(textBox4.Text);
                    convarible.RndWaitTimeRange = int.Parse(textBox9.Text);
                    convarible.rndTimeModification();
                    convarible.rndPixelModification();
                    textBox5.Text = convarible.RndTimeRange.ToString();
                    textBox4.Text = convarible.RndPixelRange.ToString();

                    convarible.NeedSupply = true;
                    convarible.Team_index[0] = int.Parse(comboBox1.Text.ToString());
                    convarible.Team_index[1] = int.Parse(comboBox2.Text.ToString());
                    convarible.Team_index[2] = int.Parse(comboBox3.Text.ToString());
                    convarible.Team_GO[0] = checkBox1.Checked;
                    convarible.Team_GO[1] = checkBox2.Checked;
                    convarible.Team_GO[2] = checkBox3.Checked;
                    OverTime = int.Parse(textBox6.Text);
                    DelayStart = int.Parse(textBox1.Text) * 60;
                    StopTime = int.Parse(textBox2.Text) * 60 * 60 + int.Parse(textBox3.Text) * 60;//转换成秒
                    listBox1.Items.Insert(0, currentTimestr + " 读取设置完成");
                    listBox1.Items.Insert(0, currentTimestr + " 开始倒计时");

                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    groupBox3.Enabled = false;
                    checkBox1.Enabled = false;
                    checkBox2.Enabled = false;
                    checkBox3.Enabled = false;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBox6.Enabled = false;
                    comboBox7.Enabled = false;
                    comboBox8.Enabled = false;
                    comboBox9.Enabled = false;
                    comboBox10.Enabled = false;
                    comboBox11.Enabled = false;
                    comboBox12.Enabled = false;
                    fleet4count = 0;
                    fleet3count = 0;
                    fleet2count = 0;

                    label10.Text = comboBox1.Text.ToString();
                    label9.Text = comboBox2.Text.ToString();
                    label8.Text = comboBox3.Text.ToString();
                    label16.Text = fleet4count.ToString();
                    label17.Text = fleet3count.ToString();
                    label18.Text = fleet2count.ToString();

                    _switch = 1;
                    if (DelayStart > 0)
                    {
                        delaytimer.Enabled = true;
                    }
                    else
                    {
                        if (checkBox7.Checked == true)
                        {
                            int all;
                            if ((int.Parse(textBox8.Text) < currentTime.Hour) || (int.Parse(textBox8.Text) == currentTime.Hour && int.Parse(textBox7.Text) <= currentTime.Minute))
                            {
                                //先计算到当天0点的剩余时间
                                int Day1Hour = 24 - (currentTime.Hour + 1);
                                int Day1min = 60 - (currentTime.Minute + 1);
                                int Day1sec = 60 - currentTime.Second;
                                int Day1alltosec = Day1Hour * 60 * 60 + Day1min * 60 + Day1sec;
                                //计算第二天的剩余时间
                                int Day2hour = int.Parse(textBox8.Text);
                                int Day2min = int.Parse(textBox7.Text);
                                int Day2alltosec = Day2hour * 60 * 60 + Day2min * 60;
                                //计算综合
                                all = Day1alltosec + Day2alltosec;
                            }
                            else
                            {
                                int lefthour = int.Parse(textBox8.Text) - (currentTime.Hour + 1);
                                int leftmin = 60 - (currentTime.Minute + 1);
                                int leftsec = 60 - currentTime.Second;
                                all = int.Parse(textBox7.Text) * 60 + lefthour * 60 * 60 + leftmin * 60 + leftsec;
                            }
                            StopTime = all;
                            autostop.Enabled = true;
                        }


                        label70.Text = "脚本开始";
                        listBox1.Items.Insert(0, currentTimestr + " 脚本开始");
                        fleetJudge();
                    }
                }
                else
                {
                    MessageBox.Show("自动结束时间早于延迟时间或者时间太短", "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请至少选择一支舰队", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //threadmanager.StartThread(GamehWnd, threadmanager);            
        }

        private void fleetJudge()
        {            
            fleet2 = int.Parse(comboBox4.Text.ToString()) * 60 * 60 + int.Parse(comboBox9.Text.ToString()) * 60 + 59;
            fleet3 = int.Parse(comboBox5.Text.ToString()) * 60 * 60 + int.Parse(comboBox8.Text.ToString()) * 60 + 59;
            fleet4 = int.Parse(comboBox6.Text.ToString()) * 60 * 60 + int.Parse(comboBox7.Text.ToString()) * 60 + 59;
            if(fleet2 <= 59){
                fleet2 = 0;
            }
            else
            {
                label5.Text = "正在远征";
            }
            if(fleet3 <= 59){
                fleet3 = 0;
            }
            else
            {
                label6.Text = "正在远征";
            }
            if (fleet4 <= 59)
            {
                fleet4 = 0;
            }
            else
            {
                label7.Text = "正在远征";
            }

            if(checkBox4.Checked == true){
                if (int.Parse(textBox2.Text) + int.Parse(textBox3.Text) > 0)
                {
                    autostop.Enabled = true;
                }
                else
                {
                    checkBox4.Checked = false;
                }

            }

            if (convarible.Team_GO[0] == true)
            {
                fleet2timer.Enabled = true;
            }

            Thread.Sleep(100);

            if (convarible.Team_GO[1] == true)
            {
                fleet3timer.Enabled = true;
            }

            Thread.Sleep(100);

            if (convarible.Team_GO[2] == true)
            {
                fleet4timer.Enabled = true;
            }         
        }

        private void StopThread()
        {            

            threadmanager[0].EndThread(2);
            threadmanager[1].EndThread(3);
            threadmanager[2].EndThread(4);
            convarible.Threadflag[0] = false;
            convarible.Threadflag[1] = false;
            convarible.Threadflag[2] = false;
            fleet2timer.Enabled = false;
            fleet3timer.Enabled = false;
            fleet4timer.Enabled = false;
            delaytimer.Enabled = false;
            Overtimer.Enabled = false;
            autostop.Enabled = false;

            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            checkBox1.Enabled = true;
            checkBox2.Enabled = true;
            checkBox3.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;
            comboBox6.Enabled = true;
            comboBox7.Enabled = true;
            comboBox8.Enabled = true;
            comboBox9.Enabled = true;
            comboBox10.Enabled = true;
            comboBox11.Enabled = true;
            comboBox12.Enabled = true;

            _switch = 0;

            if((checkBox4.Checked == true || checkBox7.Checked == true) && checkBox5.Checked == true){
                autoshutdown();
            }
        }

        private void autoshutdown()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe"; // 启动命令行程序
            proc.StartInfo.UseShellExecute = false; // 不使用Shell来执行,用程序来执行
            proc.StartInfo.RedirectStandardError = true; // 重定向标准输入输出
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true; // 执行时不创建新窗口
            proc.Start();

            string commandLine = @"shutdown /f /s /t 600";
            proc.StandardInput.WriteLine(commandLine);
            DialogResult cancel = MessageBox.Show("将在10mins后自动关机", "自动关机", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (cancel == DialogResult.OK)
            {
                commandLine = @"shutdown /a";
                proc.StandardInput.WriteLine(commandLine);
            }

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool AnyAlive = false;
            for (int i = 0; i < 3;i++ )
            {
                if (threadmanager[i].MainThread != null)
	            {
                    if (threadmanager[i].MainThread.IsAlive)
                    {
                        AnyAlive = true;
                        break;
                    }
	            }
            }
            if (AnyAlive)
            {
                DialogResult question = MessageBox.Show("脚本正在运行是否关闭程序?\n\r关闭选择Yes 最小化选择No 取消选择Cancel", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (question == DialogResult.Yes)
                {
                    this.StopThread();
                    this.SaveLog();
                    e.Cancel = false;
                }
                if (question == DialogResult.No)
                {
                    this.WindowState = FormWindowState.Minimized;
                    e.Cancel = true;
                }
                else if (question == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            FindKCV();
        }

        private double rndint()
        {
            double rnd;
            Random ra = new Random();
            rnd = ra.NextDouble();
            return rnd;
        }

        private int RndTime()
        {
            int rnd;
            rnd = (int)(rndint() * convarible.RndWaitTimeRange);
            return rnd;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RECT rc = new RECT();
            GetWindowRect(KCVhWnd, ref rc);
            KCVposX = rc.Left;
            KCVposY = rc.Top;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            const int SWP_NOSIZE = 0x0001;
            SetWindowPos(KCVhWnd, (IntPtr)(0), screenHeight, screenWidth, 0, 0, SWP_NOSIZE);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            const int SWP_NOSIZE = 0x0001;
            SetWindowPos(KCVhWnd, (IntPtr)(0), KCVposX, KCVposY, 0, 0, SWP_NOSIZE);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            imageCapture captureBMP = new imageCapture();
            Bitmap bmp = new Bitmap(800, 480);
            bmp = captureBMP.GetWindowImage(GamehWnd);
            string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            String Timestr = currentTime.ToString("HHmmss");
            bmp.Save(dir + Timestr + ".jpg", ImageFormat.Jpeg);
            Process.Start(dir + Timestr + ".jpg");
            listBox1.Items.Insert(0, currentTimestr + " 已截屏");
            listBox1.Items.Insert(0, currentTimestr + " 保存路径为:" + dir + Timestr + ".jpg");
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                MenuItem menuItem_Abort = new MenuItem();
                menuItem_Abort.Index = 1;
                menuItem_Abort.Text = "停止";
                menuItem_Abort.Click += new System.EventHandler(this.menuItem_Abort_Click);

                MenuItem menuItem_Exit = new MenuItem();
                menuItem_Exit.Index = 0;
                menuItem_Exit.Text = "退出";
                menuItem_Exit.Click += new System.EventHandler(this.menuItem_Exit_Click);

                ContextMenu contextMenu1 = new ContextMenu();
                contextMenu1.MenuItems.AddRange(
                    new System.Windows.Forms.MenuItem[]
                    {
                        menuItem_Abort,
                        menuItem_Exit
                    });

                notifyIcon.ContextMenu = contextMenu1;

                this.Hide();
                this.notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
                this.notifyIcon.ShowBalloonTip(1000, null, "程序仍在运行", ToolTipIcon.Info);//
            } 
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
	        {
		        this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.notifyIcon.Visible = false; 
	        }
        }

        private void menuItem_Abort_Click(object sender, System.EventArgs e)
        {
            this.StopThread();
        }

        private void menuItem_Exit_Click(object sender, System.EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.notifyIcon.Visible = false;
            this.Close();
        }

        private void SaveLog()
        {
            int NUM = listBox1.Items.Count;
            String[] listboxLog = new String[NUM];
            String Log = "";
            for (int i = 0; i < NUM; i++)
            {
                Log += listBox1.Items[i].ToString() + "\r\n";
            }

            string dir = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            StreamWriter w = new StreamWriter(dir + "Log.txt", true);
            w.WriteLine(Log);
            w.Close();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true && checkBox7.Checked == true)
            {
                checkBox4.Checked = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true && checkBox7.Checked == true)
            {
                checkBox7.Checked = false;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (int.Parse(textBox2.Text) + int.Parse(textBox3.Text) > 0)
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (int.Parse(textBox7.Text) + int.Parse(textBox8.Text) > 0)
            {
                checkBox7.Checked = true;
            }
            else
            {
                checkBox7.Checked = false;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.SaveLog();
        }
    }

}
