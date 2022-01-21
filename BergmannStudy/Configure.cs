// Copyright (C) 2020-2021, Ed. Züblin AG
// All Rights Reserved.

using Autodesk.Revit.UI;
using PluginUtil.Loger;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Log = PluginUtil.Loger.Log;

// using Serilog.Sinks.Http;


namespace SCOPE_RevitPluginLogic.Utils;

public static class Configure{
	/// <summary>
	///     Configure common logger
	/// </summary>
	/// <param name="commandName"></param>
	/// <param name="docName"></param>
	/// <returns></returns>
	public static void ConfigureLogger(string? _docName = "") {
		Log.AddDirectory(_docName);

		Log.ClearLog();
	}

 
}