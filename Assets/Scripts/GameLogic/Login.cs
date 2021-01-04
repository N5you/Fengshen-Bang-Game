using System;
using System.Collections;
using UnityEngine;
using XGame.Client.Packets;
using resource;

enum ServerConnType
{
	enum_None_Server,
	enum_Login_Server,
    enum_Char_Server,
	enum_Game_Server,
}

enum CheckPasswordResult	// 账号密码查询结果
{
	CPR_DBOException,		// 数据库操作异常
	CPR_NoAccount,			// 没找到用户
	CPR_WrongPassword,		// 存在账户名但是密码不正确
	CPR_Right,				// 账户密码正确
	CPR_PassportDisconnect,	// 与PassportServer断开连接
	CPR_PassportNotReady,	// PassportServer没准备好
};


public class ServerInfo
{
	public ServerInfo(int nID, string strIP, int nPort, string strName, bool bState)
	{
		this.ID = nID;
		this.IP = strIP;
		this.Port = nPort;
		this.Name = strName;
        this.IsActive = bState;
    }
    public int ID { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
}


class Login
{
	private ServerConnType	ServerConnType		= ServerConnType.enum_None_Server;
	private Hashtable AllCharServer;
    public ServerInfo CharServerInfo { get; private set; }
    public ServerInfo GameServerInfo { get; private set; }

    public uint PassportId { get; private set; }
    public ulong PlayerId { get; set; }
    public uint CheckCode { get; private set; }
	private bool IsStartLoadCharScene = false;
	
	public Login()
	{
        AllCharServer = new Hashtable();
	}
	
	public void OnServerDisconnected()
	{
		ServerConnType = ServerConnType.enum_None_Server;
	}
	
	public bool ConnectToLoginServer()
	{
		if(ServerConnType.enum_None_Server != ServerConnType)
		{
			return true;
		}
		
		string[] str = LogicApp.SP.LoginServer.Split(new char[]{':'});
		
		if(XLogicWorld.SP.NetManager.ConnectServer(str[0], Convert.ToInt32(str[1])))
		{	
			ServerConnType = ServerConnType.enum_Login_Server;
			
			CL_Connected.Builder builder = CL_Connected.CreateBuilder();
			builder.SetEmpty(0);
			CL_Connected msg = builder.Build();
			XLogicWorld.SP.NetManager.SendDataToServer((int)CL_Protocol.eCL_Connected, msg);
			return true;
		}
		return false;
	}

	public void ApplyCheckAccount(string strAccountName, string strPassword)
	{
		CL_CheckAccount.Builder builder = CL_CheckAccount.CreateBuilder();
		builder.SetName(strAccountName);
		builder.SetPassword(strPassword);
		CL_CheckAccount msg = builder.Build();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CL_Protocol.eCL_CheckAccount, msg);
	}
	
	public void ApplySelectCharServer(int nServerID)
	{
		if(ServerConnType.enum_Login_Server != ServerConnType)
			return;

        if (!AllCharServer.Contains(nServerID))
        {
            return;
        }
		
		ServerInfo server = (ServerInfo)AllCharServer[nServerID];
		if(!server.IsActive)
			return;
		
		CL_SelectCharServer.Builder builder = CL_SelectCharServer.CreateBuilder();
		builder.SetServerID(nServerID);
		CL_SelectCharServer msg = builder.Build();
		XLogicWorld.SP.NetManager.SendDataToServer((int)CL_Protocol.eCL_SelectCharServer, msg);
	}
	
	public void OnCheckAccountResult(LC_CheckAccountResult msg)
	{
		switch((CheckPasswordResult)msg.Result)
		{
		case(CheckPasswordResult.CPR_DBOException):
			Log.Write(LogLevel.INFO, "OnCheckAccountResult: DataBase Operation Exception");
			break;
			
		case(CheckPasswordResult.CPR_NoAccount):
			Log.Write(LogLevel.INFO, "OnCheckAccountResult: Account does not exists");
			break;
			
		case(CheckPasswordResult.CPR_WrongPassword):
			Log.Write(LogLevel.INFO, "OnCheckAccountResult: Wrong Password");
			break;

        case (CheckPasswordResult.CPR_Right):
            Log.Write(LogLevel.INFO, "OnCheckAccountResult: Right");
            // 保存登录信息
            PassportId = msg.PassportId;
            CheckCode = msg.CheckCode;
            break;
		case(CheckPasswordResult.CPR_PassportDisconnect):
			Log.Write(LogLevel.INFO, "OnCheckAccountResult: PassportServer Disconnected");
			break;
			
		case(CheckPasswordResult.CPR_PassportNotReady):
			Log.Write(LogLevel.INFO, "OnCheckAccountResult: PassportServer has not been ready");
			break;
		}
	}
	
	public void OnCharServerInfo(LC_CharServerInfo msg)
	{
        //--4>TODO: 此处应该接收多个数据包, 最好给个开始和结束信息, 在接收全部之后隐藏登陆界面显示选择服务器界面
		ServerInfo server = new ServerInfo(msg.ServerID, "", 0, msg.ServerName, msg.ServerState);
        AllCharServer.Add(msg.ServerID, server);
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eLoginUI);
		XEventManager.SP.SendEvent(EEvent.UI_Show, EUIPanel.eServerListUI);
		XEventManager.SP.SendEvent(EEvent.ServerList_AddServerInfo, server);
	}

    public void OnSelectServerRet(LC_SelectServerRet msg)
	{
        if (msg.Result == 0)
        {
            Log.Write(LogLevel.ERROR, "[ERROR] OnSelectServerRet: can not connect to server.");
        }
        else
        {
            XNetworkManager net = XLogicWorld.SP.NetManager;
            CharServerInfo = new ServerInfo(msg.ServerId, msg.ServerIp, msg.ServerPort, "CharServer", true);
            // 断开 Login
            net.DisconnectServer();

            //--4>TODO: 隐藏服务器选择界面

            // 连接到 CharServer
            if (net.ConnectServer(CharServerInfo.IP, CharServerInfo.Port))
    		{	
    			ServerConnType = ServerConnType.enum_Char_Server;

                // 加载角色信息
                CCh_LoadCharList.Builder builder = CCh_LoadCharList.CreateBuilder();
                builder.SetPassportId(PassportId);
                builder.SetCheckCode(CheckCode);
                net.SendDataToServer((int)CCh_Protocol.eCCh_LoadCharList, builder.Build());
    		}
        }
	}
	
	public void OnGameServerInfo(ChC_GameServerInfo msg)
    {
        //--4>TODO: 开始连接 gameserver 服务器, 断开 CharServer
        XNetworkManager net = XLogicWorld.SP.NetManager;
        GameServerInfo = new ServerInfo(msg.ServerId, msg.ServerIp, msg.ServerPort, "GameServer", true);
        // 断开 CharServer
        net.DisconnectServer();
        // 连接到 GameServer
        if (net.ConnectServer(GameServerInfo.IP, GameServerInfo.Port))
        {
            ServerConnType = ServerConnType.enum_Game_Server;
            // 加载角色信息
            CS_EnterGame.Builder builder = CS_EnterGame.CreateBuilder();
            builder.SetPassportId(PassportId);
            builder.SetPlayerId(PlayerId);
            builder.SetCheckCode(CheckCode);
            builder.SetServerId(msg.ServerId);
            builder.SetSceneId(msg.SceneId);
            net.SendDataToServer((int)CS_Protocol.eCS_EnterGame, builder.Build());
        }
    }
	
	private void EnterGame(int nCharIndex, ulong lPlayerId)
	{
		CCh_EnterGame.Builder builder = CCh_EnterGame.CreateBuilder();
		builder.CharIndex = nCharIndex;
		builder.PlayerId = lPlayerId;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CCh_Protocol.eCCh_EnterGame, builder.Build());
		XLogicWorld.SP.LoginProc.PlayerId = lPlayerId;
	}
	
    public void OnLoadCharList(ChC_LoadCharList msg)
    {
        if (msg.HasCharInfo0 && msg.CharInfo0.PlayerId > 0)
        {
			// 目前的机制, 如果第一个角色有效直接进入游戏
			EnterGame(0, msg.CharInfo0.PlayerId);
        }
        else
        {
			//we need the ui first loaded
			XResourcePanel uiCutsceneResource = XResourceManager.GetResource(XResourcePanel.ResTypeName,(uint)(EUIPanel.eCutSceneDialog)) as XResourcePanel;
            
			if(uiCutsceneResource.IsLoadDone() )
			{
				playInitCutScene(null );
			}
			else
			{
				uiCutsceneResource.CurPanel 	= EUIPanel.eCutSceneDialog;
				uiCutsceneResource.ParentTF 	= LogicApp.SP.UIRoot.UIAnchors [(int)EUIAnchor.eCenter];
				XResourceManager.StartLoadResource(XResourcePanel.ResTypeName,(uint)(EUIPanel.eCutSceneDialog));
				uiCutsceneResource.ResLoadEvent +=new XResourceBase.LoadCompletedDelegate(playInitCutScene );
			}
			
        }
    }
	
	private void playInitCutScene(DownloadItem item )
	{
		// 进入选角场景, 显示角色选择界面
		XCutSceneMgr.SP.playInitCutScene(200000,loadCreateCharScene );
	}
	
	private void loadCreateCharScene()
	{
		if(!IsStartLoadCharScene)
		{
			IsStartLoadCharScene	= true;
			XLogicWorld.SP.LoadScene(100203, ESceneType.CharScene);
		}
	}
	
    public void OnCreatePlayer(ChC_CreatePlayer msg)
    {
		// 创建完角色以后直接进入游戏
		EnterGame(msg.CharIndex, msg.Info.PlayerId);
    }

    public void OnEnterGame(SC_EnterGame msg)
    {
        // 进入游戏
		XEventManager.SP.SendEvent(EEvent.UI_Hide, EUIPanel.eCharOper);
		//XEventManager.SP.SendEvent(EEvent.UI_ShowMainUI);
        XLogicWorld.SP.LoadScene((int)(msg.SceneId), ESceneType.NormalScene);
    }
}
