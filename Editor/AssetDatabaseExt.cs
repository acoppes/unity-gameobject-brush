using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    public static class AssetDatabaseExt
    {
        [Flags]
        public enum FindOptions
        {
            None = 0,
            ConsiderChildren = 1
        }

        public static List<T> FindAssets<T>(string[] folders = null) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}", folders);
            return guids.Select(g => AssetDatabase.LoadAssetAtPath<T>(
                AssetDatabase.GUIDToAssetPath(g))).ToList();
        }

        public static List<GameObject> FindPrefabs<T>(FindOptions options = 0, string[] folders = null)
        {
            return FindPrefabs(new []{typeof(T)}, options, folders);
        }
        
        public static List<GameObject> FindPrefabs<T1, T2>(FindOptions options = 0, string[] folders = null)
        {
            return FindPrefabs(new []{typeof(T1), typeof(T2)} , options, folders);
        }
        
        public static List<GameObject> FindPrefabs<T1, T2, T3>(FindOptions options = 0, string[] folders = null)
        {
            return FindPrefabs(new []{typeof(T1), typeof(T2), typeof(T3)} , options, folders);
        }

        public static List<GameObject> FindPrefabs(IEnumerable<Type> types, FindOptions options, string[] folders)
        {
            var considerChildren = options.HasFlag(FindOptions.ConsiderChildren);

            var guids = AssetDatabase.FindAssets("t:Prefab", folders);

            var prefabs = guids.Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(
                AssetDatabase.GUIDToAssetPath(g))).ToList();

            if (considerChildren)
            {
                IEnumerable<GameObject> result = prefabs;
                // By default is the AND of all specified Types
                foreach (var type in types)
                {
                    result = result.Where(p => p.GetComponentInChildren(type) != null);
                }
                return result.ToList();
            }
            else
            {
                IEnumerable<GameObject> result = prefabs;
                // By default is the AND of all specified Types
                foreach (var type in types)
                {
                    result = result.Where(p => p.GetComponent(type) != null);
                }
                return result.ToList();
            }
        }
    }
}