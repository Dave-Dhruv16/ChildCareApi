using ChildCareApi.Models;
using ChildCareApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChildCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutoringRequestController : ControllerBase
    {
        private readonly ITutoringRequestRepository _tutoringRequestRepository;

        public TutoringRequestController(ITutoringRequestRepository tutoringRequestRepository)
        {
            _tutoringRequestRepository = tutoringRequestRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _tutoringRequestRepository.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _tutoringRequestRepository.GetRequestByIdAsync(id);
            if (request == null)
                return NotFound(new { message = $"Tutoring request with ID {id} not found." });

            return Ok(request);
        }

        [HttpGet("GetByStudentId/{studentId}")]
        public async Task<IActionResult> GetRequestsByStudentId(int studentId)
        {
            var requests = await _tutoringRequestRepository.GetRequestsByStudentIdAsync(studentId);
            return Ok(requests);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddRequest([FromBody] TutoringRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request data." });

            await _tutoringRequestRepository.AddRequestAsync(request);
            return CreatedAtAction(nameof(GetRequestById), new { id = request.RequestId }, request);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] TutoringRequest request)
        {
            if (id != request.RequestId)
                return BadRequest(new { message = "RequestId in URL and payload do not match." });

            await _tutoringRequestRepository.UpdateRequestAsync(request);
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _tutoringRequestRepository.DeleteRequestAsync(id);
            return NoContent();
        }

        [HttpPut("Accept/{requestId}")]
        public async Task<IActionResult> AcceptRequest(int requestId, [FromBody] int tutorId)
        {
            if (tutorId <= 0)
            {
                return BadRequest(new { message = "Tutor ID must be a positive integer." });
            }

            var result = await _tutoringRequestRepository.AcceptRequestAsync(requestId, tutorId);

            if (!result)
            {
                return NotFound(new { message = $"Tutoring request with ID {requestId} not found or already accepted/rejected." });
            }

            return Ok(new { message = $"Request ID {requestId} has been accepted by Tutor ID {tutorId}." });
        }


        [HttpPut("Reject/{requestId}")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            await _tutoringRequestRepository.RejectRequestAsync(requestId);
            return Ok(new { message = $"Request ID {requestId} has been rejected." });
        }
    }
}
