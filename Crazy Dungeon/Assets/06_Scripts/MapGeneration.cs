using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] private GameData m_data;
    
    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private RuleTile m_groundTile;
    public EType m_groundType;
    public EType m_wallType;

    private void Start()
    {
        GenerateRoom();
    }

    private void GenerateRoom()
    {
        //Set type of wall and ground
        SetRoomTypes();
        
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
    }

    private void SetRoomTypes()
    {
        var wallList = m_data.RuleTile.Where(r => r.obj == EObject.wall).ToList();
        var groundList = m_data.RuleTile.Where(r => r.obj == EObject.ground).ToList();
        
        int randGroundType = Random.Range(0, groundList.Count()); // random to select ground type btw id in GameData
        int randWallType = Random.Range(0, wallList.Count()); // random to select wall type btw id in GameData

        m_groundTile.m_TilingRules[0].m_Sprites[0] = groundList[randGroundType].sprite;
        m_groundTile.m_TilingRules[1].m_Sprites[0] = wallList[randWallType].sprite;

        m_groundType = groundList[randGroundType].type;
        m_wallType = wallList[randWallType].type;
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
}
