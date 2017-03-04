namespace MariGold.Data
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	
	public sealed class DynamicDataConverter : IDynamicDataConverter
	{
		private dynamic GetObject(IDataReader dr)
		{
			DbObject obj = new DbObject();
			
			for (int i = 0; dr.FieldCount > i; i++)
			{
				obj.Add(dr.GetName(i), dr.GetValue(i));
			}
			
			return obj;
		}
		
		public dynamic Get(IDataReader dr)
		{
			if (dr.Read())
			{
				return GetObject(dr);
			}
			
			return null;
		}
		
		public IEnumerable<dynamic> GetEnumerable(IDataReader dr)
		{
			while (dr.Read())
			{
				yield return GetObject(dr);
			}
		}
		
		public IList<dynamic> GetList(IDataReader dr)
		{
			IList<dynamic> list = new List<dynamic>();
			
			while (dr.Read())
			{
				list.Add((DbObject)GetObject(dr));
			}
			
			return list;
		}
	}
}
