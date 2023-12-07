
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.AssetImporters;
using UnityEngine;


public class MapRend : MonoBehaviour
{
    public const int chnkSize = 150;

    public float noiseScale;

    [Range(0, 1)]
    public float factor_1;
    public float factor_2;

    public int seed;
    public Vector2 offset;
    public float meshHeightMul = 1.0f;
    public AnimationCurve meshCurve;

    Queue<ThreadInfo<Maps>> mapThreadQueue = new Queue<ThreadInfo<Maps>>();
    Queue<ThreadInfo<MeshData>> meshThreadQueue = new Queue<ThreadInfo<MeshData>>();
    public dataStorage[] area;
    public void drawMap()
    {
        Maps map = GenerateMap(Vector2.zero);
        Display_Template display = FindObjectOfType<Display_Template>();
        display.DrawTexture(CreateTexture.ColorMapTex(map.color, chnkSize, chnkSize));

        display.DrawMesh(MeshRend.MeshGen(map.noise, meshHeightMul, meshCurve), CreateTexture.ColorMapTex(map.color, chnkSize, chnkSize));
    }
    public Maps GenerateMap(Vector2 currLoc)
    {
        float[,] noiseMap = Noise.NoiseGen(chnkSize, chnkSize, seed, noiseScale, factor_1, factor_2, currLoc + offset);

        Color[] colorMap = new Color[chnkSize * chnkSize];
        for (int y = 0; y < chnkSize; y++)
        {
            for (int x = 0; x < chnkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < area.Length; i++)
                {
                    if (currentHeight <= area[i].height)
                    {
                        colorMap[y * chnkSize + x] = area[i].colour;
                        break;
                    }
                }
            }
        }
        return new Maps(noiseMap, colorMap);
    }

    public void requestMap(Vector2 currLoc, Action<Maps> callback)
    {
        ThreadStart MapThreadstart = delegate
        {
            dataMap(currLoc, callback);
        };
        new Thread(MapThreadstart).Start();
    }
    public void dataMap(Vector2 currLoc, Action<Maps> callback)
    {
        Maps map = GenerateMap(currLoc);
        lock (mapThreadQueue) //no other thread can execute at the same time. 
        {
            mapThreadQueue.Enqueue(new ThreadInfo<Maps>(callback, map));
        }

    }
    public void requestMesh(Maps map, Action<MeshData> callback)
    {
        ThreadStart MeshthreadStart = delegate {
            dataMesh(map, callback);
        };

        new Thread(MeshthreadStart).Start();
    }
    public void dataMesh(Maps map, Action<MeshData> callback)
    {
        MeshData meshData = MeshRend.MeshGen(map.noise, meshHeightMul, meshCurve);
        lock (meshThreadQueue)
        {
            meshThreadQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
        }
    }
    private void Update()
    {
        if(mapThreadQueue.Count > 0)
        {
            for (int j = 0; j < mapThreadQueue.Count; j++)
            {
                ThreadInfo<Maps> info = mapThreadQueue.Dequeue();
                info.callback(info.parameter);
            }
        }
        if (meshThreadQueue.Count > 0)
        {
            for (int j = 0; j < meshThreadQueue.Count; j++)
            {
                ThreadInfo<MeshData> info = meshThreadQueue.Dequeue();
                info.callback(info.parameter);
            }
        }

        if (mapThreadQueue.Count > 0)
        {
            int j = 0;
            while (j < mapThreadQueue.Count)
            {
                ThreadInfo<Maps> info = mapThreadQueue.Dequeue();
                info.callback(info.parameter);
                j++;
            }
        }
        if (meshThreadQueue.Count > 0)
        {
            int j = 0;
            while (j < meshThreadQueue.Count)
            {
                ThreadInfo<MeshData> info = meshThreadQueue.Dequeue();
                info.callback(info.parameter);
                j++;
            }
        }
    }


    void OnValidate()
    {
        if (factor_2 < 1)
        {
            factor_2 = 1;
        }
    }

    struct ThreadInfo<T>
    {
        public Action<T> callback;
        public T parameter;
        public ThreadInfo (Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct dataStorage
{
    public float height;
    public Color colour;
}

public struct Maps
{
    public float[,] noise;
    public Color[] color;
    public Maps(float[,] heightMap, Color[] colorMap)
    {
        this.noise = heightMap;
        this.color = colorMap;
    }
}