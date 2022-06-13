using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NielsEngine.Properties;
using static NielsEngine.Common;

namespace NielsEngine
{
    public class Main
    {
        private readonly Timer _timer;
        private bool _skipper = false;

        public Main()
        {
            _timer = new Timer(1000){AutoReset = true};
            _timer.Elapsed+=TimerOnElapsed;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_skipper) return;
            try
            {
                _skipper = true;
                refetch:
                if (File.Exists(_confPath))
                {
                    Security.DecryptFile(_confPath, Settings.Default.accesskey);
                    if (!File.Exists(_confPath))
                        goto refetch;
                    var dat = File.ReadAllText(_confPath);
                    tracker = JObject.Parse(dat);
                }
                else
                {
                    IniTmp();
                    tracker = new JObject
                    {
                        ["key"] = Security.Value(),
                        ["name"] = "Supreme Stores",
                        ["branch"] = new JObject
                            {
                                ["code"] = "01",
                                ["name"] = "MAIN BRANCH"
                            },
                        ["database"] = new JObject
                        {
                            ["db"] = "pos_backoff",
                            ["username"] = "support",
                            ["password"] = "$w0Rk$",
                            ["host"] = "127.0.0.1",
                            ["port"] = "5432",
                        }
                    };
                }
                tracker["last_fetch"] = DateTime.Now;


                DataParser dp = new DataParser();
                dp.PrepareSalesData();
                var wk = tracker.ContainsKey("last_week") ? tracker["last_week"] : "Unknown";
                string[] lines = new string[] { $"Data Checking at {DateTime.Now} Current: {wk}" };
                File.AppendAllLines($"{_commonAppFolder}\\push_logs_{DateTime.Now:yy_MM_dd}.txt", lines);
                tracker["sleep_time"] = DateTime.Now;
                var str = JsonConvert.SerializeObject(tracker);
                File.WriteAllText(_confPath, str);
                Security.EncryptFile(_confPath, Settings.Default.accesskey);
                _skipper = false;
            }
            catch (Exception e)
            {
                log(e);
                _skipper = false;
            }
        }

        public void Start()
        {
            log("Service Started");
            _timer.Start();
        }
        public void Stop()
        {
            log("Service Stopped");
            _timer.Stop();
        }

    }
}
