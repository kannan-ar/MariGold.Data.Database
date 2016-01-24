namespace MariGold.Data
{
	using System;
	using System.Data;
	using System.Collections.Generic;
	
	public static class DataReaderExtensions
	{
		/// <summary>
		/// Checks whether the given name is exists in the IDataReader. 
		/// Returns null if the IDataReader is null or the field is not exists. index value will be -1 in such case.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool HasColumn(this IDataReader dr, string name, out int index)
		{
			index = 0;

			if (dr.IfDbNull())
			{
				return false;
			}

			for (; dr.FieldCount >= index; index++)
			{
				if (string.Compare(dr.GetName(index), name, true) == 0)
				{
					return true;
				}
			}

			//Failed to find the field name. Thus the index set to -1
			index = -1;

			return false;
		}

		/// <summary>
		/// Returns true if the IDataReader is null or closed.
		/// </summary>
		/// <param name="dr"></param>
		/// <returns></returns>
		public static bool IfDbNull(this IDataReader dr)
		{
			if (dr == null)
			{
				return true;
			}

			if (dr.IsClosed)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the IDataReader is null or the value of given index is DBNull.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool IfDbNull(this IDataReader dr, int index)
		{
			if (dr.IfDbNull())
			{
				return true;
			}

			if (dr.IsDBNull(index))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the IDataReader or the value in the column of given field name is null.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool IfDbNull(this IDataReader dr, string name, out int index)
		{
			if (!dr.HasColumn(name, out index))
			{
				return true;
			}

			if (dr.IsDBNull(index))
			{
				return true;
			}

			return false;
		}

		#region Int16

		/// <summary>
		/// Get the System.Int16 value using the column index. Throws exception if the value is not type System.Int16.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Int16 ConvertToInt16(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt16(index);
		}

		/// <summary>
		/// Get the System.Int16 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int16 ConvertToInt16(this IDataReader dr, int index, Int16 defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt16(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int16 value from the index of given field name. Throws exception if the value is not type System.Int16.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Int16 ConvertToInt16(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt16(index);
		}

		/// <summary>
		///  Get the System.Int16 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int16 ConvertToInt16(this IDataReader dr, string name, Int16 defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt16(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int16 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int16? ConvertToInt16(this IDataReader dr, int index, Int16? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt16(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int16 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int16? ConvertToInt16(this IDataReader dr, string name, Int16? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt16(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion Int16

		#region Int32

		/// <summary>
		/// Get the System.Int32 value using the column index. Throws exception if the value is not type System.Int32.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Int32 ConvertToInt32(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt32(index);
		}

		/// <summary>
		/// Get the System.Int32 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int32 ConvertToInt32(this IDataReader dr, int index, Int32 defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt32(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int32 value from the index of given field name. Throws exception if the value is not type System.Int32.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Int32 ConvertToInt32(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt32(index);
		}

		/// <summary>
		/// Get the System.Int32 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int32 ConvertToInt32(this IDataReader dr, string name, Int32 defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt32(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int32 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int32? ConvertToInt32(this IDataReader dr, int index, Int32? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt32(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int32 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int32? ConvertToInt32(this IDataReader dr, string name, Int32? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt32(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion Int32

		#region Int64

		/// <summary>
		/// Get the System.Int64 value using the column index. Throws exception if the value is not type System.Int64.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Int64 ConvertToInt64(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt64(index);
		}

		/// <summary>
		/// Get the System.Int64 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int64 ConvertToInt64(this IDataReader dr, int index, Int64 defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt64(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int64 value from the index of given field name. Throws exception if the value is not type System.Int64.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Int64 ConvertToInt64(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetInt64(index);
		}

		/// <summary>
		/// Get the System.Int64 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int64 ConvertToInt64(this IDataReader dr, string name, Int64 defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt64(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int64 value of index column. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int64? ConvertToInt64(this IDataReader dr, int index, Int64? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt64(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Int64 value from the index of given field name. Returns defaultValue if IDataReader is null or method fails to get the value.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static Int64? ConvertToInt64(this IDataReader dr, string name, Int64? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetInt64(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion Int64

		#region String

		/// <summary>
		/// Get the System.String from the column index. Throws exception if the IDataReader is null or value cannot convert to System.String.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string ConvertToString(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetString(index);
		}

		/// <summary>
		/// Get the System.String from the column index. Returns defaultValue if the value cannot convert to System.String.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string ConvertToString(this IDataReader dr, int index, string defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetString(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.String from the given field name index. Throws exception if the IDataReader is null or value cannot convert to System.String.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string ConvertToString(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetString(index);
		}

		/// <summary>
		/// Get the System.String from the index of given field name. Returns defaultValue if the value cannot convert to System.String.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string ConvertToString(this IDataReader dr, string name, string defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetString(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion String

		#region DataTime

		/// <summary>
		/// Get the System.DateTime from the column index. Throws exception if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static DateTime ConvertToDateTime(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetDateTime(index);
		}

		/// <summary>
		/// Get the System.DateTime from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime ConvertToDateTime(this IDataReader dr, int index, DateTime defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDateTime(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.DateTime from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime? ConvertToDateTime(this IDataReader dr, int index, DateTime? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDateTime(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.DateTime from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime? ConvertToDateTime(this IDataReader dr, string name, DateTime? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDateTime(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.DateTime from the index of given field name. Throws exception if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DateTime ConvertToDateTime(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetDateTime(index);
		}

		/// <summary>
		/// Get the System.DateTime from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.DateTime.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime ConvertToDateTime(this IDataReader dr, string name, DateTime defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDateTime(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion DateTime

		#region Decimal

		/// <summary>
		/// Get the System.Decimal from the column index. Throws exception if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static decimal ConvertToDecimal(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetDecimal(index);
		}

		/// <summary>
		/// Get the System.Decimal from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal ConvertToDecimal(this IDataReader dr, int index, decimal defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDecimal(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Decimal from the index of given field name. Throws exception if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static decimal ConvertToDecimal(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetDecimal(index);
		}

		/// <summary>
		/// Get the System.Decimal from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal ConvertToDecimal(this IDataReader dr, string name, decimal defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDecimal(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Decimal from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal? ConvertToDecimal(this IDataReader dr, int index, decimal? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDecimal(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Decimal from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.Decimal.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal? ConvertToDecimal(this IDataReader dr, string name, decimal? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetDecimal(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion Decimal

		#region Bool

		/// <summary>
		/// Get the System.Boolean from the column index. Throws exception if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool ConvertToBoolean(this IDataReader dr, int index)
		{
			if (dr.IfDbNull(index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetBoolean(index);
		}

		/// <summary>
		/// Get the System.Boolean from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool ConvertToBoolean(this IDataReader dr, int index, bool defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetBoolean(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Boolean from the index of given field name. Throws exception if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool ConvertToBoolean(this IDataReader dr, string name)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				throw new InvalidOperationException("Reader is null");
			}

			return dr.GetBoolean(index);
		}

		/// <summary>
		/// Get the System.Boolean from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool ConvertToBoolean(this IDataReader dr, string name, bool defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetBoolean(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Boolean from the column index. Returns defaultValue if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="index"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool? ConvertToBoolean(this IDataReader dr, int index, bool? defaultValue)
		{
			if (dr.IfDbNull(index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetBoolean(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the System.Boolean from the index of given field name. Returns defaultValue if the IDataReader is null or value cannot convert to System.Boolean.
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool? ConvertToBoolean(this IDataReader dr, string name, bool? defaultValue)
		{
			int index;

			if (dr.IfDbNull(name, out index))
			{
				return defaultValue;
			}

			try
			{
				return dr.GetBoolean(index);
			}
			catch (InvalidCastException)
			{
				return defaultValue;
			}
		}

		#endregion Bool
        
		#region Entity
        
		public static T Get<T>(this IDataReader dr)
			 where T : class, new()
		{
			var converter = Db.GetConverter();
			
			return converter.Get<T>(dr);
		}
        
		public static dynamic Get(this IDataReader dr)
		{
			var converter = Db.GetDynamicConvertor();
			
			return converter.Get(dr);
		}
        
		public static IList<T> GetList<T>(this IDataReader dr)
			 where T : class, new()
		{
			var converter = Db.GetConverter();
        	
			return converter.GetList<T>(dr);
		}
        
		public static IList<dynamic> GetList(this IDataReader dr)
		{
			var converter = Db.GetDynamicConvertor();
        	
			return converter.GetList(dr);
		}
        
		public static IEnumerable<T> GetEnumerable<T>(IDataReader dr)
			 where T : class, new()
		{
			var converter = Db.GetConverter();
        	
			return converter.GetEnumerable<T>(dr);
		}
        
		public static IEnumerable<dynamic> GetEnumerable(IDataReader dr)
		{
			var converter = Db.GetDynamicConvertor();
        	
			return converter.GetEnumerable(dr);
		}
        
		#endregion Entity
	}
}
