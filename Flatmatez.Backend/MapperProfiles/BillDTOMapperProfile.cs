using AutoMapper;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.MapperProfiles
{
	public class BillDTOMapperProfile : Profile
	{
		public BillDTOMapperProfile()
		{
			CreateMap<Bill, BillDTO>()
				.ForMember(x =>
					x.GroupId,
					opt => opt.MapFrom(c => c.Group.Id));
		}
	}
}
