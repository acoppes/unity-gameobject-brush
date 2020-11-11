using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public abstract class BrushModifierAsset : ScriptableObject
    {
        public abstract void ApplyModifier(ScriptableBrushBaseAsset brush);
    }
}