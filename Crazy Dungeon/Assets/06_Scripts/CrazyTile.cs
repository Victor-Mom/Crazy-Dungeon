using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "CrazyDungeon/Crazy Tile")]
public class CrazyTile : RuleTile
{
    [SerializeField] private TileBase m_doorTile;

    private EGroundEffect m_currentGroundEffect;
    private EWallEffect m_currentWallEffect;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other == m_doorTile)
        {
            other = this;
        }

        return base.RuleMatch(neighbor, other);
    }

    public void SetEffects(EGroundEffect a_groundEffect, EWallEffect a_wallEffect)
    {
        m_currentGroundEffect = a_groundEffect;
        m_currentWallEffect = a_wallEffect;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref UnityEngine.Tilemaps.TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        Matrix4x4 transform = Matrix4x4.identity;
        foreach (TilingRule rule in m_TilingRules)
        {
            if (RuleMatches(rule, position, tilemap, ref transform))
            {
                
            }
        }
    }
}
