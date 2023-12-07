using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreateTexture
{

    public static Texture2D ColorMapTex(Color[] colorMap, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(colorMap);
        tex.Apply();
        return tex;
    }


    public static Texture2D HeightMapTex(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = createColor(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return ColorMapTex(colorMap, width, height);
    }

    public static Color createColor(Color a, Color b, float t)
    {
        t = Mathf.Clamp01(t); // Ensure t is within the 0-1 range

        float r = Mathf.Lerp(a.r, b.r, t);
        float g = Mathf.Lerp(a.g, b.g, t);
        float bVal = Mathf.Lerp(a.b, b.b, t);
        float aVal = Mathf.Lerp(a.a, b.a, t);

        return new Color(r, g, bVal, aVal);
    }

}