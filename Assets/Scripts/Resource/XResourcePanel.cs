namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	
	public class XResourcePanel : XResourceBase
	{
		public static string ResTypeName	= "UIPanel";
		public EUIPanel		CurPanel;
		public Transform	ParentTF;
		public GameObject	PanelObject;
		public bool			IsAddEvent = false;
		private ArrayList	mArrayList = new ArrayList();
		
		public void AddPanelKey(uint key)
		{
			mArrayList.Add(key);
		}
		
		public static void Register()
		{
			XResourceManager.AddResourceType(ResTypeName,typeof(XResourcePanel));
		}
		
		public  void CreateUI(DownloadItem item)
		{
#if RES_DEBUG
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
			if(PanelObject != null && mArrayList.Count == 0)
				return ;
			
			if(mArrayList.Count == 0)
			{
				PanelObject = XUtil.Instantiate(go, ParentTF, go.transform.localPosition, go.transform.localRotation.eulerAngles);
		        if (null == PanelObject)
		        {
		            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, 生成UI: {0} 时出错了", CurPanel.ToString());
		            return;
		        }
		        PanelObject.SetActive(false);
		        XUIBaseLogic uiBaseLogic = PanelObject.GetComponent<XUIBaseLogic>();
		        if (null == uiBaseLogic)
		        {
		            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, UI: {0} 没有XBaseLogic", CurPanel.ToString());
		            return;
		        }
				
				XEventManager.SP.SendEvent(EEvent.UI_OnCreated, uiBaseLogic, CurPanel,(uint)0);
			}
			else
			{
				for(int i = 0; i < mArrayList.Count; i++)
				{
					GameObject newPanelObject = XUtil.Instantiate(go, ParentTF, go.transform.localPosition, go.transform.localRotation.eulerAngles);
			        if (null == newPanelObject)
			        {
			            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, 生成UI: {0} 时出错了", CurPanel.ToString());
			            return;
			        }
			        newPanelObject.SetActive(false);
			        XUIBaseLogic uiBaseLogic = newPanelObject.GetComponent<XUIBaseLogic>();
			        if (null == uiBaseLogic)
			        {
			            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, UI: {0} 没有XBaseLogic", CurPanel.ToString());
			            return;
			        }
					
					uiBaseLogic.PanelKey	= (uint)mArrayList[i];
					XEventManager.SP.SendEvent(EEvent.UI_OnCreated, uiBaseLogic, CurPanel,(uint)mArrayList[i]);
				}
				mArrayList.Clear();
			}
			
//	        PanelObject = XUtil.Instantiate(go, ParentTF, go.transform.localPosition, go.transform.localRotation.eulerAngles);
//	        if (null == PanelObject)
//	        {
//	            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, 生成UI: {0} 时出错了", CurPanel.ToString());
//	            return;
//	        }
//	        PanelObject.SetActive(false);
//	        XUIBaseLogic uiBaseLogic = PanelObject.GetComponent<XUIBaseLogic>();
//	        if (null == uiBaseLogic)
//	        {
//	            Log.Write(LogLevel.ERROR, "[ERROR] XWeUIRoot, UI: {0} 没有XBaseLogic", CurPanel.ToString());
//	            return;
//	        }
			
//			if(mArrayList.Count == 0)
//			{
//				XEventManager.SP.SendEvent(EEvent.UI_OnCreated, uiBaseLogic, CurPanel,(uint)0);
//			}
//			else
//			{
//				for(int i = 0; i < mArrayList.Count; i++)
//				{
//					uiBaseLogic.PanelKey	= (uint)mArrayList[i];
//					XEventManager.SP.SendEvent(EEvent.UI_OnCreated, uiBaseLogic, CurPanel,(uint)mArrayList[i]);
//				}
//				mArrayList.Clear();
//			}
			
		}
		
		public  void OriginalUI(DownloadItem item)
		{
#if RES_DEBUG
			GameObject go = item.go as GameObject;
#else
			GameObject go = item.ab.mainAsset as GameObject;
#endif
			XEventManager.SP.SendEvent(EEvent.UI_OnOriginal, go.GetComponent<XUIBaseLogic>(), CurPanel);
		}
		
		public override void Load()
		{
			MainAssetPath	= ServiceInfo.res_path + ResTypeName + '/';
			DependAssetPath	= ServiceInfo.res_path + "Shared/Shared";
			
			base.Load();
		}
		
	}
}

