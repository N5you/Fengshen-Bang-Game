using System;
using System.Collections.Generic;
using UnityEngine;

//整体功能：
//1.文本替换 [selfName] 自己名字 [selfDuty] 自己职业
//2.人物名称 [name:小明] 弹出右键菜单
//3.物品超链接 [Owner:GUID,Item:GUID] 弹出显示物品ToolTip
//4.寻路超链接 [mapid:id,npcid:id]
//4.普通图片[Atlas:id,Sprite:name] 显示图片
//5.序列帧动画 未定
//6.字体大小 未定(需要修改，不受控件的缩放比例，需要单独的属性)
//7.战斗录像 未定
//8.活动功能链接 未
//[特殊字符串替换],不区分大小写

//深度拷贝本来打算使用序列化和反序列化，但是考虑到引擎中的类对象，NGUI类对象有引用到
//无法给他们的类添加[Serializable]标记，只得自己实现

#region ParseStringMgr
public class ParseStringMgr
{	
	private static ParseStringMgr m_this = new ParseStringMgr();
	public static  ParseStringMgr SP { get { return m_this; } }
	
	private ParseStringMgr()
	{		
		mIsInit	= false;
		mFont	= null;
		
		XEventManager.SP.AddHandler(EnterHandler,EEvent.MainPlayer_EnterGame);
	}
	
	public void EnterHandler(EEvent evt, params object[] args)
	{
		InitReplaceDict();
	}
	
	public void InitReplaceDict()
	{
		string keyName = "player";
		mReplaceDict.Add(keyName.ToLower(),XLogicWorld.SP.MainPlayer.Name);
		
		string keyDuty = "class";		
		int index = XLogicWorld.SP.MainPlayer.Class;
		mReplaceDict.Add(keyDuty.ToLower(),GlobalU3dDefine.JobName[index]);
	}
	
	public enum HyperLinkDes
	{
		HyperLink_None,
		HyperLink_UnderLine,
	}
	
	public string GetOriginalText() { return mOriginalText; }
	
	private UIFont 	mFont;
	private string 	mOriginalText;

	private Color	mCurColor	= new Color(0.0f,0.0f,0.0f,1.0f);
	private float 	mAlpha = 1.0f;
	public bool	Encoding	{get;set;}
	
	private Dictionary<string,string>	mReplaceDict	= new Dictionary<string, string>();
	
	private bool	mEnableOutLine		= false;
	private Color	mOutLineColor	= new Color(1f,1f,1f,1f);
	
	private bool	mEnableUnderLine	= false;
	private Color	mUnderLineColor = new Color(1f,1f,1f,1f);
	
	
	private bool 	mEnableShadow		= false;
	private Color	mShadowColor = new Color(1f,1f,1f,1f);
	
	private bool	mEnableHyperLink	= false;
	private Color	mHyperLinkColor;
	private string	mHyperData;
	
	private Vector2 EffectDistance = new Vector2();
	
	private bool	mIsInit;
	public GameObject mGo = null;
	
	private delegate bool HandleStrParse(RenderStr rs,string str);
	private Dictionary<string,HandleStrParse>	mElementMap	= new Dictionary<string,HandleStrParse>();
	
	#region delegate register
	private bool HandleShadow(RenderStr rs,string str)
	{
		if(string.IsNullOrEmpty(str) || (str.ToLower() != "true" && str.ToLower() != "false"))
			return false;
		
		mEnableShadow	= Convert.ToBoolean(str);		
		return true;
	}
	
	private bool HandleShadowColor(RenderStr rs,string str)
	{
		if(str.Length != 6)
			return false;
		Color color = NGUITools.ParseColor(str,0);
		if (NGUITools.EncodeColor(color) == str.ToUpper())
		{
			mShadowColor	= color;
			return true;
		}
		return false;
	}
	
	private bool HandleColor(RenderStr rs,string str)
	{
		if(str.Length != 6)
			return false;
		Color color = NGUITools.ParseColor(str,0);
		if (NGUITools.EncodeColor(color) == str.ToUpper())
		{
			mCurColor	= color;
			return true;
		}
		return false;
	}
	private bool HandleUnderLine(RenderStr rs,string str)
	{
		if(string.IsNullOrEmpty(str) || (str.ToLower() != "true" && str.ToLower() != "false"))
			return false;
		
		mEnableUnderLine	= Convert.ToBoolean(str);
		return true;
	}
	
	private bool HandleUnderLineColor(RenderStr rs,string str)
	{
		if(str.Length != 6)
			return false;
		Color color = NGUITools.ParseColor(str,0);
		if (NGUITools.EncodeColor(color) == str.ToUpper())
		{
			mUnderLineColor	= color;
			return true;
		}
		return false;
	}	
	
	private bool HandleOutLine(RenderStr rs,string str)
	{
		if(string.IsNullOrEmpty(str) || (str.ToLower() != "true" && str.ToLower() != "false"))
			return false;
		
		mEnableOutLine	= Convert.ToBoolean(str);
		return true;
	}
	
	private bool HandleOutLineColor(RenderStr rs,string str)
	{
		if(str.Length != 6)
			return false;
		Color color = NGUITools.ParseColor(str,0);
		if (NGUITools.EncodeColor(color) == str.ToUpper())
		{
			mOutLineColor	= color;
			return true;
		}
		return false;
	}	
	
	private bool HandleWidget(RenderStr rs,string str)
	{
		RenderStrWidgetComponent com = new RenderStrWidgetComponent(str,mFont);
		com.SetGameObject(mGo);
		return true;
	}
	
	private bool HandleSprite(RenderStr rs,string str)
	{
		string[] result = str.Split(new char[]{'.'});
		if(result.Length < 2)
			return false;
		
		RenderStrSpriteComponent com = new RenderStrSpriteComponent(str,mFont);
		com.Label				= rs.Label;
		com.AtlasID		= Convert.ToUInt32(result[0]);
		com.SpriteName	= result[1];
		int xwidth = 0;
		int ywidth = 0;
		if ( result.Length == 4 )
		{
			xwidth = int.Parse(result[2]);
			ywidth = int.Parse(result[3]);
		}
		com.LoadSprite(xwidth, ywidth);
		rs.AppendComponent(com);
		return true;
	}
	
	private bool HandleLinkEnable(RenderStr rs,string str)
	{
		if(string.IsNullOrEmpty(str))
			return false;
		
		HyperLinkDes des = (HyperLinkDes)Convert.ToInt16(str);
		if(des == HyperLinkDes.HyperLink_None)
		{
			mEnableHyperLink	= false;
			mEnableUnderLine	= false;
			mHyperData			= "";
		}
		else if(des == HyperLinkDes.HyperLink_UnderLine)
		{
			mEnableHyperLink	= true;
			mEnableUnderLine	= true;
		}
		
		mHyperLinkColor	= mCurColor;
		
		return true;
	}
	
	private bool HandleLinkData(RenderStr rs,string str)
	{
		mHyperData = str;
		return true;
	}
	
	private bool HandleLinkColor(RenderStr rs,string str)
	{
		if(str.Length != 6)
			return false;
		Color color = NGUITools.ParseColor(str,0);
		if (NGUITools.EncodeColor(color) == str.ToUpper())
		{
			mHyperLinkColor	= color;
			if(mEnableUnderLine)
				mUnderLineColor	= mHyperLinkColor;
			return true;
		}
		return false;
	}
	#endregion
	
	public void Parse(string input_String,UIFont font,Color color,RenderStr rs,UILabel.Effect effectType,Color effectColor,Vector2 effectDistance)
	{
		//重新初始化参数
		ResetParam();
		rs.Clear();
		
		if(string.IsNullOrEmpty(input_String) || font == null)
			return ;
		
		if(!mIsInit)
			InitParseElement();
		
		mOriginalText	= input_String;
		mFont			= font;
		mCurColor		= color;
		mAlpha			= color.a;
		
		if(effectType == UILabel.Effect.Outline)
		{
			input_String 	= "[outline=true]" + input_String;
			mOutLineColor	= effectColor;
			EffectDistance	= effectDistance;
		}
		else if(effectType == UILabel.Effect.Shadow)
		{
			input_String = "[shadow=true]" + input_String;
			mShadowColor	= effectColor;
			EffectDistance	= effectDistance;
		}
		//替换文本
		input_String = ReplaceText(input_String);
		
		string curr_section = "";
		
		int curPos = 0;
		
		//不解析的话，直接添加
		if(!Encoding)
		{
			AppendRenderedText(rs,input_String);
			return ;
		}
		
		while(curPos < input_String.Length)
		{
			int index = input_String.IndexOf('[',curPos);
			if(index == -1)
			{
				curr_section += input_String.Substring(curPos);
				curPos	= input_String.Length;
			}
			else if(index >= curPos)
			{
				curr_section += input_String.Substring(curPos,index - curPos);
				int endPos = input_String.IndexOf(']',index);
				if(endPos == -1)
				{
					curr_section = input_String.Substring(curPos);
					curPos = input_String.Length;
				}
				else
				{
					AppendRenderedText(rs,curr_section);
					curr_section = "";
					
					string sub = input_String.Substring(index + 1,endPos - index - 1);
					if(!processControlString(rs,sub))
					{
						//AppendRenderedText(rs,"[");
						//AppendRenderedText(rs,"");
						curPos = index + 1;
					}
					else
					{
						curPos = endPos + 1;
					}
					
					continue;					
				}				
			}
			else
			{
				curr_section += input_String.Substring(curPos,index - curPos - 1);
				curr_section += '['; 
				curPos = index + 1;
				continue;
			}
			
			AppendRenderedText(rs,curr_section);
			curr_section = "";
		}
		
		return ;
	}
	
	private void AppendRenderedText(RenderStr rs,string text)
	{
		int cpos = 0;
		while(text.Length > cpos)
		{
			int nlpos = text.IndexOf('\n',cpos);
			int len = (nlpos != -1 ? nlpos : text.Length) - cpos;
			if(len > 0)
			{
				RenderStrTextComponent rtc = new RenderStrTextComponent(text.Substring(cpos,len),mFont);
				rtc.FontColor 			= new Color(mCurColor.r,mCurColor.g,mCurColor.b,mAlpha);
				rtc.IsEnableOutLine		= mEnableOutLine;				
				//rtc.OutLineColor		= mOutLineColor;
				rtc.OutLineColor		= new Color(mOutLineColor.r,mOutLineColor.g,mOutLineColor.b,mAlpha);
				
				rtc.IsEnableUnderLine	= mEnableUnderLine;
				//rtc.UnderLineColor		= mUnderLineColor;
				rtc.UnderLineColor		= new Color(mUnderLineColor.r,mUnderLineColor.g,mUnderLineColor.b,mAlpha);
				
				rtc.IsEnableShadow		= mEnableShadow;
				//rtc.ShadowColor			= mShadowColor;
				rtc.ShadowColor			= new Color(mShadowColor.r,mShadowColor.g,mShadowColor.b,mAlpha);
				
				rtc.EnCoding			= Encoding;
				rtc.EffectDistance		= EffectDistance;
				
				//添加超链接
				rtc.IsEnableHyperLink	= mEnableHyperLink;
				//rtc.HyperLinkColor		= mHyperLinkColor;
				rtc.HyperLinkColor		= new Color(mHyperLinkColor.r,mHyperLinkColor.g,mHyperLinkColor.b,mAlpha);;
				rtc.HyperLinkData		= mHyperData;
				
				rtc.Label				= rs.Label;
				
				rs.AppendComponent(rtc);
			}
			if (nlpos != -1)
				rs.AppendLineBreak();
			
			cpos += len + 1;
		}
	}
	
	private bool processControlString(RenderStr rs,string text)
	{
		//所有的控制串都以{var}={value}的形式存在
		if(text.IndexOf('=') == -1)
			return false;
		
		string[] array = text.Split('=');
		if(array.Length != 2)
			return false;
		if(mElementMap.ContainsKey(array[0].ToLower()))
		{
			return mElementMap[array[0].ToLower()](rs,array[1]);
		}
		else
		{
			//日志报错
		}
		
		return false;
	}
	
	
	public string ReplaceText(string text)
	{
		int curPos = 0;
		while(curPos < text.Length)
		{
			int startPos = text.IndexOf('[',curPos);
			if(startPos == -1)
				break;
			
			int endPos = text.IndexOf(']',startPos);
			if(endPos == -1)
				break;
			
			string sub = text.Substring(startPos + 1,endPos - startPos - 1);
			
			if(!string.IsNullOrEmpty(sub) && mReplaceDict.ContainsKey(sub.ToLower()))
			{
				text = text.Replace(text.Substring(startPos,endPos - startPos+1),mReplaceDict[sub.ToLower()]);
				curPos	= mReplaceDict[sub].Length + startPos;
			}
			else
			{
				curPos = endPos + 1;
			}
		}
		
		return text;
	}
	
	//查找第一个符合的控制字符的位置,-1标示未找到
	public int FindControlStringPos(string input_String,int startPos)
	{
		if(string.IsNullOrEmpty(input_String))
			return -1;
		
		int endPos = input_String.IndexOf(']',startPos);
		if(endPos == -1)
			return -1;
		
		string sub = input_String.Substring(startPos + 1,endPos - startPos - 1);
		if(string.IsNullOrEmpty(sub))
			return -1;
		
		if(sub.IndexOf('=') == -1)
			return -1;
		
		string[] array = sub.Split('=');
		if(array.Length != 2)
			return -1;
		if(string.IsNullOrEmpty(array[1]))
			return -1;
		if(mElementMap.ContainsKey(array[0].ToLower()))
		{
			return endPos;
		}
		
		return -1;
	}
	
	private void InitParseElement()
	{
		//都要小写字符
		mElementMap.Add("outline",new HandleStrParse(HandleOutLine));
		mElementMap.Add("outlineColor",new HandleStrParse(HandleOutLineColor));
		mElementMap.Add("color",new HandleStrParse(HandleColor));
		mElementMap.Add("underline",new HandleStrParse(HandleUnderLine));
		mElementMap.Add("underlineColor",new HandleStrParse(HandleUnderLineColor));
		mElementMap.Add("shadow",new HandleStrParse(HandleShadow));
		mElementMap.Add("shadowColor",new HandleStrParse(HandleShadowColor));
		mElementMap.Add("sprite",new HandleStrParse(HandleSprite));
		
		//超链接
		mElementMap.Add("link",new HandleStrParse(HandleLinkEnable));
		mElementMap.Add("linkdata",new HandleStrParse(HandleLinkData));
		mElementMap.Add("linkcolor",new HandleStrParse(HandleLinkColor));
		
		mIsInit	= true;		
	}
	
	private void ResetParam()
	{
		mOriginalText	= "";
		mEnableOutLine	= false;
		mEnableUnderLine= false;
		mEnableHyperLink= false;
		mEnableShadow	= false;
	}
	
	
}
#endregion
