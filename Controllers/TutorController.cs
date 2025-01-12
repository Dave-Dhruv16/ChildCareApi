using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorRepository _tutorRepository;

        public TutorController(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllTutors()
        {
            var tutors = await _tutorRepository.GetAllTutorsAsync();
            return Ok(tutors);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetTutorById(int id)
        {
            var tutor = await _tutorRepository.GetTutorByIdAsync(id);
            if (tutor == null) return NotFound(new { message = $"Tutor with ID {id} not found." });
            return Ok(tutor);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddTutor([FromBody] Tutor tutor)
        {
            if (tutor == null) return BadRequest(new { message = "Invalid tutor data." });

            await _tutorRepository.AddTutorAsync(tutor);
            return CreatedAtAction(nameof(GetTutorById), new { id = tutor.TutorId }, tutor);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateTutor(int id, [FromBody] Tutor tutor)
        {
            if (id != tutor.TutorId)
            {
                return BadRequest(new { message = "TutorId in URL and payload do not match." });
            }

            var result = await _tutorRepository.UpdateTutorAsync(tutor);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound(new { message = $"Tutor with ID {id} not found." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            var result = await _tutorRepository.DeleteTutorAsync(id);
            if (result > 0)
            {
                return NoContent();
            }
            return NotFound(new { message = $"Tutor with ID {id} not found." });
        }
    }
}
