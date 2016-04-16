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
            bool a = true;
            while(a){
                if(TeamNO == 2){
                    a = convarible.Threadflag[1] || convarible.Threadflag[2];
                    if (a == false) break;
                }else if(TeamNO == 3){
                    a = convarible.Threadflag[0] || convarible.Threadflag[2];
                    if (a == false) break;
                }
                else if (TeamNO == 4){
                    a = convarible.Threadflag[0] || convarible.Threadflag[1];
                    if (a == false) break;
                }
                Thread.Sleep(500);
            }
            convarible.Threadflag[TeamNO - 2] = true;
            threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "已启动");
            threadmanager[TeamNO - 2].Sendlabelmessage("线程" + (TeamNO - 1) + "已启动");
            convarible.NeedSupply = true;

            Gohome();
            threadmanager[TeamNO - 2].Sendlistboxmessage("准备开始补给");
            while(convarible.NeedSupply == true){
                Thread.Sleep(200 + this.RndTime());
                GoSupply();
            }
            GoExpedition();
            convarible.Threadflag[TeamNO - 2] = false;
        }

        private void Gohome ()
        {
            string FleetBack;
            string TaskBack;
            string LaunchBack;
            string LaunchBack2;
            string ExpeditionBack;
            string HomeBack; 

            //返回母港的函数，包含了坑爹网络的情况，包含了在其他画面的情况
            threadmanager[TeamNO - 2].Sendlistboxmessage("计划返回母港");
            threadmanager[TeamNO - 2].Sendlabelmessage("返回母港");
            //当不在母港的时候
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
                LaunchBack = actioneevent.GetPixelColor(GameHwnd, 66, 264);
                LaunchBack2 = actioneevent.GetPixelColor(GameHwnd, 66, 264);
                if (FleetBack == "A39F3F" || FleetBack == "ADA94D" || FleetBack == "ACA84B" || FleetBack == "A19F42")
                {
                    actioneevent.LeftClick(GameHwnd, 760 + this.RndPixel(), 441 + this.RndPixel());
                    threadmanager[TeamNO - 2].Sendlistboxmessage("检测到远征归还");
                    GetExpedition();
                }                   
                else if (TaskBack == "858519")
                {
                    actioneevent.LeftClick(GameHwnd, 76 + this.RndPixel(), 446 + this.RndPixel());
                }
                else if (LaunchBack == "7A8689" || LaunchBack2 == "2F74B4")
                {
                    actioneevent.LeftClick(GameHwnd, 66 + this.RndPixel(), 264 + this.RndPixel());
                }
                Thread.Sleep(500 + this.RndTime());
                //==========判断是否有远征队回来
                ExpeditionBack = actioneevent.GetPixelColor(GameHwnd,521,28);
                if(ExpeditionBack == "A2B226"){ 
                    Thread.Sleep(200 + this.RndTime());
                    threadmanager[TeamNO - 2].Sendlistboxmessage("检测到远征归还");
                    GetExpedition();
                }
                //==========判断是否回港
                Thread.Sleep(1000 + this.RndTime());
                HomeBack = actioneevent.GetPixelColor(GameHwnd, 196, 240);
                if(HomeBack == "209BFB"){
                    break; 
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            Thread.Sleep(300 + this.RndTime());
            //到达此处确定已经返回母港
            threadmanager[TeamNO - 2].Sendlistboxmessage("返回母港成功");
            threadmanager[TeamNO - 2].Sendlabelmessage("返回母港成功");
        }

        private void GetExpedition()
        {
            threadmanager[TeamNO - 2].Sendlistboxmessage("准备迎接远征队");
            threadmanager[TeamNO - 2].Sendlabelmessage("准备迎接远征队");
            String NextFlag;
            String SecFlag;
            bool hasExpGet = false;
            convarible.NeedSupply = true;

            actioneevent.LeftClick(GameHwnd, 521 + this.RndPixel(), 28 + this.RndPixel());//点击旗帜
            /*
		    逻辑如下 hasExpGet 变量存储是否取得远征
	        如果再执行 第二个while循环时候还有右下角小点点的话，那么一直循环直到小点点失效，但是hasExpGet 已经设置真，这时候只要结束这个循环就能结束子程序
		    */
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            for (int i = 0, j = 0; i == 1 && j == 1 && hasExpGet == true; )
            {
                Thread.Sleep(1000 + this.RndTime());
                NextFlag = actioneevent.GetPixelColor(GameHwnd, 760, 441);
                //第一次检查旗帜
                if(NextFlag == "A39F3F" || NextFlag == "ADA94D" || NextFlag == "A19F42"){
                    j = 0;
                    hasExpGet = true;
                    actioneevent.LeftClick(GameHwnd, 755 + this.RndPixel(), 432 + this.RndPixel());
                    Thread.Sleep(100 + this.RndTime());
                }else
                {
                    j = 1;
                }

                Thread.Sleep(100 + this.RndTime());

                //2队同时归来的情况
                SecFlag = actioneevent.GetPixelColor(GameHwnd, 521, 28);
                if(SecFlag == "A2B226"){
                    i = 0;
                    hasExpGet = false;
                    actioneevent.LeftClick(GameHwnd, 521 + this.RndPixel(), 28 + this.RndPixel());
                    Thread.Sleep(100 + this.RndTime());
                }
                else
                {
                    i = 1;
                }
                Thread.Sleep(500 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("迎接远征队完成");
        }

        private void GoSupply()
        {
            //调用此过程需要先调用GoHome!!!!!
            //此时当做全部没补给，挨个来一遍
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始补给");
            threadmanager[TeamNO - 2].Sendlabelmessage("开始补给");
            string SupplyIcon;
            string CheckApply;

            threadmanager[TeamNO - 2].Sendlistboxmessage("尝试进入补给");
            bool a=true;
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while(a){
                SupplyIcon = actioneevent.GetPixelColor(GameHwnd, 82, 220);
                if (SupplyIcon == "777777")
                {
                    actioneevent.LeftClick(GameHwnd, 82 + this.RndPixel(), 220 + this.RndPixel());
                    a = false;
                }
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            
            bool b=true;
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while(b){
                //当不在补给界面持续等待
                CheckApply = actioneevent.GetPixelColor(GameHwnd, 745, 118);
                if(CheckApply == "007FC9"){
                    b = false;
                }
                Thread.Sleep(200 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            threadmanager[TeamNO - 2].Sendlistboxmessage("进入补给成功");
            //补给1队
            actioneevent.LeftClick(GameHwnd, 146 + this.RndPixel(), 118 + this.RndPixel());
            ApplySupply(1);
            threadmanager[TeamNO - 2].Sendlistboxmessage("补给1队完成");
           
            //补给2队           
            if(convarible.Team_GO[0] == true && convarible.TeamSupply[0] == true)
            {
                Thread.Sleep(250 + this.RndTime());                
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                while (actioneevent.GetPixelColor(GameHwnd, 182, 118) != "A1A023" 
                    && actioneevent.GetPixelColor(GameHwnd, 182, 118) != "A0A935")
                {
                    actioneevent.LeftClick(GameHwnd, 182 + (this.RndPixel() / 2), 118 + (this.RndPixel() / 2)); 
                }
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                ApplySupply(2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("补给2队完成");
            }
            
            //补给3队
            if (convarible.Team_GO[1] == true && convarible.TeamSupply[1] == true)
            {
                Thread.Sleep(250 + this.RndTime());
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                while(actioneevent.GetPixelColor(GameHwnd, 206, 118) != "ADAD53")
                {
                    actioneevent.LeftClick(GameHwnd, 206 + (this.RndPixel() / 2), 118 + (this.RndPixel() / 2));
                }
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                ApplySupply(3);
                threadmanager[TeamNO - 2].Sendlistboxmessage("补给3队完成");
            }
            
            //补给4队
            if (convarible.Team_GO[2] == true && convarible.TeamSupply[2] == true)
            {
                Thread.Sleep(250 + this.RndTime());
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                while (actioneevent.GetPixelColor(GameHwnd, 236, 118) != "BEBF86")
                {
                    actioneevent.LeftClick(GameHwnd, 236 + (this.RndPixel() / 2), 118 + (this.RndPixel() / 2));
                }
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                ApplySupply(4);
                threadmanager[TeamNO - 2].Sendlistboxmessage("补给4队完成");
            }
           
            Thread.Sleep(250 + this.RndTime());

            convarible.NeedSupply = false;
            threadmanager[TeamNO - 2].Sendlistboxmessage("补给完成");
            threadmanager[TeamNO - 2].Sendlabelmessage("补给完成");
            Gohome();
        }

        private void ApplySupply(int number){
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            int i = 0;
            while (i == 0 )
            {
                String SelectAll;
                actioneevent.MOUSEMOVE(GameHwnd, 0, 0);
                Thread.Sleep(100 + this.RndTime());
                actioneevent.MOUSEMOVE(GameHwnd, 120, 120);
                Thread.Sleep(250 + this.RndTime());
                SelectAll = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                if (SelectAll != "87959B")
                {
                    actioneevent.LeftClick(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                    //Thread.Sleep(200 + this.RndTime());
                    //actioneevent.LeftClick(GameHwnd, 695 + this.RndPixel(), 442 + this.RndPixel());
                    threadmanager[TeamNO - 2].Sendlistboxmessage("正在补给");
                    Thread.Sleep(1000 + this.RndTime());
                }
                actioneevent.MOUSEMOVE(GameHwnd, 0, 0);
                Thread.Sleep(100 + this.RndTime());
                actioneevent.MOUSEMOVE(GameHwnd, 120, 120);
                Thread.Sleep(250 + this.RndTime());
                SelectAll = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                if (SelectAll == "87959B"//120,120
                    && actioneevent.GetPixelColor(GameHwnd,120,165) != "000000" //120,165
                    && actioneevent.GetPixelColor(GameHwnd,120,220) != "000000"//120,220
                    && actioneevent.GetPixelColor(GameHwnd,120,270) != "000000"//120,270
                    && actioneevent.GetPixelColor(GameHwnd, 120, 320) != "000000"//120,320
                    && actioneevent.GetPixelColor(GameHwnd, 120, 375) != "000000"//120,375
                    && actioneevent.GetPixelColor(GameHwnd, 120, 425) != "000000")//120,425
                {
                    i = 1;
                }
            }
            if (number > 1)
            {
                convarible.TeamSupply[number - 2] = false;
            }

            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
        }

        private void GoExpedition()
        {
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始远征");
            threadmanager[TeamNO - 2].Sendlabelmessage("开始远征");
            string GoExpeditionIcon;
            string CheckExpedition;
            #region
            //string DecideExp;
            //string Exping;
            //string CheckgIfGo;
            //string Wait;
            #endregion
            bool w;

            threadmanager[TeamNO - 2].Sendlistboxmessage("计划点击出击");
            actioneevent.LeftClick(GameHwnd, 197 + this.RndPixel(), 244 + this.RndPixel());
            w = true;
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while(w){//当不在出击界面持续等待(出现“远征”按钮则认为已经载入完毕）
                GoExpeditionIcon = actioneevent.GetPixelColor(GameHwnd, 676, 249);
                if(GoExpeditionIcon == "B58B40"){
                    w = false;
                }
                Thread.Sleep(200 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("点击出击成功");

            threadmanager[TeamNO - 2].Sendlistboxmessage("计划点击远征");
            actioneevent.LeftClick(GameHwnd, 676 + this.RndPixel(), 249 + this.RndPixel());//点击远征按钮
            w = true;
            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (w)
            {//当不在出击界面持续等待(出现“远征”按钮则认为已经载入完毕）
                CheckExpedition = actioneevent.GetPixelColor(GameHwnd, 589, 271);
                if (CheckExpedition == "9F9E23")
                {
                    w = false;
                }
                Thread.Sleep(200 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            threadmanager[TeamNO - 2].Sendlistboxmessage("点击远征成功");

            TeamGoExpedition(TeamNO);
            //Thread.Sleep(4000 + this.RndTime());
            //TeamGoExpedition(3);
            //Thread.Sleep(4000 + this.RndTime());
            //TeamGoExpedition(4);

            #region
            ////第二队远征 395,117
            //if(convarible.Team2Go == true)
            //{
            //    if(convarible.Team2_index>0)
            //    {
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队准备开始远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第二舰队准备开始远征");
            //        bool result = MoveToExpMap(convarible.Team2_index);
            //        if (result)
            //        {
            //            Thread.Sleep(200 + this.RndTime());
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队尝试移动到指定地图");
            //            DecideExp = actioneevent.GetPixelColor(GameHwnd, 677, 444);
            //            if (DecideExp == "989463") //远征决定
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队移动成功");
            //                actioneevent.LeftClick(GameHwnd, 677 + this.RndPixel(), 444 + this.RndPixel());
            //                Thread.Sleep(300 + this.RndTime());
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("选择第二舰队");
            //                actioneevent.LeftClick(GameHwnd,395 + this.RndPixel(),117 + this.RndPixel());//=======第二舰队
            //                Thread.Sleep(300 + this.RndTime());
            //                Exping = actioneevent.GetPixelColor(GameHwnd, 624, 445);
            //                if (Exping == "4D4D4D")//正在远征
            //                {
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队正在远征");
            //                    actioneevent.LeftClick(GameHwnd,145 + this.RndPixel(),436 + this.RndPixel());
            //                    Thread.Sleep(300 + this.RndTime());
            //                }
            //                else
            //                {
            //                    actioneevent.LeftClick(GameHwnd, 624 + this.RndPixel(), 445 + this.RndPixel());//远征开始
            //                    for (int i = 1; i == 1; )
            //                    {
            //                        Thread.Sleep(100 + this.RndTime());
            //                        threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            //                        Wait = actioneevent.GetPixelColor(GameHwnd, 272,464);
            //                        if(Wait == "C2DB8D"){
            //                            i = 0;
            //                        }                                   
            //                    }
            //                    threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队开始远征");
            //                    threadmanager[TeamNO - 2].Sendlabelmessage("第二舰队开始远征");
            //                    Thread.Sleep(4000 + this.RndTime());
            //                }
            //            }
            //            else
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队移动失败");
            //            }
            //        }
            //        else
            //        {
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队移动失败");
            //        }
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第二舰队已远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第二舰队已远征");
            //        threadmanager[TeamNO - 2].Sendtimemessage("fleet2finish");
            //        CheckgIfGo = actioneevent.GetPixelColor(GameHwnd, 702, 278);
            //    }
            //}

            //Thread.Sleep(4000 + this.RndTime());

            ////第三队远征 424，117
            //if (convarible.Team3Go == true)
            //{
            //    if (convarible.Team3_index > 0)
            //    {
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队准备开始远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第三舰队准备开始远征");
            //        bool result = MoveToExpMap(convarible.Team3_index);
            //        if (result)
            //        {
            //            Thread.Sleep(200 + this.RndTime());
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队尝试移动到指定地图");
            //            DecideExp = actioneevent.GetPixelColor(GameHwnd, 677, 444);
            //            if (DecideExp == "989463") //远征决定
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队移动成功");
            //                actioneevent.LeftClick(GameHwnd, 677 + this.RndPixel(), 444 + this.RndPixel());
            //                Thread.Sleep(300 + this.RndTime());
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("选择第三舰队");
            //                actioneevent.LeftClick(GameHwnd, 424 + this.RndPixel(), 117 + this.RndPixel());//=======第三舰队
            //                Thread.Sleep(300 + this.RndTime());
            //                Exping = actioneevent.GetPixelColor(GameHwnd, 624, 445);
            //                if (Exping == "4D4D4D")//正在远征
            //                {
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队正在远征");
            //                    actioneevent.LeftClick(GameHwnd, 145 + this.RndPixel(), 436 + this.RndPixel());
            //                    Thread.Sleep(300 + this.RndTime());
            //                }
            //                else
            //                {
            //                    actioneevent.LeftClick(GameHwnd, 624 + this.RndPixel(), 445 + this.RndPixel());//远征开始
            //                    for (int i = 1; i == 1; )
            //                    {
            //                        Thread.Sleep(100 + this.RndTime());
            //                        threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            //                        Wait = actioneevent.GetPixelColor(GameHwnd, 272, 464);
            //                        if (Wait == "C2DB8D")
            //                        {
            //                            i = 0;
            //                        }
            //                    }
            //                    threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队开始远征");
            //                    threadmanager[TeamNO - 2].Sendlabelmessage("第三舰队开始远征");
            //                    Thread.Sleep(4000 + this.RndTime());
            //                }
            //            }
            //            else
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队移动失败");
            //            }
            //        }
            //        else
            //        {
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队移动失败");
            //        }
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第三舰队已远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第三舰队已远征");
            //        threadmanager[TeamNO - 2].Sendtimemessage("fleet3finish");
            //        CheckgIfGo = actioneevent.GetPixelColor(GameHwnd, 702, 278);
            //    }
            //}
            

            //Thread.Sleep(4000 + this.RndTime());

            ////第四队远征 453，117
            //if (convarible.Team4Go == true)
            //{
            //    if (convarible.Team4_index > 0)
            //    {
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队准备开始远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第四舰队准备开始远征");
            //        bool result = MoveToExpMap(convarible.Team4_index);
            //        if (result)
            //        {
            //            Thread.Sleep(200 + this.RndTime());
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队尝试移动到指定地图");
            //            DecideExp = actioneevent.GetPixelColor(GameHwnd, 677, 444);
            //            if (DecideExp == "989463") //远征决定
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队移动成功");
            //                actioneevent.LeftClick(GameHwnd, 677 + this.RndPixel(), 444 + this.RndPixel());
            //                Thread.Sleep(300 + this.RndTime());
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("选择第四舰队");
            //                actioneevent.LeftClick(GameHwnd, 453 + this.RndPixel(), 117 + this.RndPixel());//=======第四舰队
            //                Thread.Sleep(300 + this.RndTime());
            //                Exping = actioneevent.GetPixelColor(GameHwnd, 624, 445);
            //                if (Exping == "4D4D4D")//正在远征
            //                {
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队正在远征");
            //                    actioneevent.LeftClick(GameHwnd, 145 + this.RndPixel(), 436 + this.RndPixel());
            //                    Thread.Sleep(300 + this.RndTime());
            //                }
            //                else
            //                {
            //                    actioneevent.LeftClick(GameHwnd, 624 + this.RndPixel(), 445 + this.RndPixel());//远征开始
            //                    for (int i = 1; i == 1; )
            //                    {
            //                        Thread.Sleep(100 + this.RndTime());
            //                        threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            //                        Wait = actioneevent.GetPixelColor(GameHwnd, 272, 464);
            //                        if (Wait == "C2DB8D")
            //                        {
            //                            i = 0;
            //                        }
            //                    }
            //                    threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            //                    threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队开始远征");
            //                    threadmanager[TeamNO - 2].Sendlabelmessage("第四舰队开始远征");
            //                    Thread.Sleep(4000 + this.RndTime());
            //                }
            //            }
            //            else
            //            {
            //                threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队移动失败");
            //            }
            //        }
            //        else
            //        {
            //            threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队移动失败");
            //        }
            //        threadmanager[TeamNO - 2].Sendlistboxmessage("第四舰队已远征");
            //        threadmanager[TeamNO - 2].Sendlabelmessage("第四舰队已远征");
            //        threadmanager[TeamNO - 2].Sendtimemessage("fleet4finish");
            //        CheckgIfGo = actioneevent.GetPixelColor(GameHwnd, 702, 278);
            //    }
            //}
            #endregion

        }

        private void TeamGoExpedition(int TeamNO)
        {
            string DecideExp;
            string Exping;
            string CheckgIfGo;
            string Wait;

            //舰队远征
            if (convarible.Team_GO[TeamNO - 2] == true)
            {
                if (convarible.Team_index[TeamNO - 2] > 0)
                {
                    threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队准备开始远征");
                    threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO.ToString() + "舰队准备开始远征");
                    bool result = MoveToExpMap(convarible.Team_index[TeamNO - 2]);
                    if (result)
                    {
                        Thread.Sleep(200 + this.RndTime());
                        threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队尝试移动到指定地图");
                        DecideExp = actioneevent.GetPixelColor(GameHwnd, 677, 444);
                        if (DecideExp == "989463") //远征决定
                        {
                            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队移动成功");
                            actioneevent.LeftClick(GameHwnd, 677 + this.RndPixel(), 444 + this.RndPixel());
                            Thread.Sleep(300 + this.RndTime());
                            threadmanager[TeamNO - 2].Sendlistboxmessage("选择第" + TeamNO.ToString()+ "舰队");
                            actioneevent.LeftClick(GameHwnd, 395 + 29 * (TeamNO - 2) + this.RndPixel(), 117 + this.RndPixel());//=======第TeamNO舰队
                            Thread.Sleep(300 + this.RndTime());
                            Exping = actioneevent.GetPixelColor(GameHwnd, 624, 445);
                            if (Exping == "4D4D4D")//正在远征
                            {
                                threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队正在远征");
                                actioneevent.LeftClick(GameHwnd, 145 + this.RndPixel(), 436 + this.RndPixel());
                                Thread.Sleep(300 + this.RndTime());
                            }
                            else
                            {
                                actioneevent.LeftClick(GameHwnd, 624 + this.RndPixel(), 445 + this.RndPixel());//远征开始
                                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                                for (int i = 1; i == 1; )
                                {
                                    Thread.Sleep(100 + this.RndTime());
                                    Wait = actioneevent.GetPixelColor(GameHwnd, 272, 464);
                                    if (Wait == "C2DB8D")
                                    {
                                        i = 0;
                                    }
                                }
                                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                                threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队开始远征");
                                threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO.ToString()+ "舰队开始远征");
                                Thread.Sleep(4000 + this.RndTime());
                            }
                        }
                        else
                        {
                            threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队移动失败");
                        }
                    }
                    else
                    {
                        threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队移动失败");
                    }
                    threadmanager[TeamNO - 2].Sendlistboxmessage("第" + TeamNO.ToString()+ "舰队已远征");
                    threadmanager[TeamNO - 2].Sendlabelmessage("第" + TeamNO.ToString()+ "舰队已远征");
                    threadmanager[TeamNO - 2].Sendtimemessage("fleet" + TeamNO.ToString() + "finish");
                    CheckgIfGo = actioneevent.GetPixelColor(GameHwnd, 702, 278);
                }
            }
        }

        private bool MoveToExpMap(int index)
        {
            bool state = false;
            int Area = 0 ;
            int Loc = 0;
            threadmanager[TeamNO - 2].Sendlistboxmessage("开始移动到远征地图");
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
                    break;
                case 1:
                    actioneevent.LeftClick(GameHwnd, 140 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    state = true;
                    break;
                case 2:
                    actioneevent.LeftClick(GameHwnd, 195 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    state = true;
                    break;
                case 3: 
                    actioneevent.LeftClick(GameHwnd, 260 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    state = true;
                    break;
                case 4: 
                    actioneevent.LeftClick(GameHwnd, 310 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    state = true;
                    break;
                case 5: 
                    actioneevent.LeftClick(GameHwnd, 370 + this.RndPixel(), 440 + this.RndPixel());
                    Thread.Sleep(300 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 325 + this.RndPixel(), 175 + (Loc - 1) * 30 + this.RndPixel());
                    state = true;
                    break;
                default:
                    break;

            }

            return state;
        }

        private double rndint()
        {
            double rnd;
            Thread.Sleep(100);
            Random ra = new Random();
            rnd = ra.NextDouble();
            return rnd;
        }

        private int RndTime()
        {
            int rnd;
            rnd = (int)(rndint() * convarible.RndTimeRange - convarible.RndTimeRange / 2);
            return rnd;
        }

        private int RndPixel()
        {
            int rnd;
            rnd = (int)(rndint() * convarible.RndPixelRange - convarible.RndPixelRange / 2);
            return rnd;
        }

    }
}
