using System;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

namespace _game.Scripts.Lobby
{
    [CreateAssetMenu(menuName = "Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {
        public List<MapInfo> Maps;
    }

    [Serializable]
    public struct MapInfo
    {
        public string MapName;
        public Color MapThumbnail;
        [SceneDropdown] public string SceneName;
    }
}
