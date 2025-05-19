using System.Net;
using foodOrderingApp.data;
using foodOrderingApp.interfaces;
using foodOrderingApp.reprositries;
using foodOrderingApp.repositories;
using foodOrderingApp.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using foodOrderingApp.models;
using System.Text.Json.Serialization;
using foodOrderingApp.obj;
using foodOrderingApp.services;
using Microsoft.Extensions.FileProviders;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using foodOrderingApp.services.Redis;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AppRole = foodOrderingApp.models.Role;
using foodOrderingApp.repositories.cart;
using foodOrderingApp.repositories.dashboard;


var builder = WebApplication.CreateBuilder(args);
// Register the service with Dependency Injection
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IResturantRepository, RestaurantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRespository>();
builder.Services.AddScoped<ICategoryReopsitory, CategoryReopsitory>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPayment, CashfreePayment>();
builder.Services.AddScoped<IDiscountCouponRepository, DiscountCouponRespository>();

builder.Services.AddScoped<Filters>();
builder.Services.AddScoped<FirebaseService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "FoodAppCache:";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!)
);

builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddScoped<ICartRepository,CartRepository>();
builder.Services.AddScoped<RestaurantDashboard>();
builder.Services.AddScoped<AdminDashboard>();




builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });



// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodOrdering API", Version = "v1" });
    // var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


string jwtKey = builder.Configuration["Jwt:Key"]!;
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
            ClockSkew = TimeSpan.FromMinutes(1)
        };

    });

// Define Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Scenario 1: Only authenticate (no specific role)
    options.AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
    // Scenario 2: Require Admin role
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole(UserRepository.Roles(AppRole.Admin));
    });
    // Scenario 3: Require User role
    options.AddPolicy("CustomerOnly", policy =>
    {
        policy.RequireRole(UserRepository.Roles(AppRole.Customer));
    });

    // Scenario 4: Require owner  role
    options.AddPolicy("OwnerOnly", policy =>
   {
       policy.RequireRole(UserRepository.Roles(AppRole.Owner));
   });
    // Scenario 4: Require either Admin OR User role
    // With RequireRole, listing multiple roles is an OR condition
    options.AddPolicy("AdminOrCustomers", policy =>
    {
        policy.RequireRole(UserRepository.Roles(AppRole.Admin), UserRepository.Roles(AppRole.Customer));
    });
    // Scenario 5:  Authorize with Both Admin and User Roles
    // Multiple RequireRole calls within a policy are treated as AND conditions
    options.AddPolicy("AdminAndCustomers", policy =>
        policy.RequireRole(UserRepository.Roles(AppRole.Admin))
              .RequireRole(UserRepository.Roles(AppRole.Customer)));
});



builder.Services.AddAuthorization();

// firebase strup
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("./foodorderingapp-f2115-firebase-adminsdk-fbsvc-b8e9e9e42b.json"),
});



var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
    Console.WriteLine($"✅ Created /upload folder at: {uploadPath}");
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
       Path.Combine(builder.Environment.ContentRootPath, "uploads")
    ),
    RequestPath = "/uploads"
});


//CORS
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Use(async (context, next) =>
{
    await next(); 
    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"status\": 404, \"message\": \"Endpoint not found\"}");
    }
});




app.Run();


