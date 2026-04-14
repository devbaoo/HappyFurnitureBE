using HappyFurnitureBE.Application.DTOs.Contact;
using HappyFurnitureBE.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyFurnitureBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a contact form (public). Requires valid reCAPTCHA token.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContactResponse>> Create([FromBody] CreateContactRequest request)
    {
        try
        {
            var response = await _contactService.CreateContactAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact creation failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating contact");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get all contacts (admin only). Filter by isRead. Supports pagination.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetAll(
        [FromQuery] bool? isRead,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var (items, total) = await _contactService.GetAllContactsAsync(isRead, page, pageSize);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);
            return Ok(new { items, totalCount = total, pageNumber = page, pageSize, totalPages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting contacts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get contact by id (admin only).
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ContactResponse>> GetById(int id)
    {
        try
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            return Ok(contact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting contact {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Mark a contact as read (admin only).
    /// </summary>
    [HttpPatch("{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            await _contactService.MarkAsReadAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking contact {Id} as read", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete a contact (admin only).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _contactService.DeleteContactAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting contact {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
