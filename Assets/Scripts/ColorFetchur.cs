using UnityEngine;

static class ColorFetchur
{
    static Color[] colors = {
        Color.HSVToRGB(0.6f, 0.9f, 0.9f),
        Color.HSVToRGB(0.16f, 0.9f, 0.9f),
        Color.HSVToRGB(0.08f, 1, 0.9f),
        Color.HSVToRGB(0, 0.9f, 0.9f),
        Color.HSVToRGB(0.33f, 0.9f, 0.9f)
        };

    public static Color GetColour(foeState state) { return GetColour((int)state); }
    public static Color GetColour(int index) { return colors[index]; }
}
