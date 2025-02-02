using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public class PoolOfObjects<T> : MonoBehaviour
    {
        private readonly Queue<T> _poolOfFliers;
        private readonly Transform _parentTransform;
        private readonly GameObject _prefab;

        public PoolOfObjects(GameObject prefab, Transform parentTransform)
        {
            _poolOfFliers = new Queue<T>();
            _prefab = prefab;
            _parentTransform = parentTransform;
        }

        public void Put(T poolObj)
        {
            _poolOfFliers.Enqueue(poolObj);
        }

        public T Get()
        {
            if (_poolOfFliers.Count <= 0) Create();
            var poolObj = _poolOfFliers.Dequeue();
            return poolObj;
        }

        private void Create()
        {
            var poolObj = Instantiate(_prefab, _parentTransform);
            var component = poolObj.GetComponent<T>();
            Put(component);
        }
    }
}