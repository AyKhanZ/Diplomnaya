using Baim_API.Models.Authentication.Login;
using Baim_API.Models.Authentication.SignUp;
using Baim_API.Validations;
using Bussines.Models;
using Bussines.Services.Classes;
using Bussines.Services.Interfaces;
using DB.DbContexts;
using DB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

namespace Baim_API.Controllers;

[ApiController]
[Route("Authentication")]
public class AuthenticationController : ControllerBase
{
	private readonly BaimContext _dbContext;
	private readonly UserManager<AspNetUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly SignInManager<AspNetUser> _signInManager;
	private readonly IUserStore<AspNetUser> _userStore;
	private readonly IConfiguration _configuration;
	private readonly IEmailService _emailService;

	public AuthenticationController(BaimContext dbContext,
		UserManager<AspNetUser> userManager,
		IUserStore<AspNetUser> userStore,
		SignInManager<AspNetUser> signInManager,
		RoleManager<IdentityRole> roleManager,
		IConfiguration configuration,
		IEmailService emailService)
	{
		_dbContext = dbContext;
		_userManager = userManager;
		_userStore = userStore;
		_signInManager = signInManager;
		_roleManager = roleManager;
		_configuration = configuration;
		_emailService = emailService;
	} 

	[HttpPost("Registration")]
	public async Task<IActionResult> Registration([FromBody] RegisterUser model, string role)
	{
		if (!ModelState.IsValid) return BadRequest("Invalid model state");

		var emailCheckResult = AuthValidation.CheckEmail(model.Email);
		if (emailCheckResult == false) return BadRequest("Invalid email format !");

		var passwordCheckResult = AuthValidation.CheckPassword(model.Password);
		if (passwordCheckResult == false) return BadRequest("Password must be more than 6 and less than 40 characters long and special symbol");

		if (await _dbContext.Users.AnyAsync(a => a.Email == model.Email)) return BadRequest("User already exists");

		var newUser = new AspNetUser
		{
			Email = model.Email,
			NormalizedEmail = model.Email.ToUpper()
		};

		var result = new IdentityResult();
		if (await _roleManager.RoleExistsAsync(role))
		{
			await _userStore.SetUserNameAsync(newUser, model.Email, CancellationToken.None);
			await _userManager.GetUserIdAsync(newUser);
			result = await _userManager.CreateAsync(newUser, model.Password);

			if (!result.Succeeded)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					new Response { Status = "Error", Message = "User failed to create!" });
			}
			await _userManager.AddToRoleAsync(newUser, role);

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
			var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = newUser.Email }, Request.Scheme);
			if (confirmationLink != null)
			{
				var message = new Message(new string[] { newUser.Email }, "Confirmation email link", confirmationLink);

				message.HtmlFilePath = "./wwwroot/index.html";
				_emailService.SendEmail(message, confirmationLink);
				return StatusCode(StatusCodes.Status200OK,
						new Response { Status = "Success", Message = $"User created & email sent to {newUser.Email} Successfully! " });
			}
			return StatusCode(StatusCodes.Status500InternalServerError,
					new Response { Status = "Error", Message = "Confirmation link does not exist!" });
		}
		else
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					new Response { Status = "Error", Message = "This role does not exist!" });
		} 
	} 

	// SetPassword
	// ResetPassword
	// ChangePassword
	// Надо добавить условие if EmailConfirmed 
	[HttpPost("Login")]
	public async Task<IActionResult> Login([FromBody] LoginUser model)
	{
		if (ModelState.IsValid)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
				if (result.Succeeded)
				{
					var roles = await _userManager.GetRolesAsync(user); 
					var role = roles.FirstOrDefault(); 

					if (role == null) return BadRequest("User does not have any roles!");

					var tokenString = AuthService.GenerateTokenString(user,role,_configuration);
					return Ok(new { UserId = user.Id, Token = tokenString });
				}
				return BadRequest("Invalid login attempt");
			}
		}
		return BadRequest("Not valid attempt");
	}


	[HttpGet("ConfirmEmail")]
	public async Task<IActionResult> ConfirmEmail(string token, string email)
	{
		var user = await _userManager.FindByEmailAsync(email);

		if (user != null)
		{
			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (result.Succeeded)
			{
				return StatusCode(StatusCodes.Status200OK,
					new Response { Status = "Success", Message = "Email verified Successfully! " });
			}
		}
		return StatusCode(StatusCodes.Status500InternalServerError,
					new Response { Status = "Error", Message = "This user does not exist" });
	}  
}