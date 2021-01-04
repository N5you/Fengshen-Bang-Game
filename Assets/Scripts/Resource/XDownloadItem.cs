namespace resource
{
	using System;
	using System.Collections.Generic;
	//using System.Runtime.CompilerServices;
	//using System.Runtime.InteropServices;
	using UnityEngine;
	
#if  RES_DEBUG
	using UnityEditor;
#endif

	public class DownloadItem : IDisposable
	{
		private string _back_server_path = "http://localhost/WebPlayer/";
		private string _original_src_path = "http://localhost/WebPlayer/";
		private float _time_start;
		private bool _use_cache;
		public AssetBundle ab;
		public string act_url = string.Empty;
		public object data;
		public string error;
		public bool hasError;
		public bool IsDone;
		public int priority;
		public int retryTimes;
		private static bool ServerRejectPostRequest	= true;
		public int size;
		public string url;
		public int version;
		public WWW www;
		public UnityEngine.Object go;

		public event LoadCompletedDelegate LoadCompletedEvent;
		public bool IsAddDelegate = false;

		public DownloadItem(string url, int version, int size, int priority, int tryTimes)
		{
			this.url = url;
			this.version = version;
			this.size = size;
			this.priority = priority;
			this.IsDone = false;
			this.retryTimes = tryTimes;
		}

		public void _FireLoadCompleted()
		{
			if (this.LoadCompletedEvent != null)
			{
				this.LoadCompletedEvent(this);
				this.LoadCompletedEvent = null;
			}
		}

		public bool CheckIsDone()
		{
			if (this.www != null)
			{
				if (this.www.isDone)
				{
					if (!string.IsNullOrEmpty(this.www.error))
					{
						if (this.www.error.Contains("rewind wasn't possible"))
						{
							Debug.Log("http resonse error, change POST to GET. error:" + this.www.error);
							ServerRejectPostRequest = true;
						}
						string error = this.www.error;
						if (this.Retry())
						{
							return false;
						}
						this.hasError = true;
						this.error = error;
						return true;
					}
					if ((!this._use_cache || (this.www.assetBundle == null)) && !string.IsNullOrEmpty(this.www.text))
					{
						string str2 = this.www.text.Substring(0, Mathf.Min(this.www.text.Length, 300));
						if (str2.ToLower().Contains("<html"))
						{
							Debug.Log("http resonse error:" + str2);
							if (this.Retry())
							{
								return false;
							}
							this.hasError = true;
							this.error = "Download Error! " + this.www.url + "\n" + this.www.text;
							return true;
						}
					}
					return true;
				}
				if ((this.www.progress != 0f) || ((Time.time - this._time_start) <= 10f))
				{
					return false;
				}
				if (this.Retry())
				{
					return false;
				}
				this.hasError = true;
				this.error = string.Format("Timeout, start:{0}, now:{1}, pass:{2}", this._time_start, Time.time, Time.time - this._time_start);
			}
			return true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.www != null)
			{
				if (disposing)
				{
				}
				AssetBundleDestroyer.Add(this.ab);
				this.www.Dispose();
				this.www = null;
			}
		}

		private void DownloadByBackupServer()
		{
			string url = !this.url.StartsWith("http://") ? this.url : (this.url + "?ver=" + this.version);
			this.act_url = this.GetBackServerUrl(url);
			this.www = new WWW(this.act_url);
		}

		~DownloadItem()
		{
			this.Dispose(false);
		}

		private string GetBackServerUrl(string url)
		{
			if (url.Contains(this._original_src_path))
			{
				Debug.Log("Download From backup server " + url);
				return url.Replace(this._original_src_path, this._back_server_path);
			}
			return url;
		}

		public static float GetLoadingProgress(IEnumerable<DownloadItem> list)
		{
			int num;
			int num2;
			float num3;
			GetLoadingProgress(list, out num, out num2, out num3);
			return num3;
		}

		public static void GetLoadingProgress(IEnumerable<DownloadItem> list, out int bytesLoaded, out int bytesTotal, out float prog)
		{
			int num = 0;
			int num2 = 0;
			bool flag = true;
			IEnumerator<DownloadItem> enumerator = list.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DownloadItem current = enumerator.Current;
					if (current.size > 0)
					{
						num2 += current.size;
						if (current.www != null)
						{
							num += !current.IsDone ? ((int)(current.size * current.www.progress)) : current.size;
						}
					}
					flag = flag && current.IsDone;
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			bytesLoaded = num;
			bytesTotal = num2;
			prog = !flag ? ((num2 != 0) ? (((float)bytesLoaded) / ((float)bytesTotal)) : 0f) : 1f;
		}

		public static bool IsAllDone(IEnumerable<DownloadItem> list)
		{
			IEnumerator<DownloadItem> enumerator = list.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DownloadItem current = enumerator.Current;
					if (!current.IsDone)
					{
						return false;
					}
				}
			}
			finally
			{
				if (enumerator == null)
				{
				}
				enumerator.Dispose();
			}
			return true;
		}

		private bool Retry()
		{
			Debug.LogWarning(string.Format("DownloadItem.Retry, retryTimes:{0}, url:{1}", this.retryTimes, this.url));
			if (this.www != null)
			{
				this.www.Dispose();
				this.www = null;
			}
			if (this.retryTimes <= 0)
			{
				return false;
			}
			this.retryTimes--;
			this.version++;
			if (this.retryTimes <= 2)
			{
				this.Start(this._use_cache, true);
			}
			else
			{
				this.Start(this._use_cache);
			}
			return true;
		}

		public void Start(bool useCache)
		{
			this.Start(useCache, false);
		}

		public void Start(bool useCache, bool use_back_server)
		{
#if RES_DEBUG
			go = AssetDatabase.LoadMainAssetAtPath("Assets/" + this.url);
#else
			if (this.www != null)
			{
				throw new InvalidOperationException();
			}
			this._use_cache = useCache;
			if (useCache)
			{
				if (!use_back_server)
				{
					string str;
					if(this.version == 0)
						str	= this.url;
					else
					 	str = !this.url.StartsWith("http://") ? this.url : (this.url + "_" + this.version + ".asset");
					object[] args = new object[] { string.Format("LoadFromCacheOrDownload, url:{0}, version:{1}", str, this.version) };
					Log.AddTempLog(args);
					this.act_url = str;
					this.www = WWW.LoadFromCacheOrDownload(str, this.version);
				}
				else
				{
					this.DownloadByBackupServer();
				}
			}
			else if (!use_back_server)
			{
				if ((Config.LANG == "cn") && !ServerRejectPostRequest)
				{
					WWWForm form = new WWWForm();
					form.AddField("ver", this.version);
					string str2 = !this.url.StartsWith("http://") ? this.url : (this.url + "_" + this.version + ".asset");
					this.act_url = str2;
					this.www = new WWW(this.url, form);
				}
				else
				{
					string url;
					if(this.version == 0)
						url	= this.url;
					else
					 	url = !this.url.StartsWith("http://") ? this.url : (this.url + "_" + this.version + ".asset");
					this.act_url = url;
					this.www = new WWW(url);
				}
			}
			else
			{
				this.DownloadByBackupServer();
			}
			this._time_start = Time.time;
#endif
		}

		public delegate void LoadCompletedDelegate(DownloadItem item);
	}
}

