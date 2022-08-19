using BlogAPI.Config;
using BlogPost.Service.Config;
using BlogPost.Service.Helpers;
using Microsoft.OpenApi.Models;
using NLog;
using BlogAPI.Middleware;
var myAllowSpecificOrigins = "myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy  =>
                      {
                        //   policy.WithOrigins("http://localhost:8080/")
                        //   .AllowAnyHeader()
                        //   .AllowAnyMethod();
                          policy.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddBlogAuthentication(builder.Configuration);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogAPI", Version = "v1" });
});

builder.Services.AddSettings(builder.Configuration);
builder.Services.SetupCosmosDb(builder.Configuration);
builder.Services.AddBlogApiServices();
builder.Services.AddBlogRepositories();
builder.Services.AutoMapperConfigurations();
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogAPI v1"));
}

app.UseMiddleware();

app.UseHttpsRedirection();

app.UseRouting();

// global cors policy
// app.UseCors(x => x
//     .AllowAnyMethod()
//     .AllowAnyHeader()
//     .SetIsOriginAllowed(origin => true) // allow any origin
//     .AllowCredentials()); // allow credentials

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();