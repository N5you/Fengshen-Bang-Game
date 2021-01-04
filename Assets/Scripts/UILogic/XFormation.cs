using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UILogic/XFormation")]
public class XFormation : XDefaultFrame
{
	public static readonly int MAX_SHOW_MODEL_NUM = 5;
	public static readonly int MAX_FORMATION_POS_NUM = 9;
	public UIImageButton leftBtn;
	public UIImageButton rightBtn;

	public GameObject[] GoModelList = new GameObject[MAX_SHOW_MODEL_NUM];

	public UILabel[] NameList = new UILabel[MAX_SHOW_MODEL_NUM];
	public UILabel[] LevelList = new UILabel[MAX_SHOW_MODEL_NUM];
	public UISprite[] SpriteList = new UISprite[MAX_SHOW_MODEL_NUM];
	public UITexture[] TextureList = new UITexture[MAX_SHOW_MODEL_NUM];
	public UISprite[] PetTypeSprite = new UISprite[MAX_SHOW_MODEL_NUM];

	public UIImageButton[] FormationPosArray = new UIImageButton[MAX_FORMATION_POS_NUM];
	public ModelState[] LogicModelArray = new ModelState[MAX_SHOW_MODEL_NUM];
	public FormationPos[] LogicFormationPos = new FormationPos[MAX_FORMATION_POS_NUM];
	public UITexture[] FormationPosTextureList = new UITexture[MAX_FORMATION_POS_NUM];


	//5格待布阵对象
	public class ModelState
	{
		private UILabel mName;
		private UILabel mLevel;
		private int mIndex;
		public UITexture RoleHeadTex;
		private UISprite	mSprite;
		public int PetIndex;
		private UISprite mPetType;

		public ModelState (UILabel level, UILabel name, UITexture texture, UISprite sprite, UISprite petType)
		{
			mName = name;
			mLevel = level;
			
			RoleHeadTex = texture;
			mSprite = sprite;
			mPetType = petType;

			UIEventListener listener = UIEventListener.Get (mSprite.gameObject);
			listener.onDrag += OnDragSprite;
			listener.onDrop += OnDropSprite;
			listener.onHover += OnModelHover;

			PetIndex = 0;
		}
		
		public void SetActiveModel(bool isActive)
		{
			if (RoleHeadTex == null)
				return;			
			
			RoleHeadTex.gameObject.SetActive (isActive);
		}
		
		public void SetNameAndLevel(int level, string name, int itemIndex, string petTypeSprite)
		{
			mLevel.text = "Lv" + level.ToString ();
			if (level == 0)
				mLevel.text = "";
			mName.text = name;
			mPetType.spriteName = petTypeSprite;
			if(!string.IsNullOrEmpty(petTypeSprite))
			{
				mPetType.ResetSize();
				mPetType.gameObject.SetActive(true);
			}
			mIndex = itemIndex;
		}
		
		public void ReSet()
		{
			PetIndex = 0;
			mLevel.text = "";
			mName.text = "";
			mPetType.gameObject.SetActive(false);
		}
		
		public void ClickSprite(GameObject go)
		{
			
		}
		
		public void OnDragSprite(GameObject go, Vector2 delta)
		{
			if (UICamera.currentTouchID > -2) {
				if (!XDragMgr.SP.IsDraging) {					
					XDragMgr.SP.IsDraging = true;
					XDragMgr.SP.PetIndex = PetIndex;
					XDragMgr.SP.ModelIndex = mIndex;
					XDragMgr.SP.DragType = EDrag_Type.EDrag_Type_ModelPos;
					
					XEventManager.SP.SendEvent (EEvent.Formation_BeginDrag, PetIndex, mIndex);
				}
			}
		}
		
		public void OnDropSprite(GameObject go, GameObject draggedObject)
		{
			if (XDragMgr.SP.IsDraging) {
				XDragMgr.SP.IsDraging = false;
				XEventManager.SP.SendEvent (EEvent.Cursor_ClearModel, EUIPanel.eCursor);
				XEventManager.SP.SendEvent (EEvent.Formation_BeginDrop, PetIndex, mIndex, EDrag_Type.EDrag_Type_ModelPos);
			}
		}

		public void OnModelHover(GameObject go, bool state)
		{
			if (state) {
				if (!XDragMgr.SP.IsDraging)
					XEventManager.SP.SendEvent (EEvent.Formation_ShowModelTip, PetIndex);
			} else {
				XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
			}
		}
	}


	//9格布阵对象
	public class FormationPos
	{
		public enum FormationStyle
		{
			defualt = 0,
			attack = 1,
			defense = 2,
		}
		public UIImageButton FormationPosButton;
		public int mIndex;
		public int PetIndex;
		private EDrag_Type mType = EDrag_Type.EDrag_Type_FormationPos;
		private Color mOrginalColor;
		public UITexture ModelTexture;
		
		public FormationPos (UIImageButton go, int index, UITexture texture)
		{
			FormationPosButton = go;
			mIndex = index;
			ModelTexture = texture;

			UIEventListener listener = UIEventListener.Get (FormationPosButton.gameObject);
			listener.onDrag += OnDragSprite;
			listener.onDrop += OnDropSprite;
			listener.onHover += OnModelHover;
			listener.onDoubleClick += DoubleClick;
		}
		
		public void ReSet()
		{
			PetIndex = 0;
			SetFormationPosStyle(FormationStyle.defualt);
		}
		
		public void OnAttackModel()
		{
			if (FormationPosButton == null)
				return;
			
//			CharacterController cc = mGO.GetComponentInChildren<CharacterController>();
//			if(cc == null)
//				return ;
//			
//			SkinnedMeshRenderer[] renderList = cc.GetComponentsInChildren<SkinnedMeshRenderer>();
//			foreach(SkinnedMeshRenderer smr in renderList)
//			{				
//				mOrginalColor	= smr.materials[0].GetColor("_Color");
//				if(mOrginalColor != new Color(0.0f,0.0f,0.0f,0.0f))
//					break ;
//			}
		}

		public void OnDragSprite(GameObject go, Vector2 delta)
		{
			if (UICamera.currentTouchID > -2) {
				if (!XDragMgr.SP.IsDraging) {
					XDragMgr.SP.IsDraging = true;
					XDragMgr.SP.PetIndex = PetIndex;
					XDragMgr.SP.FormationPos = mIndex;
					XDragMgr.SP.DragType = mType;

					XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
					XEventManager.SP.SendEvent (EEvent.Formation_BeginFormationDrag, mIndex, XDragMgr.SP.PetIndex);
				}
			}
		}
		
		public void OnDropSprite(GameObject go, GameObject draggedObject)
		{
			if (XDragMgr.SP.IsDraging) {
				UICamera.currentTouch.DropIsDeal = true;
				XDragMgr.SP.IsDraging = false;
				XEventManager.SP.SendEvent (EEvent.Cursor_ClearModel, EUIPanel.eCursor);
				XEventManager.SP.SendEvent (EEvent.Formation_SetFormationPos, PetIndex, mIndex, EDrag_Type.EDrag_Type_FormationPos);
			}
		}
		
		public void OnModelHover(GameObject go, bool state)
		{
			if (FormationPosButton == null)
				return;
			
			//CharacterController cc = mGO.GetComponentInChildren<CharacterController> ();
			//if (cc == null)
			//	return;

			if(ModelTexture.material == null)
				return;
			
			if (state) {
//				SkinnedMeshRenderer[] renderList = cc.GetComponentsInChildren<SkinnedMeshRenderer>();
//				foreach(SkinnedMeshRenderer smr in renderList)
//				{
//					foreach(Material mat in smr.materials)
//					{
//						mat.color	=	new Color(1.0f,1.0f,1.0f,1.0f);
//					}
//				}
				if (!XDragMgr.SP.IsDraging)
					XEventManager.SP.SendEvent (EEvent.Formation_ShowModelTip, PetIndex);
			} else {
//				SkinnedMeshRenderer[] renderList = cc.GetComponentsInChildren<SkinnedMeshRenderer>();
//				foreach(SkinnedMeshRenderer smr in renderList)
//				{
//					foreach(Material mat in smr.materials)
//					{
//						mat.color	=	mOrginalColor;
//					}
//				}
				
				XEventManager.SP.SendEvent (EEvent.UI_Hide, EUIPanel.eToolTipB);
			}
			
		}
		
		public void DoubleClick(GameObject go)
		{			
			XEventManager.SP.SendEvent (EEvent.Formation_DoubleClickModel, PetIndex, mIndex);			
		}

		public void SetFormationPosStyle(XFormation.FormationPos.FormationStyle style)
		{
			switch(style)
			{
				case FormationStyle.defualt:
					this.FormationPosButton.normalSprite = "11000022";
					this.FormationPosButton.hoverSprite = "11000023";
					this.FormationPosButton.pressedSprite = "11000023";
					this.FormationPosButton.UpdateImage();
					break;
				case FormationStyle.attack:
					this.FormationPosButton.normalSprite = "11000180";
					this.FormationPosButton.hoverSprite = "11000179";
					this.FormationPosButton.pressedSprite = "11000179";
					this.FormationPosButton.UpdateImage();
					break;
				case FormationStyle.defense:
					this.FormationPosButton.normalSprite = "11000181";
					this.FormationPosButton.hoverSprite = "11000182";
					this.FormationPosButton.pressedSprite = "11000182";
					this.FormationPosButton.UpdateImage();
					break;
			}
		}
	}
	
	public void ReSet()
	{
		for (int i = 0; i < MAX_SHOW_MODEL_NUM; i++) {
			LogicModelArray [i].ReSet ();
		}
		
		for (int j = 0; j < MAX_FORMATION_POS_NUM; j++) {
			LogicFormationPos [j].ReSet ();
		}
	}
	
	public void HideItem()
	{
		for (int i = 0; i < MAX_SHOW_MODEL_NUM; i++) {
			LogicModelArray [i].ReSet ();
			LogicModelArray [i].SetActiveModel (false);
		}
	}
	
	public override bool Init()
	{	
		base.Init ();
		
		for (int i = 0; i < MAX_SHOW_MODEL_NUM; i++) {
			LogicModelArray [i] = new ModelState (LevelList [i], NameList [i], TextureList [i], SpriteList [i], PetTypeSprite[i]);
		}
		
		for (int j = 0; j < MAX_FORMATION_POS_NUM; j++) {
			LogicFormationPos [j] = new FormationPos (FormationPosArray [j], j,FormationPosTextureList[j]);
		}

		UIEventListener listenerLeftBtn = UIEventListener.Get(leftBtn.gameObject);
		listenerLeftBtn.onClick += OnClickLeftBtn;

		UIEventListener listenerRightBtn = UIEventListener.Get(rightBtn.gameObject);
		listenerRightBtn.onClick += OnClickRightBtn;
		
		UIEventListener listenExit = UIEventListener.Get(ButtonExit);
		listenExit.onClick += Exit;
		return true;
	}
	
	public override void Show ()
	{
		Vector3 showPos = new Vector3(-350, -30, 0);
		XNewPlayerGuideManager.SP.handleShowGuide2((int)XNewPlayerGuideManager.GuideType.Guide_BuZhenPetUse, 2, showPos, this.gameObject);
		
		StartEffect(900064, FormationPosArray[2].gameObject, new Vector3(7, 0, -20), new Vector3(4, 2, 1));
		StartEffect(900064, TextureList[0].gameObject.transform.parent.gameObject, new Vector3(2.61f, 20.88f, -20f), new Vector3(2.5f, 3f, 1f));
		
		base.Show ();
	}
	
	public void Exit(GameObject go)
	{		
		XEventManager.SP.SendEvent(EEvent.UI_Hide,EUIPanel.eFormation);
	}

	private void OnClickLeftBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Formation_LeftView);
	}

	private void OnClickRightBtn(GameObject go)
	{
		XEventManager.SP.SendEvent(EEvent.Formation_RightView);
	}
	
	#region effect
	private LinkedList<XU3dEffect>	m_effect = new LinkedList<XU3dEffect>();
	private LinkedList<GameObject>	mFuncGO = new LinkedList<GameObject>();
	private LinkedList<Vector3>	mListPos = new LinkedList<Vector3>();
	private LinkedList<Vector3>	mListSize = new LinkedList<Vector3>();

	public void StartEffect(uint effectid, GameObject go, Vector3 pos, Vector3 size)
	{
		mListPos.AddLast(new Vector3(pos.x, pos.y, pos.z));
		mListSize.AddLast(new Vector3(size.x, size.y, size.z));
		mFuncGO.AddLast(go);
		XU3dEffect effect = new XU3dEffect(effectid, EffectLoadedHandle);
		m_effect.AddLast(effect);
	}
	private void EffectLoadedHandle(XU3dEffect effect)
    {
		if ( 0 == mFuncGO.Count )
			return;
		
		GameObject lastGameObj = mFuncGO.First.Value;
		Vector3 lastPos = mListPos.First.Value;
		Vector3 lastSize = mListSize.First.Value;
		mFuncGO.RemoveFirst();
		mListPos.RemoveFirst();
		mListSize.RemoveFirst();
        effect.Layer = GlobalU3dDefine.Layer_UI_2D;
        effect.Parent = lastGameObj.transform;
        effect.LocalPosition =  new Vector3(lastPos.x, lastPos.y, lastPos.z); // new Vector3(0, -5, -10);
        effect.Scale = new Vector3(lastSize.x, lastSize.y, lastSize.z); // new Vector3(500, 500, 1);
    }
	public void StopEffect()
	{
		foreach(XU3dEffect effect in m_effect)
		{
			effect.Destroy();
		}
		m_effect.Clear();
	}
	#endregion
}