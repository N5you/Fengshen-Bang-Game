using UnityEngine;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;
using resource;

/* UIAtlas和DynamicObjectManager的中间层
 * 注: UIAtlas一旦加载, 游戏过程中不卸载
 */
public class XUIDynamicAtlas : XSingleton<XUIDynamicAtlas>
{
	public class SpriteOper
	{
		public delegate void OnDone();
		
		public SpriteOper(UISprite s, string str)
			: this(s, str, false, null)
		{
			
		}
		
		public SpriteOper(UISprite s, string str, bool bResetSize, OnDone o)
		{
			sprite = s;
			spriteName = str;
			resetSize = bResetSize;
			onDone = o;
		}
		
		public UISprite sprite = null;
		public string spriteName = string.Empty;
		public bool resetSize = false;
		public OnDone onDone;
	}
	
	private SortedList<int, UIAtlas> m_doneAtlas = new SortedList<int, UIAtlas>();
	private SortedList<int, List<SpriteOper>> m_waitSprite = new SortedList<int, List<SpriteOper>>();
	private List<XResourceAtlas> m_atlas = new List<XResourceAtlas>();		// keep DynamicAtlas的索引
	
	public void SetSprite(UISprite sprite, int nAtlasId, string spriteName)
	{
		SetSprite(sprite, nAtlasId, spriteName, false, null);
	}
	
	public void SetSprite(UISprite sprite, int nAtlasId, string spriteName, bool resetSize, SpriteOper.OnDone onDone)
	{
		if(null == sprite) 
			return;
		
		if(m_doneAtlas.ContainsKey(nAtlasId))
		{
			sprite.atlas = m_doneAtlas[nAtlasId];
			sprite.spriteName = spriteName;
			if(resetSize) sprite.ResetSize();
			if(null != onDone) onDone();
			return;
		}
		if(!m_waitSprite.ContainsKey(nAtlasId))
		{
			m_waitSprite.Add(nAtlasId, new List<SpriteOper>());
			XResourceAtlas resAtlas = XResourceManager.GetResource(XResourceAtlas.ResTypeName,(uint)nAtlasId) as XResourceAtlas;
			if(resAtlas == null)
			{
				Log.Write(LogLevel.ERROR,"cant find Atlas {0}",nAtlasId);
				return ;
			}
			resAtlas.AtlasID	= nAtlasId;
			if(resAtlas.IsLoadDone())
				resAtlas.LoadCompleted(resAtlas.MainAsset.DownLoad);
			else
			{
				XResourceManager.StartLoadResource(XResourceAtlas.ResTypeName,(uint)nAtlasId);
				resAtlas.ResLoadEvent	+= new XResourceBase.LoadCompletedDelegate(resAtlas.LoadCompleted);
				m_atlas.Add(resAtlas);
			}
		}
		m_waitSprite[nAtlasId].Add(new SpriteOper(sprite, spriteName, resetSize, onDone));
	}
	
	public void onAtlasDone(int nId, GameObject go)
	{
		if(m_doneAtlas.ContainsKey(nId) || !m_waitSprite.ContainsKey(nId))
		{
			Log.Write(LogLevel.WARN, "XUIDynamicAtlas, 逻辑出现错误, 资源管理器提供了一个不关心的Atlas, 将出现不可预计的错误 {0}", nId);
			return;
		}
		UIAtlas atlas = go.GetComponent<UIAtlas>();
		m_doneAtlas.Add(nId, atlas);
		List<SpriteOper> list = m_waitSprite[nId];
		for(int i=0; i<list.Count; i++)
		{
			SpriteOper so = list[i];
			if(null == so.sprite) continue;
			so.sprite.atlas = atlas;
			so.sprite.spriteName = so.spriteName;
			if(so.resetSize) so.sprite.ResetSize();
			if(null != so.onDone) so.onDone();
		}
		m_waitSprite.Remove(nId);
	}
			
	private void onAtlasError(int nId)
	{
		Log.Write(LogLevel.WARN, "XUIDynamicAtlas, 资源管理器返回了一个错误Atlas, {0}", nId);
		m_waitSprite.Remove(nId);
	}
}

