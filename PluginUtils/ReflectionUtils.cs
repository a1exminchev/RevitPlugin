 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScopeUtils;

namespace PluginUtil;

/// <summary>
///     Class ReflectionUtils.
/// </summary>
public static class ReflectionUtils{
	/// <summary>
	// Copyright (C) 2020-2021, Ed. Züblin AG
	// All Rights Reserved.
	/// The not sort types
	/// </summary>
	private static readonly List<Type> NotSortTypes = new(){
		typeof(string), typeof(int), typeof(bool), typeof(double), typeof(long), typeof(decimal), typeof(float), typeof(char), typeof(ulong)
	};

	/// <summary>
	///     Gets the generic method.
	/// </summary>
	/// <param name="e">The e.</param>
	/// <param name="add">The add.</param>
	/// <returns>MethodInfo.</returns>
	private static MethodInfo? GetGenericMethod(object e, string add) {
		var methods = e.GetType()
		               .GetMethods()
		               .Where(x => x.Name.Equals(add))
		               ?.FirstOrDefault();


		return methods;
	}

	/// <summary>
	///     Check if parameters types equal to constructor arguments types
	/// </summary>
	/// <param name="constructorParameters">The constructor parameters.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns><c>true</c> if [is situable arguments] [the specified constructor parameters]; otherwise, <c>false</c>.</returns>
	private static bool IsSituableArgs(List<Type> constructorParameters, List<Type> arguments) {
		if (constructorParameters.Count == arguments.Count) {
			for (var i = 0; i < constructorParameters.Count; i++) {
				var constrArg = constructorParameters[i];
				var arg       = arguments[i];

				if (constrArg == arg) {
					continue;
				}


				if (constrArg.IsInterface) {
					if (arg.HaveInterface(constrArg)) {
						continue;
					}
				} else if (arg.IsSubclassOf(constrArg)) {
					continue;
				}

				return false;
			}

			return true;
		}

		return false;
	}

	private static void ExtractFromCollection<T>(object? value, ref List<T> retutnValue) {
		if (value == null) {
			return;
		}

		var objType = value.GetType();

		if (objType.IsDictionary()) {
			if (objType.GetGenericArguments()[1].IsSimpleType()) {
				return;
			}

			var collection = (value as IDictionary)?.Values;

			if (collection != null) {
				foreach (var val in collection) {
					ExtractAllTypeElements(val, ref retutnValue);
				}
			}
		} else if (objType.IsList()) {
			if (objType.GetGenericArguments()[0].IsSimpleType()) {
				return;
			}

			foreach (var val in (value as IList)!) {
				ExtractAllTypeElements(val, ref retutnValue);
			}
		}
	}

	public static void CopyPropsTo(object donor, object pacient) {
		try {
			var donorProps = donor.GetType().GetProperties();

			var props = pacient.GetType()
			                   .GetProperties()
			                   .ToDictionary(x => x.Name);

			foreach (var pr in donorProps) {
				if (props.ContainsKey(pr.Name)) {
					var pacientProp = props[pr.Name];
					var canWrite    = pacientProp?.CanWrite ?? false;
					if (canWrite) {
						try {
							var value = pr.GetValue(donor);
							pacientProp?.SetValue(pacient, value);
						} catch (Exception e) {
							// e.LogError("Error ");
							e.LogWarning($"Error on copy property {pr.Name} from {donor.GetType().Name} " +
							             $"to {pacient.GetType().Name} in method {nameof(CopyPropsTo)} " +
							             $"in class {nameof(ReflectionUtils)}");
						}
					}
				}
			}
		} catch (Exception e) {
			throw e.GetException($"Error in on copy props for {pacient.GetType().Name} from  {donor.GetType().Name}!");
		}
	}

	/// <summary>
	///     Haves the interface.
	/// </summary>
	/// <param name="e">The e.</param>
	/// <param name="interfac">The interfac.</param>
	/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
	public static bool HaveInterface(this object e, Type interfac) {
		Type? elType = null;
		if (e is Type tp) {
			elType = tp;
		} else {
			elType = e.GetType();
		}

		var intrface = elType.GetInterface(interfac.Name);
		return intrface != null;
	}


	/// <summary>
	///     Determines whether the specified e is list.
	/// </summary>
	/// <param name="e">The e.</param>
	/// <returns><c>true</c> if the specified e is list; otherwise, <c>false</c>.</returns>
	public static bool IsList(this object e) {
		return e.HaveInterface(typeof(IList));
	}

	/// <summary>
	///     Determines whether the specified e is enumerable.
	/// </summary>
	/// <param name="e">The e.</param>
	/// <returns><c>true</c> if the specified e is enumerable; otherwise, <c>false</c>.</returns>
	public static bool IsEnumerable(this object e) {
		return e.HaveInterface(typeof(IEnumerable));
	}

	/// <summary>
	///     Determines whether the specified e is dictionary.
	/// </summary>
	/// <param name="e">The e.</param>
	/// <returns><c>true</c> if the specified e is dictionary; otherwise, <c>false</c>.</returns>
	public static bool IsDictionary(this object e) {
		return e.HaveInterface(typeof(IDictionary));
	}


	/// <summary>
	///     Creates the new object.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="Exception">({type} - {e})</exception>
	public static object? CreateNewObject(this Type type) {
		try {
			if (type == typeof(IList)) {
				return CreateNewList(type.GenericTypeArguments.FirstOrDefault());
			}

			var retutnValue = Activator.CreateInstance(type);

			return retutnValue;
		} catch (Exception e) {
			throw new Exception($"({type} - {e})");
		}
	}


	/// <summary>
	///     Creates the new object.
	/// </summary>
	/// <returns>System.Object.</returns>
	/// <exception cref="Exception">({type} - {e})</exception>
	public static T? CreateNewObject<T>() {
		var type = typeof(T);
		try {
			if (type == typeof(IList)) {
				return (T)CreateNewList(type.GenericTypeArguments.FirstOrDefault())!;
			}

			var retutnValue = Activator.CreateInstance(type);

			return (T)retutnValue!;
		} catch (Exception e) {
			throw new Exception($"({type} - {e})");
		}
	}

	/// <summary>
	///     Creates the new list.
	/// </summary>
	/// <param name="mapEl">The map el.</param>
	/// <returns>IList.</returns>
	public static IList? CreateNewList(Type? mapEl) {
		var generic = typeof(List<>);
		if (mapEl != null) {
			var constructed = generic.MakeGenericType(mapEl);

			return constructed.CreateNewObject() as IList;
		}

		return new List<object>();
	}

	/// <summary>
	///     Creates the new list2.
	/// </summary>
	/// <param name="mapEl">The map el.</param>
	/// <returns>System.Object.</returns>
	public static object? CreateNewList2(Type mapEl) {
		var generic     = typeof(List<>);
		var constructed = generic.MakeGenericType(mapEl);


		return constructed.CreateNewObject() as IList;
	}

	/// <summary>
	///     Gets the list argument.
	/// </summary>
	/// <param name="propType">Type of the property.</param>
	/// <returns>Type.</returns>
	public static Type? GetListArgument(Type propType) {
		var listElementType = propType.GenericTypeArguments.FirstOrDefault();
		if (listElementType == null) {
			listElementType = propType.GetElementType();
		}

		return listElementType;
	}

	/// <summary>
	///     Check if type is base C# type like int, string etc
	/// </summary>
	/// <param name="o">The o.</param>
	/// <returns><c>true</c> if [is simple type] [the specified o]; otherwise, <c>false</c>.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public static bool IsSimpleType(this object o) {
		var   typeCod = TypeCode.Object;
		Type? t       = null;

		if (o is Type type) {
			typeCod = Type.GetTypeCode(type);
			t       = type;
		} else {
			t       = o.GetType();
			typeCod = Type.GetTypeCode(t);
		}


		switch (typeCod) {
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:

			case TypeCode.String:
				return true;
			default: {
				if (t.IsEnum) {
					return true;
				}

				if (t.Name.Equals(nameof(Object))) {
					return true;
				}

				return false;
			}
		}
	}


	/// <summary>
	///     Appends to list.
	/// </summary>
	/// <param name="newList">The new list.</param>
	/// <param name="value">The value.</param>
	public static void AppendToList(object newList, object value) {
		if (newList.HaveInterface(typeof(IList<>))) {
			if (value is IList list) {
				var method = GetGenericMethod(newList, "Add");

				foreach (var el in list) {
					method?.Invoke(newList, new[]{ el });
				}
			}
		}
	}

	/// <summary>
	///     Calls the void method.
	/// </summary>
	/// <param name="propVal">The property value.</param>
	/// <param name="getDbIdName">Name of the get database identifier.</param>
	public static void CallVoidMethod(object propVal, string getDbIdName) {
		var method = propVal.GetType().GetMethod(getDbIdName);
		if (method != null) {
			method.Invoke(propVal, null);
		}
	}

	/// <summary>
	///     Gets the property.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="creatorTypeNameName">Name of the creator type name.</param>
	/// <returns>PropertyInfo.</returns>
	public static PropertyInfo? GetProperty(Type type, string creatorTypeNameName) {
		var props = type.GetProperties();

		return props.FirstOrDefault(x => x.Name.Equals(creatorTypeNameName));
	}


	/// <summary>
	///     Constructs the element.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>System.Object.</returns>
	public static T ConstructElement<T>(params object[]? arguments) {
		return (T)ConstructElement(typeof(T), arguments);
	}

	/// <summary>
	///     Constructs the element.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>System.Object.</returns>
	public static object? ConstructElement(Type type, params object[]? arguments) {
		var constructors = type.GetConstructors();
		if (arguments?.Length < 1) {
			return Activator.CreateInstance(type);
		}

		if (arguments != null) {
			var argTypes = arguments.Select(x => x.GetType()).ToList();
			foreach (var constructor in constructors) {
				var constructorParameters = constructor.GetParameters().Select(x => x.ParameterType).ToList();

				if (IsSituableArgs(constructorParameters, argTypes)) {
					return constructor.Invoke(arguments);
				}
			}
		}

		return null;
	}

	/// <summary>
	///     Gets the property value.
	/// </summary>
	/// <param name="o">The o.</param>
	/// <param name="propName">Name of the property.</param>
	/// <returns>System.Nullable&lt;System.Object&gt;.</returns>
	public static object? GetPropertyValue(this object o, string propName) {
		var prop = GetProperty(o, propName);

		if (prop != null) {
			return prop.GetValue(o);
		}

		return null;
	}

	/// <summary>
	///     Gets the property.
	/// </summary>
	/// <param name="o">The o.</param>
	/// <param name="propName">Name of the property.</param>
	/// <returns>System.Nullable&lt;PropertyInfo&gt;.</returns>
	public static PropertyInfo? GetProperty(this object o, string propName) {
		var t = o.GetType();

		return t.GetProperties().FirstOrDefault(x => x.Name == propName);
	}

	/// <summary>
	///     Gets the property value.
	/// </summary>
	/// <param name="o">The o.</param>
	/// <param name="propName">Name of the property.</param>
	/// <param name="propType">Type of the property.</param>
	/// <returns>System.Nullable&lt;System.Object&gt;.</returns>
	public static object? GetPropertyValue(this object o
	                                       , string    propName
	                                       , out Type? propType) {
		var t = o.GetType();

		var prop = t.GetProperties().FirstOrDefault(x => x.Name == propName);
		propType = prop?.PropertyType;
		if (prop != null) {
			return prop.GetValue(o);
		}

		return null;
	}

 

	/// <summary>
	///     Gets the default.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>System.Object.</returns>
	public static object? GetDefault(Type type) {
		if (type.IsValueType) {
			return Activator.CreateInstance(type);
		}

		return null;
	}

	/// <summary>
	///     Gets the type of the generic value.
	/// </summary>
	/// <param name="dict">The dictionary.</param>
	/// <returns>Type.</returns>
	public static Type GetGenericValueType(object dict) {
		Type? t = null;
		if (dict is Type type) {
			t = type;
		} else {
			t = dict.GetType();
		}

		var arguments = t.GetGenericArguments();

		if (dict.IsDictionary()) {
			return arguments[1];
		}

		return arguments[0];
	}

	public static T CreateFrom<T>(object propDonor) {
		var newObj = CreateNewObject<T>();
		if (newObj == null) {
			return ConstructElement<T>();
		}

		CopyPropsTo(propDonor, newObj);


		return newObj;
	}

	public static void ExtractAllTypeElements<T>(this object obj, ref List<T> retutnValue) {
		if (obj.GetType().IsSimpleType()) {
			return;
		}

		if (obj.GetType().IsEnumerable()) {
			ExtractFromCollection(obj, ref retutnValue);
			return;
		}

		if (obj is T t) {
			retutnValue.Add(t);
		}

		var elementType = obj.GetType();
		var allProps    = elementType.GetProperties();

		foreach (var prop in allProps.Where(x => !x.PropertyType.IsSimpleType())) {
			if (!prop.PropertyType.IsSimpleType()) {
				try {
					var value = prop.GetValue(obj);
					if (value == null) {
						continue;
					}

					if (prop.PropertyType.IsEnumerable()) {
						ExtractFromCollection(value, ref retutnValue);
					} else {
						ExtractAllTypeElements(value, ref retutnValue);
					}
				} catch (Exception e) {
					e.LogError($"Error on proceed parameter {prop.Name}:{prop.PropertyType}!");
				}
			}
		}
	}
}