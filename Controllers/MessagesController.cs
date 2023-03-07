using AutoMapper;
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
    public class MessagesController : ControllerBase
    {
        private readonly SellYourStuffWebApiContext _context;
        private readonly IMapper _mapper;

        public MessagesController(SellYourStuffWebApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessage()
        {
            var messages = await _context.Message.ToListAsync();
            return Ok(messages.Select(msg => _mapper.Map<MessageResponseDto>(msg)));
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageResponseDto>> GetMessage(int id)
        {
            var message = await _context.Message.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }
            var msgDto = _mapper.Map<MessageResponseDto>(message);
            return Ok(msgDto);
        }

        // GET: api/receivedByUser/5
        [HttpGet("receivedByUser/{id}")]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessagesReceivedByUser(int id)
        {
            var messages = await _context.Message.Where(msg => msg.RecipientId == id).ToListAsync();
            await _context.SaveChangesAsync();
            return Ok(messages.Select(msg => _mapper.Map<MessageResponseDto>(msg)));
        }

        // GET: api/sentByUser/5
        [HttpGet("sentByUser/{id}")]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetMessagesSentByUser(int id)
        {
            var messages = await _context.Message.Where(msg => msg.AuthorId == id).ToListAsync();
            await _context.SaveChangesAsync();
            return Ok(messages.Select(msg => _mapper.Map<MessageResponseDto>(msg)));
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //PATCH: api/Ads/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateMessage(int id, JsonPatchDocument<Message> updatedMsg)
        {
            var message = await _context.Message.FindAsync(id);
            if (message != null)
            {
                updatedMsg.ApplyTo(message);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<ActionResult<MessageResponseDto>> PostMessage(MessageRequestDto newMessage)
        {
            var message = _mapper.Map<Message>(newMessage);
            _context.Message.Add(message);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetMessage", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Message.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.Id == id);
        }
    }
}
