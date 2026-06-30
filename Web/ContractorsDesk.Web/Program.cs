using ContractorsDesk.WebPortal.Middlewares;
using ContractorsDesk.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using ContractorsDesk.WebPortal.Helpers;
using ContractorsDesk.Services.Hubs;
using Hangfire;
using Autofac.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.ConfigureDbContext();
builder.Services.AddSignalR();

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Case-insensitive deserialization
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
		options.JsonSerializerOptions.AllowTrailingCommas = true;
	});

builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
   o.TokenLifespan = TimeSpan.FromHours(1));

builder.Services.Configure<FormOptions>(options =>
{
	options.MultipartBodyLengthLimit = 209715200; // Set the limit to 200 MB (50 * 1024 * 1024 bytes)
});

builder.ConfigureJWTAuthentication();
builder.ConfigureAutoMapper();
builder.BuildConfigurations();
builder.ConfigureDependencies();
builder.ConfigureCORS();

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSession();
builder.Services.AddDinkToPdf();
builder.Logging.AddConsole();
builder.Services.AddHttpClient();

builder.AddHangfire();
builder.ConfigureRedis();

builder.Services.AddSwaggerGen();

var app = builder.Build();
var serviceProvider = app.Services.GetRequiredService<IServiceProvider>();



app.UseSession();
app.UseHttpsRedirection();
app.ConfigureStaticFiles();

app.UseRouting();


HtmlHelperExtensions.Configure(app.Services);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Dashboard}/{id?}"
);


app.UseSwagger();

app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	options.RoutePrefix = "api";
});

app.UseRouting();

app.MapDefaultControllerRoute();

app.UseCors("CorsPolicy");
app.UseAuthentication();

//app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<ApiExceptionHandlerMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.MapControllers();
app.MapHub<ContractorDeskHub>("/contractorDeskHub");
app.SetupHangfireService(serviceProvider);



app.Run();
