using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwitchTest
{

    public class DevicePing
    {
        public string Ip { get; set; }

        public string Descripcion { get; set; }

        public StatusSwitch Status { get; set; }
        public int StatusFailCount { get; set; }
        public int AlertCountMax { get; set; }

        public int SamplingTime { get; set; }

        public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);
        public event AlarmEventHandler Alarm;

        Ping pingSender = new Ping();
        PingOptions options = new PingOptions();

        private bool IsCheckStatus { get; set; }
        private Thread threadCheckStatus { get; set; }

        public DevicePing()
        {
            Status = new StatusSwitch();
            StatusFailCount = 0;
            SamplingTime = 1000;
        }

        public void Start()
        {
            threadCheckStatus = new Thread(CheckStatus);
            IsCheckStatus = true;
            threadCheckStatus.Start();
        }

        public void Stop()
        {
            IsCheckStatus = false;
        }

        public void CheckStatus()
        {
            while (IsCheckStatus)
            {
                GetPortStatus();
                Thread.Sleep(SamplingTime);
            }
        }
        public void GetPortStatus()
        {
            
            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(Ip, timeout, buffer, options);
            if (reply.Status != IPStatus.Success)
            {
                if (StatusFailCount < AlertCountMax)
                {

                    StatusFailCount++;
                }
                else
                {
                    if (StatusFailCount == AlertCountMax)
                    {
                        StatusFailCount++;
                        Alarm?.Invoke(this, new AlarmEventArgs(0, StatusSwitch.DISCONNECTED));
                    }
                }
            }
            else
            {
                if (StatusFailCount > AlertCountMax)
                {
                    Alarm?.Invoke(this, new AlarmEventArgs(0, StatusSwitch.CONNECTED));
                }
                StatusFailCount = 0;
            }
        }
        public static string InstanceToString(uint[] instance)
        {
            StringBuilder str = new StringBuilder();
            foreach (uint v in instance)
            {
                if (str.Length == 0)
                    str.Append(v);
                else
                    str.AppendFormat(".{0}", v);
            }
            return str.ToString();
        }
    }

}
