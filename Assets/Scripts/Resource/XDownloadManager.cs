namespace resource
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using UnityEngine;
	using System.Net;

	public static class XDownloadManager
	{
		private static Dictionary<string, WeakReference> cache = new Dictionary<string, WeakReference>();
		private static int CompareDownloadItem(DownloadItem left,DownloadItem right)
		{
			return left.priority - right.priority;
		}
		
		public static bool CacheAuthorized = false;
        public static bool hasError = false;
        public static bool hasItemDone = false;
        public static string LastErrorMsg = null;
        public static string LastErrorUrl = null;
        private static List<DownloadItem> loading = new List<DownloadItem>();
        public static int maxLoading = 5;
        private static bool needSort;
        public static int sizeLoaded = 0;
        private static List<DownloadItem> tmpList = new List<DownloadItem>();
        public static int totalLoaded = 0;
        private static List<DownloadItem> waiting = new List<DownloadItem>();

        public static bool CachingAuthorize(string name, string domain, long size, int expiration, string singature)
        {
			CacheAuthorized	= true;
			
//            CacheAuthorized = Caching.Authorize(name, domain, size, expiration, singature);
//            if (!CacheAuthorized)
//            {
//                UnityEngine.Debug.LogWarning(string.Format("Caching.Authorize Failed. name:{0}, domain:{1}, absoluteURL:{2}", name, domain, Application.absoluteURL));
//            }
//            else
//            {
//                UnityEngine.Debug.Log(string.Format("Caching.Authorize Succeeded. name:{0}, domain:{1}", name, domain));
//            }
            return CacheAuthorized;
        }

        public static void Cancel(DownloadItem item)
        {
            waiting.Remove(item);
        }

        public static void CancelAll()
        {
            waiting.Clear();
            List<DownloadItem> list = DebugGetAllDownloads();
            foreach (DownloadItem item in list)
            {
                if (item.www != null)
                {
                    item.www.Dispose();
                }
            }
            list.Clear();
        }

        public static List<DownloadItem> DebugGetAllDownloads()
        {
            List<DownloadItem> list = new List<DownloadItem>();
            foreach (KeyValuePair<string, WeakReference> pair in cache)
            {
                DownloadItem target = pair.Value.Target as DownloadItem;
                if (target != null)
                {
                    list.Add(target);
                }
            }
            return list;
        }

        public static void DebugGetDownloadInfo(out int numItems, out int numLoading, out int numWaiting)
        {
            int num = 0;
            foreach (KeyValuePair<string, WeakReference> pair in cache)
            {
                if (pair.Value.Target is DownloadItem)
                {
                    num++;
                }
            }
            numItems = num;
            numLoading = loading.Count;
            numWaiting = waiting.Count;
        }

        public static string GetDomainFromUrl(string url)
        {
            Match match = Regex.Match(url, @"(?<=://)[a-zA-Z\.0-9]+(?=\/)");
            return ((match == null) ? null : match.Value.ToString());
        }

        public static DownloadItem GetDownload(string url)
        {
            return GetDownload(url, 0, 0, 0);
        }

        public static DownloadItem GetDownload(string url, int version, int size, int priority)
        {
            string key = url;
			string realResName = key + '_' + version.ToString();
            DownloadItem target = null;
            if (cache.ContainsKey(realResName))
            {
                WeakReference reference = cache[realResName];
                target = reference.Target as DownloadItem;
                if (target != null)
                {
                    if ((!target.IsDone && !loading.Contains(target)) && !waiting.Contains(target))
                    {
                        waiting.Add(target);
                        needSort = true;
                    }
                    return target;
                }
            }
            int tryTimes = !IsImportant(url) ? 5 : 5;
            target = new DownloadItem(url, version, size, priority, tryTimes);
            waiting.Insert(0, target);
            needSort = true;
            cache[realResName] = new WeakReference(target);
            return target;
        }

        public static string GetIpsFromUrl(string url)
        {
            if (!url.StartsWith("http://"))
            {
                return string.Empty;
            }
            string str = null;
            string domainFromUrl = GetDomainFromUrl(url);
            if (domainFromUrl == null)
            {
                return str;
            }
            try
            {
                IPAddress[] hostAddresses = Dns.GetHostAddresses(domainFromUrl);
                if (hostAddresses.Length <= 0)
                {
                    return str;
                }
				
				return "";
//                if (<>f__am$cacheF == null)
//                {
//                    <>f__am$cacheF = ip => ip.ToString();
//                }
//                return string.Join(",", Array.ConvertAll<IPAddress, string>(hostAddresses, <>f__am$cacheF));
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        public static string GetLoadErrorDetail()
        {
            return (!hasError ? string.Empty : string.Format("Load Error, url:{0}\nmsg:{1}", LastErrorUrl, LastErrorMsg));
        }

        public static void Init(bool bAutoUpdate)
        {	
			return ;
            if (bAutoUpdate)
            {
                CoroutineManager.StartCoroutine(Loop());
            }
        }

        private static bool IsAssetBundle(string url)
        {
			return true;
			
			
            url = url.ToLower();
            if (url.Contains("?"))
            {
                char[] separator = new char[] { '?' };
                url = url.Split(separator)[0];
            }
            return ((((url.EndsWith(".ab") || url.EndsWith(".asset")) || (url.EndsWith(".scene") || url.EndsWith(".lightmap"))) || url.EndsWith(".aio")) || url.EndsWith(".config"));
        }

        private static bool IsImportant(string url)
        {
            url = url.ToLower();
            if (url.Contains("?"))
            {
                char[] separator = new char[] { '?' };
                url = url.Split(separator)[0];
            }
           	return (((url.EndsWith(".scene") || url.EndsWith(".config")) || url.EndsWith(".shield")) || url.EndsWith(".csv.gz"));
        }	
		
		
		public static IEnumerator Loop()
		{
			while(true)
			{
				XDownloadManager.Update();
				yield return 0;
			}
		}	
		
		public static void OnPriorityChanged()
        {
            needSort = true;
        }
		
		public static void Update()
        {
            AssetBundleDestroyer.DoDestroy();
            tmpList.Clear();
            foreach (DownloadItem item in loading)
            {
                if (item.CheckIsDone())
                {
                    tmpList.Add(item);
                }
            }
            hasItemDone = tmpList.Count > 0;
            if (hasItemDone)
            {
                foreach (DownloadItem item2 in tmpList)
                {
                    loading.Remove(item2);
                    item2.IsDone = true;
                    totalLoaded++;
					
#if RES_DEBUG
#else
					if (!item2.hasError && IsAssetBundle(item2.url))
                    {
                        object[] args = new object[] { "get assetBundle" };
                        Log.AddTempLog(args);
                        item2.ab = item2.www.assetBundle;
                        object[] objArray2 = new object[] { "get assetBundle: " + item2.ab };
                        Log.AddTempLog(objArray2);
                        if (item2.ab == null)
                        {
                            item2.hasError = true;
                            item2.error = "Can not get www.assetBundle";
                        }
                    }
                    if (!item2.hasError)
                    {
                        sizeLoaded += item2.size;
                    }
                    else
                    {
                        hasError = true;
                        LastErrorUrl = item2.url;
                        LastErrorMsg = item2.error;
                        object[] objArray3 = new object[] { item2.error, item2.url, item2.version, GetIpsFromUrl(item2.url) };
                        UnityEngine.Debug.LogError(string.Format("Load Error: {0}, url: {1}, version: {2}, ips:{3}", objArray3));
                    }
#endif
                    
                    item2._FireLoadCompleted();
                }
                tmpList.Clear();
            }
            int count = maxLoading - loading.Count;
            if ((count > 0) && (waiting.Count > 0))
            {
                if (needSort)
                {
                    needSort = false;                   
                    SortUtils.InsertionSort<DownloadItem>(waiting, CompareDownloadItem);
                }
                if (count > waiting.Count)
                {
                    count = waiting.Count;
                }
                for (int i = 0; i < count; i++)
                {
                    DownloadItem item3 = waiting[0];
                    waiting.Remove(item3);
                    loading.Add(item3);
                    bool useCache = CacheAuthorized && IsAssetBundle(item3.url);
                    item3.Start(useCache);
                }
            }
        }

	}
}