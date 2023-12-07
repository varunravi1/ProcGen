using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshRend {

	public static MeshData MeshGen(float[,] heightMap, float heightMultiplier, AnimationCurve curve) {
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);



		int incrementfactor = 1;
		int noVertices = (width - 1) / incrementfactor + 1;
		MeshData meshData = new MeshData (noVertices, noVertices);
		int vertexIndex = 0;

		for (int y = 0; y < height; y+= incrementfactor) {
			for (int x = 0; x < width; x+= incrementfactor) {
				lock(curve)
				{
                    meshData.vertices[vertexIndex] = new Vector3(((width - 1) / -2.0f) + x, curve.Evaluate(heightMap[x, y]) * heightMultiplier, ((height - 1) / -2.0f) - y);
                }
				
				meshData.norm [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

				if (x < width - 1 && y < height - 1) {
					meshData.createPolygon (vertexIndex, vertexIndex + noVertices + 1, vertexIndex + noVertices);
					meshData.createPolygon (vertexIndex + noVertices + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		return meshData;

	}
}

public class MeshData {
	public Vector3[] vertices;
	public int[] poly;
	public Vector2[] norm;

	int index;

	public MeshData(int length, int height) {
		vertices = new Vector3[length * height];
		norm = new Vector2[length * height];
		poly = new int[(length-1)*(height-1)*6];
	}

	public void createPolygon(int x, int y, int z) {
        poly[index] = x;
        index++;
        poly[index] = y;
        index++;
        poly[index] = z;
        index += 1;
    }

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = poly;
		mesh.uv = norm;
		mesh.RecalculateNormals ();
		return mesh;
	}

}
