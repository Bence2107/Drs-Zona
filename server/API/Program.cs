using System.Security.Claims;
using System.Text;
using Context;
using Context.Configuration;
using Entities.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Implementations;
using Repositories.Implementations.News;
using Repositories.Implementations.Polls;
using Repositories.Implementations.RaceTracks;
using Repositories.Implementations.Standings;
using Repositories.Interfaces;
using Repositories.Interfaces.News;
using Repositories.Interfaces.Polls;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Implementations;
using Services.Implementations.images;
using Services.Interfaces;
using Services.Interfaces.images;
using Services.Types;

EnvFile.Load();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
});

if (builder.Environment.IsDevelopment())
{
    builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
    builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
}

var baseDirectory = builder.Environment.ContentRootPath;

// Tests swap in an in-memory database, so they only need the JWT secret
var isTesting = builder.Environment.IsEnvironment("Testing");
string[] requiredVariables = isTesting ? ["JWT_SECRET"] : ["JWT_SECRET", ..DatabaseConfig.RequiredVariables];
EnvVars.Require(requiredVariables);

var jwtSettings = new JwtSettings
{
    Secret = EnvVars.Get("JWT_SECRET"),
    Issuer = EnvVars.GetOptional("JWT_ISSUER") ?? "MotorsportSocialNetwork",
    Audience = EnvVars.GetOptional("JWT_AUDIENCE") ?? "MotorsportSocialNetworkUsers",
    ExpirationMinutes = EnvVars.GetOptionalInt("JWT_EXPIRATION_MINUTES") ?? 1440
};

// HS256 rejects shorter keys, but only when the first token is signed; fail at startup instead
if (Encoding.UTF8.GetByteCount(jwtSettings.Secret) < 32)
{
    throw new InvalidOperationException("JWT_SECRET must be at least 32 bytes long.");
}

if (!isTesting)
{
    builder.Services.AddDbContext<EfContext>(options =>
        options.UseNpgsql(DatabaseConfig.BuildConnectionString())
    );
}

builder.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(jwtSettings));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            RoleClaimType = ClaimTypes.Role
        };
        
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("EditorOrAdmin", policy => policy.RequireRole("Editor", "Admin"))
    .AddPolicy("AuthorOrAdmin", policy => policy.RequireRole("Author", "Admin"));


//Repositories:
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<IArticlesRepository, ArticlesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

builder.Services.AddScoped<IPollOptionsRepository, PollOptionsRepository>();
builder.Services.AddScoped<IPollsRepository, PollsRepository>();
builder.Services.AddScoped<IPollVotesRepository, PollVotesRepository>();

builder.Services.AddScoped<ICircuitsRepository, CircuitsRepository>();
builder.Services.AddScoped<IGrandsPrixRepository, GrandsPrixRepository>();

builder.Services.AddScoped<IBrandsRepository, BrandsRepository>();
builder.Services.AddScoped<IConstructorCompetitionRepository, ConstructorCompetitionRepository>();
builder.Services.AddScoped<IConstructorsChampionshipsRepository, ConstructorsChampionshipsRepository>();
builder.Services.AddScoped<IConstructorsRepository, ConstructorsRepository>();
builder.Services.AddScoped<IContractsRepository, ContractsRepository>();
builder.Services.AddScoped<IDriverParticipationRepository, DriverParticipationRepository>();
builder.Services.AddScoped<IDriversChampionshipsRepository, DriversChampionshipsRepository>();
builder.Services.AddScoped<IDriversRepository, DriversRepository>();
builder.Services.AddScoped<IPollVotesRepository, PollVotesRepository>();
builder.Services.AddScoped<IResultsRepository, ResultsRepository>();
builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
builder.Services.AddScoped<IQualifyingResultRepository, QualifyingResultRepository>();

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICircuitImagesService, CircuitImageService>();
builder.Services.AddScoped<IChampionshipService, ChampionshipService>();
builder.Services.AddScoped<IConstructorsService, ConstructorsService>();
builder.Services.AddScoped<IContractsService, ContractsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentVotesRepository, CommentVotesRepository>();
builder.Services.AddScoped<IGrandPrixService, GrandPrixService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<IStandingsService, StandingsService>();

// Unset keeps uploads where they always were (content root/uploads); Docker points this at a volume
var uploadsPath = Path.GetFullPath(EnvVars.GetOptional("UPLOADS_PATH") ?? "uploads", baseDirectory);
// A fresh Docker volume or a new prod folder starts empty, and /uploads must be served from the first request
Directory.CreateDirectory(uploadsPath);
builder.Services.AddSingleton(new UploadSettings(uploadsPath));

builder.Services.AddScoped<IArticleImageService, ArticleImageService>();
builder.Services.AddScoped<IUserImageService, UserImageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DRS Zona - API",

        Version = "1.0"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
    
builder.Services.AddOpenApi();

var corsOriginsEnv = Environment.GetEnvironmentVariable("CORS_ORIGINS");
var corsOrigins = !string.IsNullOrEmpty(corsOriginsEnv)
    ? corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    : ["http://localhost:4200"];

builder.Services.AddCors(options => {
    options.AddPolicy("AngularPolicy", policy => {
        policy.WithOrigins(corsOrigins) 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AngularPolicy");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var cacheControlHeader = new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (!ctx.File.Name.EndsWith(".jpg") && !ctx.File.Name.EndsWith(".jpeg") &&
            !ctx.File.Name.EndsWith(".png")) return;
        ctx.Context.Response.Headers.CacheControl = "no-cache";
    }
};

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
    OnPrepareResponse = cacheControlHeader.OnPrepareResponse
});

var clientPath = builder.Environment.IsDevelopment()
    ? Path.Combine(baseDirectory, "..", "..", "..", "..", "client", "dist", "drs-zona", "browser")
    : Path.Combine(baseDirectory, "..", "..", "..", "client", "dist", "drs-zona", "browser");

if (Directory.Exists(clientPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(clientPath),
        RequestPath = ""
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

public abstract partial class Program { }