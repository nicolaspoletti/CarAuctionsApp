using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionService.Data;
using AuctionService.Entities;
using AuctionService.DTOs;
using AutoMapper;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Auctions
        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            foreach (var auction in auctions)
            {
                System.Console.WriteLine(auction);
            }

            return _mapper.Map<List<AuctionDTO>>(auctions);
        }

        // GET: api/Auctions/40490065-dac7-46b6-acc4-0000000
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            return _mapper.Map<AuctionDTO>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO createdAuction) 
        {
            var auction  = _mapper.Map<Auction>(createdAuction);
            // PENDING: use Identity to validate users.
            auction.Seller = "test";

            _context.Auctions.Add(auction);

            var result  = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not add auction to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDTO>(auction));    
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO) 
        {
            // Get the auction that matches the id:
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            // Pending: Check that seller name matches the username.
            
            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null) return NotFound();

            _context.Auctions.Remove(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(result);
        }

        private bool AuctionExists(Guid id)
        {
            return _context.Auctions.Any(e => e.Id == id);
        }
    }
}
