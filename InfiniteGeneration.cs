using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGeneration : MonoBehaviour
{
    public const float horizon = 200;
    public Transform player;
    public static Vector2 playerpos;
    static MapRend render;
    public Material mapMat;
    int chnkSize;
    int chnksVisible;

    Dictionary<Vector2, block> blockLocation = new Dictionary<Vector2, block>();
    List<block> chnkList = new List<block>(); 
    private void Start()
    {
        render = FindObjectOfType<MapRend>();
        chnkSize = MapRend.chnkSize - 1;
        chnksVisible = Mathf.RoundToInt(horizon / chnkSize);
    }
    private void Update()
    {
        playerpos = new Vector2(player.position.x, player.position.z);
        updateChnks();
    }


    void updateChnks()
    {
        for (int i = 0; i < chnkList.Count; i++)
        {
            chnkList[i].setVis(false);
        }
        chnkList.Clear();
        int currChnkx = (int) (playerpos.x / chnkSize);
        int currChnky = (int) (playerpos.y / chnkSize);


        int y = -chnksVisible;
        while (y <= chnksVisible)
        {
            int x = -chnksVisible;
            while (x <= chnksVisible)
            {
                Vector2 viewedChnk = new Vector2(currChnkx + x, currChnky + y);

                if (blockLocation.ContainsKey(viewedChnk))
                {
                    blockLocation[viewedChnk].updateChnks();
                    if (blockLocation[viewedChnk].isVis())
                    {
                        chnkList.Add(blockLocation[viewedChnk]);
                    }
                }
                else
                {
                    blockLocation.Add(viewedChnk, new block(viewedChnk, chnkSize, transform, mapMat));
                }

                x++;
            }
            y++;
        }
    }
    public class block
    {
        GameObject mesh;
        Vector2 pos;

        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public block(Vector2 coord, int size, Transform head, Material mat) {
            pos = coord * size;
            bounds = new Bounds(pos, Vector2.one * size);
            Vector3 newPosition = new Vector3(pos.x, 0, pos.y);
            mesh = new GameObject("Block");
            meshRenderer = mesh.AddComponent<MeshRenderer>();
            meshFilter = mesh.AddComponent<MeshFilter>();
            meshRenderer.material = mat;
            mesh.transform.position = newPosition;
            mesh.transform.parent = head;
            setVis(false);
            render.requestMap(pos,  onRecieveMapData);
        }
        void onRecieveMapData(Maps map)
        {

            Texture2D update_color_texture = CreateTexture.ColorMapTex(map.color, MapRend.chnkSize, MapRend.chnkSize);
            meshRenderer.material.mainTexture = update_color_texture;
            render.requestMesh(map, onRecieveMeshData);
            print("MapDataReceived");
        }
        void onRecieveMeshData(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void updateChnks()
        {
            float distbetplayerandchnk = Mathf.Sqrt(bounds.SqrDistance(playerpos));
            bool inFOV = distbetplayerandchnk <= horizon;
            setVis(inFOV);

        }
        public void setVis(bool vis)
        {
            mesh.SetActive(vis);
        }
        public bool isVis()
        {
            return mesh.activeSelf;
        }
    }
}
