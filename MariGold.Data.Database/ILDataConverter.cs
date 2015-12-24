namespace MariGold.Data
{
	using System;
	using System.Data;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Collections.Generic;
	using System.Collections.Concurrent;

	/// <summary>
	/// Creates dynamic methods to iterate through data reader and generate CLR objects using IL Emit.
	/// </summary>
	public sealed class ILDataConverter : IDataConverter
	{
		private static ConcurrentDictionary<Type,Delegate> methods;
		
		private MethodInfo GetDrMethod(Type drType, string propTypeName)
		{
			MethodInfo drMethod = null;

			switch (propTypeName)
			{
				case "Int32":
					drMethod = drType.GetMethod("GetInt32");
					break;

				case "String":
					drMethod = drType.GetMethod("GetString");
					break;

				case "Boolean":
					drMethod = drType.GetMethod("GetBoolean");
					break;

				case "Char":
					drMethod = drType.GetMethod("GetChar");
					break;

				case "DateTime":
					drMethod = drType.GetMethod("GetDateTime");
					break;

				case "Decimal":
					drMethod = drType.GetMethod("GetDecimal");
					break;

				case "Double":
					drMethod = drType.GetMethod("GetDouble");
					break;

				case "Single":
					drMethod = drType.GetMethod("GetFloat");
					break;

				case "Int16":
					drMethod = drType.GetMethod("GetInt16");
					break;

				case "Int64":
					drMethod = drType.GetMethod("GetInt64");
					break;
			}
            
			return drMethod;
		}

		static ILDataConverter()
		{
			methods = new ConcurrentDictionary<Type, Delegate>();
		}
        
		private Func<IDataReader, T> GetReaderFunc<T>(IDataReader dr)
		{
			Type type = typeof(T);
			Delegate del;
            
			if (!methods.TryGetValue(type, out del))
			{
				Type drType = typeof(IDataRecord);

				var method = new DynamicMethod("g", type, new[] { drType });
            
				var il = method.GetILGenerator();

				var t = il.DeclareLocal(type);

				var ctor = type.GetConstructor(Type.EmptyTypes);

				if (ctor == null)
				{
					throw new InvalidOperationException("Type " + type.ToString() + " does not have default constructor");
				}

				il.Emit(OpCodes.Newobj, ctor);

				il.Emit(OpCodes.Stloc_0);

				for (int i = 0; i < dr.FieldCount; i++)
				{
					string fieldName = dr.GetName(i);

					var propInfo = type.GetProperty(fieldName);

					if (propInfo != null)
					{
						var methodInfo = propInfo.GetSetMethod();
						string propTypeName = propInfo.PropertyType.Name;

						MethodInfo drMethod = null;

						drMethod = GetDrMethod(drType, propTypeName);

						if (drMethod == null)
						{
							throw new NotSupportedException("Unknown DataType : " + propTypeName);
						}

						il.Emit(OpCodes.Ldloc_0);
						il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Ldc_I4, i);

						il.Emit(OpCodes.Callvirt, drMethod);

						il.Emit(OpCodes.Callvirt, methodInfo);
					}
				}

				il.Emit(OpCodes.Ldloc, t);

				il.Emit(OpCodes.Ret);
				
				del = method.CreateDelegate(typeof(Func<IDataReader, T>));
				
				methods.TryAdd(type, del);
			}
			
			return (Func<IDataReader, T>)del;
		}

		/// <summary>
		/// Creates and returns the generic type T object and initilizes its public properties with the values of matching fields from given IDataReader.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dr"></param>
		/// <returns></returns>
		public T Get<T>(IDataReader dr)
		{
			T item = default(T);

			if (dr.Read())
			{
				Func<IDataReader, T> func = GetReaderFunc<T>(dr);

				item = func(dr);
			}

			return item;
		}

		/// <summary>
		/// Creates and returns a list of generic type T from given IDataReader.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dr"></param>
		/// <returns></returns>
		public IList<T> GetList<T>(IDataReader dr)
		{
			IList<T> list = new List<T>();

			if (dr.Read())
			{
				Func<IDataReader, T> func = GetReaderFunc<T>(dr);

				do
				{
					list.Add(func(dr));

				} while (dr.Read());
			}

			return list;
		}

		/// <summary>
		/// Creates and returns an IEnumerable type T from given IDataReader
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dr"></param>
		/// <returns></returns>
		public IEnumerable<T> GetEnumerable<T>(IDataReader dr)
		{
			if (dr.Read())
			{
				Func<IDataReader, T> func = GetReaderFunc<T>(dr);

				do
				{
					yield return func(dr);

				} while (dr.Read());
			}
		}
	}
}
