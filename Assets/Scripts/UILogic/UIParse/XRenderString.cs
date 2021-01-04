using UnityEngine;
using System;
using System.Collections.Generic;

#region RenderString
public class RenderStr
{	
	public void Clear()
	{
		mLineList.Clear();
		
		for(int i = 0; i < mComponentList.Count; i++)
		{
			mComponentList[i].Clear();
		}
		
		mComponentList.Clear();
	}
	
	public UIFont.Alignment	StrAlignment 	{get;set;}
	public UIFont	StrFont  				{get;set;}
	public UILabel	Label					{get;set;}
	public int LineWidth 				 	{get;set;}
	public float LabelScale					{get;set;}
	public string ProcessedString 			{get;set;}
	private List<RenderStrComponent> mComponentList	= new List<RenderStrComponent>();
	private List<LineInfo>			 mLineList	= new List<LineInfo>();	
	
	public struct LineInfo
	{
		public LineInfo(int start,int count)
		{
			mStartIndex	= start;
			mCount		= count;
		}
		public int mStartIndex;
		public int mCount;
	}
	
	public void AppendLineBreak()
	{
		int first_Component = mLineList.Count == 0 ? 0 : (mLineList[mLineList.Count-1].mStartIndex + mLineList[mLineList.Count-1].mCount);
		mLineList.Add(new LineInfo(first_Component,0));
	}
	
	public void AppendComponent(RenderStrComponent strBase)
	{
		mComponentList.Add(strBase);
		if(mLineList.Count == 0)
			mLineList.Add(new LineInfo(0,0));
		
		if(mLineList.Count > 0)
		{
			//修改个数
			LineInfo info = mLineList[mLineList.Count-1];
			mLineList[mLineList.Count-1]	= new LineInfo(info.mStartIndex,info.mCount+1);
		}
	}
	
	public int GetLineCount()
	{
		return mLineList.Count;
	}
	
	public Vector2 GetLocalSize(int lineNumber)
	{
		if(lineNumber >= GetLineCount())
			return new Vector2();
		
		Vector2 result = new Vector2();
		
		int endCom = mLineList[lineNumber].mStartIndex + mLineList[lineNumber].mCount;
		for(int i = mLineList[lineNumber].mStartIndex; i < endCom; i++)
		{
			Vector2 size = mComponentList[i].GetLocalSize();
			result.x += size.x;
			if(size.y > result.y)
				result.y = size.y;
		}
		
		if(mLineList[lineNumber].mStartIndex == endCom && mComponentList.Count > 0)
		{
			 result.y = mComponentList[0].GetLocalSize().y;
		}
		
		return result;
	}
	
	public RenderStrTextComponent GetLinkTextComponent(Vector2 mousePos)
	{
		if(StrFont == null)
			return null;
		float fontHeight = StrFont.GetLineHeight();
		float reHeight = Math.Abs(mousePos.y);
		int lineNumber = (int)(reHeight / fontHeight);
		
		if(lineNumber < 0 || lineNumber >= mLineList.Count)
			return null;
		
		lineNumber	= mLineList.Count - 1 - lineNumber;
		
		int endCom = mLineList[lineNumber].mStartIndex + mLineList[lineNumber].mCount;
		
		float endPos 	= 0.0f;
		float startPos  = 0.0f;
		for(int i = mLineList[lineNumber].mStartIndex; i < endCom;i++)
		{
			Vector2 tempSize = mComponentList[i].GetLocalSize();
			endPos	+= tempSize.x;
			if(startPos <= mousePos.x && mousePos.x <= endPos)
			{
				RenderStrTextComponent Com = mComponentList[i] as RenderStrTextComponent;
				if(Com != null && Com.IsEnableHyperLink)
					return Com;
			}
			startPos	= endPos;
		}
		
		return null;
	}
	
	public string GetLineText(int lineNumber)
	{
		string ret = "";
		if(mLineList.Count <= lineNumber)
			return ret;
		
		int endIndex = mLineList[lineNumber].mStartIndex + mLineList[lineNumber].mCount;
		for(int i = mLineList[lineNumber].mStartIndex; i < endIndex; i++)
		{
			ret += mComponentList[i].GetText();
		}
		
		return ret;
	}
	
	public void Draw(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if ( Label.pivot == UIWidget.Pivot.TopRight )
			drawRight(verts, uvs, cols);
		else
			drawLeft(verts, uvs, cols);
	}
	
	private void drawLeft(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 startPos = new Vector2(0f,0f);
		int lineCount = GetLineCount();
		for(int i =0; i < lineCount; i++)
		{			
			int endComponent = mLineList[i].mStartIndex + mLineList[i].mCount;
			int vertOffset = verts.size;
			for(int j = mLineList[i].mStartIndex; j < endComponent;j++)
			{
				if(j < mComponentList.Count)
				{
					mComponentList[j].Draw(startPos,verts,uvs,cols);
					Vector2 temp =  mComponentList[j].GetLocalSize();
					startPos.x += temp.x;
				}
			}
			
			Vector2 scale = StrFont.GetScale();
			int offsetPixel = (int)(startPos.x * LabelScale);
			//对齐方式
			if(LineWidth == 0)
				LineWidth	= offsetPixel;
				
			StrFont.Align(verts,vertOffset,StrAlignment,offsetPixel,LineWidth,LabelScale);
			
			startPos.x = 0;
			if(i+1 < lineCount)
				startPos.y += GetLocalSize(i+1).y;
			else
				startPos.y += GetLocalSize(i).y;
		}
	}
	
	private void drawRight(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		int lineCount1 = GetLineCount();
		for(int i =0; i < lineCount1; i++)
		{			
			float len = 0f;
			int endComponent = mLineList[i].mStartIndex + mLineList[i].mCount;
			for(int j = endComponent - 1; j >= mLineList[i].mStartIndex; j--)
			{
				if(j < mComponentList.Count)
				{
					Vector2 temp =  mComponentList[j].GetLocalSize();
					len += temp.x;
					mComponentList[j].Pos = new Vector2(len, 0);
				}
			}
		}
		
		Vector2 startPos = new Vector2(0f,0f);
		int lineCount = GetLineCount();
		for(int i =0; i < lineCount; i++)
		{			
			int endComponent = mLineList[i].mStartIndex + mLineList[i].mCount;
			int vertOffset = verts.size;
			for(int j = mLineList[i].mStartIndex; j < endComponent;j++)
			{
				if(j < mComponentList.Count)
				{
					if ( 1 == mComponentList[j].type )
					{
						mComponentList[j].Pos.y = startPos.y;
						mComponentList[j].Draw(mComponentList[j].Pos,verts,uvs,cols);
					}
					else
					{
						mComponentList[j].Draw(startPos,verts,uvs,cols);
					}
					Vector2 temp =  mComponentList[j].GetLocalSize();
					startPos.x += temp.x;
				}
			}
			
			Vector2 scale = StrFont.GetScale();
			int offsetPixel = (int)(startPos.x * LabelScale);
			//对齐方式
			if(LineWidth == 0)
				LineWidth	= offsetPixel;
			StrFont.Align(verts,vertOffset,StrAlignment,offsetPixel,LineWidth,LabelScale);
			
			startPos.x = 0;
			if(i+1 < lineCount)
				startPos.y += GetLocalSize(i+1).y;
			else
				startPos.y += GetLocalSize(i).y;
		}
	}
}
#endregion
