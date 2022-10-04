using System;
using System.Collections.Generic;

namespace RelEcs
{
    internal class Trigger<T>
    {
        internal T Value;
    }

    internal class SystemList
    {
        public readonly List<Type> List;
        public SystemList() => List = ListPool<Type>.Get();
    }

    internal class LifeTime
    {
        public int Value;
    }

    internal class TriggerLifeTimeSystem : ISystem
    {
        public World World { get; set; }

        public void Run()
        {
            var query = World.Query<Entity, SystemList, LifeTime>();
            foreach (var (entity, systemList, lifeTime) in query)
            {
                lifeTime.Value++;

                if (lifeTime.Value < 2) return;

                ListPool<Type>.Add(systemList.List);
                World.Despawn(entity);
            }
        }
    }
}