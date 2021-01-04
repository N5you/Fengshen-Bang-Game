using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 采集物
public class XGatherObject : XGameObject
{
	private static SortedList<int, XGatherObject> m_allGatherObject = new SortedList<int, XGatherObject>();
	public static XGatherObject GetByStaticId(int id)
	{
		if(!m_allGatherObject.ContainsKey(id))
			return null;
		return m_allGatherObject[id];
	}
	
	internal XCfgGatherObject m_cfgGatherObject;
	
	public XGatherObject(ulong id)
		: base(id)
	{
		ObjectType = EObjectType.GatherObject;
		m_cfgGatherObject = null;
	}
	
	public override void Appear ()
	{
		base.Appear ();
		if(!m_allGatherObject.ContainsKey(m_cfgGatherObject.ID))
			m_allGatherObject.Add(m_cfgGatherObject.ID, this);
	}
	
	public  override void OnModelLoaded()
	{
		base.OnModelLoaded();
		m_ObjectModel.AddBehaviourListener(EBehaviourType.e_BehaviourType_GatherObject,this);
	}
	
	public override void SetAppearData (object data)
	{
		XGatherObjectAppearInfo info = data as XGatherObjectAppearInfo;
		if(null == info) return;
		m_cfgGatherObject = XCfgGatherObjectMgr.SP.GetConfig(info.GatherObjectId);
		if(null == m_cfgGatherObject)
		{
			Log.Write(LogLevel.WARN, "XGatherObject, can not find config of id : {0}", info.GatherObjectId);
			return;
		}
		Vector3 pos = info.Position;
		XLogicWorld.SP.SceneManager.FixTerrainHeight(ref pos);
		Position = pos;
		Direction = info.Direction;
		ModelId = m_cfgGatherObject.ModelId;
		Name = m_cfgGatherObject.Name;
	}
	
	public override void DisAppear ()
	{
		base.DisAppear ();
		if(m_allGatherObject.ContainsKey(m_cfgGatherObject.ID))
			m_allGatherObject.Remove(m_cfgGatherObject.ID);
	}
	
	public override float GetClickDistance ()
	{
		float d = (null == m_cfgGatherObject) ? 0 : m_cfgGatherObject.NeedDistance;
		return Radius() + XLogicWorld.SP.MainPlayer.Radius() + d;
	}
	
	public override void OnMouseUpAsButton (int mouseCode)
	{
		if(0 != mouseCode) return;
		Vector3 mpPos = XLogicWorld.SP.MainPlayer.Position;
		float distance = XUtil.CalcDistanceXZ(Position, mpPos);

		if(distance > GetClickDistance())
		{
			XLogicWorld.SP.MainPlayer.AutoMoveTo(this);
		}
		else
		{
			XLogicWorld.SP.MainPlayer.Rotato(this.Position);
			XProductManager.SP.ApplyGatherObject(m_cfgGatherObject, true);
		}
	}
}

