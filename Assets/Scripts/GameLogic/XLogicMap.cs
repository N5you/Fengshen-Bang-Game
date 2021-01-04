/*using UnityEngine;
using System.Collections;
using XGame.Client.Base.Map;

public class XMapData
{
    /// <summary>
    /// 地形扫描时考虑的 layer
    /// </summary>
    //public static readonly int ScanLayerMask = 1 << 2;

    public static readonly float MAX_HEIGHT = 4444.0f;

    /// <summary>
    /// 射线检测的高度
    /// 以后需要修改, 考虑记录镂空洞穴的高度?
    /// </summary>
    //public static readonly float ScanRayHeight = 4.0f;

    /// <summary>
    /// 获得地形高度和障碍等信息
    /// 目前设计的是精确到米, 如果以后要扩展细化需要和服务器的逻辑结合
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="cellMap">The cell map.</param>
    /// <param name="nCellNumX">The n cell num X.</param>
    /// <param name="nCellNumZ">The n cell num Z.</param>
	public static void GetTerrainCell(Terrain terrain, out XCellData[,] cellMap, out int nCellNumX, out int nCellNumZ, IBehaviourListener listener)
	{
        float fMapSizeX = terrain.terrainData.size.x;
        float fMapSizeZ = terrain.terrainData.size.z;
		nCellNumX = Mathf.CeilToInt(fMapSizeX);
		nCellNumZ = Mathf.CeilToInt(fMapSizeZ);
		
		cellMap = new XCellData[nCellNumX, nCellNumZ];
        for (int x = 0; x < nCellNumX; ++x)
        {
            for (int z = 0; z < nCellNumZ; ++z)
            {
                cellMap[x, z] = new XCellData();
                cellMap[x, z].Height = terrain.SampleHeight(new Vector3(x, 0.0f, z));
                cellMap[x, z].BarrierType = 0;
            }
        }

        // 获得所有阻挡包围盒 Wall, 设置 layer
        Component[] allCollider = terrain.GetComponentsInChildren(typeof(Collider));
        foreach (Component c in allCollider)
        {
            if (c.tag == GlobalU3dDefine.Tag_Block)
            {
                c.gameObject.layer = GlobalU3dDefine.Layer_Default;
            }
			if (c.tag == GlobalU3dDefine.Tag_Road)
			{
				c.gameObject.layer = GlobalU3dDefine.Layer_Decal;
				XBehaviour behaviour = c.gameObject.AddComponent<XBehaviour>();
				behaviour.BehaType = EBehaviourType.e_BehaviourType_Terrain;
				behaviour.AddListener(listener);
			}
            // 将所有包围盒, 路面, 障碍物的显示效果隐藏
            if (c.gameObject.renderer != null)
            {
                c.gameObject.renderer.enabled = false;
            }
        }

        // 射线检测所有障碍物, 并设置中间点高度
        RaycastHit hitInfo;
        Vector3 origin = new Vector3(0.0f, MAX_HEIGHT, 0.0f);
        for (int cx = 0; cx < nCellNumX; ++cx)
        {
            for (int cz = 0; cz < nCellNumZ; ++cz)
            {
                bool bNoBlock = true;
                // 9 点检测有点慢, 而且过于精细不一定好
                for (origin.x = cx + 0.1f; bNoBlock && origin.x < cx + 1.0f; origin.x += 0.4f)
                {
                    for (origin.z = cz + 0.1f; bNoBlock && origin.z < cz + 1.0f; origin.z += 0.4f)
                    {
                        if (Physics.Raycast(origin, Vector3.down, out hitInfo))
                        {
                            if (hitInfo.transform.tag == GlobalU3dDefine.Tag_Block)
                            {
                                cellMap[cx, cz].BarrierType = 1;
                                bNoBlock = false;
                            }
                        }
                    }
                }
                // 高度设置
                if (bNoBlock)
                {
                    origin.x = cx + 0.5f;
                    origin.z = cz + 0.5f;
                    origin.y = MAX_HEIGHT;
                    if (Physics.Raycast(origin, Vector3.down, out hitInfo))
                    {
                        if (hitInfo.transform.tag == GlobalU3dDefine.Tag_Block)
                        {
                            cellMap[cx, cz].BarrierType = 1;
                        }
                        else
                        {
                            cellMap[cx, cz].Height = hitInfo.point.y;
                        }
                    }
                }
            }
        }

        // 充值阻挡包围盒的 layer
        foreach (Component c in allCollider)
        {
            if (c.tag == GlobalU3dDefine.Tag_Block)
            {
                c.gameObject.layer = GlobalU3dDefine.Layer_IgnoreRaycast;
            }
        }
		
		terrain.gameObject.layer = GlobalU3dDefine.Layer_Decal;
		XBehaviour beha = terrain.gameObject.AddComponent<XBehaviour>();
		beha.BehaType = EBehaviourType.e_BehaviourType_Terrain;
		beha.AddListener(listener);
	}
}
*/
