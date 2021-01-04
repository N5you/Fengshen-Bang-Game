using System;
using UnityEngine;
using XGame.Client.Network;
using XGame.Client.Packets;
using Google.ProtocolBuffers;
using System.Collections.Generic;

class XNetworkManager
{
    private TcpPeer TcpPeerAgent;
    private PacketGate PacketGate;

    private float m_TimeSum = 0.0f;
    public static readonly float NET_TIME_INTERVAL = 40;
	
	// 过滤消息
    //--4>: 注意服务器一定也要验证
	private int m_FilterType = 0;	// 0: 不过滤, 1: 正过滤  2 : 反过滤
    private HashSet<int> m_FilterMsg;

    private delegate IMessage DCreateCommonMessage(object arg);
    private delegate IMessage DCreateArrayMessage(object arg);
    struct SCommonMessagePair
    {
        internal DCreateCommonMessage CommonMessage;
        internal DCreateArrayMessage ArrayMessage;
        internal SCommonMessagePair(DCreateCommonMessage c, DCreateArrayMessage a)
        {
            CommonMessage = c;
            ArrayMessage = a;
        }
    }
    private SortedList<string, SCommonMessagePair> m_CommonMsgMap = new SortedList<string, SCommonMessagePair>();

    public XNetworkManager()
    {
        // 初始化网络
        PacketGate = new PacketGate();
        TcpPeerAgent = new TcpPeer();
        TcpPeerAgent.OnLog += new LogHandler(this.WriteLog);
        TcpPeerAgent.OnReceivedPacket += new PacketHandler(PacketGate.ProcessPacket);

        m_FilterMsg = new HashSet<int>();
        this.InitCommonMsgMap();
    }

    ~XNetworkManager()
    {
        DisconnectServer();
        TcpPeerAgent = null;
        PacketGate = null;
    }

    public void Breathe()
    {
        m_TimeSum += Time.time;
        //if (m_TimeSum >= NET_TIME_INTERVAL)
        {
            m_TimeSum = 0.0f;
            TcpPeerAgent.Breath();
        }
    }

    public bool ConnectServer(string strAddr, int nPort)
    {
        //--4>: 在 Editor 中 isWebPlayer 总是 false, 为了方便调试暂时就这么写了
        //if (Application.isWebPlayer || Application.isEditor)
        //{
        //    if (Security.PrefetchSocketPolicy(strAddr, Define.POLICY_SERVER_PORT) == false)
        //    {
        //        if (Application.isWebPlayer)
        //        {
        //            return false;
        //        }
        //    }
        //}

        if (TcpPeerAgent.ServiceState == ENetServiceState.Running)
        {
            Log.Write(LogLevel.WARN, "LogicApp Warning: Tcp PeerAgent is Running, " +
                "if you want to connect to another server, stop current first.");
            return false;
        }
        if (!TcpPeerAgent.StartService(strAddr, nPort))
        {
            Log.Write(LogLevel.ERROR, "LogicApp Warning: ConnectServer: " + strAddr + ":" + nPort + " Failed!");
            return false;
        }
        return true;
    }

    public void DisconnectServer()
    {
        if (TcpPeerAgent.ServiceState == ENetServiceState.Running)
        {
            TcpPeerAgent.StopService();
        }
    }

    public void OnServerDisconnected()
    {
        //--4>TODO:
    }

    public void SendEmptyPacket(int cmd)
    {
        doSendData(cmd, CS_Empty.DefaultInstance);
    }

    public void SendCommonPacket<T>(int cmd, T arg)
    {
        if (arg != null && m_CommonMsgMap.ContainsKey(typeof(T).Name))
        {
            doSendData(cmd, m_CommonMsgMap[typeof(T).Name].CommonMessage(arg));
        }
    }

    public void SendCommonArray<T>(int cmd, params T[] arg)
    {
        if (arg != null && m_CommonMsgMap.ContainsKey(typeof(T).Name))
        {
            doSendData(cmd, m_CommonMsgMap[typeof(T).Name].ArrayMessage(arg));
        }
    }

    public void SendDataToServer(int cmd, IMessage msg)
    {
        doSendData(cmd, msg);
    }

    private void doSendData(int cmd, IMessage msg)
    {
		if(1 == m_FilterType && !m_FilterMsg.Contains(cmd)) 
			return;
		
		if(2 == m_FilterType && m_FilterMsg.Contains(cmd))
			return;
		
        if (null == msg)
        {
            Log.Write("[ERROR] Failed to send packet, cmd:{0} msg is null", cmd);
            return;
        }
        if (TcpPeerAgent.ServiceState == ENetServiceState.Running)
        {
            if (!TcpPeerAgent.SendPacket(cmd, msg))
            {
                Log.Write("[ERROR] Failed to send packet, cmd:{0} size:{1}", cmd, msg.SerializedSize);
            }
        }
    }

    public void WriteLog(System.Object log)
    {
        Log.Write(LogLevel.INFO, log.ToString());
    }

    private void InitCommonMsgMap()
    {
        m_CommonMsgMap.Add(typeof(int).Name, new SCommonMessagePair(
            delegate(object arg) { return CS_Int.CreateBuilder().SetData(Convert.ToInt32(arg)).Build(); },
            delegate(object arg) {
                CS_IntArr.Builder builder = CS_IntArr.CreateBuilder();
                foreach (object obj in arg as int[]) { builder.AddData(Convert.ToInt32(obj)); }
                return builder.Build();
            }
        ));
        m_CommonMsgMap.Add(typeof(uint).Name, new SCommonMessagePair(
            delegate(object arg) { return CS_UInt.CreateBuilder().SetData(Convert.ToUInt32(arg)).Build(); },
            delegate(object arg) {
                CS_UIntArr.Builder builder = CS_UIntArr.CreateBuilder();
                foreach (object obj in arg as uint[]) { builder.AddData(Convert.ToUInt32(obj)); }
                return builder.Build();
            }
        ));
        m_CommonMsgMap.Add(typeof(Int64).Name, new SCommonMessagePair(
            delegate(object arg) { return CS_Int64.CreateBuilder().SetData(Convert.ToInt64(arg)).Build(); },
            delegate(object arg) {
                CS_Int64Arr.Builder builder = CS_Int64Arr.CreateBuilder();
                foreach (object obj in arg as Int64[]) { builder.AddData(Convert.ToInt64(obj)); }
                return builder.Build();
            }
        ));
        m_CommonMsgMap.Add(typeof(float).Name, new SCommonMessagePair(
            delegate(object arg) { return CS_Float.CreateBuilder().SetData(Convert.ToSingle(arg)).Build(); },
            delegate(object arg) {
                CS_FloatArr.Builder builder = CS_FloatArr.CreateBuilder();
                foreach (object obj in arg as float[]) { builder.AddData(Convert.ToSingle(obj)); }
                return builder.Build();
            }
        ));
        m_CommonMsgMap.Add(typeof(string).Name, new SCommonMessagePair(
            delegate(object arg) { return CS_String.CreateBuilder().SetData(Convert.ToString(arg)).Build(); },
            delegate(object arg) {
                CS_StringArr.Builder builder = CS_StringArr.CreateBuilder();
                foreach (object obj in arg as string[]) { builder.AddData(Convert.ToString(obj)); }
                return builder.Build();
            }
        ));
    }
	
	// 网络层只能发送这些消息
	public void AddFilterMsg(params CS_Protocol[] cmd)
	{
		if(1 != m_FilterType)
		{
			m_FilterType = 1;
			m_FilterMsg.Clear();
		}
		for(int i=0; i<cmd.Length; i++)
		{
			m_FilterMsg.Add((int)cmd[i]);
		}
	}
	
	public void RemoveFilterMsg(params CS_Protocol[] cmd)
	{
		if(1 == m_FilterType)
		{
			for(int i=0; i<cmd.Length; i++)
			{
				m_FilterMsg.Remove((int)cmd[i]);
			}
		}
		
		if(m_FilterMsg.Count == 0)
			CancelFilter();
	}
	
	// 网络层除了这些消息, 剩下的都能发送
	public void AddUnFilterMsg(params CS_Protocol[] cmd)
	{
		if(2 != m_FilterType)
		{
			m_FilterType = 2;
			m_FilterMsg.Clear();
		}
		for(int i=0; i<cmd.Length; i++)
			m_FilterMsg.Add((int)cmd[i]);
	}
	
	public void RemoveUnFilterMsg(params CS_Protocol[] cmd)
	{
		if(2 == m_FilterType)
		{
			for(int i=0; i<cmd.Length; i++)
			{
				m_FilterMsg.Remove((int)cmd[i]);
			}
		}
	}
	
	public void CancelFilter()
	{
		m_FilterType = 0;
		m_FilterMsg.Clear();
	}
}
