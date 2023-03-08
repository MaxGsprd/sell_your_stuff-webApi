using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Models;
using SellYourStuffWebApi.Models.Dtos;

namespace SellYourStuffWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdsController : ControllerBase
    {
        private readonly SellYourStuffWebApiContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public AdsController(SellYourStuffWebApiContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
        }

        // GET: api/Ads
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAd()
        {
            var ads = await _context.Ad.ToListAsync();
            if (ads.Count > 0)
            {
                ads.ForEach(ad =>
                {
                    string adIdToStrig = ad.Id.ToString();
                    ad.AdImage = GetAdImage(adIdToStrig);
                });
            }
            await _context.SaveChangesAsync();
            return Ok(ads.Select(ad => _mapper.Map<AdResponseDto>(ad)));
        }

        // GET: api/AdsByUser
        [HttpGet("byUser/{id}")]
        public async Task<ActionResult<IEnumerable<AdResponseDto>>> GetAdsByUser(int id)
        {
            var ads = await _context.Ad.Where(ad => ad.User.Id == id).ToListAsync();
            if (ads.Count > 0)
            {
                ads.ForEach(ad =>
                {
                    string adIdToStrig = ad.Id.ToString();
                    ad.AdImage = GetAdImage(adIdToStrig);
                });
            }
            await _context.SaveChangesAsync();
            return Ok(ads.Select(ad => _mapper.Map<AdResponseDto>(ad)));
        }

        // GET: api/Ads/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdResponseDto>> GetAd(int id)
        {
            var ad = await _context.Ad.FindAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            var adDTO = _mapper.Map<AdResponseDto>(ad);
            return Ok(adDTO);
        }

        // PUT: api/Ads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAd(int id, AdRequestDto adDTO)
        {
            if (id != adDTO.Id)
            {
                return BadRequest();
            }

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

        // POST: api/Ads
        [HttpPost]
        public async Task<ActionResult<AdResponseDto>> PostAd(AdRequestDto newAd)
        {
            var ad = _mapper.Map<Ad>(newAd);
            _context.Ad.Add(ad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAd", new { id = ad.Id }, ad);
        }

        [HttpPost("uploadImage"), AllowAnonymous]
        [RequestSizeLimit(100 * 1024 * 1024)] // = 100MB
        public async Task<ActionResult> UploadImage(int id)
        {
            bool Results = false;
            try
            {
                var files = Request.Form.Files;
                foreach (IFormFile file in files)
                {
                    string filename = file.FileName;
                    string filepath = GetFilePath(filename);

                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }

                    string imagePath = filepath + "\\image.png";

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        Results = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(Results);
        }

        // DELETE: api/Ads/5
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

        [HttpDelete("imageDelete/{id}")]
        public IActionResult DeleteAdImage(string id)
        {
            string filepath = GetFilePath(id);
            string imagepath = filepath + "\\image.png";
            if (System.IO.File.Exists(imagepath))
            {
                System.IO.File.Delete(imagepath);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        private bool AdExists(int id)
        {
            return _context.Ad.Any(e => e.Id == id);
        }

        [NonAction]
        private string GetFilePath(string id)
        {
            return this._environment.WebRootPath + "\\Uploads\\Ads\\" + id;

        }

        [NonAction]
        private string GetAdImage(string id)
        {
            string imageUrl = string.Empty;
            string hostUrl = "https://sellyourstuff.azurewebsites.net";
            string filepath = GetFilePath(id);
            string imagepath = filepath + "\\image.png";
            if (!System.IO.File.Exists(imagepath))
            {
                imageUrl = hostUrl + "/uploads/Ads/no-image.png";
            }
            else
            {
                imageUrl = hostUrl + "/uploads/Ads/" + id + "/" + "image.png";
            }
            return imageUrl;
        }

    }
}
