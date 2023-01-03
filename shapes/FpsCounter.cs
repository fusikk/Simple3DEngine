using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shapes
{
    internal class FpsCounter
    {
        private Queue<long> Frames;

        public FpsCounter()
        {
            Frames = new();
        }

        public int FrameCount
        {
            get { return Frames.Count; }
            private set { }
        }

        public void Update()
        {
            if (Frames.Count > 0)
            {
                long snapshot = DateTime.UtcNow.Ticks;

                if (Frames.Peek() - snapshot < 0)
                {
                    Frames.Dequeue();
                }
            }

            AddFrame();
        }

        private void AddFrame()
        {
            long end = DateTime.UtcNow.Ticks + TimeSpan.TicksPerSecond;

            Frames.Enqueue(end);
        }


    }
}
