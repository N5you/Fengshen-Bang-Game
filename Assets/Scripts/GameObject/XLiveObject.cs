/*using System;
using UnityEngine;

//--4>TODO: 必须和服务器的 EClientMotionState 一致, 考虑用 proto 公用结构
public enum EMotionState
{
    ms_idle = 0,
    ms_run,
    ms_jump,    // 不同步跳跃, 客户端逻辑不好处理
    ms_sit,     // 修行
}

//--4>TODO: 目前的机制比较混乱, 以后添加更多状态时可能需要重构

/// <summary>
/// 能够移动的包含动作的游戏对象/物件
/// </summary>
public abstract class XLiveObject : XGameObject
{
    public EMotionState MotionState { get; private set; }

    public static readonly float MAX_JUMP_HEIGHT = 1.3f;
    public static readonly float MAX_JUMP_RISE_TIME = 0.5f;
    public static readonly float JUMP_G = -2.0f * MAX_JUMP_HEIGHT / (MAX_JUMP_RISE_TIME * MAX_JUMP_RISE_TIME);
    public static readonly float JUMP_INIT_VELOCITY = -1.0f * JUMP_G * MAX_JUMP_RISE_TIME;
    // 站立位置相对于地表的高度(应该是角色包围盒的高度)
    public static readonly float STAND_HEIGHT_ON_TERRAIN = 0.01f;

    public XLiveObject(ulong id) : base(id)
    {
        ObjectType = EObjectType.Character;
        Speed = Define.DEFAULT_RUN_SPEED;
        ResetObjectMoveInfo();
    }

    #region override XGameObject

    protected override void OnCreateModel()
    {
        if (MotionState == EMotionState.ms_idle)
        {
            PlayAnimation(EAnimName.Idle);
        }
    }

    public override bool Breathe()
    {
        bool bRet = base.Breathe();

        Vector3 posNow = GetPosition();
        switch (MotionState)
        {
            case EMotionState.ms_idle:
                doMotion_Idle(posNow);
                break;
            case EMotionState.ms_run:
                doMotion_Move(posNow);
                break;
            case EMotionState.ms_jump:
                doMotion_Jump(posNow);
                break;
            case EMotionState.ms_sit:
                doMotion_Sit(posNow);
                break;
        }
		return bRet;
    }

    public override void OnDisappear()
    {
        base.OnDisappear();
        StopObjectMove(GetPosition());
    }

    #endregion

    #region move logic interface

    protected void StartObjectMove(Vector3 fromPos, Vector3 toPos)
    {
        //--4>TODO: 考虑如果和当前位置不一致是否应该进行一些平滑处理或者额外的移动操作
        m_MoveFromPos = fromPos;
        m_MoveToPos = toPos;
        m_MoveDirection = m_MoveToPos - m_MoveFromPos;
		m_MoveDistance = XUtil.CalcDistanceXZ(m_MoveToPos, m_MoveFromPos);
        // 移动方向设置为水平方向
        m_MoveDirection.y = 0;

        if (MotionState != EMotionState.ms_jump)
        {
            changeMotionState(EMotionState.ms_run);
        }
        else
        {
            m_PreMotionState = EMotionState.ms_run;
        }

        //Log.Write(LogLevel.DEBUG, "[TEST] StartObjectMove, from:{0}, to:{1}", fromPos.ToString("F4"), toPos.ToString("F4"));
    }

    protected void StopObjectMove(Vector3 pos)
    {
        //--4>TODO: 考虑如果和当前位置不一致是否应该进行一些平滑处理或者额外的移动操作
        // 停止移动只设置水平位置, 因为可能停止水平移动时还在进行跳跃
        // 注意 SetPosition(pos) 需要修改, 因为其他玩家或者自己机器卡的时候设置位置都可能瞬移
        Vector3 posNow = GetPosition();
        pos.y = posNow.y;
        // 如果还在跳跃过程中, 先不立即设置位置, 不然忽然转方向会卡
        if (MotionState != EMotionState.ms_jump)
        {
            SetPosition(pos);
        }
        changeMotionState(EMotionState.ms_idle);
    }

    protected void StartObjectJump(Vector3 pos)
    {
        //--4>TODO: 如果其他玩家正在移动, 那起跳位置一般会有偏差, 如果直接设置位置会导致跳跃瞬移拉回
        //  暂时不管起跳点, 跳跃的时候直接执行跳跃逻辑
        //  注意 SetPosition(pos) 需要修改, 因为其他玩家或者自己机器卡的时候设置位置都可能瞬移
        //  这个需要修改, 理论上起跳点必须一致
        SetPosition(pos);
        if (MotionState == EMotionState.ms_jump)
        {
            //--4>: 理论上不应该出现这个
            Log.Write("[ERROR] StartObjectJump fail, already in jump state");
            return;
        }
        m_JumpTime = 0.0f;
        m_IsDrop = false;
        changeMotionState(EMotionState.ms_jump);
    }

    protected void StartObjectSit(Vector3 pos)
    {
        SetPosition(pos);
        changeMotionState(EMotionState.ms_sit);
    }

    protected void ResetObjectMoveInfo()
    {
        PlayAnimation(EAnimName.Idle);
        MotionState = EMotionState.ms_idle;
        m_PreMotionState = EMotionState.ms_idle;
        m_MoveFromPos = Vector3.zero;
        m_MoveToPos = Vector3.zero;
        m_MoveDirection = Vector3.zero;
		m_MoveDistance = 0.0f;
        m_JumpTime = 0.0f;
        m_IsDrop = false;
    }

    #endregion

    #region private move logic

    private Vector3 m_MoveFromPos = Vector3.zero;
    private Vector3 m_MoveToPos = Vector3.zero;
    private Vector3 m_MoveDirection = Vector3.zero;
	private float m_MoveDistance = 0.0f;
    // 跳跃之前的运动状态, 在跳跃结束后回归
    private EMotionState m_PreMotionState = EMotionState.ms_idle;
    // 距离起跳的时间
    private float m_JumpTime = 0.0f;
    // 设置当前是跳跃上升阶段还是下降阶段
    private bool m_IsDrop = false;

    //--4>TEST: 测试调试函数
    private void printMoveInfo4Debug(string msg)
    {
        Vector3 posNow = GetPosition();
        Log.Write("[TEST] {4}, posNow:{0}, FromPos:{1}, ToPos:{2}, MoveDir:{3}, JumpTime:{5}",
            posNow.ToString("F4"),
            m_MoveFromPos.ToString("F4"),
            m_MoveToPos.ToString("F4"),
            m_MoveDirection.ToString("F4"),
            msg,
            m_JumpTime);
    }

    private void changeMotionState(EMotionState st)
    {
        if (MotionState == EMotionState.ms_jump)
        {
            m_PreMotionState = st;
        }
        else
        {
            if (st == EMotionState.ms_jump)
            {
                m_PreMotionState = MotionState;
            }
            MotionState = st;
        }
        // 控制动画
        switch (MotionState)
        {
            case EMotionState.ms_idle:
                PlayAnimation(EAnimName.Idle);
                break;
            case EMotionState.ms_run:
                PlayAnimation(EAnimName.Run);
                break;
            case EMotionState.ms_jump:
                PlayAnimation(EAnimName.Jump);
                break;
            case EMotionState.ms_sit:
                PlayAnimation(EAnimName.Sit);
                break;
        }
    }

    private void doMotion_Idle(Vector3 posNow)
    {
        // 静止状态需要将玩家放到地面
        float terrainHeight = XLogicWorld.SP.SceneManager.GetTerrainHeight(posNow);
        if (terrainHeight > posNow.y)
        {
            Move(new Vector3(0.0f, terrainHeight - posNow.y, 0.0f));
        }
    }

    private void doMotion_Move(Vector3 posNow)
    {
        doMotion_Move(posNow, 0.0f);
    }

    private void doMotion_Move(Vector3 posNow, float ydir)
    {
        Vector3 posTarget = posNow + m_MoveDirection.normalized * Time.deltaTime * Speed;

        // 移动之前判断是否抵达终点, 防止走过头或者因为错位移动导致转向
        // 通过移动距离判断
        if (XUtil.CalcDistanceXZ(m_MoveFromPos, posTarget) + 0.01f >= m_MoveDistance)
        {
            posTarget = m_MoveToPos;
            changeMotionState(EMotionState.ms_idle);
        }
        if (MotionState == EMotionState.ms_run)
        {
            // 人物帖地面行走
            posTarget.y = XLogicWorld.SP.SceneManager.GetTerrainHeight(posNow);
        }
        else if (MotionState == EMotionState.ms_jump)
        {
            posTarget.y = posNow.y + ydir;
        }
        Move(posTarget - posNow);
    }

    private void doMotion_Jump(Vector3 posNow)
    {
        m_JumpTime += Time.deltaTime;
        float v = JUMP_INIT_VELOCITY + JUMP_G * m_JumpTime;
        float s = v * Time.deltaTime;
        bool bJumpOver = false;

        //--4>: 改变跳跃上升或者跳跃下落的动画
        if (v < 0.0f && !m_IsDrop)
        {
            m_IsDrop = true;
            PlayAnimation(EAnimName.Drop);
        }

        //--4>TODO: 这个判断存在问题, 需要修改
        if (m_IsDrop && IsGrounded(posNow))
        {
            s = 0.0f;
            bJumpOver = true;
        }
        if (m_PreMotionState == EMotionState.ms_run)
        {
            doMotion_Move(posNow, s);
        }
        else
        {
            Move(new Vector3(0.0f, s, 0.0f));
        }

        if (bJumpOver)
        {
            m_IsDrop = false;
            MotionState = m_PreMotionState;

            // 跳跃结束, 不调用 changeMotionState, 特殊处理, 播放过渡动画
            if (m_PreMotionState == EMotionState.ms_idle || m_PreMotionState == EMotionState.ms_sit)
            {
                PlayAnimation(EAnimName.DropStand);
                PushAnimation(EAnimName.Idle);
                MotionState = EMotionState.ms_idle;
            }
            else if (m_PreMotionState == EMotionState.ms_run)
            {
                PlayAnimation(EAnimName.DropRun);
                PushAnimation(EAnimName.Run);
            }

            m_PreMotionState = EMotionState.ms_jump;
        }
    }

    private void doMotion_Sit(Vector3 posNow)
    {
        // 打坐状态需要将玩家放到地面
        float terrainHeight = XLogicWorld.SP.SceneManager.GetTerrainHeight(posNow);
        if (terrainHeight > posNow.y)
        {
            Move(new Vector3(0.0f, terrainHeight - posNow.y, 0.0f));
        }
    }

    private void doDragToGround(Vector3 posNow)
    {
        float terrainHeight = XLogicWorld.SP.SceneManager.GetTerrainHeight(posNow);
        if (terrainHeight < posNow.y - 0.01f || terrainHeight > posNow.y + 0.01f)
        {
            Move(new Vector3(0.0f, terrainHeight - posNow.y, 0.0f));
        }
    }

    #endregion

	#region attr setting

    private XAttrLiveObject m_AttrLiveObject = new XAttrLiveObject();

    public virtual float Speed 
    {
        get { return m_AttrLiveObject.Speed; }
        set
        {
            m_AttrLiveObject.Speed = value;
        }
    }

    #endregion
}*/