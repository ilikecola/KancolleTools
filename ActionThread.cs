using System;
using System.Threading;


namespace KancolleMacro
{
    public class ActionThread
    {
        //用户设置
        //int Team2_index;
        //int Team3_index;
        //int Team4_index;
        //int NeedSupply;
        //int Team2Go;
        //int Team3Go;
        //int Team4Go;
        //时间设置
        //int TimeWaiting;
        //int RealWaiting;
        //int StopTime;
        //int RealStopTime;
        //int TimeSec;
        //int TimeMin;
        //int RndTimeRange;
        //int RndPixelRange;
        //int DelayStart;

        private ActionEvent actioneevent = new ActionEvent();
        private IntPtr GameHwnd;
        private ThreadManager[] threadmanager;
        private int TeamNO;
        public ActionThread(IntPtr hWnd, ThreadManager[] threadmanager, int TeamNO)
        {
            this.GameHwnd = hWnd;
            this.threadmanager = threadmanager;
            this.TeamNO = TeamNO;
        }

        public void MainThread()
        {
            try
            {
                //线程开始
                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "已启动");
                threadmanager[TeamNO - 2].Sendlabelmessage("线程" + (TeamNO - 1) + "已启动");
                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "正等待其他线程");

                //是否有其他进程
                while (true)
                {
                    Thread.Sleep(500 + this.RndTime());
                    if (TeamNO == 2)
                    {
                        if (!(convarible.Threadflag[1] || convarible.Threadflag[2])) break;
                    }
                    else if (TeamNO == 3)
                    {
                        if (!(convarible.Threadflag[0] || convarible.Threadflag[2])) break;
                    }
                    else if (TeamNO == 4)
                    {
                        if (!(convarible.Threadflag[0] || convarible.Threadflag[1])) break;
                    }                    
                }

                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "开始初始化");
                //初始化
                convarible.Threadflag[TeamNO - 2] = true;
                convarible.TeamSupply[TeamNO - 2] = true;

                //返回母港
                while(!Gohome())
                {
                    Thread.Sleep(1000 + this.RndTime());
                }

                Thread.Sleep(300 + this.RndTime());

                //进入补给
                threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO + "舰队准备开始补给");
                while (!GoSupply())
                {
                    Thread.Sleep(1000 + this.RndTime());
                }

                Thread.Sleep(300 + this.RndTime());

                //开始远征
                while (!GoExpedition())
                {
                    Thread.Sleep(1000 + this.RndTime());
                }
                convarible.Threadflag[TeamNO - 2] = false;

                //线程结束
            }
            catch (ThreadAbortException e)
            {

                threadmanager[TeamNO - 2].Sendlistboxmessage("线程"+ (TeamNO - 1)+ "被意外中止");
                threadmanager[TeamNO - 2].Sendlabelmessage("线程" + (TeamNO - 1) + "被意外中止");
                threadmanager[TeamNO - 2].Sendlistboxmessage("Exception message:" + e.Message);
            }
        }

        private bool Gohome ()
        {

            threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "计划返回母港");
            threadmanager[TeamNO - 2].Sendlabelmessage("返回母港");

            //初始化对象
            string FleetBack = null;
            string TaskBack = null;
            string LaunchBack = null;
            string ExpeditionBack = null;
            string HomeBack = null;
            bool success = false;

            //是否在母港界面
            HomeBack = actioneevent.GetPixelColor(GameHwnd, 196, 240);
            threadmanager[TeamNO - 2].Sendlistboxmessage("HomeBack:" + HomeBack);
            if (HomeBack == "209BFB")
            {
                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "计划点击出击");
                actioneevent.LeftClick(GameHwnd, 197 + this.RndPixel(), 244 + this.RndPixel());
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                while (true)
                {//当不在出击界面持续等待(出现“远征”按钮则认为已经载入完毕）
                    string GoExpeditionIcon = actioneevent.GetPixelColor(GameHwnd, 676, 249);
                    if (GoExpeditionIcon == "B58B40")
                    {
                        break;
                    }
                    Thread.Sleep(200 + this.RndTime());
                }
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "点击出击成功");
            }

            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (true)
            {                
                    /*
                    此过程的逻辑如下 
                    1.如果在舰队返回画面（一般指的是远征队返回画面）则不停的点鼠标左键，判断依据为那个齿轮点
                    2.如果在其他界面比如图签、出击之类的，直接返回
                    3.如果卡在菜单出来的画面，则等待
                    */
                FleetBack = actioneevent.GetPixelColor(GameHwnd, 760, 441);
                TaskBack = actioneevent.GetPixelColor(GameHwnd, 76, 446);
                LaunchBack = actioneevent.GetPixelColor(GameHwnd, 75, 260);       

                threadmanager[TeamNO - 2].Sendlistboxmessage("FleetBack:" + FleetBack);
                threadmanager[TeamNO - 2].Sendlistboxmessage("TaskBack:" + TaskBack);
                threadmanager[TeamNO - 2].Sendlistboxmessage("LaunchBack:" + LaunchBack);

                if (FleetBack == "A39F3F" || FleetBack == "ADA94D" || FleetBack == "A19F42")
                {
                    actioneevent.LeftClick(GameHwnd, 760 + this.RndPixel(), 441 + this.RndPixel());
                }                   
                else if (TaskBack == "858519")
                {
                    actioneevent.LeftClick(GameHwnd, 76 + this.RndPixel(), 446 + this.RndPixel());
                }
                else if (LaunchBack == "ADB3B7")
                {
                    actioneevent.LeftClick(GameHwnd, 66 + this.RndPixel(), 264 + this.RndPixel());
                }
                Thread.Sleep(500 + this.RndTime());

                //==========判断是否有远征队回来
                ExpeditionBack = actioneevent.GetPixelColor(GameHwnd,521,28);
                if(ExpeditionBack == "A2B226"){ 
                    Thread.Sleep(200 + this.RndTime());
                    threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "检测到远征归还");
                    while (!GetExpedition())
                    {
                        Thread.Sleep(1000 + this.RndTime());
                    }
                }

                //==========判断是否回港
                Thread.Sleep(1000 + this.RndTime());
                if (Checkhome())
                {
                    success = true;
                    break;
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            Thread.Sleep(300 + this.RndTime());

            //到达此处确定已经返回母港
            threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "返回母港成功");
            threadmanager[TeamNO - 2].Sendlabelmessage("返回母港成功");
            return success;
        }

        private bool Checkhome()
        {
            bool check = false;
            string HomeBack = null;
            HomeBack = actioneevent.GetPixelColor(GameHwnd, 196, 240);
            threadmanager[TeamNO - 2].Sendlistboxmessage("HomeBack:" + HomeBack);
            if (HomeBack == "209BFB")
            {
                check = true;
            }
            return check;
        }

        private bool GetExpedition()
        {
            threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "准备迎接远征队");
            threadmanager[TeamNO - 2].Sendlabelmessage("准备迎接远征队");

            //初始化
            string NextFlag = null;
            string SecFlag = null;
            string wait = null;
            string HomeBack = null;
            bool success = false;

            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            do
            {
                //点击旗帜
                actioneevent.LeftClick(GameHwnd, 521 + this.RndPixel(), 28 + this.RndPixel());
                //等待进入舰队结算界面
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                do
                {
                    Thread.Sleep(2000 + this.RndTime());
                    wait = actioneevent.GetPixelColor(GameHwnd, 555, 250);
                    NextFlag = actioneevent.GetPixelColor(GameHwnd, 760, 441);
                } while (!((wait == "4ADEFE" || wait == "7FBDCC") && (NextFlag == "A39F3F" || NextFlag == "ADA94D" || NextFlag == "A19F42")));
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                //点击齿轮
                do
                {
                    actioneevent.LeftClick(GameHwnd, 760 + this.RndPixel(), 441 + this.RndPixel());
                    Thread.Sleep(500 + this.RndTime());
                    HomeBack = actioneevent.GetPixelColor(GameHwnd, 196, 240);
                } while (!(HomeBack == "209BFB" || HomeBack == "1C8BE2"));
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                //是否有第二只舰队返回？
                SecFlag = actioneevent.GetPixelColor(GameHwnd, 521, 28);
            } while (HomeBack == "1C8BE2" || SecFlag == "A2B226");
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            success = true;
            threadmanager[TeamNO - 2].Sendlistboxmessage("迎接远征队完成");
            return success;
        }

        private bool GoSupply()
        {
            //调用此过程需要先调用GoHome!!!!!
            //此时当做全部没补给，挨个来一遍
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始补给");
            threadmanager[TeamNO - 2].Sendlabelmessage("开始补给");

            //初始化
            //========判断是否进入补给
            string SupplyIcon;
            string CheckApply;
            //========判断是否切换舰队
            string PreviousBefore_1 = null;
            string PreviousBefore_2 = null;
            string NextBefore_1 = null;
            string NextBefore_2 = null;
            string PreviousAfter_1 = null;
            string PreviousAfter_2 = null;
            string NextAfter_1 = null;
            string NextAfter_2 = null;
            //========判断是否成功补给
            bool success = false;

            //是否在母港
            while (!Checkhome())
            {
                if (Gohome())
                {
                    break;
                }
            }

            //进入补给界面
            threadmanager[TeamNO - 2].Sendlistboxmessage("尝试进入补给");
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (true)
            {
                SupplyIcon = actioneevent.GetPixelColor(GameHwnd, 82, 220);
                if (SupplyIcon == "777777")
                {
                    actioneevent.LeftClick(GameHwnd, 82 + this.RndPixel(), 220 + this.RndPixel());
                    break;
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            Thread.Sleep(500 + this.RndTime());

            //是否进入补给界面
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (true)
            {
                //当不在补给界面持续等待
                CheckApply = actioneevent.GetPixelColor(GameHwnd, 745, 118);
                if (CheckApply == "007FC9")
                {
                    break;
                }
                Thread.Sleep(200 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("进入补给成功");

            Thread.Sleep(250 + this.RndTime());

            //切换舰队
            PreviousBefore_1 = actioneevent.GetPixelColor(GameHwnd, 145, 120);
            PreviousBefore_2 = actioneevent.GetPixelColor(GameHwnd, 152, 120);
            NextBefore_1 = actioneevent.GetPixelColor(GameHwnd, 145 + 30 * (TeamNO - 1), 120);
            NextBefore_2 = actioneevent.GetPixelColor(GameHwnd, 152 + 30 * (TeamNO - 1), 120);
            threadmanager[TeamNO - 2].Sendlistboxmessage("PreviousBefore_1:" + PreviousBefore_1);
            threadmanager[TeamNO - 2].Sendlistboxmessage("PreviousBefore_2:" + PreviousBefore_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("NextBefore_1:" + NextBefore_1);
            threadmanager[TeamNO - 2].Sendlistboxmessage("NextBefore_2:" + NextBefore_2);
            do
            {
                actioneevent.LeftClick(GameHwnd, 145 + 30 * (TeamNO - 1) + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                PreviousAfter_1 = actioneevent.GetPixelColor(GameHwnd, 145, 120);
                PreviousAfter_2 = actioneevent.GetPixelColor(GameHwnd, 152, 120);
                NextAfter_1 = actioneevent.GetPixelColor(GameHwnd, 145 + 30 * (TeamNO - 1), 120);
                NextAfter_2 = actioneevent.GetPixelColor(GameHwnd, 152 + 30 * (TeamNO - 1), 120);
                threadmanager[TeamNO - 2].Sendlistboxmessage("PreviousAfter_1:" + PreviousAfter_1);
                threadmanager[TeamNO - 2].Sendlistboxmessage("PreviousAfter_2:" + PreviousAfter_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Changecurrent_1:" + NextAfter_1);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Changecurrent_2:" + NextAfter_2);
            } while (!(PreviousAfter_1 != PreviousBefore_1
                       && PreviousAfter_2 != PreviousBefore_2
                       && NextAfter_1 != NextBefore_1
                       && NextAfter_2 != NextBefore_2));
            threadmanager[TeamNO - 2].Sendlistboxmessage("进入补给" + TeamNO + "队完成");
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            Thread.Sleep(300 + this.RndTime());

            while (!ApplySupply(TeamNO))
            {
                Thread.Sleep(1000 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendlistboxmessage("补给" + TeamNO + "队完成");
            success = true;
            threadmanager[TeamNO - 2].Sendlistboxmessage("补给完成");
            threadmanager[TeamNO - 2].Sendlabelmessage("补给完成");

            Thread.Sleep(300 + this.RndTime());

            while (!Gohome())
            {
                Thread.Sleep(1000 + this.RndTime());
            }
            return success;
        }

        private bool ApplySupply(int number){
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始补给" + TeamNO + "队");

            //初始化
            int num = 6;
            bool success = false;
            string[] NumCheck = new string[6];
            string[] SelectallBefore = new string[2];
            string[] SelectallAfter = new string[2];
            string[,] BeforePoint = new string[6,2];
            string[,] AfterPoint = new string[6,2];
            string[,] Applyed = new string[,] { {"C0D3E1","BDCEE0"},
                                            {"BBCDDC","B9C9DB"},
                                            {"BBD0E0","BBCCDD"},
                                            {"BCD0E1","BECEDF"},
                                            {"C0D2E1","BCD0E1"},
                                            {"BACEDE","BDCFE0"} };
            bool checkapplyed = false;

            //获取初始值
            SelectallBefore[0] = actioneevent.GetPixelColor(GameHwnd, 115, 123);
            SelectallBefore[1] = actioneevent.GetPixelColor(GameHwnd, 120, 123);
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    BeforePoint[i, j] = actioneevent.GetPixelColor(GameHwnd, 115 + j * 5, 170 + i * 51);
                }
                NumCheck[i]= actioneevent.GetPixelColor(GameHwnd, 520, 170 + i * 51);
                if (NumCheck[i] == "B8C7CE")
                {
                    num = num - 1;
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("SelectallBefore:" + SelectallBefore[0] + " " + SelectallBefore[1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_1:" + BeforePoint[0, 0] + " " + BeforePoint[0, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_2:" + BeforePoint[1, 0] + " " + BeforePoint[1, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_3:" + BeforePoint[2, 0] + " " + BeforePoint[2, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_4:" + BeforePoint[3, 0] + " " + BeforePoint[3, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_5:" + BeforePoint[4, 0] + " " + BeforePoint[4, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("BeforePoint_6:" + BeforePoint[5, 0] + " " + BeforePoint[5, 1]);

            //是否已补给
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if(BeforePoint[i,j] == Applyed[i, j])
                    {
                        checkapplyed = true;
                        success = true;
                    }
                    else
                    {
                        checkapplyed = false;
                        success = false;
                        j = 2;
                        i = num;
                    }
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            //已补给
            if (checkapplyed == true)
            {
                threadmanager[TeamNO - 2].Sendlistboxmessage(TeamNO + "队已补给");
                return success;
            }
            //点击全补给
            actioneevent.MOUSEMOVE(GameHwnd, 20 + (this.RndPixel() / 2), 20 + (this.RndPixel() / 2));
            
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            do
            {
                Thread.Sleep(110 + this.RndTime());
                actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                Thread.Sleep(110 + this.RndTime());
                SelectallAfter[0] = actioneevent.GetPixelColor(GameHwnd, 115, 123);
                SelectallAfter[1] = actioneevent.GetPixelColor(GameHwnd, 120, 123);
                threadmanager[TeamNO - 2].Sendlistboxmessage("SelectallAfter:" + SelectallAfter[0] + " " + SelectallAfter[1]);
                if (SelectallAfter[0] != SelectallBefore[0] && SelectallAfter[1] != SelectallBefore[1])
                {
                    actioneevent.LeftClick(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                    threadmanager[TeamNO - 2].Sendlistboxmessage(TeamNO + "队正在补给");
                    Thread.Sleep(1000 + this.RndTime());
                }
                

                //First Check
                actioneevent.MOUSEMOVE(GameHwnd, 20 + this.RndPixel(), 20 + this.RndPixel());
                Thread.Sleep(110 + this.RndTime());
                actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                SelectallAfter[0] = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                SelectallAfter[1] = actioneevent.GetPixelColor(GameHwnd, 115, 120);
                threadmanager[TeamNO - 2].Sendlistboxmessage("SelectallAfter:" + SelectallAfter[0] + " " + SelectallAfter[1]);
            } while (!(SelectallAfter[0] == SelectallBefore[0] && SelectallAfter[1] == SelectallBefore[1]));
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            //Final Check
            SelectallAfter[0] = actioneevent.GetPixelColor(GameHwnd, 115, 123);
            SelectallAfter[1] = actioneevent.GetPixelColor(GameHwnd, 120, 123);
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    AfterPoint[i, j] = actioneevent.GetPixelColor(GameHwnd, 115 + j * 5, 170 + i * 51);
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("SelectallAfter:" + SelectallAfter[0] + " " + SelectallAfter[1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_1:" + AfterPoint[0, 0] + " " + AfterPoint[0, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_2:" + AfterPoint[1, 0] + " " + AfterPoint[1, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_3:" + AfterPoint[2, 0] + " " + AfterPoint[2, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_4:" + AfterPoint[3, 0] + " " + AfterPoint[3, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_5:" + AfterPoint[4, 0] + " " + AfterPoint[4, 1]);
            threadmanager[TeamNO - 2].Sendlistboxmessage("AfterPoint_6:" + AfterPoint[5, 0] + " " + AfterPoint[5, 1]);
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (AfterPoint[i, j] != BeforePoint[i, j]|| AfterPoint[i, j] == Applyed[i, j])
                    {
                        checkapplyed = true;
                        success = true;
                        convarible.TeamSupply[number - 2] = false;
                    }
                    else
                    {
                        checkapplyed = false;
                        success = false;
                        convarible.TeamSupply[number - 2] = true;
                        j = 2;
                        i = num;
                    }
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            return success;
        }

        private bool GoExpedition()
        {
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始远征");
            threadmanager[TeamNO - 2].Sendlabelmessage("开始远征");
            //初始化
            string GoExpeditionIcon = null;
            string CheckExpedition = null;
            bool success = false;

            //是否在母港
            while (!Checkhome())
            {
                if(Gohome())
                {
                    break;
                }
            }

            //点击出击
            threadmanager[TeamNO - 2].Sendlistboxmessage("计划点击出击");
            actioneevent.LeftClick(GameHwnd, 197 + this.RndPixel(), 244 + this.RndPixel());

            Thread.Sleep(200 + this.RndTime());

            //是否进入出击界面
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (true)
            {
                GoExpeditionIcon = actioneevent.GetPixelColor(GameHwnd, 676, 249);
                if (GoExpeditionIcon == "B58B40")
                {
                    break;
                }
                Thread.Sleep(500 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("点击出击成功");

            Thread.Sleep(200 + this.RndTime());

            //点击远征
            threadmanager[TeamNO - 2].Sendlistboxmessage("计划点击远征");
            actioneevent.LeftClick(GameHwnd, 676 + this.RndPixel(), 249 + this.RndPixel());//点击远征按钮
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (true)
            {//当不在出击界面持续等待(出现“远征”按钮则认为已经载入完毕）
                CheckExpedition = actioneevent.GetPixelColor(GameHwnd, 589, 271);
                if (CheckExpedition == "9F9E23")
                {
                    break;
                }
                Thread.Sleep(500 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("点击远征成功");

            Thread.Sleep(200 + this.RndTime());

            //选择远征区域
            while (!TeamGoExpedition(TeamNO))
            {
                Thread.Sleep(1000 + this.RndTime());
            }

            success = true;
            //远征完成
            return success;
        }

        private bool TeamGoExpedition(int TeamNO)
        {
            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO + "舰队准备开始远征");
            threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO + "舰队准备开始远征");
            //初始化
            string Wait = null;
            bool success = false;
            string _decide = null;
            //========判断是否切换舰队
            string[] PreviousBefore = new string[2];
            string[] NextBefore = new string[2];
            string[] PreviousAfter = new string[2];
            string[] NextAfter = new string[2];
            _decide = actioneevent.GetPixelColor(GameHwnd, 677, 444);

            //选择该海域所去舰队
            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO + "舰队尝试移动到指定地图");
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (!(MoveToExpMap(convarible.Team_index[TeamNO - 2]) && _decide != actioneevent.GetPixelColor(GameHwnd, 677, 444)))
            {
                Thread.Sleep(1000 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO + "舰队移动成功");
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            Thread.Sleep(200 + this.RndTime());

            //点击决定
            actioneevent.LeftClick(GameHwnd, 677 + this.RndPixel(), 444 + this.RndPixel());
            threadmanager[TeamNO - 2].Sendlistboxmessage("正在选择第" + TeamNO.ToString() + "舰队");

            //选择舰队
            //=====如果不是第二舰队需要选择，否则跳过
            if (TeamNO != 2)
            {
                for(int i = 0; i < 2; i++)
                {
                    PreviousBefore[i] = actioneevent.GetPixelColor(GameHwnd, 390 + i * 9, 117);
                    NextBefore[i] = actioneevent.GetPixelColor(GameHwnd, 390 + 29 * (TeamNO - 2) + i * 9, 117);
                }

                threadmanager[TeamNO - 2].Sendlistboxmessage("选择第" + TeamNO.ToString() + "舰队");
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                do
                {
                    actioneevent.LeftClick(GameHwnd, 395 + 29 * (TeamNO - 2) + this.RndPixel() / 2, 117 + this.RndPixel() / 2);//=======第TeamNO舰队
                    Thread.Sleep(200 + this.RndTime());
                    for (int i = 0; i < 2; i++)
                    {
                        PreviousAfter[i] = actioneevent.GetPixelColor(GameHwnd, 390 + i * 9, 117);
                        NextAfter[i] = actioneevent.GetPixelColor(GameHwnd, 390 + 29 * (TeamNO - 2) + i * 9, 117);
                    }
                } while (!(PreviousBefore[0] != PreviousAfter[0]
                            && PreviousBefore[1] != PreviousAfter[1]
                            && NextBefore[0] != NextAfter[0]
                            && NextBefore[1] != NextAfter[1]));
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            }

            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            do
            {
                Thread.Sleep(200 + this.RndTime());
                _decide = actioneevent.GetPixelColor(GameHwnd, 694, 445);
            } while (_decide != "7D8F1C");
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            //点击远征开始
            actioneevent.LeftClick(GameHwnd, 624 + this.RndPixel(), 445 + this.RndPixel());//远征开始
            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString() + "舰队开始远征");
            threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO.ToString() + "舰队开始远征");

            //是否已远征
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            do
            {
                Thread.Sleep(1000 + this.RndTime());
                Wait = actioneevent.GetPixelColor(GameHwnd, 720, 190);
                if (Wait == "D3E1EA")
                {
                    success = true;
                    break;
                }
            } while (true);
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString() + "舰队已远征");
            threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO + "舰队已远征");
            threadmanager[TeamNO - 2].Sendtimemessage("fleet" + TeamNO+ "finish");

            return success;
        }

        private bool MoveToExpMap(int index)
        {
            //初始化
            bool success = false;
            int Area = 0 ;
            int Loc = 0;
            string _check = null;
            string check = null;
            _check = actioneevent.GetPixelColor(GameHwnd, 448, 385);

            threadmanager[TeamNO - 2].Sendlistboxmessage("开始移动到远征地图");

            //分割海域和地图
            if (index >= 1 && index<=8)
            {
                Area = 1;
                Loc = index;
            }
            else if (index > 8 && index <= 16)
            {
                Area = 2;
                Loc = index - 8;
            }
            else if (index > 16 && index <= 24)
            {
                Area = 3;
                Loc = index - 16;
            }
            else if (index > 24 && index <= 32)
            {
                Area = 4;
                Loc = index - 23;
            }
            else if (index > 32 && index <= 39)
            {
                Area = 5;
                Loc = index - 32;
            }
            switch (Area)
            {
                case 0:
                    success = false;
                    break;
                case 1:
                    //第一海域无需检查
                    actioneevent.LeftClick(GameHwnd, 140 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    success = true;
                    break;
                case 2:
                    do
                    {
                        actioneevent.LeftClick(GameHwnd, 195 + this.RndPixel(), 440 + this.RndPixel());
                        check = actioneevent.GetPixelColor(GameHwnd, 448, 385);
                    } while (check == _check);
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    success = true;
                    break;
                case 3:
                    do
                    {
                        actioneevent.LeftClick(GameHwnd, 260 + this.RndPixel(), 440 + this.RndPixel());
                        check = actioneevent.GetPixelColor(GameHwnd, 448, 385);
                    } while (check == _check);
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    success = true;
                    break;
                case 4:
                    do
                    {
                        actioneevent.LeftClick(GameHwnd, 310 + this.RndPixel(), 440 + this.RndPixel());
                        check = actioneevent.GetPixelColor(GameHwnd, 448, 385);
                    } while (check == _check);
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    success = true;
                    break;
                case 5:
                    do
                    {
                        actioneevent.LeftClick(GameHwnd, 370 + this.RndPixel(), 440 + this.RndPixel());
                        check = actioneevent.GetPixelColor(GameHwnd, 448, 385);
                    } while (check == _check);
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    success = true;
                    break;
                default:
                    success = false;
                    break;

            }

            return success;
        }

        //private double rndint()
        //{
        //    double rnd;
        //    Thread.Sleep(100);
        //    Random ra = new Random();
        //    rnd = ra.NextDouble();
        //    return rnd;
        //}

        private int RndTime()
        {
            Random ra = new Random();
            int rnd;
            rnd = ra.Next(-convarible.RndTimeRange, convarible.RndTimeRange);
            //rnd = (int)(rndint() * convarible.RndTimeRange - convarible.RndTimeRange / 2);
            return rnd;
        }

        private int RndPixel()
        {
            Random ra = new Random();
            int rnd;
            rnd = ra.Next(-convarible.RndPixelRange, convarible.RndPixelRange);
            //rnd = (int)(rndint() * convarible.RndPixelRange - convarible.RndPixelRange / 2);
            return rnd;
        }

    }
}
