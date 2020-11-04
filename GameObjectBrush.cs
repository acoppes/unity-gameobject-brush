using System;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public class GameObjectBrush : MonoBehaviour
    {
        [NonSerialized]
        public GameObject prefab;
        
        [NonSerialized]
        public GameObject preview;
    }
}