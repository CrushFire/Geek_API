using Application.Services;
using DataAccess;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.AddAutoMapper(typeof(Geek_API.Mappers.AutoMapper));

builder.Services.AddScoped<IServiceUser, ServiceUser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.MapRazorPages();

app.Run();
