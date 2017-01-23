using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BicubicBezierSurface : MonoBehaviour {

    public int resolution = 20;

    Vector3[,] cp;
    [HideInInspector] public Vector3[,] bp;

    public Transform[,] controller;

    public bool surfaceGizmo = true;
    
	void Start () {
        
        initCp();
        initBp();
        calcBezierSurface();

    }
    
    void initCp() {

        controller = new Transform[4, 4];
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                controller[i, j] = transform.FindChild(i.ToString() + j.ToString()).transform;
            }
        }

        cp = new Vector3[4, 4];
        for (var zIdx = 0; zIdx < 4; zIdx++) {
            controller[zIdx, 0].localPosition = new Vector3(-1.5f, 0, zIdx - 1.5f);
            controller[zIdx, 1].localPosition = new Vector3(-0.5f, 0, zIdx - 1.5f);  // middle left
            controller[zIdx, 2].localPosition = new Vector3(0.5f, 0, zIdx - 1.5f);   // middle right
            controller[zIdx, 3].localPosition = new Vector3(1.5f, 0, zIdx - 1.5f);   // right
        }

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                cp[i, j] = controller[i, j].localPosition;
            }
        }
    }
	
    void initBp() {
        bp = new Vector3[resolution + 1, resolution + 1];
        for (int u = 0; u < resolution + 1; u++) {
            for (int v = 0; v < resolution + 1; v++) {
                bp[u, v] = Vector3.zero;
            }
        }
    }
    
    void calcBezierSurface() {
        int m = 3, n = 3;

        float density = 1f / (float)resolution;
        float u, v;
        var tmp = 0.0;
        for (int uIdx = 0; uIdx <= resolution; uIdx++) {
            u = density * uIdx;
            for (int vIdx = 0; vIdx <= resolution; vIdx++) {
                v = density * vIdx;
                double tmpX = 0, tmpY = 0, tmpZ = 0;
                for (int i = 0; i <= m; i++) {
                    for (int j = 0; j <= n; j++) {
                        tmp = Bernstein(n, i, u) * Bernstein(m, j, v);
                        tmpX += cp[i, j].x * tmp;
                        tmpY += cp[i, j].y * tmp;
                        tmpZ += cp[i, j].z * tmp;
                    }
                }
                bp[uIdx,vIdx].x = (float)tmpX;
                bp[uIdx,vIdx].y = (float)tmpY;
                bp[uIdx,vIdx].z = (float)tmpZ;
            }
        }
    }

    double nCr(int n, int r) {
        int j = n;
        double ans = 1f;
        if (n == 0 || r == 0) return ans;
        for (var i = 1; i <= r; j--, i++) {
            ans *= j / (float)i;
        }
        return ans;
    }

    double Bernstein(int n, int i, float uv)  {
        double result = nCr(n, i) * Mathf.Pow(uv, i) * Mathf.Pow(1f - uv, n - i);
        return result;
    }
    
    void Update () {
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                cp[i, j] = controller[i, j].localPosition;
            }
        }

        calcBezierSurface();
    }

    void OnDrawGizmos() {
        if (Application.isPlaying) {
            if(surfaceGizmo) drawBezierSurface();
            drawController();
        }
    }

    void drawController() {
        Gizmos.color = Color.white;
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                Gizmos.DrawWireSphere(transform.TransformPoint(cp[i, j]), 0.1f);
            }
        }

        for (int i = 0; i < 4; i++) {
            // 横
            Gizmos.DrawLine(transform.TransformPoint(cp[i, 0]), transform.TransformPoint(cp[i, 1]));
            Gizmos.DrawLine(transform.TransformPoint(cp[i, 1]), transform.TransformPoint(cp[i, 2]));
            Gizmos.DrawLine(transform.TransformPoint(cp[i, 2]), transform.TransformPoint(cp[i, 3]));

            //縦
            Gizmos.DrawLine(transform.TransformPoint(cp[0, i]), transform.TransformPoint(cp[1, i]));
            Gizmos.DrawLine(transform.TransformPoint(cp[1, i]), transform.TransformPoint(cp[2, i]));
            Gizmos.DrawLine(transform.TransformPoint(cp[2, i]), transform.TransformPoint(cp[3, i]));
        }
    }

    void drawBezierSurface() {
        Gizmos.color = Color.blue;
        for (var u = 0; u <= resolution; u++) {
            for (var v = 0; v < resolution; v++) {
                Gizmos.DrawLine(bp[u, v], bp[u, v + 1]);
            }
        }
        for (var v = 0; v <= resolution; v++) {
            for (var u = 0; u < resolution; u++) {
                Gizmos.DrawLine(bp[u, v], bp[u + 1, v]);
            }
        }
    }
}
