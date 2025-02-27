using Microsoft.EntityFrameworkCore;
using P01_2022GM650_2022AC601.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Inyeccion por dependencias del string de conexion al contexto
builder.Services.AddDbContext<reservasDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("reservasDBConnection")
	)
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
