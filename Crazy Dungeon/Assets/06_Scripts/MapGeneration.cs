using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    #region Attributes
    
    #region Fields
    
    [SerializeField] private RoomGenerationData m_data;
    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private RuleTile m_ruleTile;
    [SerializeField] private TileBase m_doorsTile;
    [SerializeField] private TileBase m_perlinTile;
    [SerializeField] private TileBase m_cellularTile;

    #endregion
    
    #region Consts
    
    // Rule Tile
    private const int GROUND = 0;
    private const int WALL = 1;
    
    // Corner Generation
    private const int TOP_RIGHT = 0;
    private const int TOP_LEFT = 1;
    private const int BOTTOM_RIGHT = 2;
    private const int BOTTOM_LEFT = 3;
    private static readonly List<int> s_corners = new() { TOP_RIGHT , TOP_LEFT , BOTTOM_RIGHT , BOTTOM_LEFT };

    #endregion
    
    #region Privates
    
    private List<List<bool>> m_mapState;
    private List<Vector2Int> m_baseCornersSize;
    
    private EGroundEffect m_groundEffect;
    private EWallEffect m_wallEffect;
    
    #endregion

    #endregion

    public void GenerateRoom()
    {
        SetRoomEffects();

        GenerateBaseShape();
        GenerateCornersNoise();

        CellularModulation();
        GenerateDoors();
    }

    #region Private Methods

    private void SetRoomEffects()
    {
        var groundTileData = m_data.GroundTilesData[Random.Range(0, m_data.GroundTilesData.Count)];
        var wallTileData = m_data.WallTilesData[Random.Range(0, m_data.WallTilesData.Count)];

        m_groundEffect = groundTileData.Effect;
        m_wallEffect = wallTileData.Effect;

        m_ruleTile.m_TilingRules[GROUND].m_Sprites[0] = groundTileData.Sprite;
        m_ruleTile.m_TilingRules[WALL].m_Sprites[0] = wallTileData.Sprite;
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
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_ruleTile);
                }
            }
        }
    }

    private void GenerateCornersNoise()
    {
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
                        m_tilemap.GetTile(new Vector3Int(x, y, 0)) == m_ruleTile)
                    {
                        continue;
                    }
                
                    if (perlin.GetValueAt(x, y) >= m_data.CornerPerlinThreshold)
                    {
                        m_tilemap.SetTile(new Vector3Int(x, y, 0), m_data.DisplayDebugPerlinOnTilemap ? m_perlinTile : m_ruleTile);
                    }
                }
            }
            
            perlin.Reseed();
        }
    }

    #region Cellular Modulation

    private void CellularModulation()
    {
        for (int i = 0; i < m_data.CellularModulationCount; i++)
        {
            SaveMapState();
            List<List<float>> weights = ComputeFashionWeights();
            ApplyModulation(weights);
        }
    }

    private void SaveMapState()
    {
        m_mapState = new List<List<bool>>();
        for (int x = -m_data.MaxRoomSize.x / 2; x < m_data.MaxRoomSize.x / 2; x++)
        {
            m_mapState.Add(new List<bool>());
            for (int y = -m_data.MaxRoomSize.y / 2; y < m_data.MaxRoomSize.y / 2; y++)
            {
                bool isTile = m_tilemap.GetTile(new Vector3Int(x, y, 0)) != null;
                m_mapState[x + m_data.MaxRoomSize.x / 2].Add(isTile);
            }
        }
    }
    

    private List<List<float>> ComputeFashionWeights()
    {
        List<List<float>> weights = new List<List<float>>();
        for (int x = 0; x < m_data.MaxRoomSize.x; x++)
        {
            weights.Add(new List<float>());
            for (int y = 0; y < m_data.MaxRoomSize.y; y++)
            {
                weights[x].Add(0f);
                var neighbors = GetNeighborsList(x, y);
                
                // Apply weights
                foreach (var neighbor in neighbors)
                {
                    bool tileFull = m_mapState[x][y];
                    bool neighborFull = m_mapState[neighbor.x][neighbor.y];
                    weights[x][y] += m_data.FashionWeight(tileFull, neighborFull);
                }
            }
        }

        return weights;
    }

    private void ApplyModulation(List<List<float>> a_weights)
    {
        TileBase tileToDisplay = m_data.DisplayDebugCellularOnTilemap ? m_cellularTile : m_ruleTile;
        
        for (int x = 0; x < m_data.MaxRoomSize.x; x++)
        {
            for (int y = 0; y < m_data.MaxRoomSize.y; y++)
            {
                var neighbors = GetNeighborsList(x, y);
                float weight = a_weights[x][y];
                bool state = m_mapState[x][y];
                
                foreach (var neighbor in neighbors)
                {
                    if (a_weights[neighbor.x][neighbor.y] < weight) continue;
                    state = m_mapState[neighbor.x][neighbor.y];
                    weight = a_weights[neighbor.x][neighbor.y];
                }

                Vector3Int tilePos = new Vector3Int(x - m_data.MaxRoomSize.x / 2, y - m_data.MaxRoomSize.y / 2, 0);
                m_tilemap.SetTile(tilePos, state ? tileToDisplay : null);
            }
        }
    }

    #endregion

    #region Utils
    private List<Vector2Int> GetNeighborsList(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (x > 0) { neighbors.Add(new Vector2Int(x - 1, y)); }
        if (y > 0) { neighbors.Add(new Vector2Int(x, y - 1)); }
        if (x < m_data.MaxRoomSize.x - 1) { neighbors.Add(new Vector2Int(x + 1, y)); }
        if (y < m_data.MaxRoomSize.y - 1) { neighbors.Add(new Vector2Int(x, y + 1)); }
        return neighbors;
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

    #endregion

    private void GenerateDoors()
    {
        //Random nb doors and place
        List<Boolean> doorsPlace = new List<Boolean>(){false, false, false, false};
        
        //0 bottom ; 1 left ; 2 top ; 3 right
        List<Vector3Int> directionDoors = new List<Vector3Int>()
        {
            new Vector3Int(0, -1),
            new Vector3Int(-1, 0),
            new Vector3Int(0, 1),
            new Vector3Int(1, 0),
        };

        int doorStart = Mathf.FloorToInt(-m_data.DoorSize / 2);
        int doorEnd = Mathf.CeilToInt(m_data.DoorSize / 2);
        
        while (doorsPlace.Count(d => d) < 2)
        {
            for (int i = 0; i < 4; i++)
            {
                doorsPlace[i] = Random.value > 0.5; //Random placement of the doors
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (!doorsPlace[i]) continue;
            
            Vector3Int curPos = (Vector3Int)m_data.MaxRoomSize * directionDoors[i];
            bool doorIsEdged = false;
            bool tunnelConnected = false;
            int bound = (int)Vector3Int.Distance(Vector3Int.zero, (Vector3Int)m_data.MaxRoomSize * directionDoors[i]);
            int j = bound;
            
            for (; !doorIsEdged; j--)
            {
                doorIsEdged = false;
                curPos -= directionDoors[i];
                
                for (int y = doorStart; y < doorEnd; y++) //check if all doorTile is border or not
                {
                    var pos = new Vector3Int(curPos.x == 0 ? y : curPos.x, curPos.y == 0 ? y : curPos.y);
                    if(m_tilemap.GetTile(pos) != null) { doorIsEdged = true; }
                }
            }
            
            //draw Door
            for (int y = doorStart; y < doorEnd; y++)
            {
                var pos = new Vector3Int(curPos.x == 0 ? y : curPos.x, curPos.y == 0 ? y : curPos.y);
                m_tilemap.SetTile(pos, m_doorsTile); // Draw door
            }

            Vector3Int dirBorder = new Vector3Int(Mathf.Abs(directionDoors[(i+1)%4].x), Mathf.Abs(directionDoors[(i+1)%4].y));
            
            m_tilemap.SetTile(curPos+dirBorder*(doorStart-1), m_ruleTile); // Draw border door
            m_tilemap.SetTile(curPos+dirBorder*doorEnd, m_ruleTile); // Draw border door


            for (; !tunnelConnected; j--) // draw tunnel
            {
                tunnelConnected = true;
                curPos -= directionDoors[i];
                
                for (int y = doorStart-1; y < doorEnd+1; y++) //check if all doorTile is border or not
                {
                    var pos = new Vector3Int(curPos.x == 0 ? y : curPos.x, curPos.y == 0 ? y : curPos.y);
                    if(m_tilemap.GetTile(pos) == null) { tunnelConnected = false; }
                    m_tilemap.SetTile(pos, m_ruleTile);
                }
            }
        }
    }

    #endregion
}
