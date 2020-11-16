namespace Gemserk.Tools.ObjectPalette.Editor
{
    public static class BrushExtensions
    {
        public static void CreatePreview(this IBrush b, PaletteSelection s)
        {
            b.CreatePreview(s.selection);
        }
    }
}