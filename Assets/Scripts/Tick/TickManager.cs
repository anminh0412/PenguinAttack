using System.Collections.Generic;
using Other;
using Plugins.Tick;

namespace Tick
{
    public class TickManager : MonoSingleton<TickManager>
    {
        private readonly List<ITickable>      tickables      = new();
        private readonly List<IFixedTickable> fixedTickables = new();
        private readonly List<ILateTickable>  lateTickables  = new();

        private void Update()
        {
            Tick();
        }

        private void FixedUpdate()
        {
            FixedTick();
        }

        private void LateUpdate()
        {
            LateTick();
        }

        public void Add(object obj)
        {
            if (obj is ITickable tick) this.tickables.Add(tick);
            if (obj is IFixedTickable fixedTick) this.fixedTickables.Add(fixedTick);
            if (obj is ILateTickable lateTick) this.lateTickables.Add(lateTick);
        }

        public void Remove(object obj)
        {
            if (obj is ITickable tick) this.tickables.Remove(tick);
            if (obj is IFixedTickable fixedTick) this.fixedTickables.Remove(fixedTick);
            if (obj is ILateTickable lateTick) this.lateTickables.Remove(lateTick);
        }

        public void Tick()
        {
            for (var i = this.tickables.Count - 1; i >= 0; i--)
            {
                var t = this.tickables[i];
                if (t == null)
                {
                    this.tickables.RemoveAt(i);
                    continue;
                }

                t.Tick();
            }
        }

        public void FixedTick()
        {
            for (var i = this.fixedTickables.Count - 1; i >= 0; i--)
            {
                var t = this.fixedTickables[i];
                if (t == null)
                {
                    this.fixedTickables.RemoveAt(i);
                    continue;
                }

                t.FixedTick();
            }
        }

        public void LateTick()
        {
            for (var i = this.lateTickables.Count - 1; i >= 0; i--)
            {
                var t = this.lateTickables[i];
                if (t == null)
                {
                    this.lateTickables.RemoveAt(i);
                    continue;
                }

                t.LateTick();
            }
        }
    }

}