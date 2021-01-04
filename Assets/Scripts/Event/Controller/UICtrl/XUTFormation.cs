using UnityEngine;
using XGame.Client.Packets;
using System.Collections.Generic;
using System.Collections;
class XUTFormation : XUICtrlTemplate<XFormation>
{
	public static int MAX_SHOW_PET_NUM = 20;
	public static int MAIN_PLAYER_PET_INDEX = MAX_SHOW_PET_NUM + 1;

	//界面下面的宠物数组
	public class ModelItemArray
	{
		public ModelItemArray ()
		{
			start = 0;
			end = 0;
			cureentPage = 1;
			IsLastPage = false;
		}

		public void ReSet()
		{
			start = 0;
			end = 0;
			for (int i =0; i < PetList.Count; i++) {
				if (PetList [i] != null)
					PetList [i] = null;
			}	
			PetList.Clear ();

			foreach (SingleModelRTT rtt in m_ModelRTTList) {
				if (rtt != null)
					rtt.DestoryModelRtt ();
			}
			m_ModelRTTList.Clear ();
		}
		
		public void AddCharacter(XPet ch, int petIndex)
		{
			PetAndPos petPos = new PetAndPos ();
			petPos.mPet = ch;
			petPos.mPos = petIndex;
			PetList.Add (petPos);
		}

		public void DelCharacter(int petIndex)
		{
			ulong petid = 0;
			for (int i = 0; i < PetList.Count; i++) {
				PetAndPos temp = PetList [i];
				if (temp.mPos == petIndex) {
					petid = temp.mPet.ID;
					PetList.Remove (temp);
					break;
				}
			}
		}

		public void ShowLeftView(XFormation ui)
		{
			if(cureentPage == 1)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,9901);
				return;
			}
			--CureentPage;
			ShowAllModel(ui);
		}
		
		public void ShowRightView(XFormation ui)
		{
			if(IsLastPage)
			{
				XNoticeManager.SP.Notice(ENotice_Type.ENotice_Type_Operator,9902);
				return;
			}
			++CureentPage;
			ShowAllModel(ui);
		}

		public void ShowAllModel(XFormation ui)
		{
			//清除所有数据
			ui.HideItem ();
			foreach (SingleModelRTT rtt in m_ModelRTTList) {
				if (rtt != null)
					rtt.DestoryModelRtt ();
			}
			m_ModelRTTList.Clear ();


			//获得显示宠物的列表
			start = 0;
			if (PetList.Count > XFormation.MAX_SHOW_MODEL_NUM)
			{
				//获得显示初始地址
				start = (cureentPage - 1) * XFormation.MAX_SHOW_MODEL_NUM;

				int MaxPage = (int)Mathf.Ceil ((float)PetList.Count / (float)XFormation.MAX_SHOW_MODEL_NUM);

				if(cureentPage < MaxPage)
				{
					end = start + XFormation.MAX_SHOW_MODEL_NUM;
					IsLastPage = false;
				}else
				{
					if(PetList.Count % XFormation.MAX_SHOW_MODEL_NUM == 0)
						end = start + XFormation.MAX_SHOW_MODEL_NUM;
					else
						end = start + PetList.Count % XFormation.MAX_SHOW_MODEL_NUM;
					IsLastPage = true;
				}
			}
			else
			{
				end = start + PetList.Count;
				IsLastPage = true;
			}

			if (end == start)
				return;

			if (end >= 2)
				PetList.Sort (new PetCompare ());

			//显示宠物
			for (int i = start; i < end; i++) {

				int idx = i - start;
				SingleModelRTT rtt = XModelRTTMgr.SP.AddModelRTT (PetList [i].mPet.ModelId, ui.LogicModelArray [idx].RoleHeadTex, 0f, -1.8f, 2f);
				//SingleModelRTT rtt = XModelRTTMgr.SP.AddObjectRTT (PetList [i].mPet.ObjectModel.mainModel, ui.LogicModelArray [i].RoleHeadTex, 1.5f, new Vector3 (0f, 0f, 1.0f), Vector3.zero, new Vector3 (1.0f, 1.0f, 1.0f), true);
				ui.LogicModelArray [idx].PetIndex = PetList [i].mPos;
				rtt.SetCameraSize (0.4f);

				//设置宠物类型的图片
				string petSprite = "";
				XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig (PetList [i].mPet.PetID);
				if (cfg == null) {
					Debug.LogWarning ("XUTFormation, formation cfg is null, not found PetID");
				} else {
					switch (cfg.AttackType) {
						case 1:
							petSprite = "11008017";
							break;
						case 2:
							petSprite = "11008018";
							break;
					}
				}
				ui.LogicModelArray [idx].SetNameAndLevel (PetList [i].mPet.Level, PetList [i].mPet.Name, idx, petSprite);
				ui.LogicModelArray [idx].SetActiveModel (true);
				m_ModelRTTList.Add (rtt);
			}
		}
		
		public XCharacter GetChar(int petIndex)
		{
			for (int i =0; i < PetList.Count; i++) {
				if (PetList [i] != null && PetList [i].mPos == petIndex) {
					return PetList [i].mPet;
				}
			}
			
			return null;
		}

		private class PetCompare : IComparer<PetAndPos>
		{
			public int Compare(PetAndPos x, PetAndPos y)
			{
				return x.mPet.ClassLevel > y.mPet.ClassLevel ? 1 : -1;
			}
		}

		public class PetAndPos
		{
			public XPet mPet;
			public int	mPos;
		}

		public int CureentPage 
		{
			get{
				return cureentPage;
			}
			set{
				if(value < 1)
					value = 1;
				cureentPage = value;
			}
		}
		public bool IsLastPage;
		public List<PetAndPos>	PetList = new List<PetAndPos> ();
		private List<SingleModelRTT> m_ModelRTTList = new List<SingleModelRTT> ();
		private int start;
		private int end;
		private int cureentPage;
	}

	//阵形位置上的映射图片
	public class FormationTextureItemArray
	{
		private XCfgFormation formationCfg = new XCfgFormation ();
		private SingleModelRTT[] m_rtt = new SingleModelRTT[XFormation.MAX_FORMATION_POS_NUM];
		private XU3dModel[] m_mainModel = new XU3dModel[XFormation.MAX_FORMATION_POS_NUM];
		private XU3dModel m_mainPlayerModel = null;

		public int MainPlayerPos { get; set; }

		//删除映射图片对象
		public void DestoryIndex(int index)
		{
			if (m_rtt [index] != null) {
				m_rtt [index].DestoryModelRtt ();
				m_rtt [index] = null;
			}
			if (m_mainModel [index] != null) {
				m_mainModel [index].Destroy ();
				m_mainModel [index] = null;
			}
		}

		//更新映射图片上的模型数据
		public SingleModelRTT UpdateUIRTT(XFormation ui, int posIndex, XCharacter xchar, bool isMainPlayer)
		{
			if (ui == null || xchar == null || !ui.gameObject.activeSelf)
				return null;
			DestoryIndex (posIndex);

			if (m_rtt [posIndex] == null && !isMainPlayer) {

				XPet pet = (XPet)xchar;
				formationCfg = XCfgFormationMgr.SP.GetConfig (pet.PetID);
				if (formationCfg == null)
					return null;


				uint modelid = xchar.ModelId;
				m_mainModel [posIndex] = new XU3dModel ("Model" + posIndex, modelid);
				m_mainModel [posIndex].Layer = GlobalU3dDefine.Layer_UI_2D;
				m_mainModel [posIndex].Parent = ui.transform;
				m_mainModel [posIndex].Scale = new Vector3 (50, 50, 50);
				m_mainModel [posIndex].LocalPosition = new Vector3 (0, -10, -10);
				m_mainModel [posIndex].Direction = new Vector3 (0, 90, 0);

				m_rtt [posIndex] = XModelRTTMgr.SP.AddObjectRTT (m_mainModel [posIndex],
				                                                 ui.LogicFormationPos [posIndex].ModelTexture, 
				                                                 5.0f, 				//camsize
				                                                 new Vector3 (0f, -1.5f, 20f), 	//position x,y,z
				                                                 new Vector3 (8, 111, 337), 		//rotation x,y,z
				                                                 new Vector3 (1, 1, 1), true);	//scale x,y,z
				
				m_mainModel [posIndex].PlayAnimation (EAnimName.Fight, 1.0f, false);
			}

			if (isMainPlayer) {
				uint modelid = XLogicWorld.SP.MainPlayer.ModelId;
				m_mainPlayerModel = new XU3dModel ("Formation_MainPlayer", modelid);
				
				m_mainPlayerModel.Layer = GlobalU3dDefine.Layer_UI_2D;
				m_mainPlayerModel.Parent = ui.transform;
				m_mainPlayerModel.Scale = new Vector3 (50, 50, 50);
				m_mainPlayerModel.LocalPosition = new Vector3 (0, -10, -10);
				m_mainPlayerModel.Direction = new Vector3 (0, 90, 0);
				
				m_rtt [posIndex] = XModelRTTMgr.SP.AddObjectRTT (m_mainPlayerModel,
				                                                 ui.LogicFormationPos [posIndex].ModelTexture, 
				                                                 1.5f, 				//camsize
				                                                 new Vector3 (0f, -1.5f, 20f), 	//position x,y,z
				                                                 new Vector3 (8, 111, 337), 		//rotation x,y,z
				                                                 new Vector3 (1, 1, 1), true);	//scale x,y,z
				
				m_mainPlayerModel.PlayAnimation (EAnimName.Fight, 1.0f, false);
			}

			return m_rtt [posIndex];
		}

		public void ReSet()
		{			
			for (int cnt = 0; cnt < XFormation.MAX_FORMATION_POS_NUM; ++cnt) {
				if (m_rtt [cnt] != null) {
					m_rtt [cnt].DestoryModelRtt ();
					m_rtt [cnt] = null;
				}
			}

			for (int xnt = 0; xnt < XFormation.MAX_FORMATION_POS_NUM; ++xnt) {
				if (m_mainModel [xnt] != null) {
					m_mainModel [xnt].Destroy ();
					m_mainModel [xnt] = null;
				}
			}
			MainPlayerPos = 0;
		}
	}
	
	private ModelItemArray mModelItemArray = new ModelItemArray ();
	private FormationTextureItemArray m_FormationTextureItemArray = new FormationTextureItemArray ();
	
	public XUTFormation ()
	{
		XEventManager.SP.AddHandler (DragHandler, EEvent.Formation_BeginDrag);
		XEventManager.SP.AddHandler (DropHandler, EEvent.Formation_BeginDrop);
		
		XEventManager.SP.AddHandler (FormationDragHandler, EEvent.Formation_BeginFormationDrag);
		XEventManager.SP.AddHandler (SetFormationPosHandler, EEvent.Formation_SetFormationPos);
		
		XEventManager.SP.AddHandler (ShowModelTip, EEvent.Formation_ShowModelTip);
		XEventManager.SP.AddHandler (ClickModel, EEvent.Formation_DoubleClickModel);

		XEventManager.SP.AddHandler (LeftViewHandler,EEvent.Formation_LeftView);
		XEventManager.SP.AddHandler (RighttViewHandler,EEvent.Formation_RightView);
		
	}
	
	private void InitModelItem()
	{
		//阵型上的数据删除
		m_FormationTextureItemArray.ReSet ();
		
		//构建逻辑对象数组
		mModelItemArray.ReSet ();
		
		int petIndex = 0;
		foreach (XPet pet in XLogicWorld.SP.PetManager.AllPet) {
			if (null == pet) {
				petIndex++;
				continue;
			}
			if (pet.BattlePos <= 0)
				//未上阵宠物添加至待上阵表中
				mModelItemArray.AddCharacter (pet, petIndex);
			else {
				m_FormationTextureItemArray.UpdateUIRTT (LogicUI, (int)pet.BattlePos - 1, pet, false);
				LogicUI.LogicFormationPos [(int)pet.BattlePos - 1].PetIndex = petIndex;
				XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig ((uint)pet.PetID);
				if (cfg == null)
					Debug.Log ("XUTFormaiton, Init Pet Data not found  the pet id");
				else
					LogicUI.LogicFormationPos [(int)pet.BattlePos - 1].SetFormationPosStyle ((XFormation.FormationPos.FormationStyle)cfg.AttackType);
			}
			petIndex++;
		}
		ShowMainPlayer ();
		mModelItemArray.ShowAllModel (LogicUI);
	}

	public void LeftViewHandler(EEvent evt, params object[] args)
	{
		mModelItemArray.ShowLeftView(LogicUI);
	}

	public void RighttViewHandler(EEvent evt, params object[] args)
	{
		mModelItemArray.ShowRightView(LogicUI);
	}

	public void DragHandler(EEvent evt, params object[] args)
	{
		XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eCursor);
		Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		int petIndex = (int)args [0];
		
		XCharacter ch = GetCharacter (petIndex);
		if (ch == null)
			return;
		
		XEventManager.SP.SendEvent (EEvent.Cursor_UpdateModel, ch.ModelId, petIndex == MAIN_PLAYER_PET_INDEX);
	}

	public void DropHandler(EEvent evt, params object[] args)
	{
		if (LogicUI == null || !LogicUI.gameObject.activeSelf)
			return;
		
		int toPetIndex = (int)args [0];
		int toPosIndex = (int)args [1];
		EDrag_Type toType = (EDrag_Type)args [2];
		
		SwapDragModel (toPetIndex, toPosIndex, toType);
	}
	
	public void FormationDragHandler(EEvent evt, params object[] args)
	{
		if (LogicUI == null || !LogicUI.gameObject.activeSelf)
			return;
		
		int posIndex = (int)args [0];
		int petIndex = (int)args [1];

		XCharacter charModel = GetCharacter (petIndex);
		if (charModel == null)
			return;
		
		m_FormationTextureItemArray.DestoryIndex (posIndex);
		LogicUI.FormationPosTextureList [posIndex].enabled = false;
		XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eCursor);
		XEventManager.SP.SendEvent (EEvent.Cursor_UpdateModel, charModel.ModelId, petIndex == XUTFormation.MAIN_PLAYER_PET_INDEX);

	}
	
	public void SetFormationPosHandler(EEvent evt, params object[] args)
	{
		if (LogicUI == null || !LogicUI.gameObject.activeSelf)
			return;
		
		int toPetIndex = (int)args [0];
		int toPosIndex = (int)args [1];
		EDrag_Type toType = (EDrag_Type)args [2];

		SwapDragModel (toPetIndex, toPosIndex, toType);
	}
	
	public void ShowModelTip(EEvent evt, params object[] args)
	{
		if (LogicUI == null || !LogicUI.gameObject.activeSelf)
			return;
		
		int petIndex = (int)args [0];

		XCharacter ch = GetCharacter (petIndex);
		if (ch == null)
			return;

		string name = "";
		string content = "";
		//不是主角色
		if (petIndex != MAIN_PLAYER_PET_INDEX) {
			XPet tpet = ch as XPet;
			XCfgPetBase petbaseCfg = XCfgPetBaseMgr.SP.GetConfig ((uint)tpet.PetID);
			XCfgFormation petFormationCfg = XCfgFormationMgr.SP.GetConfig ((uint)tpet.PetID);

			//名称处理
			name = "[color=00ff00]";
			name += tpet.Name;
			if (petFormationCfg.AttackType == (int)XFormation.FormationPos.FormationStyle.defense)
				name += XStringManager.SP.GetString (406);//防御
			if (petFormationCfg.AttackType == (int)XFormation.FormationPos.FormationStyle.attack)
				name += XStringManager.SP.GetString (407);//攻击
			name += " ";
			name += "[color=f89a4b]";
			name += tpet.Level;
			name += XStringManager.SP.GetString (405);//等级

			//内容处理
			content = "[color=00d8ff]";
			content += XStringManager.SP.GetString (400) + tpet.Aptitude.ToString () + XStringManager.SP.GetString (404);
			content += '\n';
			content += XStringManager.SP.GetString (401) + petbaseCfg.AptitudeName;//特殊技能
			content += '\n';
			content += '\n';
			content += "[color=00d8ff]";
			content += XStringManager.SP.GetString (402) + tpet.Hp.ToString ();//血量
			content += '\n';
			content += "[color=ff5400]";
			content += XStringManager.SP.GetString (403) + "200";//战斗力
			content += '\n';
			content += "[color=aca48d]";
			if (petFormationCfg.AttackType == (int)XFormation.FormationPos.FormationStyle.defense)
				content += XStringManager.SP.GetString (408);//提示
			if (petFormationCfg.AttackType == (int)XFormation.FormationPos.FormationStyle.attack)
				content += XStringManager.SP.GetString (409);//提示
		}else
		{
			//名称处理
			name = "[color=00ff00]";
			name += ch.Name;
			name += " ";
			name += "[color=f89a4b]";
			name += ch.Level;
			name += XStringManager.SP.GetString (405);//等级
			
			//内容处理
			content += "[color=00d8ff]";
			content += XStringManager.SP.GetString (402) + ch.Hp.ToString ();//血量
			content += '\n';
			content += "[color=ff5400]";
			content += XStringManager.SP.GetString (403) + "200";//战斗力
			content += '\n';
			content += "[color=aca48d]";
			content += XStringManager.SP.GetString (410);//提示
		}

		XEventManager.SP.SendEvent (EEvent.UI_Show, EUIPanel.eToolTipB);
		XEventManager.SP.SendEvent (EEvent.ToolTip_B, name, "", content);
	}
	
	public void ClickModel(EEvent evt, params object[] args)
	{
		int petIndex = (int)args [0];
		int posIndex = (int)args [1];
		
		XCharacter ch = GetCharacter (petIndex);
		if (ch == null)
			return;
		
		if (petIndex == MAIN_PLAYER_PET_INDEX) {
			XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 9900);
			return;
		}
		
		if (posIndex < 0 || posIndex >= XFormation.MAX_FORMATION_POS_NUM)
			return;
		
		XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipA);
		
		XPet pet = ch as XPet;		
		m_FormationTextureItemArray.DestoryIndex (posIndex);
		LogicUI.FormationPosTextureList [posIndex].enabled = false;
		LogicUI.LogicFormationPos [posIndex].SetFormationPosStyle (XFormation.FormationPos.FormationStyle.defualt);
		mModelItemArray.AddCharacter (pet, petIndex);
		mModelItemArray.ShowAllModel (LogicUI);
		LogicUI.LogicFormationPos [posIndex].PetIndex = -1;
		
		SendServerBattlePos ((uint)petIndex, 0);
	}
	
	private void SwapDragModel(int toPetIndex, int toPosIndex, EDrag_Type toType)
	{
		XNewPlayerGuideManager.SP.handleGuideFinish((int)XNewPlayerGuideManager.GuideType.Guide_BuZhenPetUse);
		LogicUI.StopEffect();
		
		if (XDragMgr.SP.DragType == EDrag_Type.EDrag_Type_FormationPos) {
			if (toType == EDrag_Type.EDrag_Type_FormationPos) {
				int fromPetIndex = XDragMgr.SP.PetIndex;
				int fromPosIndex = XDragMgr.SP.FormationPos;

				//拖动源不存在任何宠物
				if (fromPetIndex <= 0)
					return;

				XCharacter petChar = GetCharacter (fromPetIndex);
				if (petChar != null) {

					bool isMain = fromPetIndex == MAIN_PLAYER_PET_INDEX;
					m_FormationTextureItemArray.UpdateUIRTT (LogicUI, toPosIndex, petChar, isMain);

					if (!isMain) {
						XPet pet = petChar as XPet;
						XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig (pet.PetID);
						if (cfg == null)
							Debug.LogError ("XUTFormation, not found petid in Formation Config file");
						LogicUI.LogicFormationPos [toPosIndex].SetFormationPosStyle ((XFormation.FormationPos.FormationStyle)cfg.AttackType);
						LogicUI.LogicFormationPos [fromPosIndex].SetFormationPosStyle (XFormation.FormationPos.FormationStyle.defualt);
					}else
					{
						LogicUI.LogicFormationPos [toPosIndex].SetFormationPosStyle (XFormation.FormationPos.FormationStyle.defualt);
						LogicUI.LogicFormationPos [fromPosIndex].SetFormationPosStyle (XFormation.FormationPos.FormationStyle.defualt);
					}

					SendServerBattlePos ((uint)fromPetIndex, (uint)toPosIndex + 1);

					//如果拖动位置没有变化，侧打开原位置的映射贴图	
					if (fromPosIndex == toPosIndex) {
						if(!isMain)
						{
							XPet pet = petChar as XPet;
							if (!LogicUI.FormationPosTextureList [fromPosIndex].enabled)
								LogicUI.FormationPosTextureList [fromPosIndex].enabled = true;
							XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig (pet.PetID);
							if (cfg == null)
								Debug.LogError ("XUTFormation, not found petid in Formation Config file");
							LogicUI.LogicFormationPos [fromPosIndex].SetFormationPosStyle ((XFormation.FormationPos.FormationStyle)cfg.AttackType);
						}else
						{
							if (!LogicUI.FormationPosTextureList [fromPosIndex].enabled)
								LogicUI.FormationPosTextureList [fromPosIndex].enabled = true;
						}

						return;
					}

					//如果拖动位置有变化，侧打开目标位置的映射贴图
					if (fromPosIndex != toPosIndex) {
						if (!LogicUI.FormationPosTextureList [toPosIndex].enabled)
							LogicUI.FormationPosTextureList [toPosIndex].enabled = true;
					}
				}

				petChar = GetCharacter (toPetIndex);
				if (petChar != null) {
					bool isOMain = toPetIndex == MAIN_PLAYER_PET_INDEX;
					m_FormationTextureItemArray.UpdateUIRTT (LogicUI, fromPosIndex, petChar, isOMain);
					if (!isOMain) {
						XPet pet = (XPet)petChar;
						XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig (pet.PetID);
						if (cfg == null)
							Debug.LogError ("XUTFormation, not found petid in Formation Config file");
						LogicUI.LogicFormationPos [fromPosIndex].SetFormationPosStyle ((XFormation.FormationPos.FormationStyle)cfg.AttackType);
					}
					SendServerBattlePos ((uint)toPetIndex, (uint)fromPosIndex + 1);

					//目标位置上存在宠物，显示目标映射图片
					if (toPetIndex != 0) {
						if (!LogicUI.FormationPosTextureList [fromPosIndex].enabled)
							LogicUI.FormationPosTextureList [fromPosIndex].enabled = true;
					}
				}

				LogicUI.LogicFormationPos [toPosIndex].PetIndex = fromPetIndex;
				LogicUI.LogicFormationPos [fromPosIndex].PetIndex = toPetIndex;

			} else if (toType == EDrag_Type.EDrag_Type_ModelPos) {

					int fromPetIndex = XDragMgr.SP.PetIndex;
					int fromPosIndex = XDragMgr.SP.FormationPos;
				
					XCharacter petChar = GetCharacter (fromPetIndex);
					if (petChar == null)
						return;
					//拖动主角色
					if (fromPetIndex == MAIN_PLAYER_PET_INDEX) {
						m_FormationTextureItemArray.UpdateUIRTT (LogicUI, fromPosIndex, petChar, true);
						XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 9900);
						//重新显示主角色位置的映射资源图片	
						if (!LogicUI.FormationPosTextureList [fromPosIndex].enabled)
							LogicUI.FormationPosTextureList [fromPosIndex].enabled = true;
						return;
					}
				
					if (fromPosIndex < 0 || fromPosIndex >= XFormation.MAX_FORMATION_POS_NUM)
						return;
				
					XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipA);
				
					XPet pet = petChar as XPet;
					m_FormationTextureItemArray.DestoryIndex (fromPosIndex);
					LogicUI.FormationPosTextureList [fromPosIndex].enabled = false;
					LogicUI.LogicFormationPos [fromPosIndex].SetFormationPosStyle (XFormation.FormationPos.FormationStyle.defualt);
					mModelItemArray.AddCharacter (pet, fromPetIndex);
					mModelItemArray.ShowAllModel (LogicUI);

					LogicUI.LogicFormationPos [fromPosIndex].PetIndex = 0;
					//下架
					SendServerBattlePos ((uint)fromPetIndex, 0);

				}
		} else if (XDragMgr.SP.DragType == EDrag_Type.EDrag_Type_ModelPos) {
				if (toType == EDrag_Type.EDrag_Type_FormationPos) {
					int fromPetIndex = XDragMgr.SP.PetIndex;
					int fromPosIndex = XDragMgr.SP.ModelIndex;

					XCharacter petChar = GetCharacter (fromPetIndex);
					if (petChar == null)
						return;

					//目标是主角，不能将主角替换至下架
					if (toPetIndex == MAIN_PLAYER_PET_INDEX) {
						XNoticeManager.SP.Notice (ENotice_Type.ENotice_Type_Operator, 9900);
						return;
					}

					m_FormationTextureItemArray.UpdateUIRTT (LogicUI, toPosIndex, petChar, false);
					//重新显示主角色位置的映射资源图片	
					if (!LogicUI.FormationPosTextureList [toPosIndex].enabled)
						LogicUI.FormationPosTextureList [toPosIndex].enabled = true;
					XPet pet = (XPet)petChar;
					XCfgFormation cfg = XCfgFormationMgr.SP.GetConfig (pet.PetID);
					if (cfg == null)
						Debug.LogError ("XUTFormation, not found petid in Formation Config file");
					LogicUI.LogicFormationPos [toPosIndex].SetFormationPosStyle ((XFormation.FormationPos.FormationStyle)cfg.AttackType);
					SendServerBattlePos ((uint)fromPetIndex, (uint)toPosIndex + 1);

					//set Orignal
					mModelItemArray.DelCharacter (fromPetIndex);
					XCharacter toChar = GetCharacter (toPetIndex);
					XPet toPetChar = toChar as XPet;
					if (toPetChar != null) {
						mModelItemArray.AddCharacter (toPetChar, toPetIndex);
						//下架
						SendServerBattlePos ((uint)toPetIndex, 0);
					}
					
					mModelItemArray.ShowAllModel (LogicUI);
					LogicUI.LogicFormationPos [toPosIndex].PetIndex = fromPetIndex;	
				}
			}
	}

	private void ShowMainPlayer()
	{	
		m_FormationTextureItemArray.UpdateUIRTT (LogicUI, (int)XLogicWorld.SP.MainPlayer.BattlePos - 1, XLogicWorld.SP.MainPlayer, true);
		LogicUI.LogicFormationPos [(int)XLogicWorld.SP.MainPlayer.BattlePos - 1].PetIndex = MAIN_PLAYER_PET_INDEX;
		m_FormationTextureItemArray.MainPlayerPos = (int)XLogicWorld.SP.MainPlayer.BattlePos - 1;
	}
	
	private XCharacter GetCharacter(int petIndex)
	{
		if (petIndex < 0)
			return null;
		
		if (petIndex == XUTFormation.MAIN_PLAYER_PET_INDEX) {
			return XLogicWorld.SP.MainPlayer;
		}
		
		XCharacter ch = XLogicWorld.SP.PetManager.AllPet [petIndex];
		
		return ch;
	}
	
	public override void OnShow()
	{
		base.OnShow ();	
		LogicUI.ReSet ();
		InitModelItem ();
	}
	
	public override void OnHide()
	{
		base.OnHide ();
		mModelItemArray.ReSet ();
		m_FormationTextureItemArray.ReSet();
	}
	
	private void MainPlayerBattlePosChange(uint battlePos)
	{
		if (XLogicWorld.SP.MainPlayer.BattlePos != battlePos) {
			CS_SetPlayerBattlePos.Builder builder = CS_SetPlayerBattlePos.CreateBuilder ();
			builder.SetBattlePos (battlePos);
			XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_SetPlayerBattlePos, builder.Build ());
			XLogicWorld.SP.MainPlayer.BattlePos = battlePos;
		}
	}
	
	private void PetBattlePosChange(uint petPosition, uint battlePos)
	{
		CS_SetPetBattlePos.Builder builder = CS_SetPetBattlePos.CreateBuilder ();
		builder.SetPosition (petPosition);
		builder.SetBattlePos (battlePos);
		
		XLogicWorld.SP.NetManager.SendDataToServer ((int)CS_Protocol.eCS_SetPetBattlePos, builder.Build ());
	}
	
	private void SendServerBattlePos(uint petIndex, uint battlePos)
	{		
		if (petIndex == MAIN_PLAYER_PET_INDEX)
			MainPlayerBattlePosChange (battlePos);
		else {
			XPet pet = GetCharacter ((int)petIndex) as XPet;
			if (pet != null)
				PetBattlePosChange (pet.Index, battlePos);
		}
			
	}
}
