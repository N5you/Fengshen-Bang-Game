namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class SingleDependAsset
	{
		public uint 	AssetID;
		public uint		Version;
		public uint		Size;
		public string	ResPath;
		public DownloadItem	DownLoad;

		//资源下载完成的通知，只应该有1次，不应该出现多次加载资源的请求。
		//对于请求一个正在加载的资源，靠外部的Resouce类来处理这种问题
		public delegate void SDALoadCompletedDelegate(DownloadItem item);
		public SDALoadCompletedDelegate LoadCompletedEvent;
		
		public  void LoadCompleted(DownloadItem item)
		{
			if(LoadCompletedEvent != null)
				LoadCompletedEvent(item);
		}
		
		public SingleDependAsset(uint id,uint version,uint size)
		{
			AssetID		= id;
			Version		= version;
			Size		= size;
		}
		
		public bool IsLoadDone()
		{
			if(DownLoad == null)
				return false;
			
			return DownLoad.IsDone;
		}
		
		public virtual void Load(string path)
		{
			DownLoad = XDownloadManager.GetDownload(path + AssetID.ToString(),(int)Version,(int)(Size),0);
			DownLoad.LoadCompletedEvent	+= LoadCompleted;
		}
	}
	
	public class SingleMainAsset : SingleDependAsset
	{		
		public uint		TotalSize;
		public string	ResName;
		
		public SingleMainAsset(uint id,uint version,string Name,uint TotalSize,uint size) : base(id,version,size)
		{
			TotalSize	= TotalSize;
			ResName		= Name;
		}
		
		public override void Load(string path)
		{
#if RES_DEBUG
			DownLoad = XDownloadManager.GetDownload(ResName,(int)Version,(int)Size,0);
			DownLoad.LoadCompletedEvent	+= LoadCompleted;			
#else
			base.Load(path);
#endif
		}
	}
	
	
	
	public class XResourceBase
	{
		public object 	Type		{get;set;}
		public SingleMainAsset		MainAsset;
		protected List<SingleDependAsset>	mDependList	= new List<SingleDependAsset>();
		
		public delegate void LoadCompletedDelegate(DownloadItem item);
		public event LoadCompletedDelegate ResLoadEvent;
		
		public string MainAssetPath;
		public string DependAssetPath;

		public bool IsLoading = false;
		
		public void AddDependAsset(uint id,uint version,uint size)
		{
			SingleDependAsset temp = new SingleDependAsset(id,version,size);			
			mDependList.Add(temp);
		}
		
		public void SetMainAsset(uint id,uint version,string Name,uint TotalSize,uint size)
		{
			MainAsset	= new SingleMainAsset(id,version,Name,TotalSize,size);	
		}
		
		public virtual bool IsLoadDone()
		{
			if(!MainAsset.IsLoadDone())
				return false;
			
			foreach(SingleDependAsset temp in mDependList)
			{
				if(!temp.IsLoadDone())
					return false;
			}
			
			return true;
		}
		
		public virtual void Load()
		{
			if(IsLoading)
				return ;

			MainAsset.Load(MainAssetPath);
			if(MainAsset.LoadCompletedEvent == null)
				MainAsset.LoadCompletedEvent	= LoadCompleted;
			else
			{
				Log.Write(LogLevel.WARN,"XResourceBase AssetID is {0}",MainAsset.AssetID);
			}

			
			foreach(SingleDependAsset temp in mDependList)
			{
				temp.Load(DependAssetPath);	
				if(temp.LoadCompletedEvent == null)
					temp.LoadCompletedEvent	= LoadCompleted;
				else
				{
					Log.Write(LogLevel.WARN,"XResourceBase AssetID is {0}",MainAsset.AssetID);
				}			
			}
		}
		
		public void LoadCompleted(DownloadItem item)
		{
			if(IsLoadDone())
			{
#if RES_DEBUG
#else
				foreach(SingleDependAsset temp in mDependList)
				{
					//temp.DownLoad.ab.LoadAll();
				}
				//MainAsset.DownLoad.ab.LoadAll();
#endif
				if(ResLoadEvent != null)
					ResLoadEvent(MainAsset.DownLoad);
				else
				{
					Log.Write(LogLevel.WARN,"LoadCompleted is NULL AssetID is {0}",MainAsset.AssetID);
				}

			}
	}					
		public List<DownloadItem>	GatherDownloadItem()
		{
			List<DownloadItem> list = new List<DownloadItem>();
			list.Add(MainAsset.DownLoad);
			foreach(SingleDependAsset temp in mDependList)
			{
				list.Add(temp.DownLoad);
			}
			
			return list;
		}
	}
}
