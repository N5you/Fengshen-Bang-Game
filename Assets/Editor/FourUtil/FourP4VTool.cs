using System;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Threading;

class FourP4VTool : AssetPostprocessor
{
    public static readonly string CMD_PATH = "cmd.exe";
    public static readonly string P4V_TOOL_NAME = "4p4vtool.bat";

    //private static Process MainP4VProcess = null;

    public static void ConnectP4V()
    {
        /*--4>: 只启动一次命令行然后传递命令过来有问题, 改成每次重新开启
        //--4>: 需要更多判断
        if (MainP4VProcess == null)
        {
            MainP4VProcess = new Process();
            MainP4VProcess.StartInfo.FileName = CMD_PATH;
            MainP4VProcess.StartInfo.UseShellExecute = false;
            MainP4VProcess.StartInfo.CreateNoWindow = true;
            MainP4VProcess.StartInfo.RedirectStandardInput = true;
            MainP4VProcess.StartInfo.RedirectStandardOutput = true;
            //MainP4VProcess.StartInfo.Arguments = "";
            //string output = p.StandardOutput.ReadToEnd();
            MainP4VProcess.Start();
        }
        */
    }

    public static void DisconnectP4v()
    {
        /*--4>: 需要更多判断
        if (MainP4VProcess != null)
        {
            MainP4VProcess.Close();
            MainP4VProcess = null;
        }
        */
    }

    #region AssetPostprocessor
    static void OnPostprocessAllAssets(
                string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        if (!FourUtil.IsRunning)
        {
            return;
        }

        string strAllImportedAssets = "";
        foreach (string assetPath in importedAssets)
        {
            strAllImportedAssets += FourUtil.GetAssertAndMetaFullPath(assetPath);
        }
        P4VAdd(strAllImportedAssets);
        P4VCheckout(strAllImportedAssets);

        string strAllDeletedAssets = "";
        foreach (string assetPath in deletedAssets)
        {
            strAllDeletedAssets += FourUtil.GetAssertAndMetaFullPath(assetPath);
        }
        P4VDelete(strAllDeletedAssets);

        /*
        string strAllMovedAssets = "";
        foreach (string assetPath in movedAssets)
        {
            strAllMovedAssets += FourUtil.GetAssertAndMetaFullPath(assetPath);
        }
        P4VAdd(strAllImportedAssets);
        */
    }
    #endregion

    #region p4v function

    public static void P4VAdd(string strAllFile)
    {
        DoP4VJob("add -d -f ", strAllFile);
    }

    public static void P4VCheckout(string strAllFile)
    {
        DoP4VJob("edit ", strAllFile);
    }

    public static void P4VRevert_Unchanged(string strAllFile)
    {
        DoP4VJob("revert -a ", strAllFile);
    }

    public static void P4VDelete(string strAllFile)
    {
        DoP4VJob("delete -v ", strAllFile);
    }

    public static void P4VRevert(string strAllFile)
    {
        DoP4VJob("revert ", strAllFile);
    }

    public static void DoP4VJob(string p4vCmd, string arg)
    {
        if (p4vCmd == null || p4vCmd == "" || arg == null || arg == "")
        {
            return;
        }
        Process p = new Process();
        p.StartInfo.FileName = GetP4VToolPath();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        //p.StartInfo.RedirectStandardInput = true;
        //p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.Arguments = p4vCmd + " " + arg;
        p.Start();
        p.WaitForExit();
        //string output = p.StandardOutput.ReadToEnd();
        //Log.Write("DoP4Job: cmd:{0}, output:{1}, file:{2}", p4vCmd, output, GetP4VToolPath());
        p.Close();
    }

    static string GetP4VToolPath()
    {
        string[] appPath = EditorApplication.applicationPath.Split(char.Parse("/"));
        appPath[appPath.Length - 2] = P4V_TOOL_NAME;
        return string.Join("/", appPath, 0, appPath.Length - 1);
    }

    #endregion
}
