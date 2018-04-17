using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace AkkaWebJob
{
    public class TimeScheduledWebJob // Other class names are available.
    {
        public void DoSomethingOnATimer([TimerTrigger("*/10 * * * * *", RunOnStartup = false)] TimerInfo timerInfo, TextWriter log)
        {
            System.Console.WriteLine("Doing it!");
            log.WriteLine("Executing function");
        }
    }

}