using BarkServerNet;
using DotAPNS;
using DotAPNS.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApns<ApnsJwtOptions>(builder.Configuration.GetSection("HttpTimeout").Get<int?>() ?? 100,
    o => o.UseApnsJwt(builder.Configuration.GetApnsJwtOptions("ApnsJwtOptions:Apnjwt")));
builder.Services.AddDbContext<DeviceDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
builder.Services.AddScoped<IDeviceServer, DeviceServer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
