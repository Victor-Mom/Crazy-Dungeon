using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    private const float MAX_SEED = 1000f;
    private float m_seed;

    public PerlinNoise()
    {
        Reseed();
    }

    public void Reseed()
    {
        m_seed = Random.value * MAX_SEED;
    }

    public float GetValueAt(int x, int y)
    {
        return Mathf.PerlinNoise(x + m_seed, y + m_seed);
    }

    public List<List<float>> GetValuesInRange(Vector2Int a_start, Vector2Int a_end)
    {
        List<List<float>> valuesInRange = new List<List<float>>();
        
        for (int x = a_start.x; x < a_end.x; x++)
        {
            valuesInRange.Add(new List<float>());
            for (int y = a_start.y; y < a_end.y; y++)
            {
                valuesInRange[x].Add(Mathf.PerlinNoise(x + m_seed, y + m_seed));
            }
        }

        return valuesInRange;
    }

    public List<List<float>> GetValuesInRange(int a_xStart, int a_yStart, int a_xEnd, int a_yEnd) =>
        GetValuesInRange(new Vector2Int(a_xStart, a_yStart), new Vector2Int(a_xEnd, a_yEnd));
}