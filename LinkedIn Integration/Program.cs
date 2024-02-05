using LinkedIn_Integration;
using LinkedIn_Integration.Services;
using LinkedIn_Integration.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<LinkedInOptions>(builder.Configuration.GetSection("LinkedInOptions"));
builder.Services.AddSingleton<IPostService, PostService>();
builder.Services.AddSingleton<ICommentService, CommentService>();
builder.Services.AddSingleton<IEntityEngagementService, EntityEngagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
