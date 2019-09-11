using AutoMapper;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.MapperProfiles
{
	public class BillMapperProfile : Profile
	{
		public BillMapperProfile()
		{
			CreateMap<BillDTO, Bill>()
				.ForMember(x =>
					x.CreatedAt,
					opt => opt.Ignore())
				.ForMember(x =>
					x.MarkedForDeletion,
					opt => opt.Ignore())
				.ForMember(x =>
					x.ModifiedAt,
					opt => opt.Ignore())
				.ForMember(x =>
					x.Group,
					opt => opt.Ignore())
				.ForMember(x =>
					x.UserBills,
					opt => opt.Ignore());
		}
	}
}
