using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MeshCreator : MonoBehaviour {

    public BicubicBezierSurface surfaceScript;
    Mesh mesh;

    // Update is called once per frame
    void Update () {
        CreateVertices();
	}

    void CreateVertices() {

        
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        Vector3[] vertices = new Vector3[(surfaceScript.resolution + 1) * (surfaceScript.resolution + 1)];
        for (int i = 0, y = 0; y <= surfaceScript.resolution; y++) {
            for (int x = 0; x <= surfaceScript.resolution; x++, i++) {
                vertices[i] = surfaceScript.bp[y, x];
            }
        }
        mesh.vertices = vertices;

        int[] triangles = new int[surfaceScript.resolution * surfaceScript.resolution * 6];
        for (int ti = 0, vi = 0, y = 0; y < surfaceScript.resolution; y++, vi++) {
            for (int x = 0; x < surfaceScript.resolution; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + surfaceScript.resolution + 1;
                triangles[ti + 5] = vi + surfaceScript.resolution + 2;
            }
        }
        mesh.triangles = triangles;
    }

    void OnGUI() {
        if (GUILayout.Button("Create")) {
            AssetDatabase.CreateAsset(mesh, "Assets/Meshes/BezierSurface.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
