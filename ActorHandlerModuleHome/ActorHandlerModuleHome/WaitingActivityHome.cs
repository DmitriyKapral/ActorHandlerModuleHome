using System;

using ActorModule;


namespace ActorHandlerModuleHome
{
    public class WaitingActivityHome : IActivity
    {
        public int Priority { get; set; } = 1;
        public TimeInterval TimeHome { get; }
        public WaitingActivityHome(TimeInterval timeHome)
        {
            TimeHome = timeHome;
        }
        public bool Update(Actor actor, double deltaTime)
        {
            string start = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine(start);
            Console.WriteLine(TimeHome.End.ToString());
            if (start == TimeHome.End.ToString())
            {
                Console.WriteLine("end");
                return true;
            }
            Console.WriteLine("hello");
            return false;
        }
    }
}
