using UnityEngine;
using resource;

public enum Cursor_Type
{
	Cursor_Type_None		= 0,
	Cursor_Type_Link		= 1,
	Cursor_Type_Seal		= 2,
	Cursor_Type_Split		= 3,
	Cursor_Type_Num			= 4,
}

public class CursorMgr
{
	private static CursorMgr m_this = new CursorMgr();
    public static CursorMgr SP { get { return m_this; } }
	private Cursor_Type CurType;
	
	
	public void SetCurSor(Cursor_Type type)
	{
		if(type == Cursor_Type.Cursor_Type_None)
			Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
		else
		{
			XResourceCursor resCursor = XResourceManager.GetResource(XResourceCursor.ResTypeName,(uint)type) as XResourceCursor;
			if(resCursor == null)
			{
				Log.Write(LogLevel.ERROR,"cant find CurSor ID {0}",(uint)type);
				return ;
			}
			if(resCursor.IsLoadDone())
			{
				Cursor.SetCursor(resCursor.MainAsset.DownLoad.go as Texture2D,Vector2.zero,CursorMode.Auto);
			}
			else
			{
				XResourceManager.StartLoadResource(XResourceCursor.ResTypeName,(uint)type);
				resCursor.ResLoadEvent	+= LoadCursorCompleted;
			}			
		}
		
		CurType	= type;
			
	}
	
	public void LoadCursorCompleted(DownloadItem item)
	{
		SetCurSor(CurType);
	}
}
