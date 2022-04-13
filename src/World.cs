using System;

namespace Bitron.Ecs
{
    public sealed class World
    {
        private EntityMeta[] _entityMetas;
        private int _entityCount;
        
        private int _despawnedEntityCount;
        private int[] _despawnedEntities;

        private IStorage[] _pools;
        private int _poolCount;

        public Entity Spawn()
        {
            int id;
            int gen;

            if (_despawnedEntityCount > 0)
            {
                id = _despawnedEntities[--_despawnedEntityCount];
                gen = -(_entityMetas[id]).Gen;
            }
            else
            {
                id = _entityCount++;
                gen = 1;
            }

            _entityMetas[id].Gen = gen;

            return new Entity() { Id = id, Gen = gen };
        }

        public void Despawn(Entity entity)
        {
            ref var meta = ref _entityMetas[entity.Id];

            if (meta.Gen < 0)
            {
                return;
            }

            if (meta.ComponentCount > 0)
            {
                var index = 0;
                while (meta.ComponentCount > 0 && index < _poolCount)
                {
                    for (; index < _poolCount; index++)
                    {
                        if (_pools[index].Has(entity))
                        {
                            _pools[index++].Remove(entity);
                            break;
                        }
                    }
                }
            }

            if (_despawnedEntityCount == _despawnedEntities.Length)
            {
                Array.Resize(ref _despawnedEntities, _despawnedEntityCount << 1);
            }

            meta.Gen = -(meta.Gen + 1);

            _despawnedEntities[_despawnedEntityCount++] = entity.Id;
        }

        public Storage<Component> GetStorage<Component>() where Component : struct
        {
            var typeId = ComponentType<Component>.Id;

            if (typeId == _poolCount)
            {
                Array.Resize(ref _pools, _poolCount << 1);
                _pools[typeId] = new Storage<Component>();
                _poolCount++;
            }

            return _pools[typeId] as Storage<Component>;
        }
    }

    internal struct EntityMeta
    {
        public int Gen;
        public int ComponentCount;
    }

    internal class ComponentType
    {
        protected static int counter = 0;
    }

    internal class ComponentType<T> : ComponentType
    {
        public static readonly int Id;

        static ComponentType() => Id = counter++;
    }
}