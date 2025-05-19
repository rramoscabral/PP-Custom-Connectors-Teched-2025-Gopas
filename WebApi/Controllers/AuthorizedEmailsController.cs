using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAppDemo.DataLayer.DBContext;
using MyAppDemo.DataLayer.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;


namespace MyAppDemo.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizedEmailsController : ControllerBase
    {
        private readonly WebAPIDbContext _dbContext;

        public AuthorizedEmailsController(WebAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// DTO to register new authorized users.
        /// </summary>
        public class AuthorizedEmailDto
        {

            /// <summary>
            /// User email address.
            /// </summary>
            [Required]
            [StringLength(100)]
            [EmailAddress]
            [Display(Name = "Email", Description = "User email address.")]
            public required string Email { get; set; }

            /// <summary>
            /// Webhok service type.
            /// </summary>
            [Required]
            [EnumDataType(typeof(ServiceType))]
            [Display(Name = "Service type", Description = "Webhok service type.")]
            public ServiceType Service { get; set; }

            /// <summary>
            /// API Key associated with the user.
            /// </summary>
            [Required]
            [StringLength(100)]
            [Display(Name = "API Key", Description = "API Key associated with the user.")]
            public required string ApiKey { get; set; }
        }

        /// <summary>
        /// Registers a new authorized user.
        /// </summary>
        /// <param name="dto">User data.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Registers a new authorized user.",
            Description = "Registers a user to a service with an API key. ",
            OperationId = "AddAuthorizedUser"
        )]
        public async Task<IActionResult> RegisterAuthorizedEmail([FromBody] AuthorizedEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _dbContext.AuthorizedEmails
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return BadRequest("Email is already registered.");

            var authorizedEmail = new AuthorizedEmail
            {
                Email = dto.Email,
                Service = dto.Service,
                ApiKey = dto.ApiKey,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.AuthorizedEmails.Add(authorizedEmail);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(RegisterAuthorizedEmail), new { id = authorizedEmail.AuthorizedEmailId }, authorizedEmail);
        }
    }
}
