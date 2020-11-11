using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public abstract class BrushModifierAsset : ScriptableObject
    {
        public virtual void ApplyModifier(ScriptableBrushBaseAsset brush)
        {
            
        }

        public virtual void UpdatePosition(ScriptableBrushBaseAsset brush)
        {
            
        }
    }
}