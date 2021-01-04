public enum EStateId:int
{
	// XCharacter基础行为状态
	esIdle,			// 待机
	esFight,		// 战斗
	esDead,			// 死亡
	esSit,			// 打坐
	esMove,			// 移动状态: 跑动 & 原地跳 & 位移跳
	//---------------------
	
	// XMainPlayer操作状态
	esMainPlayerNone,	// 主角默认状态(什么操作都没有)
	esAutoMoveToPos,	// 自动寻路到点
	esAutoMoveToObj,	// 自动寻路到对象
	esKeyMove,			// 键盘移动
	esNavigate,			// 导航状态
	esNavKill,			// 连续杀人导航
	esMainSit,			// 打坐状态
	esPreEnterScene,	// 传送前状态

	//---------------------
	esBattle_Null,			//空状态
	esBattle_Move,			//使用技能前对象移动
	esBattle_RunForward,	//向前跑
	esBattle_RunBack,		//跑回
	esBattle_Move_Back,		//移动回来
	esBattle_Pre_UseSkill,	//移动到了目标位置，使用技能前的逻辑处理
	esBattle_Use_Skill,		//使用技能状态，主要处理技能的更新
	esBattle_After_UseSkill,//技能结束处理
	esBattle_Cut_Scene,//技能结束处理
	
	esBattle_StrikedRunBack,
}

public enum EStateEvent:int
{
	// XCharacter基础行为状态机事件
	esMoveTo,			// 移动到目标点
	esStopMove,			// 停止移动
	esJump,				// 跳跃
	esJumpOver,			// 跳跃完成
	esSit,				// 打坐
	esCancelSit,		// 取消打坐
	esStartFight,		// 进入战斗
	esQuitFight,		// 退出战斗
	esHp,				// 血量变化
	esAnimation,		// 播动画
	esSpeed,			// 移动速度发生变化
	esStartProduct,		// 开始生产
	esQuitProduct,		// 停止生产
	//----------------------
	
	// XMainPlayer操作状态机事件
	esAutoMoveToPos,	// 自动寻路到点
	esAutoMoveToObj,	// 自动寻路到对象
	esKeyMoveDir,		// 键盘移动方向改变
	esNavigateTo,		// 开始导航
	esNavigateKill,		//
	esForceSetPosition,	// 强行设置坐标, 终止当前操作(来自服务器)
	//----------------------
	
	esObjectAppear,		// 对象出生
	esBeginLoadLevel,	// 开始加载场景
	esLevelLoaded,		// 场景加载完成
	
	esRotateTo,			// 转向目标点
}