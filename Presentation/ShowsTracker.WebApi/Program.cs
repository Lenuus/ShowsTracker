using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShowsTracker.Application;
using ShowsTracker.Application.Service.Account;
using ShowsTracker.Application.Service.Auth;
using ShowsTracker.Application.Service.Genre;
using ShowsTracker.Application.Service.Show;
using ShowsTracker.Application.Service.ShowUser;
using ShowsTracker.Application.Service.User;
using ShowsTracker.Application.Service.Voting;
using ShowsTracker.Common.Helpers;
using ShowsTracker.Domain;
using ShowsTracker.Persistence;
using Swashbuckle.Swagger;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using ShowsTracker.Application.Service.Email;
using ShowTracker.MyAnimeListService.Services;
using Quartz;
using ShowsTracker.Application.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MainDbContext>(
    option => option
                    .UseSqlServer(builder.Configuration.GetConnectionString("MainDbContext"))
                    .AddInterceptors(new SoftDeleteInterceptor()));

builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication().AddGoogle(option =>
{
    option.ClientId = builder.Configuration["Google:ClientId"];
    option.ClientSecret = builder.Configuration["Google:ClientSecret"];
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<PasswordHelper>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IClaimManager, ClaimManager>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IShowService, ShowService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IShowUserService, ShowUserService>();
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IMyAnimeListService, MyAnimeListService>();
builder.Services.AddDataProtection();
builder.Services.Configure<MailSettingsOptions>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

AppDomain.CurrentDomain.GetAssemblies()
    .SelectMany(f => f.GetTypes())
    .Where(f => typeof(IApplicationService).IsAssignableFrom(f) && !f.IsInterface)
    .ToList().ForEach(f => builder.Services.AddTransient(f.GetInterface($"I{f.Name}"), f));

builder.Services.AddMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed(origin => true);
        });
});

builder.Services.AddControllers();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("ShowCreationJob");
    q.AddJob<ShowCreationJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ShowCreationJob-trigger")
        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 0))
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
