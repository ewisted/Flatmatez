using Flatmatez.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data.Models
{
	public static class DbModelExtensions
	{
		public static T MergeFromDTO<T>(this T model, object DTO)
		{
			var commonProps = from modelProp in model.GetType().GetProperties()
							  from dtoProp in DTO.GetType().GetProperties()
							  where modelProp.Name == dtoProp.Name && modelProp.PropertyType == dtoProp.PropertyType
							  select new KeyValuePair<PropertyInfo, PropertyInfo>(modelProp, dtoProp);
			foreach (var prop in commonProps)
			{
				var dtoValue = prop.Value.GetValue(DTO);
				if (dtoValue != null)
				{
					prop.Key.SetValue(model, dtoValue);
				}
			}
			return model;
		}
	}
}
