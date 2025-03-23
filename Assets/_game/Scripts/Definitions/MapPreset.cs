using UnityEngine;

namespace _game.Scripts.Definitions
{
    [CreateAssetMenu(fileName = "MapPreset", menuName = "Scriptable Objects/MapPreset")]
    public class MapPreset : ScriptableObject
    {
        [field: SerializeField] public MapSettings MapSettings { get; private set; } = new MapSettings();
    }
}
