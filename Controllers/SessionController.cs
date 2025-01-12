using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionController(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSessions()
        {
            return Ok(await _sessionRepository.GetAllSessionsAsync());
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(id);
            if (session == null)
                return NotFound(new { message = $"Session with ID {id} not found." });

            return Ok(session);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddSession([FromBody] Session session)
        {
            await _sessionRepository.AddSessionAsync(session);
            return CreatedAtAction(nameof(GetSessionById), new { id = session.SessionId }, session);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSession(int id, [FromBody] Session session)
        {
            if (id != session.SessionId)
                return BadRequest(new { message = "SessionId in URL and payload do not match." });

            var result = await _sessionRepository.UpdateSessionAsync(session);
            if (result > 0)
                return NoContent();

            return NotFound(new { message = $"Session with ID {id} not found." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            await _sessionRepository.DeleteSessionAsync(id);
            return NoContent();
        }
    }
}
    