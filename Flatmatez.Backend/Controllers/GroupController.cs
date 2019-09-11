using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flatmatez.Backend.Data.Abstractions;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using Flatmatez.Common.Models.Sync;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flatmatez.Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GroupController : ControllerBase
	{
		private readonly IFlatmatezDbRepo _repo;
		public GroupController(IFlatmatezDbRepo repo)
		{
			_repo = repo;
		}

		// GET: api/Group/SyncRequest
		[HttpGet("{userId}")]
		public async Task<IActionResult> SyncRequest(string userId)
		{
			var response = await _repo.GetSyncChangesForUser(userId);

			if (response == null)
			{
				return NotFound();
			}

			return Ok(response);
		}

		// GET: api/Group/GetUsersInGroup
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUsersInGroup(string id)
		{
			var users = await _repo.GetAllUsersInGroup(id);
			if (users == null)
			{
				return NotFound();
			}
			return Ok(users);
		}

		// POST: api/Group/NewGroupWithInitialUser
		[HttpPost]
		public async Task<IActionResult> NewGroupWithInitialUser([FromBody] GroupUserDTO user)
		{
			try
			{
				var groupId = await _repo.AddNewGroup(user.GroupName);
				user = await _repo.AddUserToGroup(groupId, user);
				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		// POST: api/Group/AddUserToExistingGroup
		[HttpPost]
		public async Task<IActionResult> AddUserToExistingGroup([FromBody] GroupUserDTO user)
		{
			try
			{
				user = await _repo.AddUserToGroup(user.GroupId, user);
				return Ok(user);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		// PUT: api/Group/5
		[HttpPut]
		public async Task<IActionResult> UpdateUser([FromBody] GroupUserDTO user)
		{
			try
			{
				await _repo.UpdateUser(user);
				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			try
			{
				var user = await _repo.MarkObjectForDeletionByGUID<GroupUser>(id);
				if (user != null)
				{
					return Ok();
				}
				else
				{
					return NotFound();
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}
	}
}
