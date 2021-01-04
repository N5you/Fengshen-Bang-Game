// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public	class NgObject
{
	public static GameObject CreateGameObject(GameObject prefabObj)
	{
		GameObject newChild = (GameObject)NcSafeTool.SafeInstantiate(prefabObj);
		return newChild;
	}

	// 霸烙坷宏璃飘甫 积己茄促.
	public static GameObject CreateGameObject(GameObject parent, string name)
	{
		return CreateGameObject(parent.transform, name);
	}
	public static GameObject CreateGameObject(MonoBehaviour parent, string name)
	{
		return CreateGameObject(parent.transform, name);
	}
	public static GameObject CreateGameObject(Transform parent, string name)
	{
		GameObject newChild = new GameObject(name);
		if (parent != null)
		{	// 盔夯 transform阑 蜡瘤 矫难林磊
 			NcTransformTool	trans	= new NcTransformTool(newChild.transform);
			newChild.transform.parent = parent;
 			trans.CopyToLocalTransform(newChild.transform);
		}
		return newChild;
	}

	// 努沸 霸烙坷宏璃飘甫 积己茄促.
	public static GameObject CreateGameObject(GameObject parent, GameObject prefabObj)
	{
		return CreateGameObject(parent.transform, prefabObj);
	}
	public static GameObject CreateGameObject(MonoBehaviour parent, GameObject prefabObj)
	{
		return CreateGameObject(parent.transform, prefabObj);
	}
	public static GameObject CreateGameObject(Transform parent, GameObject prefabObj)
	{
		GameObject newChild = (GameObject)NcSafeTool.SafeInstantiate(prefabObj);
		if (parent != null)
		{	// 盔夯 transform阑 蜡瘤 矫难林磊
 			NcTransformTool	trans	= new NcTransformTool(newChild.transform);
			newChild.transform.parent = parent;
 			trans.CopyToLocalTransform(newChild.transform);
		}
		return newChild;
	}

	// 努沸 霸烙坷宏璃飘甫 积己茄促.
	public static GameObject CreateGameObject(GameObject parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		return CreateGameObject(parent.transform, prefabObj, pos, rot);
	}
	public static GameObject CreateGameObject(MonoBehaviour parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		return CreateGameObject(parent.transform, prefabObj, pos, rot);
	}
	public static GameObject CreateGameObject(Transform parent, GameObject prefabObj, Vector3 pos, Quaternion rot)
	{
		GameObject newChild;

		if (NcSafeTool.IsSafe() == false)
			return null;
		newChild = (GameObject)NcSafeTool.SafeInstantiate(prefabObj, pos, rot);
		if (parent != null)
		{	// 盔夯 transform阑 蜡瘤 矫难林磊
 			NcTransformTool	trans	= new NcTransformTool(newChild.transform);
			newChild.transform.parent = parent;
 			trans.CopyToLocalTransform(newChild.transform);
		}
		return newChild;
	}

	// 霸烙坷宏璃飘狼 瞒老靛甫 昏力茄促.
	public static void RemoveAllChildObject(GameObject parent)
	{
		for (int n = parent.transform.childCount-1; 0 <= n; n--)
		{
			if (n < parent.transform.childCount)
			{
				Transform	obj = parent.transform.GetChild(n);
				Object.DestroyImmediate(obj.gameObject);
			}
// 			obj.parent = null;
// 			Object.Destroy(obj.gameObject);
		}
	}

	// 哪器惩飘甫 积己茄促.(乐栏搁 胶诺)
	public static Component CreateComponent(Transform parent, System.Type type)
	{
		return CreateComponent(parent.gameObject, type);
	}
	public static Component CreateComponent(MonoBehaviour parent, System.Type type)
	{
		return CreateComponent(parent.gameObject, type);
	}
	public static Component CreateComponent(GameObject parent, System.Type type)
	{
		Component com = parent.GetComponent(type);
		if (com != null)
			return com;
		else
		{
			com = parent.AddComponent(type);
			return com;
		}
	}

	// 瞒老靛狼 葛电 坷宏璃飘甫 八祸茄促
	public static Transform FindTransform(Transform rootTrans, string name)
	{
		Transform dt = rootTrans.Find(name);
		if (dt)
			return dt;
		else
		{
			foreach (Transform child in rootTrans)
			{
				dt = FindTransform(child, name);
				if (dt)
					return dt;
			}
		}
		return null;
	}

	public static bool FindTransform(Transform rootTrans, Transform findTrans)
	{
		if (rootTrans == findTrans)
			return true;
		else
		{
			foreach (Transform child in rootTrans)
				if (FindTransform(child, findTrans))
					return true;
		}
		return false;
	}

	// 瞒老靛狼 葛电 Mesh material阑 函版茄促
	public static Material ChangeMeshMaterial(Transform t, Material newMat)
	{
		MeshRenderer[]	ren = t.GetComponentsInChildren<MeshRenderer>(true);
		Material		reMat = null;
		for (int n = 0; n < ren.Length; n++)
		{
			reMat = ren[n].material;
			ren[n].material = newMat;
		}
		return reMat;
	}

	// 瞒老靛狼 葛电 SkinnedMesh material狼 color甫 函版茄促
	public static void ChangeSkinnedMeshColor(Transform t, Color color)
	{
		SkinnedMeshRenderer[]	ren = t.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int n = 0; n < ren.Length; n++)
			ren[n].material.color = color;
	}

	// 瞒老靛狼 葛电 Mesh material狼 color甫 函版茄促
	public static void ChangeMeshColor(Transform t, Color color)
	{
		MeshRenderer[]	ren = t.GetComponentsInChildren<MeshRenderer>(true);
		for (int n = 0; n < ren.Length; n++)
			ren[n].material.color = color;
	}

	// 瞒老靛狼 葛电 SkinnedMesh material狼 alpha甫 函版茄促
	public static void ChangeSkinnedMeshAlpha(Transform t, float alpha)
	{
		SkinnedMeshRenderer[]	ren = t.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		for (int n = 0; n < ren.Length; n++)
		{
			Color al = ren[n].material.color;
			al.a = alpha;
			ren[n].material.color = al;
		}
	}

	// 瞒老靛狼 葛电 Mesh material狼 Alpha甫 函版茄促
	public static void ChangeMeshAlpha(Transform t, float alpha)
	{
		MeshRenderer[]	ren = t.GetComponentsInChildren<MeshRenderer>(true);
		for (int n = 0; n < ren.Length; n++)
		{
			Color al = ren[n].material.color;
			al.a = alpha;
			ren[n].material.color = al;
		}
	}

	// 辑宏器窃 瞒老靛 府胶飘甫 备己秦辑 府畔茄促.
	public static Transform[] GetChilds(Transform parentObj)
	{
		Transform[] arr = parentObj.GetComponentsInChildren<Transform>(true);
		Transform[] arr2 = new Transform[arr.Length - 1];
		for (int i = 1; i < arr.Length; i++)
		{
			arr2[i - 1] = arr[i];
		}
		return arr2;
	}

	// 辑宏急琶 瞒老靛 府胶飘甫 name栏肺 家泼备己秦辑 府畔茄促.
	public static SortedList GetChildsSortList(Transform parentObj, bool bSub)
	{
		SortedList	sortList = new SortedList();

		if (bSub)
		{
			Transform[] arr	= parentObj.GetComponentsInChildren<Transform>(true);

			for (int i = 1; i < arr.Length; i++)
			{
// 				Debug.Log(arr[i]);
//	 			Debug.Log(arr[i].name);
				sortList.Add(arr[i].name, arr[i]);
			}
		} else {
			for (int i = 0; i < parentObj.childCount; i++)
			{
				Transform trans = parentObj.GetChild(i);
				sortList.Add(trans.name, trans);
			}
		}
		return sortList;
	}

	// 辑宏器窃 tag啊 乐绰 霉 obj 府畔茄促.
	public static GameObject FindObjectWithTag(GameObject rootObj, string findTag)
	{
		if (rootObj == null)
			return null;
		if (rootObj.tag == findTag)
			return rootObj;

		for (int n = 0; n < rootObj.transform.childCount; n++)
		{
			GameObject	find = FindObjectWithTag(rootObj.transform.GetChild(n).gameObject, findTag);
			if (find != null)
				return find;
		}
		return null;
	}

	// 辑宏器窃 layer啊 乐绰 霉 obj 府畔茄促.
	public static GameObject FindObjectWithLayer(GameObject rootObj, int nFindLayer)
	{
		if (rootObj == null)
			return null;
		if (rootObj.layer == nFindLayer)
			return rootObj;

		for (int n = 0; n < rootObj.transform.childCount; n++)
		{
			GameObject	find = FindObjectWithLayer(rootObj.transform.GetChild(n).gameObject, nFindLayer);
			if (find != null)
				return find;
		}
		return null;
	}

	// tag疙阑 葛滴 官槽促.
	public static void ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (rootObj == null)
			return;
		rootObj.layer = nLayer;
		for (int n = 0; n < rootObj.transform.childCount; n++)
			ChangeLayerWithChild(rootObj.transform.GetChild(n).gameObject, nLayer);
	}

	// mesh count ================================================================================
	public static void GetMeshInfo(GameObject selObj, bool bInChildren, out int nVertices, out int nTriangles, out int nMeshCount)
	{
		Component[] skinnedMeshes;
		Component[] meshFilters;

		nVertices	= 0;
		nTriangles	= 0;
		nMeshCount	= 0;

		if (selObj == null)
			return;

		if (bInChildren)
		{
			skinnedMeshes = selObj.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
			meshFilters = selObj.GetComponentsInChildren(typeof(MeshFilter));
		} else
		{
			skinnedMeshes = selObj.GetComponents(typeof(SkinnedMeshRenderer));
			meshFilters = selObj.GetComponents(typeof(MeshFilter));
		}

		ArrayList totalMeshes = new ArrayList(meshFilters.Length + skinnedMeshes.Length);

		for (int meshFiltersIndex = 0; meshFiltersIndex < meshFilters.Length; meshFiltersIndex++)
		{
			MeshFilter meshFilter = (MeshFilter)meshFilters[meshFiltersIndex];
			totalMeshes.Add(meshFilter.sharedMesh);
		}

		for (int skinnedMeshIndex = 0; skinnedMeshIndex < skinnedMeshes.Length; skinnedMeshIndex++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)skinnedMeshes[skinnedMeshIndex];
			totalMeshes.Add(skinnedMeshRenderer.sharedMesh);
		}

		for (int i = 0; i < totalMeshes.Count; i++)
		{
			Mesh mesh = (Mesh)totalMeshes[i];
			if (mesh != null)
			{
				nVertices += mesh.vertexCount;
				nTriangles += mesh.triangles.Length / 3;
				nMeshCount++;
			}
		}
	}

	public static void GetMeshInfo(Mesh mesh, out int nVertices, out int nTriangles, out int nMeshCount)
	{
		nVertices	= 0;
		nTriangles	= 0;
		nMeshCount	= 0;

		if (mesh == null)
			return;

		if (mesh != null)
		{
			nVertices += mesh.vertexCount;
			nTriangles += mesh.triangles.Length / 3;
			nMeshCount++;
		}
	}
}
