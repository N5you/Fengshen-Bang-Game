using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using XGame.Client.Base.Map;

//--4>TODO: 需要修改输出方式, 和服务器对接, 实现整块读取
public class GenerateLogicMap
{
    static readonly string TERRAIN_SETTING_PATH = "/../../ServerRun/LogicSettings/Terrain/";

	[MenuItem("Tools/GenerateLogicMap")]
	public static void Execute()
	{
        string strObjFilePath = Application.dataPath + TERRAIN_SETTING_PATH + Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
		
		int XCellSum, ZCellSum;
		XCellData[,] CellArr;
		XSceneManager.ScanTerrain(Terrain.activeTerrain, out CellArr, out XCellSum, out ZCellSum);
		
		// 将处理好的格式文件写入到文件中
		FileStream fileStream = new FileStream(strObjFilePath, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(XCellSum);
		binaryWriter.Write(ZCellSum);
		for(int x=0; x<XCellSum; x++)
		{
			for(int z=0; z<ZCellSum; z++)
			{
				int nCell = 0;
				nCell = (int)Mathf.Ceil(CellArr[x, z].Height * 100);
				nCell <<= 4;
				nCell |= CellArr[x, z].BarrierType;
				binaryWriter.Write(nCell);
			}
		}
		
		binaryWriter.Close();
		fileStream.Close();
		
/*		XmlDocument xmlDoc = new XmlDocument();
		XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", null);
		xmlDoc.AppendChild(xmlDec);
		
		XmlElement root = xmlDoc.CreateElement("Terrain");
		xmlDoc.AppendChild(root);
		
		for(int i=0; i<XCellSum; i++)
		{
			for(int j=0; j<ZCellSum; j++)
			{
				XmlElement  element = xmlDoc.CreateElement("Cell");
				element.SetAttribute("X", "" + i);
				element.SetAttribute("Z", "" + j);
				element.SetAttribute("Height", "" + CellArr[i, j].Height);
				element.SetAttribute("BarrierType", "" + CellArr[i, j].BarrierType);
				root.AppendChild(element);
			}
		}
		xmlDoc.Save(strXmlFilePath);*/
		
		Debug.Log("Generate LogicMap " + Application.loadedLevelName + " Finished!");
	}
}
