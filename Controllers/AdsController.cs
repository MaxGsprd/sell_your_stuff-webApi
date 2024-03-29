﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Interfaces;
using SellYourStuffWebApi.Models;
using SellYourStuffWebApi.Models.Dtos;
using SellYourStuffWebApi.Models.Dtos.AdDtos;

namespace SellYourStuffWebApi.Controllers
{
    [Route("api/Cl-Ad")]
    [ApiController]
    [Authorize]
    public class AdsController : ControllerBase
    {
        private readonly SellYourStuffWebApiContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public AdsController(SellYourStuffWebApiContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        //GET: api/Ads
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAds()
        {
            var ads = await _context.Ad.ToListAsync();
            return Ok(ads.Select(ad => _mapper.Map<AdResponseDto>(ad)));
        }

        //GET: api/Ads/ByUser/5
        [HttpGet("byUser/{id}")]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAdsByUser(int id)
        {
            var ads = await _context.Ad.Where(ad => ad.User.Id == id).ToListAsync();
            return Ok(ads.Select(ad => _mapper.Map<AdResponseDto>(ad)));
        }

        //GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdResponseDto>> GetAd(int id)
        {
            var ad = await _context.Ad.FindAsync(id);
            if (ad == null) return NotFound();
            var adDTO = _mapper.Map<AdResponseDto>(ad);
            return Ok(adDTO);
        }

        //PUT: api/Ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, AdRequestDto adDTO)
        {
            if (id != adDTO.Id) return BadRequest();

            var ad = _mapper.Map<Ad>(adDTO);
            _context.Entry(ad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return CreatedAtAction("GetAd", new { id = ad.Id }, ad);
        }

        //PATCH: api/Ads/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAd(int id, JsonPatchDocument<Ad> updatedAd)
        {
            var ad = await _context.Ad.FindAsync(id);
            if (ad != null)
            {
                updatedAd.ApplyTo(ad);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        //POST: api/Ads
        [HttpPost]
        public async Task<ActionResult<AdResponseDto>> PostAd(AdRequestDto newAd)
        {
            var ad = _mapper.Map<Ad>(newAd);
            _context.Ad.Add(ad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAd", new { id = ad.Id }, ad);
        }

         //POST: api/add/photo/5
        [HttpPost("add/photo/{adId}")]
        [RequestSizeLimit(100 * 1024 * 1024)] // = 100MB
        public async Task<ActionResult> AddPhoto(IFormFile file, int adId)
        {
            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error);
            }
            var ad = await _context.Ad.FindAsync(adId);
            var photo = new Photo
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (ad?.Photos?.Count == 0)
            {
                photo.IsPrimary = true;
            }
            ad?.Photos?.Add(photo);
            _mapper.Map<PhotoDto>(photo);
            await _context.SaveChangesAsync();
            return Ok(201);
        }

        //POST: api/setPrimaryPhoto/5/5
        [HttpPost("setPrimaryPhoto/{adId}/{publicId}")]
        [RequestSizeLimit(100 * 1024 * 1024)] // = 100MB
        public async Task<ActionResult> setPrimaryPhoto(int adId, string publicId)
        {
            var ad = await _context.Ad.FindAsync(adId);
            if (ad == null) return BadRequest("No such ad exists");

            var photo = ad?.Photos?.FirstOrDefault(p => p.PublicId == publicId);
            if (photo == null) return BadRequest("No such ad photo exists");

            if (photo.IsPrimary) return BadRequest("This photo is alreay the primary photo");

            var currentPirmary = ad?.Photos?.FirstOrDefault(p => p.IsPrimary);
            if (currentPirmary != null) currentPirmary.IsPrimary = false;

            photo.IsPrimary = true;
            await _context.SaveChangesAsync();
            return Ok(200);
        }

        //DELETE: api/Ads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _context.Ad.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            _context.Ad.Remove(ad);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //DELETE: api/Ads/deletePhoto/5
        [HttpDelete("deletePhoto/{adId}/{publicId}")]
        public async Task<IActionResult> DeletePhoto(int adId, string publicId)
        {
            var ad = await _context.Ad.FindAsync(adId);
            if (ad == null) return BadRequest("No such ad exists");

            var photo = ad?.Photos?.FirstOrDefault(p => p.PublicId == publicId);
            if (photo == null) return BadRequest("No such ad photo exists");
            if (photo.IsPrimary) return BadRequest("You can not delete the primary photo");

            var result = await _photoService.DeletePhotoAsync(publicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
            
            ad?.Photos?.Remove(photo); 
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool AdExists(int id)
        {
            return _context.Ad.Any(e => e.Id == id);
        }
    }
}
