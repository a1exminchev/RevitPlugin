using System;
using CSharpFunctionalExtensions;
using ScopeUtils;

namespace PluginUtil;

public static class ResultExtensions{
	public static Result<T> ToResult<T>(this T value) {
		if (value is null) {
			return Result.Failure<T>($"Value is null");
		}
		return Result.Success<T>(value);
	}

	public static Result<T> ToResult<T>(this T r, Exception ex) {
		return Result.Failure<T>(ex.ToString());
	}

	public static void LogError<T>(this Result<T> r) {
		if (r.IsFailure) {
			new Exception(r.Error).LogError();
		}
	}
}