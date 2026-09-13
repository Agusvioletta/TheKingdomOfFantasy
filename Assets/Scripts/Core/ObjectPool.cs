using System.Collections.Generic;
using UnityEngine;

namespace TKOF.Core
{
    /// <summary>
    /// PATRÓN: POOL (Object Pool) genérico.
    /// Reutiliza instancias en vez de Instantiate/Destroy constantemente.
    /// Se usa por ejemplo para los AudioSource de pisadas/cadenas de Brutus
    /// (ver FootstepAudioPool) y podría reutilizarse para partículas de polvo
    /// al agacharse, impactos, etc.
    /// </summary>
    /// <typeparam name="T">Componente que se va a poolear (ej: AudioSource, ParticleSystem)</typeparam>
    public class ObjectPool<T> where T : Component
    {
        // ESTRUCTURA DE DATOS: List<T>, usada como pila de disponibles.
        private readonly List<T> _available = new List<T>();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, Transform parent, int initialSize)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < initialSize; i++)
            {
                _available.Add(CreateNew());
            }
        }

        private T CreateNew()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T Get()
        {
            if (_available.Count == 0)
            {
                return CreateNew();
            }
            int lastIndex = _available.Count - 1;
            T item = _available[lastIndex];
            _available.RemoveAt(lastIndex);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            _available.Add(item);
        }
    }
}
