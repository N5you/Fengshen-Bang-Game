using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("NGUI/Interaction/RadioButton")]
public class UIRadioButton : MonoBehaviour
{
	public delegate void OnRadioChanged(int nIndex);
	
	private class RadioBox
	{
		public delegate void OnSelect(int nIndex);
		
		private UICheckbox m_CheckBox;
		private int m_nIndex;
		public OnSelect onSelect;
		public bool isChecked;
		
		public RadioBox(UICheckbox box, int nIndex, OnSelect callBack)
		{
			m_CheckBox = box;
			m_CheckBox.onStateChange += onStateChange;
			m_nIndex = nIndex;
			onSelect += callBack;
			isChecked = false;
		}
		
		private void onStateChange(bool state)
		{
			if(isChecked && !state)
			{
				m_CheckBox.isChecked = true;
				return;
			}
			
			if(!isChecked && state)
			{
				isChecked = state;
				onSelect(m_nIndex);
			}
		}
	}
	
	public UICheckbox[] m_CheckBoxArr;
	private List<RadioBox> m_RadioBoxArr;
	private int m_nCurrentSelect = -1;
	public OnRadioChanged onRadioChanged;
	
	public int CurrentSelect 
	{ 
		get 
		{ 
			return m_nCurrentSelect; 
		}
		set
		{
			if(0 > value || value >= m_RadioBoxArr.Count)
				return;
			//m_RadioBoxArr[value].isChecked = true;
			m_CheckBoxArr[value].isChecked = true;
		}
	}
	
	void Awake ()
	{
		m_RadioBoxArr = new List<RadioBox>();
		for(int i=0; i<m_CheckBoxArr.Length; i++)
		{
			m_CheckBoxArr[i].isChecked = false;
			m_RadioBoxArr.Add(new RadioBox(m_CheckBoxArr[i], i, onSelect));
		}
		CurrentSelect = 0;
	}
	
	public void RandSelect()
	{
		CurrentSelect = Random.Range(0, m_RadioBoxArr.Count);
	}
	
	private void onSelect(int nIndex)
	{
		if(0 > nIndex || nIndex >= m_RadioBoxArr.Count || m_nCurrentSelect == nIndex)
			return;
		
		m_nCurrentSelect = nIndex;
		
		// 取消选中其他RadioBox
		for(int i=0; i<m_RadioBoxArr.Count; i++)
		{
			if(i == m_nCurrentSelect) continue;
			m_RadioBoxArr[i].isChecked = false;
			m_CheckBoxArr[i].isChecked = false;
		}
		
		if(null != onRadioChanged) 
			onRadioChanged(m_nCurrentSelect);
	}
}

