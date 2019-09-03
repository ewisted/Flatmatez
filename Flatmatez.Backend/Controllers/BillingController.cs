using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flatmatez.Common.Models.DTOs;
using Flatmatez.Backend.Data;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.Sync;
using AutoMapper;

namespace Flatmatez.Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BillingController : ControllerBase
	{
		private readonly FlatmatezDbRepo _repo;
		private readonly IMapper _mapper;

		public BillingController(FlatmatezDbRepo repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		// GET: api/Billing
		[HttpGet("{userId}")]
		public async Task<ActionResult<SyncResponse>> SyncRequest(string userId)
		{
			var response = await _repo.GetSyncChangesForUser(userId);

			if (response == null)
			{
				return NotFound();
			}

			return Ok(response);
		}

		// GET: api/Billing/5
		[HttpGet("{billId}")]
		public async Task<ActionResult<IEnumerable<BillDTO>>> GetBillsForUser(string userId)
		{
			var bills = await _repo.GetAllBillsForUser(userId);

			if (bills == null)
			{
				return NotFound();
			}

			return Ok(bills);
		}

		// PUT: api/Billing/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutBill(string id, BillDTO billDTO)
		{
			if (id != billDTO.Id)
			{
				return BadRequest();
			}

			try
			{
				await _repo.UpdateBill(billDTO);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!(await _repo.Exists<Bill>(id)))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return Ok();
		}

		// POST: api/Billing
		[HttpPost]
		public async Task<ActionResult<BillDTO>> PostBill(BillDTO billDTO)
		{
			try
			{
				// No id should be set by the client, as it will be overridden in the repo
				billDTO = await _repo.AddNewBill(billDTO);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}

			return CreatedAtAction("GetBillDTO", new { id = billDTO.Id }, billDTO);
		}

		// DELETE: api/Billing/5
		[HttpDelete("{id}")]
		public async Task<ActionResult<BillDTO>> DeleteBill(string id)
		{
			try
			{
				var bill = await _repo.MarkObjectForDeletionByGUID<Bill>(id);
				return _mapper.Map<BillDTO>(bill);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex);
			}
		}
	}
}
