using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float[,] NoiseGen(int width, int height, int seed, float scale, float factor_1, float factor_2, Vector2 offset)
    {
        PerlinNoiseGen perlin = new PerlinNoiseGen();
        float[,] perlinMap = new float[width, height];

        System.Random random_num = new System.Random(seed);
        Vector2[] ocOff = new Vector2[4];
        for (int i = 0; i < 3; i++)
        {
            float offsetX = random_num.Next(-100000, 100000) + offset.x;
            float offsetY = random_num.Next(-100000, 100000) - offset.y;
            ocOff[i] = new Vector2(offsetX, offsetY);
        }

        float top = -100000.1f;
        float bottom = 100000.1f;


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float size = 1;
                float occurences = 1;
                float peak = 0;
                for (int j = 0; j < 3; j++)
                {
                    float sampleX = (x - (width / 2.0f) + ocOff[j].x) / scale * occurences;
                    float sampleY = (y - (height / 2.0f) + ocOff[j].y) / scale * occurences;

                    float perlinValue = perlin.Noise(sampleX, sampleY);
                    peak += perlinValue * size;

                    size = size * factor_1;
                    occurences = occurences *  factor_2;
                }

                if (peak > top)
                {
                    top = peak;
                }
                else if (peak < bottom)
                {
                    bottom = peak;
                }
                perlinMap[x, y] = peak;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                perlinMap[x, y] = Norm(bottom, top, perlinMap[x, y]);
            }
        }

        return perlinMap;
    }
    private static float Norm(float a, float b, float value)
    {
        if (a == b) return 0f;

        return Mathf.Clamp01((value - a) / (b - a));
    }

}