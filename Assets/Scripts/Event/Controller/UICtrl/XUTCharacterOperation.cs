using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

class XUTCharacterOperation : XUICtrlTemplate<XCharacterOperationUI>
{
	public XUTCharacterOperation()
	{
		RegEventAgent_CheckCreated(EEvent.CharOper_SelectClassSex, SelectClassSex);
		RegEventAgent_CheckCreated(EEvent.CharOper_RandomName, OnRandomName);
		RegEventAgent_CheckCreated(EEvent.CharOper_CreatePlayer, CreatePlayer);
	}

    private void SelectClassSex(EEvent evt, params object[] args)
    {
        if (args.Length < 2)
        {
            return;
        }
		// 根据性别随机产生名字
        doRandomName((EShareSex)args[0]);
    }

    private void OnRandomName(EEvent evt, params object[] args)
    {
        if (args.Length < 1)
        {
            return;
        }
        doRandomName((EShareSex)args[0]);
    }

    private void doRandomName(EShareSex sex)
    {
        string firstName = null;
        if (sex == EShareSex.eshSex_Male)
        {
            firstName = XCfgMaleFirstNameMgr.SP.ItemTable[Random.Range(0, XCfgMaleFirstNameMgr.SP.ItemTable.Count)].FirstName;
        }
        else if (sex == EShareSex.eshSex_Female)
        {
            firstName = XCfgFemaleFirstNameMgr.SP.ItemTable[Random.Range(0, XCfgFemaleFirstNameMgr.SP.ItemTable.Count)].FirstName;
        }
        if (firstName != null)
        {
			string lastName = XCfgLastNameMgr.SP.ItemTable[Random.Range(0, XCfgLastNameMgr.SP.ItemTable.Count)].LastName;
			LogicUI.SetPlayerName(lastName + firstName);
        }
    }


	private void CreatePlayer(EEvent evt, params object[] args)
	{
		string strName = (string)(args[0]);
		//--4>TODO: 名字需要作进一步验证, 包括过滤字等
		if (strName == null || strName.Length < Define.MIN_PLAYER_NAME_LEN || strName.Length > Define.MAX_PLAYER_NAME_LEN)
		{
			//--4>TODO: MSG BOX
			Log.Write("[DEBUG] name len invalid");
			return;
		}
		
		CCh_CreatePlayer.Builder builder = CCh_CreatePlayer.CreateBuilder();
		//--4>: 目前只能创建一个角色
		builder.CharIndex = 0;
        builder.Sex = (uint)(args[1]);
		builder.Class = (uint)(args[2]);
		builder.PlayerName = strName;
		XLogicWorld.SP.NetManager.SendDataToServer((int)CCh_Protocol.eCCh_CreatePlayer, builder.Build());
	}
}

