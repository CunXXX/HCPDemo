
using Common;
using HCPService.Repositories;
using HCPService.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWASM", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5076") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<IDBService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("找不到連線字串");
    return new MSSQLService(connectionString);
});

// 註冊Repository
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

builder.Services.AddScoped<AssetService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HCP Asset Transfer API V1");
    });
}


app.UseHttpsRedirection();

app.UseCors("AllowBlazorWASM");

app.UseAuthorization();

app.MapControllers();

app.Run();
