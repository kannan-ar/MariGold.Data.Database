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
			where T: class, new()
		{
			Type type = typeof(T);
			Delegate del;
            
			if (!methods.TryGetValue(type, out del))
			{
				Type readerType = typeof(IDataReader);
				Type recordType = typeof(IDataRecord);
				Type stringType = typeof(String);
				Type intType = typeof(Int32);

				MethodInfo getName = recordType.GetMethod("GetName");
				MethodInfo compare = stringType.GetMethod("Compare", new Type[] { stringType, stringType, typeof(bool) });
				MethodInfo fieldCount = recordType.GetProperty("FieldCount").GetGetMethod();

				//New dynamic method with IDataReader type parameter and T type return value.
				var method = new DynamicMethod("", type, new[] { readerType });
				var il = method.GetILGenerator();
				
				var entity = il.DeclareLocal(type);
				var dataRecord = il.DeclareLocal(recordType);
				var fieldName = il.DeclareLocal(stringType);
				var count = il.DeclareLocal(intType);
				var index = il.DeclareLocal(intType);

				Label end = il.DefineLabel();
				Label loopStart = il.DefineLabel();

				//Create new T type object and store at location 'entity'
				il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
				il.Emit(OpCodes.Stloc, entity);
 
				//Convert input parameter of IDataReader to IDataRecord and store at location 'dataRecord'
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, recordType);
				il.Emit(OpCodes.Stloc, dataRecord);

				//Get field count of data reader input and store it in count variable
				il.Emit(OpCodes.Ldloc, dataRecord);
				il.Emit(OpCodes.Callvirt, fieldCount);
				il.Emit(OpCodes.Stloc, count);

				//Initally load -1 to the index variable
				il.Emit(OpCodes.Ldc_I4, -1);
				il.Emit(OpCodes.Stloc, index);

				//Starts loop through the data reader columns
				il.MarkLabel(loopStart);

				//Add +1 to the index variable
				il.Emit(OpCodes.Ldloc, index);
				il.Emit(OpCodes.Ldc_I4, 1);
				il.Emit(OpCodes.Add);
				il.Emit(OpCodes.Stloc, index);

				//Go to the lable 'end' if the index varible is greather than count variable
				il.Emit(OpCodes.Ldloc, index);
				il.Emit(OpCodes.Ldloc, count);
				il.Emit(OpCodes.Bge, end);

				//Get the field name of data reader at the index of 'index' variable 
				il.Emit(OpCodes.Ldloc, dataRecord);
				il.Emit(OpCodes.Ldloc, index);
				il.Emit(OpCodes.Callvirt, getName);
				il.Emit(OpCodes.Stloc, fieldName);

				//Stores the jump lable and property info of each property in the type 'T'
				Dictionary<PropertyInfo, Label> jumpTable = new Dictionary<PropertyInfo, Label>();

				//Scan through the 'T' type's properties and emit il to compare property names with data reader column name
				foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					Label label = il.DefineLabel();
					jumpTable.Add(property, label);

					//Compare the data reader column name and property name
					il.Emit(OpCodes.Ldloc, fieldName);
					il.Emit(OpCodes.Ldstr, property.Name);
					il.Emit(OpCodes.Ldc_I4, 1);
					il.Emit(OpCodes.Call, compare);

					//If both values are equal, go to the label where sets the property with the data reader value.
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Ceq);
					il.Emit(OpCodes.Brtrue, label);
				}

				//It acts like default case. The control reaches here because no property names matches with data reader column name.
				//So goes to next loop iteration
				il.Emit(OpCodes.Br, loopStart);

				//Scan through the dictionary and emit instructions to set property with data reader values
				foreach (var jump in jumpTable)
				{
					il.MarkLabel(jump.Value);

					//Gets the exact method based on property type to fetch value from data reader
					MethodInfo drMethod = GetDrMethod(recordType, jump.Key.PropertyType.Name);
					MethodInfo propMethod = jump.Key.GetSetMethod();

					//fetch value from data reader and assigned it to the property
					il.Emit(OpCodes.Ldloc, entity);
					il.Emit(OpCodes.Ldloc, dataRecord);
					il.Emit(OpCodes.Ldloc, index);
					il.Emit(OpCodes.Callvirt, drMethod);
					il.Emit(OpCodes.Callvirt, propMethod);
					//Go to the next loop iteration
					il.Emit(OpCodes.Br, loopStart);
				}

				il.MarkLabel(end);
				//Loads and returns the created entity.
				il.Emit(OpCodes.Ldloc, entity);
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
			where T : class, new()
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
			 where T : class, new()
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
			 where T : class, new()
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
