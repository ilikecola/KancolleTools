using System;
using System.Threading;

namespace KancolleMacro
{
    public class MessageEventArgs : EventArgs
    {
        //定义一个代理参数类,用以传递信息,可以根据自己的需要添加属性
        public String Message; //传递字符串信息
        public MessageEventArgs(string message)
        {
            this.Message = message;
        }
    }

    public class ThreadManager
    {
        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        //定义事件
        public event MessageEventHandler listboxMessageSend;
        public event MessageEventHandler labelMessageSend;
        public event MessageEventHandler timeMessageSend;
        /*
        * 说明:定义事件处理函数,当然这里也可以不用直接在引发事件时调用this.MessageSend(sender, e);
        * 这里的参数要和事件代理的参数一样
        * */
        public void OnlistboxMessageSend(object sender, MessageEventArgs e)
        {
            if (listboxMessageSend != null)
                this.listboxMessageSend(sender, e);
        }

        public void Sendlistboxmessage(String update)
        {
            try
            {
                this.OnlistboxMessageSend(this
                    , new MessageEventArgs(update));
            }
            catch { }
        }

        public void OnlabelMessageSend(object sender, MessageEventArgs e)
        {
            if (labelMessageSend != null)
                this.labelMessageSend(sender, e);
        }

        public void Sendlabelmessage(String update)
        {
            try
            {
                this.OnlabelMessageSend(this
                    , new MessageEventArgs(update));
            }
            catch { }
        }

        public void OntimeMessageSend(object sender, MessageEventArgs e)
        {
            if (timeMessageSend != null)
                this.timeMessageSend(sender, e);
        }

        public void Sendtimemessage(String update)
        {
            try
            {
                this.OntimeMessageSend(this
                    , new MessageEventArgs(update));
            }
            catch { }
        }

        public Thread MainThread;
        //开始线程函数
        public void StartThread(IntPtr GamehWnd, ThreadManager[] threadmanager,int TeamNO)
        {
            ActionThread mythread = new ActionThread(GamehWnd, threadmanager, TeamNO);
            MainThread = new Thread(mythread.MainThread) { IsBackground = true };
            MainThread.Start();
        }
        //结束线程函数
        public void EndThread(int TeamNo)
        {
            if (MainThread != null)
            {
                if (MainThread.IsAlive)
                {
                    MainThread.Abort();
                } 
            }
        }
    }
}
