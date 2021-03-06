using System;
using System.Threading;

namespace CronNET
{
    public interface ICronJob
    {
        void execute(DateTime date_time);
        void abort();
    }

    public class CronJob : ICronJob
    {
        private readonly ICronSchedule _cron_schedule = new CronSchedule();
        private readonly ParameterizedThreadStart _thread_start;
        private Thread _thread = null;
        private object _arg = null;


        public CronJob(string schedule, ParameterizedThreadStart thread_start,object arg)
        {
            _cron_schedule = new CronSchedule(schedule);
            _thread_start = thread_start;
            _thread = new Thread(thread_start);
            _arg = arg;
        }

        private object _lock = new object();
        public void execute(DateTime date_time)
        {
            lock (_lock)
            {
                if (!_cron_schedule.isTime(date_time))
                    return;

                if (_thread.ThreadState == ThreadState.Running)
                    return;

                _thread = new Thread(_thread_start);
                
                _thread.Start(_arg);
            }
        }

        public void abort()
        {
          _thread.Abort();  
        }

    }
}
