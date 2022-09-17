using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RelEcs
{
    public static class SystemExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(this ISystem aSystem, World world)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            aSystem.World = world;
            aSystem.Run();

            stopWatch.Stop();
            world.SystemExecutionTimes.Add((aSystem.GetType(), stopWatch.Elapsed));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityBuilder Spawn(this ISystem system)
        {
            return new EntityBuilder(system.World, system.World.Spawn().Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityBuilder On(this ISystem system, Entity entity)
        {
            return new EntityBuilder(system.World, entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Despawn(this ISystem system, Entity entity)
        {
            system.World.Despawn(entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlive(this ISystem system, Entity entity)
        {
            return entity != null && system.World.IsAlive(entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this ISystem system, Entity entity) where T : class
        {
            return system.World.GetComponent<T>(entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this ISystem system, Entity entity, Entity target) where T : class
        {
            return system.World.GetComponent<T>(entity.Identity, target.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this ISystem system, Entity entity, Type type) where T : class
        {
            var typeIdentity = system.World.GetTypeIdentity(type);
            return system.World.GetComponent<T>(entity.Identity, typeIdentity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this ISystem system, Entity entity, out T component) where T : class
        {
            return system.TryGetComponent(entity, Entity.None, out component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this ISystem system, Entity entity, Entity target, out T component)
            where T : class
        {
            if (system.World.HasComponent<T>(entity.Identity, target.Identity))
            {
                component = system.World.GetComponent<T>(entity.Identity, target.Identity);
                return true;
            }

            component = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponent<T>(this ISystem system, Entity entity, Type type, out T component)
            where T : class
        {
            var typeIdentity = system.World.GetTypeIdentity(type);
            if (system.World.HasComponent<T>(entity.Identity, typeIdentity))
            {
                component = system.World.GetComponent<T>(entity.Identity, typeIdentity);
                return true;
            }

            component = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this ISystem system, Entity entity) where T : class
        {
            return system.World.HasComponent<T>(entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this ISystem system, Entity entity, Entity target) where T : class
        {
            return system.World.HasComponent<T>(entity.Identity, target.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this ISystem system, Entity entity, Type type) where T : class
        {
            var typeIdentity = system.World.GetTypeIdentity(type);
            return system.World.HasComponent<T>(entity.Identity, typeIdentity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity GetTarget<T>(this ISystem system, Entity entity) where T : class
        {
            return system.World.GetTarget<T>(entity.Identity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity[] GetTargets<T>(this ISystem system, Entity entity) where T : class
        {
            return system.World.GetTargets<T>(entity.Identity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Send<T>(this ISystem system, T trigger) where T : class
        {
            system.World.Send(trigger);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TriggerQuery<T> Receive<T>(this ISystem system) where T : class
        {
            return system.World.Receive<T>(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddElement<T>(this ISystem system, T element) where T : class
        {
            system.World.AddElement(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReplaceElement<T>(this ISystem system, T element) where T : class
        {
            system.World.ReplaceElement(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrReplaceElement<T>(this ISystem system, T element) where T : class
        {
            if (system.World.HasElement<T>()) system.World.ReplaceElement(element);
            else system.World.AddElement(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetElement<T>(this ISystem system) where T : class
        {
            return system.World.GetElement<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElement<T>(this ISystem system, out T element) where T : class
        {
            if (system.World.HasElement<T>())
            {
                element = system.World.GetElement<T>();
                return true;
            }

            element = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElement<T>(this ISystem system) where T : class
        {
            return system.World.HasElement<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveElement<T>(this ISystem system) where T : class
        {
            system.World.RemoveElement<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAll<T>(this ISystem system) where T : class
        {
            foreach (var entity in QueryBuilder(system).Has<T>().Build())
            {
                system.On(entity).Remove<T>();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DespawnAllWith<T>(this ISystem system) where T : class
        {
            foreach (var entity in QueryBuilder(system).Has<T>().Build())
            {
                system.Despawn(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<Entity> Query(this ISystem system)
        {
            return new QueryBuilder<Entity>(system.World).Build();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C> Query<C>(this ISystem system)
            where C : class
        {
            return new QueryBuilder<C>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2> Query<C1, C2>(this ISystem system)
            where C1 : class
            where C2 : class
        {
            return new QueryBuilder<C1, C2>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3> Query<C1, C2, C3>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
        {
            return new QueryBuilder<C1, C2, C3>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4> Query<C1, C2, C3, C4>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
        {
            return new QueryBuilder<C1, C2, C3, C4>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4, C5> Query<C1, C2, C3, C4, C5>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4, C5, C6> Query<C1, C2, C3, C4, C5, C6>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4, C5, C6, C7> Query<C1, C2, C3, C4, C5, C6, C7>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4, C5, C6, C7, C8> Query<C1, C2, C3, C4, C5, C6, C7, C8>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
            where C8 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Query<C1, C2, C3, C4, C5, C6, C7, C8, C9> Query<C1, C2, C3, C4, C5, C6, C7, C8, C9>(
            this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
            where C8 : class
            where C9 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8, C9>(system.World).Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<Entity> QueryBuilder(this ISystem system)
        {
            return new QueryBuilder<Entity>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C> QueryBuilder<C>(this ISystem system)
            where C : class
        {
            return new QueryBuilder<C>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2> QueryBuilder<C1, C2>(this ISystem system)
            where C1 : class
            where C2 : class
        {
            return new QueryBuilder<C1, C2>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3> QueryBuilder<C1, C2, C3>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
        {
            return new QueryBuilder<C1, C2, C3>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4> QueryBuilder<C1, C2, C3, C4>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
        {
            return new QueryBuilder<C1, C2, C3, C4>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4, C5> QueryBuilder<C1, C2, C3, C4, C5>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4, C5, C6> QueryBuilder<C1, C2, C3, C4, C5, C6>(this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4, C5, C6, C7> QueryBuilder<C1, C2, C3, C4, C5, C6, C7>(
            this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8> QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8>(
            this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
            where C8 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8>(system.World);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8, C9> QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8, C9>(
            this ISystem system)
            where C1 : class
            where C2 : class
            where C3 : class
            where C4 : class
            where C5 : class
            where C6 : class
            where C7 : class
            where C8 : class
            where C9 : class
        {
            return new QueryBuilder<C1, C2, C3, C4, C5, C6, C7, C8, C9>(system.World);
        }
    }

    public sealed class SystemGroup
    {
        readonly List<ISystem> _systems = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SystemGroup Add(ISystem aSystem)
        {
            _systems.Add(aSystem);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run(World world)
        {
            foreach (var system in _systems)
            {
                system.Run(world);
            }
        }
    }
}