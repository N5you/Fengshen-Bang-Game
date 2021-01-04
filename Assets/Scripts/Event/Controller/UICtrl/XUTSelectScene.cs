using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class XUTSelectScene : XUICtrlTemplate<XSelectScene>
{
	private class SceneInfo
	{
		public SceneInfo(int nIndex, uint PassID,string strName, bool bLock,int sceneLevel,int starLevel)
		{
			Index = nIndex;
			this.PassID	= PassID;
			Name = strName;
			Lock = bLock;
			SceneLevel	= sceneLevel;
			StarLevel	= starLevel;
		}
		
		public int Index;
		public uint PassID;
		public string Name;
		public bool Lock;
		public int  SceneLevel;
		public int  StarLevel;
	}
	
	public static XTransPoint TransPoint = null;
	
	private List<SceneInfo> m_HoldScenInfo;
	
	public XUTSelectScene()
	{
		m_HoldScenInfo = new List<SceneInfo>();
		//XEventManager.SP.AddHandler(Clear, EEvent.SelectScene_Clear);
		//XEventManager.SP.AddHandler(SetName, EEvent.SelectScene_SetName);
		//XEventManager.SP.AddHandler(AddScene, EEvent.SelectScene_AddScene);
		//XEventManager.SP.AddHandler(ChooseScene, EEvent.SelectScene_ChooseScene);
	}
	
	public override void OnCreated(object arg)
	{
		base.OnCreated(arg);
		for(int i=0; i<m_HoldScenInfo.Count; i++)
		{
			SceneInfo info = m_HoldScenInfo[i];;
			LogicUI.AddScene(info.Index,info.PassID,info.Name,info.Lock,info.SceneLevel,info.StarLevel);
		}
		m_HoldScenInfo.Clear();
	}
	
	private void Clear(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			return;
		}
		LogicUI.Clear();
	}
	
	private void SetName(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			return;
		}
		LogicUI.SetName((string)args[0]);
	}
	
	private void AddScene(EEvent evt, params object[] args)
	{
		if(null == LogicUI)
		{
			m_HoldScenInfo.Add(new SceneInfo((int)(args[0]),(uint)(args[1]),(string)(args[2]),(bool)(args[3]),(int)(args[4]),(int)(args[5])));
			return;
		}
		//index passID,name isLock SceneLevel starLevel
		LogicUI.AddScene((int)(args[0]),(uint)(args[1]),(string)(args[2]),(bool)(args[3]),(int)(args[4]),(int)(args[5]));
	}
	
	private void ChooseScene(EEvent evt, params object[] args)
	{
		if(null != TransPoint)
		{
			TransPoint.OnChooseScene((int)(args[0]),(int)(args[1]));
		}
	}
}

