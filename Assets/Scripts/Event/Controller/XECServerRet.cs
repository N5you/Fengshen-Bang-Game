using System;
using XGame.Client.Packets;
using System.Collections.Generic;
using XGame.Client.Base.Pattern;

class XECServerRet //: IEventCtrl
{
    public XECServerRet()
    {
    }

    public void Init()
    {
        XEventManager.SP.AddHandler(hd_OnServerReturn, EEvent.Msg_Server_Return);
    }
	
	public void Breathe()
	{
	}

    void hd_OnServerReturn(EEvent evt, params object[] args)
    {
        Msg_Return msg = null;
		if (args.Length < 1 || (msg = args[0] as Msg_Return) == null)
        {
			return;
		}
        Log.Write("code:{0}", msg.Code.ToString());
		//--4>TODO: 暂时直接提示错误返回的枚举 ID
		// 后面增加对不同错误进行不同处理的机制, 默认可以通过配置文件读取对应的提示内容/字典ID
        XEventManager.SP.SendEvent(EEvent.MessageBox, null, null, msg.Code.ToString());
    }
}
