using Azure;
using jobboard.Auth;
using jobboard.Data.Entities;
using jobboard.Data.Models;
using jobboard.Data.Repositories;
using jobboard.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using Validator = jobboard.Auth.Validator;

namespace jobboard.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly UserManager<JobBoardUser> _userManager;
    private readonly IResumesRepository _resumesRepository;
    private readonly IJobsRepository _jobsRepository;
    private readonly IJwtTokenService _jwtTokenService;
    public AuthController(UserManager<JobBoardUser> userManager, IJwtTokenService jwtTokenService, IResumesRepository resumesRepository, IJobsRepository jobsRepository)
    {
        _resumesRepository = resumesRepository;
        _jobsRepository = jobsRepository;
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register(RegisterUserDto registerUserDto)
    {
        var validator = new Validator(registerUserDto);
        var results = validator.Validate(new ValidationContext(registerUserDto));
        if (results.Any())
        {
            return BadRequest(results);
        }
        var user = await _userManager.FindByEmailAsync(registerUserDto.Email);
        if (user != null)
            return BadRequest("Toks el. pašto adresas jau naudojamas.");

        var newUser = new JobBoardUser
        {
            Email = registerUserDto.Email,
            DateOfBirth = registerUserDto.DateOfBirth,
            Created = DateTime.Today.Date,
            AboutSection = registerUserDto.AboutSection,
            Name = registerUserDto.Name,
            Surname = registerUserDto.Surname,
            UserName = registerUserDto.Surname + registerUserDto.Name
        };
        newUser.UserName = newUser.UserName + newUser.Id;
        var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);
        if (!createUserResult.Succeeded)
            return BadRequest("Nepavyko sukurti vartotojo paskyros.");

        await _userManager.AddToRoleAsync(newUser, Roles.Darbuotojas);

        return CreatedAtAction(nameof(Register), new UserDto(newUser.Id, newUser.UserName, newUser.Email));
    }

    [HttpPost]
    [Route("register/company")]
    public async Task<ActionResult> RegisterManager(RegisterCompanyDto registerCompany)
    {
        var validator = new Validator(registerCompany);
        var results = validator.Validate(new ValidationContext(registerCompany));
        if (results.Any())
        {
            return BadRequest(results);
        }
        var user = await _userManager.FindByEmailAsync(registerCompany.Email);
        if (user != null)
            return BadRequest("Toks el. pašto adresas jau naudojamas.");

        var newCompany = new JobBoardUser
        {
            Email = registerCompany.Email,
            Created = DateTime.Today.Date,
            City = registerCompany.City,
            AboutSection = registerCompany.AboutSection,
            Surname = registerCompany.Surname,
            Name = registerCompany.Name,
            Address = registerCompany.Address,
            Site = registerCompany.Site is null ? "" : registerCompany.Site,
            CompanyCode = registerCompany.CompanyCode,
            CompanyName = registerCompany.CompanyName,
            PhoneNumber = registerCompany.PhoneNumber,
            ContactPerson = registerCompany.Name + " " + registerCompany.Surname,
            UserName = registerCompany.Name + registerCompany.Surname
        };

        newCompany.UserName = newCompany.UserName + newCompany.Id;
        var createEmployerResult = await _userManager.CreateAsync(newCompany, registerCompany.Password);
        if (!createEmployerResult.Succeeded)
            return BadRequest("Nepavyko sukurti kompanijos paskyros.");

        await _userManager.AddToRoleAsync(newCompany, Roles.Darbdavys);

        return CreatedAtAction(nameof(Register), new CompanyDto(newCompany.Id, newCompany.CompanyName, newCompany.Address, newCompany.Email));
    }


    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login(LoginDto loginDto)
    {
        var validator = new Validator(loginDto);
        var results = validator.Validate(new ValidationContext(loginDto));
        if (results.Any())
        {
            return BadRequest(results);
        }
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return BadRequest("El. pašto adresas arba slaptažodis neteisingas.");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
            return BadRequest("El. pašto adresas arba slaptažodis neteisingas.");

        //valid user
        var roles = await _userManager.GetRolesAsync(user);
        var fullName = user.Name;
        var accesToken = _jwtTokenService.CreateAccessToken(/*user.UserName*/fullName, user.Id, roles);

        return Ok(new SuccessfulLoginDto(accesToken));
    }
}
