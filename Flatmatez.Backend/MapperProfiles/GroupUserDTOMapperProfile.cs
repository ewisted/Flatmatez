using AutoMapper;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.MapperProfiles
{
	public class GroupUserDTOMapperProfile : Profile
	{
		public GroupUserDTOMapperProfile()
		{
			CreateMap<GroupUser, GroupUserDTO>()
				.ForMember(x =>
					x.GroupName,
					opt => opt.MapFrom(u => u.Group.Name));
		}
	}
}
