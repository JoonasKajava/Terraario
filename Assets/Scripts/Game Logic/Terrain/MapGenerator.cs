using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public int width = 100;
    public int height = 100;

    public int SurfaceHeight = 20;

    public string seed;
    public bool UseRandomSeed;

    [Range(0, 100)]
    public int RandomFillPercent;

    int[,] Map;

    void Start()
    {
        GenerateMap();

    }

    void Update()
    {
        if (Input.GetMouseButton(0)) GenerateMap();
    }

    void GenerateMap()
    {
        Map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 10; i++)
        {
            SmoothMap();
        }

        MeshGenerator meshgen = GetComponent<MeshGenerator>();
        meshgen.GenerateMesh(Map, 1);
    }

    void RandomFillMap()
    {
        if (UseRandomSeed) seed = Time.time.ToString();

        System.Random rng = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(y > height - SurfaceHeight)
                {
                    Map[x, y] = 0;
                    continue;
                }

                if (x == 0 || x == width - 1 || y == 0)
                {
                    Map[x, y] = 1;
                }
                else
                {
                    Map[x, y] = rng.Next(0, 100) < RandomFillPercent ? 1 : 0;
                }
            }
        }

    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if(neighbourWallTiles > 4)
                {
                    Map[x, y] = 1;
                }else if(neighbourWallTiles < 4)
                {
                    Map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int x, int y)
    {
        int count = 0;

        for (int n_x = x - 1; n_x <= x + 1; n_x++)
        {
            for (int n_y = y - 1; n_y <= y + 1; n_y++)
            {
                if (n_x >= 0 && n_x < width && n_y >= 0 && n_y < height)
                {
                    if (n_x != x || n_y != y)
                    {
                        count += Map[n_x, n_y];
                    }
                }else
                {
                    count++;
                }
            }
        }
        return count;
    }


    void OnDrawGizmos()
    {
        return;
        if (Map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = Map[x, y] == 1 ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 10);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }

}
