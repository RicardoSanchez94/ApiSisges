

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServicioApi.Model.Metodos;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using ServicioApi.Model.Clases;
using ServicioApi.Model.Example;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServicioApi.Model.SisGes;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ServicioApi.Negocio;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddScoped<Negocio>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MySisgesDbcontext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("StringConexionSisges")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSisges" , Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }

    });
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });
    c.EnableAnnotations();
    c.SchemaFilter<IdocFIPagosRPExample>();
    c.OperationFilter<SwaggerRequestExampleFilter>();

}

    );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//builder.Services.AddScoped<Funciones>();
var app = builder.Build();
//app.MapGet("/Hello",(string name)=> $"Hola {name}");



//app.MapGet("/response", async () =>
//{
//    HttpClient client = new HttpClient();
//    var response = await client.GetAsync("https://jsonplaceholder.typicode.com/todos/");
//    response.EnsureSuccessStatusCode();
//    string responseBody = await response.Content.ReadAsStringAsync();
//    return responseBody;

//}
//);

//if (!builder.Environment.IsDevelopment())
//{
//    builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
//    options.HttpsPort = 443;
//});
//}

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseSwagger();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiSisges v1");
  /*  c.RoutePrefix = string.Empty;*/  // para que la documentación se muestre en la raíz
});

if (app.Environment.IsDevelopment())
{
   
}
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseHttpsRedirection();

WSHttpBinding binding = new WSHttpBinding();
binding.SendTimeout = new TimeSpan(0, 00, 0);





//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});


app.MapControllers();

//app.MapControllerRoute(name:"Swagger",pattern:"{controller = Swagger}");

app.UseStaticFiles();


app.Run();
