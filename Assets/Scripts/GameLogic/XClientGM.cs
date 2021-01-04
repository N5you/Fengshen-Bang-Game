using System;
using UnityEngine;
using XGame.Client.Base.Pattern;
using System.Collections;
using System.Reflection;
using XGame.Client.Packets;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
class XGmHandlerAttribute: Attribute
{
    public String CMD { get; set; }

    public XGmHandlerAttribute(String cmd)
    {
        CMD = cmd.ToLower();
    }
}

public class XClientGM : XSingleton<XClientGM>
{
    enum ECmdIndex
    {
        Prefix = 0,
        Cmd = 1,
        ParamBegin = 2,
    }

    static readonly string CLIENT_GM_PREFIX = "@xgm";

    private Hashtable m_GMHandler;

    public XClientGM()
    {
        m_GMHandler = new Hashtable();

        // 注册所有指令处理函数(要求必须为 public)
        foreach (MethodInfo info in this.GetType().GetMethods())
        {
            foreach (Attribute attr in Attribute.GetCustomAttributes(info, typeof (XGmHandlerAttribute)))
            {
                XGmHandlerAttribute handler = attr as XGmHandlerAttribute;
                if (m_GMHandler.Contains(handler.CMD))
                {
                    Log.Write(LogLevel.ERROR, "XClienGM: failed to reg cmd handler:{0}", handler.CMD);
                    continue;
                }
                m_GMHandler.Add(handler.CMD, info);
            }
        }
    }

    public bool DoCmd(string data)
    {
        string[] strs = data.ToLower().Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        if (strs.Length < (int)ECmdIndex.ParamBegin || strs[(int)ECmdIndex.Prefix] != CLIENT_GM_PREFIX || m_GMHandler.Contains(strs[(int)ECmdIndex.Cmd]) == false)
        {
            return false;
        }
        MethodInfo info = m_GMHandler[strs[(int)ECmdIndex.Cmd]] as MethodInfo;
        if (null == info)
        {
            return false;
        }
        info.Invoke(this, new object[] { strs });
        return true;
    }

    #region GM Handlers

    [XGmHandler("LeaveClientScene")]
    public void gm_LeaveClientScene(object[] args)
    {
        //XLogicWorld.SP.NetManager.LeaveClientScene();
    }

    [XGmHandler("Test")]
    public void gm_Test(object[] args)
    {
        for (int i = (int)ECmdIndex.ParamBegin; i < args.Length; ++i)
        {
            Log.Write(args[i].ToString());
        }
    }

	[XGmHandler("QuickBattle")]
    public void gm_QuickBattle(object[] args)
    {
        if (args.Length > 2)
        {
            int value = 0;
            if (int.TryParse(args[2].ToString(), out value))
            {
                //XLogicWorld.SP.SubSceneManager.IsQuickBattleMode = value > 0;
            }
        }
    }

    [XGmHandler("AllAuction")]
    public void gm_AllAuction(object[] args)
    {
        XLogicWorld.SP.NetManager.SendEmptyPacket((int)XGame.Client.Packets.CS_Protocol.eCS_AllAuction);
    }

    [XGmHandler("MyAuction")]
    public void gm_MyAuction(object[] args)
    {
        XLogicWorld.SP.NetManager.SendEmptyPacket((int)XGame.Client.Packets.CS_Protocol.eCS_MyAuction);
    }
	
	[XGmHandler("TestScale")]
	public void gm_TestScale(object[] args)
	{
		if(args.Length < 3)
			return;
		float value = 0;
        if (float.TryParse(args[2].ToString(), out value))
		{
			XLogicWorld.SP.MainPlayer.Scale = value;
		}
	}
	
	[XGmHandler("TestNavigate")]
	public void gm_TestNavigate(object[] args)
	{
		//temp:
		CS_Chat.Builder builder = CS_Chat.CreateBuilder();
        builder.SetChatData("[link=1][linkdata=T(3)D(100201,2,1,60,15,60)]导航到npc测试[link=0]");
        //--4>TODO: 临时设置默认为世界聊天
        builder.SetChatType(EChatType.eChatSystem_Notice);
        XLogicWorld.SP.NetManager.SendDataToServer((int)CS_Protocol.eCS_Chat, builder.Build());
	}
    #endregion
}
