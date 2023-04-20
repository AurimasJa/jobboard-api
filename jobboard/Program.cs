using jobboard;
using jobboard.Auth;
using jobboard.Data;
using jobboard.Data.Repositories;
using jobboard.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000",
                                              "https://myapiwarehouse.azurewebsites.net/",
                                              "https://myapiwarehouse.azurewebsites.net")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();  // add the allowed origins
                      });
});
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<JobBoardDbContext>();
builder.Services.AddTransient<IResumesRepository, ResumesRepository>();
builder.Services.AddTransient<ICompaniesResumesRepository, CompaniesResumesRepository>();
builder.Services.AddTransient<IJobsRepository, JobsRepository>();
builder.Services.AddTransient<ICalculations, Calculations>();
builder.Services.AddTransient<IJobResumesRepository, JobResumesRepository>();
//AUTH

builder.Services.AddIdentity<JobBoardUser, IdentityRole>().AddEntityFrameworkStores<JobBoardDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

var dbSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();
app.Run();
