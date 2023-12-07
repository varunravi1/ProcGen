using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGen
{
    private const int GradientSizeTable = 256;
    private readonly int[] _perm = new int[GradientSizeTable * 2];

    public PerlinNoiseGen()
    {
        InitPermutation();
    }

    private void InitPermutation()
    {
        System.Random rnd = new System.Random(12345);
        for (int i = 0; i < GradientSizeTable; i++)
        {
            _perm[i] = i;
        }

        for (int i = 0; i < GradientSizeTable; i++)
        {
            
            int source = rnd.Next(0, GradientSizeTable - i) + i;
            int temp = _perm[i];
            _perm[i] = _perm[i + GradientSizeTable] = _perm[source];
            _perm[source] = temp;
        }
    }

    private float Grad(int hash, float x, float y)
    {
        int h = hash & 15;
        float grad = 1 + (h & 7); 
        if ((h & 8) != 0) grad = -grad; 
        return (grad * x + grad * y);
    }

    public float Noise(float x, float y)
    {
        int X = Mathf.FloorToInt(x) & 255;
        int Y = Mathf.FloorToInt(y) & 255;
        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);
        float u = Fade(x);
        float v = Fade(y);
        int A = _perm[X] + Y, AA = _perm[A], AB = _perm[A + 1],  
            B = _perm[X + 1] + Y, BA = _perm[B], BB = _perm[B + 1];

        return Lerp(v, Lerp(u, Grad(_perm[AA], x, y), Grad(_perm[BA], x - 1, y)), Lerp(u, Grad(_perm[AB], x, y - 1), Grad(_perm[BB], x - 1, y - 1)));
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }
}

