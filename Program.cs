using ChatAPIs.DataService;
using ChatAPIs.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("angularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200/")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SharedDb>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("angularApp");
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");  // Ensure the correct hub URL

app.Run();
