using Context;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
using Services.Implementations.image;
using Services.Interfaces;
using Services.Interfaces.images;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

//Database connection
var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<EfContext>(options =>
    options.UseNpgsql(connectionString)    
);

//Repositories:
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddScoped<IArticlesRepository, ArticlesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

builder.Services.AddScoped<IPollOptionsRepository, PollOptionsRepository>();
builder.Services.AddScoped<IPollsRepository, PollsRepository>();
builder.Services.AddScoped<IVotesRepository, VotesRepository>();

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
builder.Services.AddScoped<IResultsRepository, ResultsRepository>();
builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
builder.Services.AddScoped<IVotesRepository, VotesRepository>();

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IConstructorsService, ConstructorsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IGrandPrixService, GrandPrixService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<ISeriesService, SeriesService>();

builder.Services.AddScoped<IArticleImageService, ArticleImageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DRS Zona - API",

        Version = "1.0"
    });
});
    
builder.Services.AddOpenApi();

builder.Services.AddCors(options => {
    options.AddPolicy("AngularPolicy", policy => {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:80",
                "http://localhost",
                "http://drs-zona.hu",
                "https://drs-zona.hu"
                ) 
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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "client", "dist", "drs-zona", "browser")),
    RequestPath = ""
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();