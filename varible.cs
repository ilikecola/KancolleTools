using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KancolleMacro
{
    public class convarible
    {
        //舰队设置
        //public static int Team2_index;
        //public static int Team3_index;
        //public static int Team4_index;
        public static int[] Team_index;
        //public static bool Team2Go;
        //public static bool Team3Go;
        //public static bool Team4Go;
        public static bool[] Team_GO;
        //补给设置
        //public static bool Team2Supply;
        //public static bool Team3Supply;
        //public static bool Team4Supply;
        public static bool[] TeamSupply;
        //时间设置
        public static int RndTimeRange;
        public static int RndWaitTimeRange;
        public static int RndPixelRange;
        public static int Overtime;
        public static int[] Expeditiontime;
        //线程标志符
        public static bool[] Threadflag;
        //其他设置
        public static bool NeedSupply;//设置补给flag

        public static void inital()
        {
            Expeditiontime = new int[39]{
                                        15,30,20,50,60+30,40,60,3*60,
                                        4*60,60+30,5*60,8*60,4*60,6*60,12*60,15*60,
                                        45,5*60,6*60,2*60,2*60+20,3*60,4*60,0,
                                        40*60,80*60,20*60,25*60,24*60,48*60,2*60,24*60,
                                        15,30,7*60,9*60,2*60+45,2*60+55,30*60
                                        };
            Team_index = new int[3];
            Team_GO = new bool[3];
            TeamSupply = new bool[3];
            Threadflag = new bool[3]{false,false,false};

        }

        public static void rndTimeModification()
        {
            if (RndTimeRange>100)
            {
                RndTimeRange = 100;
            }
        }

        public static void rndPixelModification()
        {
            if (RndPixelRange > 10)
            {
                RndPixelRange = 10;
            }
        }
    
    }
}
