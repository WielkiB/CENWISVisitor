using UnityEngine;

public static class ColorConverting
{
    public static void RGBToHSV(Color color, out float h, out float s, out float v)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, g, b);
        float min = Mathf.Min(r, g, b);

        float delta = max - min;

        if (delta < 0.00001f)
        {
            h = 0f;
        }
        else if (Mathf.Approximately(max, r))
        {
            h = (g - b) / delta;
        }
        else if (Mathf.Approximately(max, g))
        {
            h = 2f + (b - r) / delta;
        }
        else
        {
            h = 4f + (r - g) / delta;
        }

        h = (h < 0f) ? h + 6f : h;
        h /= 6f;
 
        s = max <= 0f ? 0f : delta / max;

        v = max;
    }

    public static Color HSVToRGB(float h, float s, float v)
    {
        float r, g, b;

        if (s <= 0f)
        {
            r = g = b = v;
        }
        else
        {
            h = (h % 1f) * 6f;
            int sector = Mathf.FloorToInt(h);
            float fraction = h - sector;
            float p = v * (1f - s);
            float q = v * (1f - s * fraction);
            float t = v * (1f - s * (1f - fraction));

            switch (sector)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }
        }

        return new Color(r, g, b, 1f);
    }
}
