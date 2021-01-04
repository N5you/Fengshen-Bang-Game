using UnityEngine;
using System.Collections;
using XGame.Client.Base.Pattern;

public class XStringManager : XSingleton<XStringManager> ,IConfigManager
{
	private Hashtable m_idstring;
	
	public XStringManager()
	{
		m_idstring = new Hashtable();
	}
	
	public bool Init(TextAsset text)
	{
//		for(int i=0; i<StaticResourceManager.SP.TextIDString.Length; i++)
//		{
//			TextAsset textAsset = StaticResourceManager.SP.TextIDString[i];
//			if(null == textAsset) continue;
//			TabFile tabFile = new TabFile(textAsset.name, textAsset.text);
//			while(tabFile.Next())
//			{
//				uint id = tabFile.Get<uint>("id");
//				string str = tabFile.Get<string>("String");
//				m_idstring.Add(id, str);
//			}
//		}
//		return true;
		
		TabFile tabFile = new TabFile(text.name, text.text);
		while(tabFile.Next())
		{
			uint id = tabFile.Get<uint>("id");
			string str = tabFile.Get<string>("String");
			if(m_idstring.ContainsKey(id))
			{
				Log.Write(LogLevel.ERROR, "[ERROR] Failed to init XStringManager id is {0}", id);
                continue;
			}
			m_idstring.Add(id, str);
		}
		
		return true;
	}
	
	public string GetString(uint id)
	{
		if(m_idstring.ContainsKey(id))
			return (string)m_idstring[id];
		return "-";
	}
}
