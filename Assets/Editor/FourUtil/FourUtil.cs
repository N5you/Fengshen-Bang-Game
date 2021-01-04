using System;
using UnityEngine;
using UnityEditor;

class FourUtil : EditorWindow
{
    public static readonly string U3D_ASSERT_DIR_NAME = "Assets";
    public static readonly string META_FILE_SUFFIX = ".meta";
    public static string ASSERT_FATHER_DIR { get; private set; }

    public static bool IsRunning { get; private set; }

    #region unity interface

    void OnEnable()
    {
        IsRunning = false;
        ASSERT_FATHER_DIR = GetAssetFatherPath();
        FourP4VTool.ConnectP4V();
    }

    void OnDestroy()
    {
        FourP4VTool.DisconnectP4v();
    }

    #endregion

    #region FourUtil GUI Window

    string m_SelectedFiles;
    string m_CommandReturn;
    Vector2 m_FileListScroll;
    static private FourUtil m_MainWindow;
    public static readonly string P4V_SELECTED_FILE_LIST = "P4V_SELECTED_FILE_LIST";

    [MenuItem("Tools/FourUtil/FourUtil #&4")]
    static void Init()
    {
        FourUtil m_MainWindow = EditorWindow.GetWindow(typeof(FourUtil)) as FourUtil;
        if (m_MainWindow != null)
        {
        }
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Perforce Tool", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal("box");

        // 不可编辑的文本框, 用 SelectableLabel 和 TextArea 有问题
        m_FileListScroll = EditorGUILayout.BeginScrollView(m_FileListScroll);
            GUI.enabled = false;
            GUI.SetNextControlName(P4V_SELECTED_FILE_LIST);
            //--4>TODO: 最好将文件当前是否已经 checkout 标记出来(考虑unity强制更改的文件)
            EditorGUILayout.TextArea(m_SelectedFiles);
            GUI.enabled = true;
        EditorGUILayout.EndScrollView();
        if (GUI.GetNameOfFocusedControl() == P4V_SELECTED_FILE_LIST)
        {
            //--4>TEST: Log.Write("[TEST] {0}", m_SelectedFiles);
            //EditorGUILayout.SelectableLabel(m_SelectedFiles);
        }

        EditorGUILayout.BeginVertical("box", GUILayout.Width(200));

            IsRunning = EditorGUILayout.Toggle("开启 P4VTool 自动检测", IsRunning);

            if (GUILayout.Button("checkout"))
            {
                CheckoutSelectedAssets();
            }
            if (GUILayout.Button("add"))
            {
                AddSelectedAssets();
            }
            if (GUILayout.Button("revert unchanged"))
            {
                RevertSelectedAssets_UnChanged();
            }
            /* 为了防止误操作, 这两个功能先隐藏了
            if (GUILayout.Button("revert"))
            {
                RevertSelectedAssets();
            }
            if (GUILayout.Button("delete"))
            {
                DeleteSelectedAssets();
            }
            */
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void OnSelectionChange()
    {
        m_SelectedFiles = GetSelectedData().Replace(" ", "\n");
        Repaint();
    }

    #endregion

    #region menu items

    [MenuItem("Tools/FourUtil/P4V Add #&A")]
    static void AddSelectedAssets()
    {
        FourP4VTool.P4VAdd("add -d -f " + GetSelectedData());
    }

    [MenuItem("Tools/FourUtil/P4V Checkout #&E")]
    static void CheckoutSelectedAssets()
    {
        FourP4VTool.P4VCheckout(GetSelectedData());
    }

    [MenuItem("Tools/FourUtil/P4V Revert Unchanged #&R")]
    static void RevertSelectedAssets_UnChanged()
    {
        FourP4VTool.P4VRevert_Unchanged(GetSelectedData());
    }

    [MenuItem("Tools/FourUtil/P4V Delete #&D")]
    static void DeleteSelectedAssets()
    {
        FourP4VTool.P4VDelete(GetSelectedData());
    }

    [MenuItem("Tools/FourUtil/P4V Revert #&U")]
    static void RevertSelectedAssets()
    {
        FourP4VTool.P4VRevert(GetSelectedData());
    }

    #endregion

    #region common functions

    public static string GetAssetFatherPath()
    {
        string strPath = Application.dataPath;
        strPath = strPath.Substring(0, strPath.Length - U3D_ASSERT_DIR_NAME.Length);
        return strPath;
    }

    public static string GetSelectedData()
    {
        string strSelectedAssets = "";
        for (int i = 0; i < Selection.instanceIDs.Length; ++i)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
            // 如果在 Hierarchy 界面或者 Scene 选中, 获得的是 Client 根目录, 排除之
            if (assetPath == "" || assetPath == null)
            {
                continue;
            }
            strSelectedAssets += GetAssertAndMetaFullPath(assetPath);
        }
        //--4>TEST: Log.Write(strSelectedAssets);
        return strSelectedAssets;
    }

    public static string GetAssertAndMetaFullPath(string assetPath)
    {
        if (assetPath == "" || assetPath == null)
        {
            return null;
        }
        string fullPath = ASSERT_FATHER_DIR + assetPath;
        string metaPath = fullPath + META_FILE_SUFFIX;
        if (System.IO.Directory.Exists(fullPath))
        {
            fullPath += "/...";
        }
        return fullPath + " " + metaPath + " ";
    }

    #endregion
}
