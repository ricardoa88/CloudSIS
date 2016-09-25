using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwitchTest
{
    public enum StatusSwitch
    {
        CONNECTED=1,
        DISCONNECTED = 2,
    }
    public class AlarmEventArgs
    {
        public int Port { get; set; }
        public StatusSwitch Status { get; set; }

        public AlarmEventArgs(int port, StatusSwitch status)
        {
            this.Port = port;
            this.Status = status;
        }
    }

    public class SwitchSnmp
    {
        public string Ip { get; set; }

        public StatusSwitch[] Status { get; set; }
        public int[] StatusFailCount { get; set; }


        public int SamplingTime { get; set; }

        public int AlertCountMax { get; set; }

        public delegate void AlarmEventHandler(object sender, AlarmEventArgs e);
        public event AlarmEventHandler Alarm;

        private bool IsCheckStatus { get; set; }
        private Thread threadCheckStatus { get; set; }

        public SwitchSnmp()
        {
            Status = new StatusSwitch[11];
            StatusFailCount = new int[11];
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
            Dictionary<String, Dictionary<uint, AsnType>> result = new Dictionary<String, Dictionary<uint, AsnType>>();

            List<uint> tableColumns = new List<uint>();
            
            AgentParameters param = new AgentParameters(SnmpVersion.Ver2, new OctetString("public"));
            IpAddress peer = new IpAddress(Ip);
           
            UdpTarget target = new UdpTarget((IPAddress)peer);
            
            Oid startOid = new Oid("1.3.6.1.2.1.2.2.1.8");
            
            Pdu bulkPdu = Pdu.GetBulkPdu();
            bulkPdu.VbList.Add(startOid);
            bulkPdu.NonRepeaters = 0;
            bulkPdu.MaxRepetitions = 100;
            Oid curOid = (Oid)startOid.Clone();
            while (startOid.IsRootOf(curOid))
            {
                SnmpPacket res = null;
                try
                {
                    res = target.Request(bulkPdu, param);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Request failed: {0}", ex.Message);
                    target.Close();

                    if (StatusFailCount[10] < AlertCountMax)
                    {

                        StatusFailCount[10]++;
                    }
                    else
                    {
                        if (StatusFailCount[10] == AlertCountMax)
                        {
                            StatusFailCount[10]++;
                            Alarm?.Invoke(this, new AlarmEventArgs(10, StatusSwitch.DISCONNECTED));
                        }
                    }
                    break;
                }
                // For GetBulk request response has to be version 2
                if (res.Version != SnmpVersion.Ver2)
                {
                    Console.WriteLine("Received wrong SNMP version response packet.");
                    target.Close();
                }
                // Check if there is an agent error returned in the reply
                if (res.Pdu.ErrorStatus != 0)
                {
                    Console.WriteLine("SNMP agent returned error {0} for request Vb index {1}",
                                      res.Pdu.ErrorStatus, res.Pdu.ErrorIndex);
                    target.Close();
                }
                // Go through the VbList and check all replies
                VbCollection lista = res.Pdu.VbList;
                foreach (Vb v in lista)
                {
                    curOid = (Oid)v.Oid.Clone();
                    
                    if (startOid.IsRootOf(v.Oid))
                    {
                        uint[] childOids = Oid.GetChildIdentifiers(startOid, v.Oid);
                        uint[] instance = new uint[childOids.Length - 1];
                        Array.Copy(childOids, 1, instance, 0, childOids.Length - 1);
                        String strInst = InstanceToString(instance);
                        uint column = childOids[0];
                        if (!tableColumns.Contains(column))
                            tableColumns.Add(column);
                        if (result.ContainsKey(strInst))
                        {
                            result[strInst][column] = (AsnType)v.Value.Clone();
                        }
                        else
                        {
                            result[strInst] = new Dictionary<uint, AsnType>();
                            result[strInst][column] = (AsnType)v.Value.Clone();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                // If last received OID is within the table, build next request
                if (startOid.IsRootOf(curOid))
                {
                    bulkPdu.VbList.Clear();
                    bulkPdu.VbList.Add(curOid);
                    bulkPdu.NonRepeaters = 0;
                    bulkPdu.MaxRepetitions = 100;
                }
            }
            target.Close();
            if (result.Count <= 0)
            {
                Console.WriteLine("No results returned.\n");
            }
            else
            {
                
                foreach (KeyValuePair<string, Dictionary<uint, AsnType>> kvp in result)
                {
                    foreach (uint column in tableColumns)
                    {
                        if (kvp.Value.ContainsKey(column)&& column<=10)
                        {
                            Status[column-1]=(StatusSwitch) int.Parse(kvp.Value[column].ToString());
                            if (Status[column - 1]==StatusSwitch.DISCONNECTED)
                            {
                                
                                if(StatusFailCount[column - 1] < AlertCountMax)
                                {
                                   
                                    StatusFailCount[column - 1]++;
                                }
                                else
                                {
                                    if (StatusFailCount[column - 1] == AlertCountMax)
                                    {
                                        StatusFailCount[column - 1]++;
                                        Alarm?.Invoke(this, new AlarmEventArgs((int)column, StatusSwitch.DISCONNECTED));
                                    }
                                }
                            }
                            else
                            {
                                if (StatusFailCount[column - 1] > AlertCountMax)
                                {
                                    Alarm?.Invoke(this, new AlarmEventArgs((int)column, StatusSwitch.CONNECTED));
                                }
                                StatusFailCount[column - 1] = 0;
                            }
                        }
                        
                    }
                }
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
