using Restoran.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173", "http://localhost:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Restaurant Management API",
        Version = "v1",
        Description = "RESTful API for restaurant order management system with JWT Authentication",
        Contact = new OpenApiContact
        {
            Name = "Restaurant System",
            Email = "info@restaurant.com"
        }
    });

    // Add JWT Bearer token to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed default users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Create admin user if not exists
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        context.Users.Add(new Restoran.Models.User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
            Role = Restoran.Models.UserRole.Admin
        });
    }

    // Create waiter user if not exists
    if (!context.Users.Any(u => u.Username == "waiter"))
    {
        context.Users.Add(new Restoran.Models.User
        {
            Username = "waiter",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
            Role = Restoran.Models.UserRole.Waiter
        });
    }

    // Create cook user if not exists
    if (!context.Users.Any(u => u.Username == "cook"))
    {
        context.Users.Add(new Restoran.Models.User
        {
            Username = "cook",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
            Role = Restoran.Models.UserRole.Cook
        });
    }

    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API V1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();