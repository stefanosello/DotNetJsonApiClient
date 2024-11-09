using JsonApiClient.Example.Server;
using JsonApiDotNetCore.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ExampleDbContext>(
    options => options.UseSqlServer("Server=mssql;Database=Library;User Id=sa;Password=My5ecr3tP@ssw0rd;TrustServerCertificate=True;"));
builder.Services.AddJsonApi<ExampleDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseJsonApi();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();