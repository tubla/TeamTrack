using TeamTrack.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Middleware
app.UseApplicationMiddleware(app.Environment);

app.Run();