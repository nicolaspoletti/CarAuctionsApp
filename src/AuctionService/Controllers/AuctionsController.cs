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
using AutoMapper.QueryableExtensions;
using MassTransit;
using Contracts;
using Microsoft.AspNetCore.Authorization;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/Auctions
        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if(!string.IsNullOrEmpty(date))
            {
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
            }

            return await query.ProjectTo<AuctionDTO>(_mapper.ConfigurationProvider).ToListAsync();
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> CreateAuction(CreateAuctionDTO createdAuction) 
        {
            var auction  = _mapper.Map<Auction>(createdAuction);
            // PENDING: use Identity to validate users.
            auction.Seller = User.Identity.Name;

            _context.Auctions.Add(auction);

            var newAuction = _mapper.Map<AuctionDTO>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction)); 
            

            var result  = await _context.SaveChangesAsync() > 0;

            if(!result) return BadRequest("Could not add auction to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, newAuction);    
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDTO updateAuctionDTO) 
        {
            // Get the auction that matches the id:
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            // Check that seller name matches the username.
            if(auction.Seller != User.Identity.Name) return Forbid();
            
            auction.Item.Make = updateAuctionDTO.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDTO.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDTO.Year ?? auction.Item.Year;
            auction.Item.Color = updateAuctionDTO.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDTO.Mileage ?? auction.Item.Mileage;

           await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));
           
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null) return NotFound();
            if(auction.Seller != User.Identity.Name) return Forbid();

            _context.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new AuctionDeleted{Id = auction.Id.ToString()});

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
