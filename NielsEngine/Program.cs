using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NielsEngine.Properties;
using SharpRaven;
using SharpRaven.Data;
using Topshelf;
using static NielsEngine.Common;

namespace NielsEngine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Main>(s =>
                {
                    s.ConstructUsing(main => new Main());
                    s.WhenStarted(main => main.Start());
                    s.WhenStopped(main => main.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("NielsEngine");
                x.SetDisplayName("Niels Engine");
                x.SetDescription("Parsing data for use in Niels Data Processing");
            });

           
            var exitCodeVal = (int) Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeVal;
        }
    }
}