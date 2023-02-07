using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] private GameData m_data;

    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private RuleTile m_groundTile;
    [SerializeField] private Tile m_tempTile;

    private void Start()
    {
        GenerateRoom();
    }

    private void GenerateRoom()
    {
        // Generate random corner sizes
        List<Vector2Int> cornersSize = new List<Vector2Int>();
        for (int i = 0; i < 4; i++)
        {
            cornersSize.Add(new Vector2Int(
                Random.Range(m_data.MinRoomSize.x / 2, m_data.MaxRoomSize.x / 2),
                Random.Range(m_data.MinRoomSize.y / 2, m_data.MaxRoomSize.y / 2)));
        }

        GenerateCorners(cornersSize);
        GeneratePerlinNoise(cornersSize);
        for (int i = 0; i < m_data.CellularModulationCount; i++)
        {
            CellularModulation();
        }
    }

    private void GenerateCorners(List<Vector2Int> a_cornersSize)
    {
        // Draw Top Right corner
        for (int x = 0; x < a_cornersSize[0].x; x++)
        {
            for (int y = 0; y < a_cornersSize[0].y; y++)
            {
                m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
            }
        }

        // Draw Top Left corner
        for (int x = 0; x > -a_cornersSize[1].x; x--)
        {
            for (int y = 0; y < a_cornersSize[1].y; y++)
            {
                m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
            }
        }

        // Draw Bottom Right corner
        for (int x = 0; x < a_cornersSize[2].x; x++)
        {
            for (int y = 0; y > -a_cornersSize[2].y; y--)
            {
                m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
            }
        }

        // Draw Bottom Left corner
        for (int x = 0; x > -a_cornersSize[3].x; x--)
        {
            for (int y = 0; y > -a_cornersSize[3].y; y--)
            {
                m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
            }
        }
    }

    private void GeneratePerlinNoise(List<Vector2Int> a_cornersSize)
    {
        Vector2Int perlinSize = new Vector2Int(
            Random.Range(m_data.MinCornerPerlinSize.x, m_data.MaxCornerPerlinSize.x),
            Random.Range(m_data.MinCornerPerlinSize.y, m_data.MaxCornerPerlinSize.y));
        float randomPerlinX = Random.value * 10000;
        float randomPerlinY = Random.value * 10000;
        for (int x = -perlinSize.x; x < a_cornersSize[0].x + perlinSize.x; x++)
        {
            for (int y = -perlinSize.y; y < a_cornersSize[0].y + perlinSize.y; y++)
            {
                if (Mathf.PerlinNoise(x + randomPerlinX, y + randomPerlinY) > m_data.CornerPerlinThreshold)
                {
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
                }
            }
        }

        perlinSize = new Vector2Int(
            Random.Range(m_data.MinCornerPerlinSize.x, m_data.MaxCornerPerlinSize.x),
            Random.Range(m_data.MinCornerPerlinSize.y, m_data.MaxCornerPerlinSize.y));
        randomPerlinX = Random.value * 10000;
        randomPerlinY = Random.value * 10000;
        for (int x = perlinSize.x; x > -a_cornersSize[1].x - perlinSize.x; x--)
        {
            for (int y = -perlinSize.y; y < a_cornersSize[1].y + perlinSize.y; y++)
            {
                if (Mathf.PerlinNoise(x + randomPerlinX, y + randomPerlinY) > m_data.CornerPerlinThreshold)
                {
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
                }
            }
        }

        perlinSize = new Vector2Int(
            Random.Range(m_data.MinCornerPerlinSize.x, m_data.MaxCornerPerlinSize.x),
            Random.Range(m_data.MinCornerPerlinSize.y, m_data.MaxCornerPerlinSize.y));
        randomPerlinX = Random.value * 10000;
        randomPerlinY = Random.value * 10000;
        for (int x = -perlinSize.x; x < a_cornersSize[2].x + perlinSize.x; x++)
        {
            for (int y = perlinSize.y; y > -a_cornersSize[2].y - perlinSize.y; y--)
            {
                if (Mathf.PerlinNoise(x + randomPerlinX, y + randomPerlinY) > m_data.CornerPerlinThreshold)
                {
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
                }
            }
        }

        perlinSize = new Vector2Int(
            Random.Range(m_data.MinCornerPerlinSize.x, m_data.MaxCornerPerlinSize.x),
            Random.Range(m_data.MinCornerPerlinSize.y, m_data.MaxCornerPerlinSize.y));
        randomPerlinX = Random.value * 10000;
        randomPerlinY = Random.value * 10000;
        for (int x = perlinSize.x; x > -a_cornersSize[3].x - perlinSize.x; x--)
        {
            for (int y = perlinSize.y; y > -a_cornersSize[3].y - perlinSize.y; y--)
            {
                if (Mathf.PerlinNoise(x + randomPerlinX, y + randomPerlinY) > m_data.CornerPerlinThreshold)
                {
                    m_tilemap.SetTile(new Vector3Int(x, y, 0), m_groundTile);
                }
            }
        }
    }

    private void CellularModulation()
    {
        //Sauvegarde de l'état de la map
        List<List<bool>> state = new List<List<bool>>();
        for (int x = -m_data.MaxRoomSize.x / 2; x < m_data.MaxRoomSize.x / 2; x++)
        {
            state.Add(new List<bool>());
            for (int y = -m_data.MaxRoomSize.y / 2; y < m_data.MaxRoomSize.y / 2; y++)
            {
                if (m_tilemap.GetTile(new Vector3Int(x, y, 0)))
                {
                    state[x + m_data.MaxRoomSize.x / 2].Add(true);
                }
                else state[x + m_data.MaxRoomSize.x / 2].Add(false);
            }
        }
        //Calcule du poid de chaques tile
        List<List<float>> weight = new List<List<float>>();
        for (int x = 0; x < state.Count; x++)
        {
            weight.Add(new List<float>());
            for (int y = 0; y < state[x].Count; y++)
            {
                weight[x].Add(0f);
                //List des voisin
                List<Vector2Int> neighbor = new List<Vector2Int>();
                if (x != 0) { neighbor.Add(new Vector2Int(x - 1, y)); }
                if (y != 0) { neighbor.Add(new Vector2Int(x, y - 1)); }
                if (y != state[x].Count - 1) { neighbor.Add(new Vector2Int(x, y + 1)); }
                if (x != state.Count - 1) { neighbor.Add(new Vector2Int(x + 1, y)); }

                for (int i = 0; i < neighbor.Count; i++)
                {
                    //True True
                    if (state[x][y] && state[neighbor[i].x][neighbor[i].y])
                    {
                        weight[x][y] += m_data.TrueTrue;
                    }
                    //True False
                    else if (state[x][y] && !state[neighbor[i].x][neighbor[i].y])
                    {
                        weight[x][y] += m_data.TrueFalse;
                    }
                    //False True
                    else if (!state[x][y] && state[neighbor[i].x][neighbor[i].y])
                    {
                        weight[x][y] += m_data.FalseTrue;
                    }
                    //False False
                    else
                    {
                        weight[x][y] += m_data.FalseFalse;
                    }
                }
            }
        }
        //Changement d'état
        int tileX = -m_data.MaxRoomSize.x / 2;
        int tileY = -m_data.MaxRoomSize.y / 2;
        for (int x = 0; x < state.Count; x++)
        {
            for (int y = 0; y < state[x].Count; y++)
            {
                //List des voisin
                List<Vector2Int> neighbor = new List<Vector2Int>();
                if (x != 0) { neighbor.Add(new Vector2Int(x - 1, y)); }
                if (y != 0) { neighbor.Add(new Vector2Int(x, y - 1)); }
                if (y != state[x].Count - 1) { neighbor.Add(new Vector2Int(x, y + 1)); }
                if (x != state.Count - 1) { neighbor.Add(new Vector2Int(x + 1, y)); }

                float tempWeight = weight[x][y];
                bool tempState = state[x][y];
                for (int i = 0; i < neighbor.Count; i++)
                {
                    if (weight[neighbor[i].x][neighbor[i].y] > tempWeight)
                    {
                        tempState = state[neighbor[i].x][neighbor[i].y];
                        tempWeight = weight[neighbor[i].x][neighbor[i].y];
                    }
                }
                if (tempState)
                {
                    m_tilemap.SetTile(new Vector3Int(tileX, tileY, 0), m_groundTile);
                }
                else m_tilemap.SetTile(new Vector3Int(tileX, tileY, 0), null);
                tileY++;
            }
            tileY = -m_data.MaxRoomSize.y / 2;
            tileX++;
        }
    }
}
