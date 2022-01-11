using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PluginUtil.Loger
{
	public static class Log
	{
		#region class props

		public static int ErrorLevel { get; set; } = 2;

		public static string LogFolder
		{
			get
			{
				if (!Directory.Exists(PathConfig.LogPath)) Directory.CreateDirectory(PathConfig.LogPath);

				return PathConfig.LogPath;
			}
		}

		public static string ModelFolder
		{
			get
			{
				var mfold = Path.Combine(LogFolder, PathConfig.ModelFolderName);
				if (!Directory.Exists(mfold)) Directory.CreateDirectory(mfold);

				return mfold;
			}
		}

		private static IIiPathConfig PathConfig
		{
			get
			{
				if (_pathConfig == null) _pathConfig = new PathConfig();

				return _pathConfig;
			}
		}


		public static string BaseLogPath
		{
			get
			{
#if !DEV
				//   if (string.IsNullOrEmpty(_debugPath)){
				var appFolderPath = LogFolder;
				if (!Directory.Exists(appFolderPath))
				{
					Directory.CreateDirectory(appFolderPath);
				}

				return appFolderPath;
				// _debugPath = appFolderPath; //Path.Combine(, "Debug.log");


				// if (!File.Exists(_debugPath)){
				//     File.Create(_debugPath);
				// }


#else
				var logPath                                  = @"C:\Debug";
				if (!string.IsNullOrEmpty(_docName)) logPath = Path.Combine(logPath, _docName);


				if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

				return logPath;


#endif
				// return _debugPath;
			}
		}


		/// <summary>
		///     Get path for log file and create app dir and log file if there not exist
		/// </summary>
		/// <summary>
		///     Get path for log file and create app dir and log file if there not exist
		/// </summary>
		public static string ErrorPath => CheckAndCreatePath("Error.log");

		public static string DebugPath => CheckAndCreatePath("Debug.log");
		public static string LogPath => CheckAndCreatePath("Log.log");

		public static string WarningPath => CheckAndCreatePath("Warning.log");

		public static string InformationPath => CheckAndCreatePath("Information.log");

		/// <summary>
		///     Get path for log file and create app dir and log file if there not exist
		/// </summary>

		public static Func<object, string> ElementToStringConvector { get; set; } = x => x?.ToString() ?? "NULL";

		public static string LogDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(_logDirectory))
#if DEV
					_logDirectory = @"C:\Debug";
#else
					_logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
												 "Bilfinger");
#endif


				return _logDirectory;
			}
		}

		public static Exception GetException(this Exception ex, string comment)
		{
			return new Exception($"({comment}:\n\t\t\t{ex})");
		}

        #endregion

        #region Methods

        #region public

        //public static Exception _GetException(this Exception exception
                                              //, string s) => new($"({s}\n\t\t\t{exception})");

        public static void ConfigLogPath(IIiPathConfig newConfig)
		{
			_pathConfig = newConfig;
		}

		/// <summary>
		///     Write exception information
		/// </summary>
		/// <param name="data"></param>
		public static void LogError(this Exception e
									, string message = "")
		{
			var resultStr = $"{DateTime.Now.Date} [Error] {message} \n{e}\n";
			resultStr.WriteTo(ErrorPath);
		}

		/// <summary>
		///     Write data to log file
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="path"></param>
		public static void LogWarning(this Exception exception
									  , string information = "")
		{
			$"{exception.Message}:{information}".WriteTo(WarningPath);
		}

		/// <summary>
		///     Write exception information
		/// </summary>
		/// <param name="data"></param>
		public static void LogError(string exeption
									, string message = "")
		{
			//  PathConfig.Logger?.Error(exeption, message);
			exeption.WriteTo(ErrorPath);
		}

		/// <summary>
		///     Write data to log file
		/// </summary>
		/// <param name="data"></param>
		/// <param name="path"></param>
		public static void LogWarning(this string data
									  , string information = "")
		{
			data.WriteTo(WarningPath);
		}

		/// <summary>
		///     Write exception information
		/// </summary>
		/// <param name="data"></param>
		public static void ThrowError(this Exception e
									  , string message = "")
		{
			throw new Exception($"({message}\n\t\t{e})");
		}


		public static void WriteTo(this object o
								   , string path = ""
								   , bool append = true)
		{
			if (string.IsNullOrEmpty(path)) path = DebugPath;

			using (var strWr = new StreamWriter(path, true))
			{
				if (o is string str)
				{
					strWr.Write(str);
				}
				else
				{
					strWr.Write(o.ToString());
				}
			}
		}


		/// <summary>
		///     Write data to log file
		/// </summary>
		/// <param name="data"></param>
		/// <param name="path"></param>
		public static void Debug(this object data
								 , string path = "")
		{
			data.WriteLog(DebugPath, "[Debug]");
		}


		/// <summary>
		///     Write data to log file
		/// </summary>
		/// <param name="data"></param>
		/// <param name="path"></param>
		public static void WriteLog(this object data
									, string path = ""
									, string type = "[Information]")
		{
			if (!PathConfig.EnableLogging) return;

			try
			{
				if (string.IsNullOrEmpty(path)) path = LogPath;

				if (!File.Exists(path))
				{
					var directory = Path.GetDirectoryName(path);
					if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
				}


				try
				{
					if (data is string str)
					{
						var res = $"{DateTime.Now} {type} {str}";
						WriteLogStr(res, path);
					}
					else if (data is IEnumerable strEn)
					{
                        using var fstream = new StreamWriter(path, true);
						// convert string to bite
						var res = "/====================================================/" + "\n";
						var count = 1;
						foreach (var pp in strEn)
						{
							res += count + ". = " + ElementToStringConvector.Invoke(pp) + "\n";
							count++;
						}

						res = $"{DateTime.Now} {type} {res}";


						fstream.Write(res);
					}
					else
					{
						if (data == null)
						{
							WriteLogStr("NULL", path);
						}
						else
						{
							var res = $"{DateTime.Now} {type} {data.ToString()}";
							WriteLogStr(res, path);
						}
					}
				}
				catch
				{
					//ignore
				}
			}
			catch (Exception e)
			{
			}
		}


		public static void ClearLog()
		{
			try
			{
				ClearLog(DebugPath);
				ClearLog(InformationPath);
				ClearLog(WarningPath);
				ClearLog(LogPath);
			}
			catch (Exception ex)
			{
			}
		}

		public static void LoggingOn(bool b)
		{
			PathConfig.EnableLogging = b;
		}

		public static void AddDirectory(string docName)
		{
			if (!string.IsNullOrEmpty(docName))
				_docName = docName;
			else
				_docName = "";
		}

		#endregion

		#region private

		private static string CheckAndCreatePath(string fileName)
		{
			var path = Path.Combine(BaseLogPath, fileName);
			if (!File.Exists(path)) File.Create(path);

			return path;
		}


		private static void ClearLog(string logPath)
		{
			if (!File.Exists(logPath)) return;

			using (var str = new FileStream(logPath, FileMode.Create))
			{
				str.Write(new byte[] { }, 0, 0);
			}
		}

		private static void WriteLogEnu(IEnumerable<string> dataList)
		{
			try
			{
				using var fileStream = new FileStream(DebugPath, FileMode.Append);
				// convert string to bite
				var res = "/====================================================/" + "\n";
				var count = 1;
				foreach (var pp in dataList)
				{
					res += count + ". = " + ElementToStringConvector.Invoke(pp) + "\n";
					count++;
				}

				var array = Encoding.Default.GetBytes(res);
				// write array of bite to file

				fileStream.Write(array, 0, array.Length);
			}
			catch
			{
				//ignore
			}
		}

		private static void WriteLogStr(string data
										, string path)
		{
			try
			{
				using var fileStream = new StreamWriter(path, true);
				// convert string to bite
				var res = "";


				res += data + "\n";

				fileStream.Write(res);
			}
			catch (Exception e)
			{
				e.LogError();
			}
		}

		#endregion

		#endregion

		#region static

		//  private static string _debugPath{ get;  }

		private static IIiPathConfig _pathConfig;
		private static string _logDirectory;
		private static string _docName;

		#endregion
	}
}