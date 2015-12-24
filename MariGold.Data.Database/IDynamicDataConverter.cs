namespace MariGold.Data
{
	using System;
	using System.Data;
	using System.Collections.Generic;
	
	/// <summary>
	/// Description of IDynamicDataConvertor.
	/// </summary>
	public interface IDynamicDataConverter
	{
		dynamic Get(IDataReader dr);
		IList<dynamic> GetList(IDataReader dr);
		IEnumerable<dynamic> GetEnumerable(IDataReader dr);
	}
}
