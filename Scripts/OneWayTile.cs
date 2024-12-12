using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/OneWayTile")]
public class OneWayTile : Tile
{
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            // 足場板にPlatformEffector2Dを設定
            PlatformEffector2D effector = go.GetComponent<PlatformEffector2D>();
            if (effector == null)
            {
                effector = go.AddComponent<PlatformEffector2D>();
            }

            effector.surfaceArc = 180f;  // 下から通過可能に設定
        }

        return base.StartUp(position, tilemap, go);
    }
}