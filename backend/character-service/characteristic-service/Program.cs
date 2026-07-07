using System.Text;
using character_service.Core.DataAccess;
using character_service.Features.Characteristics;
using character_service.Features.Ranks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtKey = builder.Configuration["Jwt:Key"]!;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = "preferred_username",
            RoleClaimType = "role"
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICharacteristicService, CharacteristicService>();
builder.Services.AddScoped<ICharacteristicRepository, CharacteristicRepository>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IRankRepository, RankRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Characteristic Service", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    
    if (!await db.Ranks.AnyAsync())
    {
        db.Ranks.AddRange(
            new Rank { Name = "Рекрут", MinTotalLevels = 0, MaxTotalLevels = 9 },
            new Rank { Name = "Авантюрист", MinTotalLevels = 10, MaxTotalLevels = 24 },
            new Rank { Name = "Рыцарь", MinTotalLevels = 25, MaxTotalLevels = 49 },
            new Rank { Name = "Командор", MinTotalLevels = 50, MaxTotalLevels = 74 },
            new Rank { Name = "Властелин", MinTotalLevels = 75, MaxTotalLevels = 99 },
            new Rank { Name = "Титан", MinTotalLevels = 100, MaxTotalLevels = int.MaxValue }
        );
        await db.SaveChangesAsync();
    }
}

app.Run();