using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using SwitchTest;

namespace WebLayer.Hubs
{
    public class SwitchAndCams : Hub
    {
        public static SwitchSnmp switch1;

        public static DevicePing Cam1;
        public static DevicePing Cam2;
        public static DevicePing Cam3;
        

        public SwitchAndCams()
        {
            if (switch1==null)
            {
                switch1 = new SwitchSnmp();
                switch1.Ip = "192.168.0.101";
                switch1.Alarm += SwitchAlarm;
                switch1.AlertCountMax = 0;
                switch1.Start();
            }

            if (Cam1 == null)
            {
                Cam1 = new DevicePing();
                Cam1.Ip = "172.19.194.96";
                Cam1.Descripcion = "Camara PTZ  96";
                Cam1.Alarm += cam_Alarm;
                Cam1.AlertCountMax = 1;
                
                Cam1.Start();
            }
            if (Cam2 == null)
            {
                Cam2 = new DevicePing();
                Cam2.Ip = "192.168.0.9";
                Cam2.Descripcion = "Camara fija 96-1";
                //Cam2.Ip = "172.19.194.185";
                Cam2.Alarm += cam_Alarm;
                Cam2.AlertCountMax = 1;
               
                Cam2.Start();
            }
            if (Cam3 == null)
            {
                Cam3 = new DevicePing();
                Cam3.Ip = "172.19.194.186";
                Cam3.Descripcion = "Camara fija 96-2";
                Cam3.Alarm += cam_Alarm;
                Cam3.AlertCountMax = 1;
               
                Cam3.Start();
            }
        }

        private void cam_Alarm(object sender, AlarmEventArgs e)
        {
            DevicePing sender_ = (DevicePing)sender;
            Clients.All.broadcastMessageCam(sender_.Descripcion, e.Status);
        }

        private void SwitchAlarm(object sender, AlarmEventArgs e)
        {
            Console.WriteLine("puerto: " + e.Port + " estado: " + e.Status);
            Clients.All.broadcastMessage(e.Port, e.Status);
        }

        public override Task OnConnected()
        {
            try
            {
               // Clients.All.broadcastMessage("asasd", "adasdas");
            }
            catch (Exception ex)
            {
                
            }

            return base.OnConnected();
        }
        
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }
}