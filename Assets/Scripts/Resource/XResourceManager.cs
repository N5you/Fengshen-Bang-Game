namespace resource
{
    using System;
	using System.Xml;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using UnityEngine;

    public static class XResourceManager
    {
		//private static Dictionary<uint, ObjectInfo> caches = new Dictionary<int, ObjectInfo>();
		
		//private static SortedList<uint, XDynamicGameObject>[] m_dynGo;
		//private static SortedList<uint, XDynamicLevel> m_levels;
		//private static SortedList<uint, XDynamicEffect> m_effects;
		
		//resourceTypeName --- <resourceID,classType>
		private static SortedList<string,SortedList<uint,XResourceBase>>	mResourceList	= new SortedList<string, SortedList<uint, XResourceBase>>();
		private static SortedList<string,object>	mName2TypeList = new SortedList<string, object>();
	
		static XResourceManager()
		{
//			m_dynGo = new SortedList<int, XDynamicGameObject>[(int)EDynGameObjectType.count];
//			for(int i=0; i<(int)EDynGameObjectType.count; i++)
//			{
//				m_dynGo[i] = new SortedList<int, XDynamicGameObject>();
//			}
//			m_levels = new SortedList<int, XDynamicLevel>();
//			m_effects = new SortedList<int, XDynamicEffect>();
		}
		
        private class ObjectInfo
        {
            public string frameinfo;
            public int id;
            public string name;
            public WeakReference objref;
            public string time;

            public ObjectInfo(UnityEngine.Object obj)
            {
                this.id = obj.GetInstanceID();
                this.name = obj.name;
                this.time = DateTime.Now.ToShortTimeString();
                this.frameinfo = MyExpception.GetFramesDesc(3, 5);
                this.objref = new WeakReference(obj);
            }
        }
		
		public static void Init()
		{
			XResourcePanel.Register();
			XResourceEffect.Register();
			XResourceTextAsset.Register();
			XResourceModel.Register();
			XResourceScene.Register();
			XResourceAtlas.Register();
			XResourceMaterial.Register();
			XResourceAudio.Register();
			XResourceCursor.Register();
		}
		
		public static void AddResourceType(string TypeName,Type t)
		{
			if(mResourceList.ContainsKey(TypeName))
			{
				Log.Write(LogLevel.ERROR,"add TypeName {0} failed",TypeName);
				return ;
			}
			
			mResourceList.Add(TypeName,new SortedList<uint,XResourceBase>());
			mName2TypeList.Add(TypeName,t);
		}
		
		public static XResourceBase	GetResource(string typeName,uint resourceID)
		{
			if(!mResourceList.ContainsKey(typeName))
			{
				return null;
			}
			
			SortedList<uint,XResourceBase>	mgr = mResourceList[typeName];
			if(!mgr.ContainsKey(resourceID))
				return null;
			
			XResourceBase	res = mgr[resourceID];			
			return res;
		}

		public static XResourceBase StartLoadResource(string typeName,uint resourceID)
		{
			if(!mResourceList.ContainsKey(typeName))
				return null;
			
			SortedList<uint,XResourceBase>	mgr = mResourceList[typeName];
			if(!mgr.ContainsKey(resourceID))
				return null;
			
			XResourceBase	res = mgr[resourceID];
			if(res.IsLoading)
				return res;

			if(!res.IsLoadDone())
				res.Load();

			res.IsLoading = true;
			
			return res;
		}
		
		public static bool ResourceIsLoaded(string typeName,uint resourceID)
		{
			if(!mResourceList.ContainsKey(typeName))
				return true;
			
			SortedList<uint,XResourceBase>	mgr = mResourceList[typeName];
			if(!mgr.ContainsKey(resourceID))
				return true;
			
			XResourceBase	res = mgr[resourceID];
			return res.IsLoadDone();
		}
		
		public static SortedList<uint,XResourceBase>	GetAllResourceByType(string typeName)
		{
			if(!mResourceList.ContainsKey(typeName))
				return null;
			
			return mResourceList[typeName];
		}
		
		public static void InitSingleResourceConfig(TextAsset textAsset)
		{
			TabFile tabFile = new TabFile(textAsset.name, textAsset.text);
			if(!mResourceList.ContainsKey(textAsset.name))
			{
				Log.Write(LogLevel.ERROR,"cant find res Type {0}",textAsset.name);
				return ;
			}
			
			if(!mName2TypeList.ContainsKey(textAsset.name))
			{
				Log.Write(LogLevel.ERROR,"cant find Class Type {0}",textAsset.name);
				return ;
			}
			
			SortedList<uint,XResourceBase>	mgr = mResourceList[textAsset.name];
			System.Object classType = mName2TypeList[textAsset.name];
			
			while(tabFile.Next())
			{
				int id = tabFile.Get<int>("id");
				string filePath = tabFile.Get<string>("FilePath");
				
				System.Type type1 	= classType as System.Type;
				object newObj = Activator.CreateInstance(type1);				
				XResourceBase go = newObj as XResourceBase;
				if(go == null)
					continue;
				
				go.Type 		= classType;
				go.SetMainAsset((uint)id,0,filePath,0,0);
				
				mgr.Add((uint)id,go);
			}
		}
		
		public static void InitWWWResourceConfig(TextAsset flie)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(flie.text);
			XmlNode root = doc.SelectSingleNode("AssetList");			
			
			foreach(KeyValuePair<string,object> temp in  mName2TypeList)
			{				
				XmlNode goRoot = root.SelectSingleNode(temp.Key);
				if(goRoot == null)
					continue;
				
				string key = temp.Key;				
				XmlNodeList goList = goRoot.SelectNodes("Asset");
				foreach(XmlNode goNode in goList)
				{
					XmlElement goEle 	= (XmlElement)goNode;
					uint id 			= Convert.ToUInt32(goEle.GetAttribute("Id"));
					uint version 		= Convert.ToUInt32(goEle.GetAttribute("Version"));
					uint TotalSize		= (uint)(Convert.ToDouble(goEle.GetAttribute("TotalSize")) * 1024.0f);
					uint Size			= (uint)(Convert.ToDouble(goEle.GetAttribute("Size")) * 1024.0f);
					string Name			= goEle.GetAttribute("Name");
					
					System.Type type1 	= temp.Value as System.Type;
					object newObj = Activator.CreateInstance(type1);
					SortedList<uint,XResourceBase> mgr = mResourceList[key];
					XResourceBase go = newObj as XResourceBase;
					if(go == null)
						continue;
					
					go.Type 		= mResourceList[temp.Key];
					go.SetMainAsset(id,version,Name,TotalSize,Size);
					
					XmlNodeList depList = goNode.SelectNodes("Depend");
					foreach(XmlNode depNode in depList)
					{
						XmlElement depEle 	= (XmlElement)depNode;
						uint AssetID		= Convert.ToUInt32(depEle.GetAttribute("Id"));
						uint Version		= Convert.ToUInt32(depEle.GetAttribute("Version"));
						uint DepSize		= (uint)(Convert.ToDouble(depEle.GetAttribute("Size")) * 1024.0f);
						go.AddDependAsset(AssetID,Version,DepSize);
					}
					
					mgr.Add(id,go);
				}
			}
			
			XEventManager.SP.SendEvent(EEvent.ResourceFileReady);
		}
    }
}


