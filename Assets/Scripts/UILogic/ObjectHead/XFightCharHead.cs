using UnityEngine;
using System.Collections;
using XGame.Client.Packets;

public enum EFlyStrType			// 飘字样式, 注意: 新增样式的时候不要插队, 在枚举最后加
{
	eFlyStrType_Miss = 0,		// 未命中
	eFlyStrType_Defence,		// 抵挡
	eFlyStrType_Dodge,			// 闪避
	eFlyStrType_Crit,			// 暴击
	eFlyStrType_Normal,			// 正常攻击
	eFlyStrType_LittleStr1,		// 字体比较小(6种效果, 效果参见buff的掉血)
	eFlyStrType_LittleStr2,
	eFlyStrType_LittleStr3,
	eFlyStrType_LittleStr4,
	eFlyStrType_LittleStr5,
	eFlyStrType_LittleStr6,
	eFlyStrType_Count,			
}

[AddComponentMenu("UILogic/ObjectHead/XFightCharHead")]
public class XFightCharHead : XObjectHead
{
	public UILabel[] FlyStringSample;
	public UISlider BloodSlider = null;
	private bool m_bIsBloodShow = false;
	
	public override void FlyString(EFlyStrType ft, string str)
	{
		if(ft >= EFlyStrType.eFlyStrType_Count)
			return;
		
		UILabel label = XUtil.Instantiate<UILabel>(FlyStringSample[(int)ft]);
		label.text = str;
		
		TweenColor color = label.GetComponent<TweenColor>();
		if(color != null)
		{
			color.Reset();
			color.enabled = true;
		}
		
		TweenScale scale = label.GetComponent<TweenScale>();
		if(scale != null)
		{
			scale.Reset();
			scale.enabled = true;
		}
		
		TweenAlpha alpha = label.GetComponent<TweenAlpha>();
		if(alpha != null)
		{
			alpha.Reset();
			alpha.enabled = true;
		}
		
		
		NcCurveAnimation cur = label.GetComponent<NcCurveAnimation>();
		if(null != cur) cur.enabled = true;
	}
		
	public override void Show ()
	{
		base.Show();
		if(!m_bIsBloodShow)
			BloodSlider.gameObject.SetActive(false);
	}
	
	public override void ShowBlood(bool b)
	{
		if(m_bIsBloodShow == b) 
			return;
		m_bIsBloodShow = b;
		if(gameObject.activeSelf)
		{
			BloodSlider.gameObject.SetActive(b);
		}
	}
	
	// 设置血条进度
	public override void SetHp(int nHp, int nMaxHp)
	{
		if(nHp > nMaxHp || nMaxHp <= 0) return;
		if(nHp <= 0)
		{
			BloodSlider.sliderValue	= 0.0f;
			ShowBlood(false);
		}			
		else
			BloodSlider.sliderValue = ((float)nHp) / ((float)nMaxHp);
	}
}
