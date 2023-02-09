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
}
