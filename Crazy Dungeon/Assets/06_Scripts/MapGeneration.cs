using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] private RoomGenerationData m_data;

    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private RuleTile m_groundTile;
    [SerializeField] private Tile m_perlinTile;

    private const int TOP_RIGHT = 0;
    private const int TOP_LEFT = 1;
    private const int BOTTOM_RIGHT = 2;
    private const int BOTTOM_LEFT = 3;
    private static readonly List<int> s_corners = new() { TOP_RIGHT , TOP_LEFT , BOTTOM_RIGHT , BOTTOM_LEFT };

    private List<Vector2Int> m_baseCornersSize;

    private void Start()
    {
        GenerateRoom();
    }

    private void GenerateRoom()
    {
        GenerateBaseShape();
        var cornersNoise = GenerateCornersNoise();
    }

    private List<Vector2Int> GetRandomVectorsForCorners(int min, int max) =>
        GetRandomVectorsForCorners(new Vector2Int(min, min), new Vector2Int(max, max));
    private List<Vector2Int> GetRandomVectorsForCorners(Vector2Int min, Vector2Int max)
    {
        int GetRandomSizeX() => Random.Range(min.x, max.x);
        int GetRandomSizeY() => Random.Range(min.y, max.y);
        
        List<Vector2Int> randomVectors = new List<Vector2Int>
        {
            new Vector2Int(GetRandomSizeX(), GetRandomSizeY()), // TOP_RIGHT
            new Vector2Int(-GetRandomSizeX(), GetRandomSizeY()), // TOP_LEFT
            new Vector2Int(GetRandomSizeX(), -GetRandomSizeY()), // BOTTOM_RIGHT
            new Vector2Int(-GetRandomSizeX(), -GetRandomSizeY()) // BOTTOM_LEFT
        };

        return randomVectors;
    }

    private void GenerateBaseShape()
    {
        // Generate base corners
        m_baseCornersSize = GetRandomVectorsForCorners(m_data.MinRoomSize / 2, m_data.MaxRoomSize / 2);

        for (int i = 0; i < s_corners.Count; i++)
        {
            foreach (int x in Utils.Range(0, m_baseCornersSize[i].x))
            {
                foreach (int y in Utils.Range(0, m_baseCornersSize[i].y))
                {
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
                }
            }
        }
    }

    private bool[][] GenerateCornersNoise()
    {
        bool[][] noiseFiltered = new bool[m_data.MaxRoomSize.x][];
        for (int i = 0; i < noiseFiltered.Length; i++)
        {
            noiseFiltered[i] = new bool[m_data.MaxRoomSize.y];
        }
        
        PerlinNoise perlin = new PerlinNoise();
        List<Vector2Int> perlinSize = GetRandomVectorsForCorners(m_data.MinCornerPerlinSize, m_data.MaxCornerPerlinSize);
        for (int i = 0; i < s_corners.Count; i++)
        {
            foreach (int x in Utils.Range(-perlinSize[i].x, m_baseCornersSize[i].x + perlinSize[i].x))
            {
                foreach (int y in Utils.Range(-perlinSize[i].y, m_baseCornersSize[i].y + perlinSize[i].y))
                {
                    if (x < -m_data.MaxRoomSize.x / 2 || x >= m_data.MaxRoomSize.x / 2 || 
                        y < -m_data.MaxRoomSize.y / 2 || y >= m_data.MaxRoomSize.y / 2 ||
                        m_tilemap.GetTile(new Vector3Int(x, y, 0)) == m_groundTile)
                    {
                        continue;
                    }
                
                    if (perlin.GetValueAt(x, y) >= m_data.CornerPerlinThreshold)
                    {
                        if (m_data.DisplayDebugPerlinOnTilemap)
                        {
                            m_tilemap.SetTile(new Vector3Int(x, y, 0), m_perlinTile);
                        }
                        noiseFiltered[x + m_data.MaxRoomSize.x / 2][y + m_data.MaxRoomSize.y / 2] = true;
                    }
                }
            }
            
            perlin.Reseed();
        }

        return noiseFiltered;
    }
}
