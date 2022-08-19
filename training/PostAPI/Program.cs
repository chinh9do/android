using Microsoft.OpenApi.Models;
using PostAPI.Middleware;
using BlogPost.Service.Config;
using AutoMapper;
using BlogPost.Service.Helpers;
using PostAPI.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PostAPI", Version = "v1" });
});

builder.Services.SetupCosmosDb(builder.Configuration);
builder.Services.AddPostApiServices();
builder.Services.AddPostApiRepositories();
builder.Services.AutoMapperConfigurations();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostAPI v1"));
}

app.UseMiddleware();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();