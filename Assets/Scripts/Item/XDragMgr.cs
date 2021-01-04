using System;
using UnityEngine;
using XGame.Client.Base.Pattern;

public enum EDrag_Type
{
	EDrag_Type_FormationPos,
	EDrag_Type_ModelPos,
}

public class XDragMgr : XSingleton<XDragMgr>
{		
	public bool IsDraging 			{get;set;}
	public ActionIcon_Type IconType {get;set;}
	public uint IconData 			{get;set;}
	
	
	//model drag data
	public EDrag_Type	DragType	{get;set;}
	public int PetIndex				{get;set;}
	public int FormationPos			{get;set;}
	public int ModelIndex			{get;set;}
	
	//shop source object from drop
	public XBaseActionIcon SourceShopItem {get;set;}
}