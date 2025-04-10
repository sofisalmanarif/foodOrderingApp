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


var builder = WebApplication.CreateBuilder(args);
// Register the service with Dependency Injection
builder.Services.AddDbContext<AppDbContext>(options=>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IResturantRepository, RestaurantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRespository>();
builder.Services.AddScoped<ICategoryReopsitory, CategoryReopsitory>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<Filters>();





builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        
    });

// Define Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Scenario 1: Only authenticate (no specific role)
    // This policy simply requires the user to be authenticated
    options.AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
    // Scenario 2: Require Admin role
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole(UserRepository.Roles(Role.Admin));
    });
    // Scenario 3: Require User role
    options.AddPolicy("CustomerOnly", policy =>
    {
        policy.RequireRole(UserRepository.Roles(Role.Customer));
    });

    // Scenario 4: Require owner  role
    options.AddPolicy("OwnerOnly", policy =>
   {
       policy.RequireRole(UserRepository.Roles(Role.Owner));
   });
    // Scenario 4: Require either Admin OR User role
    // With RequireRole, listing multiple roles is an OR condition
    options.AddPolicy("AdminOrCustomers", policy =>
    {
        policy.RequireRole(UserRepository.Roles(Role.Admin), UserRepository.Roles(Role.Customer));
    });
    // Scenario 5:  Authorize with Both Admin and User Roles
    // Multiple RequireRole calls within a policy are treated as AND conditions
    options.AddPolicy("AdminAndCustomers", policy =>
        policy.RequireRole(UserRepository.Roles(Role.Admin))
              .RequireRole(UserRepository.Roles(Role.Customer)));
});



builder.Services.AddAuthorization();


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Use(async (context, next) =>
{
    await next(); // Continue to the next middleware

    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"status\": 404, \"message\": \"Endpoint not found\"}");
    }
});
app.Run();





// Guid? GetLoggedInUsersUserId(){
//     var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//     if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
//     {
//         return null;
//     }
//     return userId;
// }
