using System.Collections.Generic;
using UnityEngine;

namespace BSPImporter.EntityFactories
{
    public class PrefabEntityFactory : IEntityFactory
    {
        private GameObject DefaultPrefab;
        private Dictionary<string, GameObject> PrefabDictionary;

        /// <summary>
        /// a template source that spawns a copy of the provided prefab
        /// </summary>
        /// <param name="defaultPrefab">Prefab to spawn if the class name isn't found in <see cref="PrefabDictionary"/>. if null, a gameobject is created with new()</param>
        public PrefabEntityFactory(GameObject defaultPrefab = null)
        {
            DefaultPrefab = defaultPrefab;
            PrefabDictionary = new Dictionary<string, GameObject>();
        }

        public PrefabEntityFactory(IEnumerable<KeyValuePair<string,GameObject>> initialItems, GameObject defaultPrefab = null) : this(defaultPrefab)
        {
            AddRange(initialItems);
        }

        public void Add(string className, GameObject prefab)
        {
            if (PrefabDictionary.ContainsKey(className))
            {
                Debug.LogWarning($"Tried to register prefab for duplicate class '{className}'");
                return;
            }
            PrefabDictionary[className] = prefab;
        }

        public void AddRange(IEnumerable<KeyValuePair<string, GameObject>> pairs)
        {
            foreach (var pair in pairs)
            {
                Add(pair.Key, pair.Value);
            }
        }

        public GameObject Spawn(string className)
        {
            if (PrefabDictionary.TryGetValue(className, out GameObject prefab))
            {
                return Object.Instantiate(prefab);
            }
            else if (DefaultPrefab != null)
            {
                return Object.Instantiate(DefaultPrefab);
            }
            else
            {
                return new GameObject();
            }
        }
    }
}
