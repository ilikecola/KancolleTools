﻿using System;
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
                bool a = true;
                while (a)
                {
                    Thread.Sleep(500 + this.RndTime());
                    if (TeamNO == 2)
                    {
                        a = convarible.Threadflag[1] || convarible.Threadflag[2];
                        if (a == false) break;
                    }
                    else if (TeamNO == 3)
                    {
                        a = convarible.Threadflag[0] || convarible.Threadflag[2];
                        if (a == false) break;
                    }
                    else if (TeamNO == 4)
                    {
                        a = convarible.Threadflag[0] || convarible.Threadflag[1];
                        if (a == false) break;
                    }
                }
                convarible.Threadflag[TeamNO - 2] = true;
                threadmanager[TeamNO - 2].Sendlistboxmessage("线程" + (TeamNO - 1) + "已启动");
                threadmanager[TeamNO - 2].Sendlabelmessage("线程" + (TeamNO - 1) + "已启动");
                convarible.TeamSupply[TeamNO - 2] = true;

                Gohome();
                threadmanager[TeamNO - 2].Sendlistboxmessage("准备开始补给");
                //while(convarible.TeamSupply[0] == true || convarible.TeamSupply[1] == true || convarible.TeamSupply[2] == true)
                //{
                //    Thread.Sleep(200 + this.RndTime());
                //    GoSupply();
                //}
                while (convarible.TeamSupply[TeamNO - 2] == true)
                {
                    Thread.Sleep(200 + this.RndTime());
                    GoSupply();
                }
                GoExpedition();
                convarible.Threadflag[TeamNO - 2] = false;
            }
            catch (ThreadAbortException e)
            {

                threadmanager[TeamNO - 2].Sendlistboxmessage("线程被中止");
                threadmanager[TeamNO - 2].Sendlabelmessage("线程被中止");
                threadmanager[TeamNO - 2].Sendlistboxmessage("Exception message:" + e.Message);
            }
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
                    Thread.Sleep(110 + this.RndTime());
                }else
                {
                    j = 1;
                }

                Thread.Sleep(110 + this.RndTime());

                //2队同时归来的情况
                SecFlag = actioneevent.GetPixelColor(GameHwnd, 521, 28);
                if(SecFlag == "A2B226"){
                    i = 0;
                    hasExpGet = false;
                    actioneevent.LeftClick(GameHwnd, 521 + this.RndPixel(), 28 + this.RndPixel());
                    Thread.Sleep(110 + this.RndTime());
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
                if(CheckApply == "007FC9")
                {
                    b = false;
                }
                Thread.Sleep(200 + this.RndTime());
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");

            threadmanager[TeamNO - 2].Sendlistboxmessage("进入补给成功");
            ////补给1队
            //actioneevent.LeftClick(GameHwnd, 145 + this.RndPixel(), 118 + this.RndPixel());
            //ApplySupply(1);
            //threadmanager[TeamNO - 2].Sendlistboxmessage("补给1队完成");


            //Team1 145,118   
            //Team2 182,118
            //Team3 
            //
            //
            //补给2队           
            if(convarible.Team_GO[TeamNO - 2] == true && convarible.TeamSupply[TeamNO - 2] == true)
            {
                string current_1 = null;
                string current_2= null;
                string changecurrent_1 = null;
                string changecurrent_2 = null;
                string previous_1 = null;
                string previous_2 = null;
                string changeprevious_1 = null;
                string changeprevious_2 = null;

                Thread.Sleep(250 + this.RndTime());
                previous_1 = actioneevent.GetPixelColor(GameHwnd, 145, 118);
                previous_2 = actioneevent.GetPixelColor(GameHwnd, 152, 118);
                changeprevious_1 = actioneevent.GetPixelColor(GameHwnd, 145 + 30 * (TeamNO - 1), 118);
                changeprevious_2 = actioneevent.GetPixelColor(GameHwnd, 152 + 30 * (TeamNO - 1), 118);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Previous_1:" + previous_1);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Previous_2:" + previous_1);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Changeprevious_1:" + changeprevious_1);
                threadmanager[TeamNO - 2].Sendlistboxmessage("Changeprevious_2:" + changeprevious_2);
                threadmanager[TeamNO - 2].Sendtimemessage("overstart");
                do
                {
                    actioneevent.LeftClick(GameHwnd, 145 + 30 * (TeamNO - 1) + (this.RndPixel() / 5), 118 + (this.RndPixel() / 5));
                    current_1 = actioneevent.GetPixelColor(GameHwnd, 145, 118);
                    current_2 = actioneevent.GetPixelColor(GameHwnd, 152, 118);
                    changecurrent_1 = actioneevent.GetPixelColor(GameHwnd, 145 + 30 * (TeamNO - 1), 118);
                    changecurrent_2 = actioneevent.GetPixelColor(GameHwnd, 152 + 30 * (TeamNO - 1), 118);
                    threadmanager[TeamNO - 2].Sendlistboxmessage("Current_1:" + current_1);
                    threadmanager[TeamNO - 2].Sendlistboxmessage("Current_2:" + current_2);
                    threadmanager[TeamNO - 2].Sendlistboxmessage("Changecurrent_1:" + changecurrent_1);
                    threadmanager[TeamNO - 2].Sendlistboxmessage("Changecurrent_2:" + changecurrent_2);
                } while (current_1 == previous_1 
                        || current_2 == previous_2 
                        || changecurrent_1 == changeprevious_1 
                        || changecurrent_2 == changeprevious_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("进入补给" + TeamNO + "队完成");
                threadmanager[TeamNO - 2].Sendtimemessage("overstop");
                ApplySupply(TeamNO);
                threadmanager[TeamNO - 2].Sendlistboxmessage("补给"+ TeamNO + "队完成");
            }
           
            threadmanager[TeamNO - 2].Sendlistboxmessage("补给完成");
            threadmanager[TeamNO - 2].Sendlabelmessage("补给完成");
            Gohome();
        }

        private void ApplySupply(int number){
            int i;
            
            string _selectall_1 = null;
            string _selectall_2 = null;
            string _select1_1 = null;
            string _select1_2 = null;
            string _select2_1 = null;
            string _select2_2 = null;
            string _select3_1 = null;
            string _select3_2 = null;
            string _select4_1 = null;
            string _select4_2 = null;
            string _select5_1 = null;
            string _select5_2 = null;
            string _select6_1 = null;
            string _select6_2 = null;

            string SelectAll_1 = null;
            string SelectAll_2 = null;
            string select1_1 = null;
            string select1_2 = null;
            string select2_1 = null;
            string select2_2 = null;
            string select3_1 = null;
            string select3_2 = null;
            string select4_1 = null;
            string select4_2 = null;
            string select5_1 = null;
            string select5_2 = null;
            string select6_1 = null;
            string select6_2 = null;


            //Preload
            actioneevent.MOUSEMOVE(GameHwnd, 20 + this.RndPixel(), 20 + this.RndPixel());

            i = 0;
            _selectall_1 = actioneevent.GetPixelColor(GameHwnd, 120, 120);
            _select1_1 = actioneevent.GetPixelColor(GameHwnd, 120, 165);
            _select2_1 = actioneevent.GetPixelColor(GameHwnd, 120, 220);
            _select3_1 = actioneevent.GetPixelColor(GameHwnd, 120, 270);
            _select4_1 = actioneevent.GetPixelColor(GameHwnd, 120, 320);
            _select5_1 = actioneevent.GetPixelColor(GameHwnd, 120, 375);
            _select6_1 = actioneevent.GetPixelColor(GameHwnd, 120, 425);

            _selectall_2 = actioneevent.GetPixelColor(GameHwnd, 115, 120);
            _select1_2 = actioneevent.GetPixelColor(GameHwnd, 115, 165);
            _select2_2 = actioneevent.GetPixelColor(GameHwnd, 115, 220);
            _select3_2 = actioneevent.GetPixelColor(GameHwnd, 115, 270);
            _select4_2 = actioneevent.GetPixelColor(GameHwnd, 115, 320);
            _select5_2 = actioneevent.GetPixelColor(GameHwnd, 115, 375);
            _select6_2 = actioneevent.GetPixelColor(GameHwnd, 115, 425);

            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_1:" + _selectall_1 + " " + _selectall_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_2:" + _select1_1 + " " + _select1_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_3:" + _select2_1 + " " + _select2_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_4:" + _select3_1 + " " + _select3_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_5:" + _select4_1 + " " + _select4_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_6:" + _select5_1 + " " + _select5_2);
            threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointPrevious_7:" + _select6_1 + " " + _select6_2);

            //判断有多少船需要补给
            int num = -1;
            if (_select1_1 == "C1D1E2" 
                && _select2_1 == "B9C8DA"
                && _select3_1 == "B8CCDE"
                && _select4_1 == "C1D0E0"
                && (_select5_1 == "D7E3EB" || _select5_1 == "C1D2E2")
                && (_select6_1 == "CDDBE3" || _select6_1 == "BDCFE0")
                && _select1_2 == "BED0E2"
                && _select2_2 == "BBCBDB"
                && _select3_2 == "B9CFE0"
                && _select4_2 == "BBD2E0"
                && (_select5_2 == "D3E2E5" || _select5_2 == "C1D2E1")
                && (_select6_2 == "C8DCE1" || _select6_2 == "BACEDE"))
            {
                threadmanager[TeamNO - 2].Sendlistboxmessage(TeamNO + "队已补给");
                i = 1;
                convarible.TeamSupply[number - 2] = false;
            }
            else if (_select2_1 != _select1_1)
            {
                _select2_1 = _select2_1 + "pass";
                _select3_1 = _select3_1 + "pass";
                _select4_1 = _select4_1 + "pass";
                _select5_1 = _select5_1 + "pass";
                _select6_1 = _select6_1 + "pass";
                num = 1;
            }
            else if (_select3_1 != _select2_1)
            {
                _select3_1 = _select3_1 + "pass";
                _select4_1 = _select4_1 + "pass";
                _select5_1 = _select5_1 + "pass";
                _select6_1 = _select6_1 + "pass";
                num = 2;
            }
            else if (_select4_1 != _select3_1)
            {
                _select4_1 = _select4_1 + "pass";
                _select5_1 = _select5_1 + "pass";
                _select6_1 = _select6_1 + "pass";
                num = 3;
            }
            else if (_select5_1 != _select4_1)
            {
                _select5_1 = _select5_1 + "pass";
                _select6_1 = _select6_1 + "pass";
                num = 4;
            }
            else if (_select6_1 != _select5_1)
            {
                _select6_1 = _select6_1 + "pass";
                num = 5;

            }
            else
            {
                num = 6;
            }

            threadmanager[TeamNO - 2].Sendtimemessage("overstart");
            while (i == 0 )
            {
                //Thread.Sleep(100 + this.RndTime());
                //actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                //SelectAll_1 = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                //SelectAll_2 = actioneevent.GetPixelColor(GameHwnd, 115, 120);
                //threadmanager[TeamNO - 2].Sendlistboxmessage("ClickCheckPoint:" + SelectAll_1 + " " + SelectAll_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("ClickCheckPointPrevious:" + _selectall_1 + " " + _selectall_2);
                do
                {
                    Thread.Sleep(110 + this.RndTime());
                    actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                    Thread.Sleep(110 + this.RndTime());
                    actioneevent.LeftClick(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                    threadmanager[TeamNO - 2].Sendlistboxmessage(TeamNO + "队正在补给");
                    Thread.Sleep(1000 + this.RndTime());

                    //First Check
                    actioneevent.MOUSEMOVE(GameHwnd, 20 + this.RndPixel(), 20 + this.RndPixel());
                    Thread.Sleep(110 + this.RndTime());
                    actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                    SelectAll_1 = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                    SelectAll_2 = actioneevent.GetPixelColor(GameHwnd, 115, 120);
                    threadmanager[TeamNO - 2].Sendlistboxmessage("ClickCheckPointCurrent:" + SelectAll_1 + " " + SelectAll_2);
                } while(SelectAll_1 != _selectall_1 && SelectAll_2 != _selectall_2);

                actioneevent.MOUSEMOVE(GameHwnd, 20 + this.RndPixel(), 20 + this.RndPixel());
                Thread.Sleep(110 + this.RndTime());
                _selectall_1 = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                actioneevent.MOUSEMOVE(GameHwnd, 120 + (this.RndPixel() / 2), 120 + (this.RndPixel() / 2));
                Thread.Sleep(250 + this.RndTime());

                //check
                SelectAll_1 = actioneevent.GetPixelColor(GameHwnd, 120, 120);
                select1_1 = actioneevent.GetPixelColor(GameHwnd, 120, 165);
                select2_1 = actioneevent.GetPixelColor(GameHwnd, 120, 220);
                select3_1 = actioneevent.GetPixelColor(GameHwnd, 120, 270);
                select4_1 = actioneevent.GetPixelColor(GameHwnd, 120, 320);
                select5_1 = actioneevent.GetPixelColor(GameHwnd, 120, 375);
                select6_1 = actioneevent.GetPixelColor(GameHwnd, 120, 425);

                SelectAll_2 = actioneevent.GetPixelColor(GameHwnd, 115, 120);
                select1_2 = actioneevent.GetPixelColor(GameHwnd, 115, 165);
                select2_2 = actioneevent.GetPixelColor(GameHwnd, 115, 220);
                select3_2 = actioneevent.GetPixelColor(GameHwnd, 115, 270);
                select4_2 = actioneevent.GetPixelColor(GameHwnd, 115, 320);
                select5_2 = actioneevent.GetPixelColor(GameHwnd, 115, 375);
                select6_2 = actioneevent.GetPixelColor(GameHwnd, 115, 425);

                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_1:" + SelectAll_1 + " " + SelectAll_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_2:" + select1_1 + " " + select1_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_3:" + select2_1 + " " + select2_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_4:" + select3_1 + " " + select3_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_5:" + select4_1 + " " + select4_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_6:" + select5_1 + " " + select5_2);
                threadmanager[TeamNO - 2].Sendlistboxmessage("CheckPointCurrent_7:" + select6_1 + " " + select6_2);

                //if (select1_1 == "C1D1E2"
                //&& select2_1 == "B9C8DA"
                //&& select3_1 == "B8CCDE"
                //&& select4_1 == "C1D0E0"
                //&& (select5_1 == "D7E3EB" || select5_1 == "C1D2E2")
                //&& (select6_1 == "CDDBE3" || select6_1 == "BDCFE0")
                //&& select1_2 == "BED0E2"
                //&& select2_2 == "BBCBDB"
                //&& select3_2 == "B9CFE0"
                //&& select4_2 == "BBD2E0"
                //&& (select5_2 == "D3E2E5" || select5_2 == "C1D2E1")
                //&& (select6_2 == "C8DCE1" || select6_2 == "BACEDE"))
                //{
                //    i = 1;
                //    convarible.TeamSupply[number - 2] = false;
                //    break;
                //}

                switch (num)
                {
                    case 1:
                        if (SelectAll_1 == _selectall_1 
                            && SelectAll_2 == _selectall_2 
                            && select1_1 != _select1_1 
                            && select1_2 != _select1_2)
                        {
                            i = 1;
                        }
                        break;
                    case 2:
                        if (SelectAll_1 == _selectall_1 
                            && select1_1 != _select1_1
                            && select2_1 != _select2_1
                            && SelectAll_2 == _selectall_2
                            && select1_2 != _select1_2
                            && select2_2 != _select2_2)
                        {
                            i = 1;
                        }
                        break;
                    case 3:
                        if (SelectAll_1 == _selectall_1
                            && select1_1 != _select1_1
                            && select2_1 != _select2_1
                            && select3_1 != _select3_1
                            && SelectAll_2 == _selectall_2
                            && select1_2 != _select1_2
                            && select2_2 != _select2_2
                            && select3_2 != _select3_2)
                        {
                            i = 1;
                        }
                        break;
                    case 4:
                        if (SelectAll_1 == _selectall_1
                            && select1_1 != _select1_1
                            && select2_1 != _select2_1
                            && select3_1 != _select3_1
                            && select4_1 != _select4_1
                            && SelectAll_2 == _selectall_2
                            && select1_2 != _select1_2
                            && select2_2 != _select2_2
                            && select3_2 != _select3_2
                            && select4_2 != _select4_2)
                        {
                            i = 1;
                        }
                        break;
                    case 5:
                        if (SelectAll_1 == _selectall_1
                            && select1_1 != _select1_1
                            && select2_1 != _select2_1
                            && select3_1 != _select3_1
                            && select4_1 != _select4_1
                            && select5_1 != _select5_1
                            && SelectAll_2 == _selectall_2
                            && select1_2 != _select1_2
                            && select2_2 != _select2_2
                            && select3_2 != _select3_2
                            && select4_2 != _select4_2
                            && select5_2 != _select5_2)
                        {
                            i = 1;
                        }
                        break;
                    case 6:
                        if (SelectAll_1 == _selectall_1
                            && select1_1 != _select1_1
                            && select2_1 != _select2_1
                            && select3_1 != _select3_1
                            && select4_1 != _select4_1
                            && select5_1 != _select5_1
                            && select6_1 != _select6_1
                            && SelectAll_2 == _selectall_2
                            && select1_2 != _select1_2
                            && select2_2 != _select2_2
                            && select3_2 != _select3_2
                            && select4_2 != _select4_2
                            && select5_2 != _select5_2
                            && select6_2 != _select6_2)
                        {
                            i = 1;
                        }
                        break;
                }
                //if (SelectAll == _selectall ||(//120,120
                //    actioneevent.GetPixelColor(GameHwnd,120,165) != _select1 //120,165
                //    && actioneevent.GetPixelColor(GameHwnd,120,220) != _select2//120,220
                //    && actioneevent.GetPixelColor(GameHwnd,120,270) != _select3//120,270
                //    && actioneevent.GetPixelColor(GameHwnd, 120, 320) != _select4//120,320
                //    && actioneevent.GetPixelColor(GameHwnd, 120, 375) != _select5//120,375
                //    && actioneevent.GetPixelColor(GameHwnd, 120, 425) != _select6))//120,425
                //{
                //    i = 1;
                //}
            }
            threadmanager[TeamNO - 2].Sendtimemessage("overstop");
            convarible.TeamSupply[number - 2] = false;
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
                                    Thread.Sleep(110 + this.RndTime());
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
            rnd = ra.Next(convarible.RndTimeRange);
            //rnd = (int)(rndint() * convarible.RndTimeRange - convarible.RndTimeRange / 2);
            return rnd;
        }

        private int RndPixel()
        {
            Random ra = new Random();
            int rnd;
            rnd = ra.Next(convarible.RndPixelRange);
            //rnd = (int)(rndint() * convarible.RndPixelRange - convarible.RndPixelRange / 2);
            return rnd;
        }

    }
}
