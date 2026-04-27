using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Build.Framework;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Packaging.Signing;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,

        ValidateAudience = true,

        ValidateLifetime = true,

        ValidateSignatureLast = true,

        ValidIssuer = "RestaurantApi",

        ValidAudience = "RestaurantApiUsers",

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"))
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "Bearer",

            In = ParameterLocation.Header,

            Description = "Enter: Bearer{your JWT token}"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    // Reference the previously defined "Bearer" security scheme.
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },

                // No scopes are required for JWT Bearer authentication.
                // This array is empty because JWT does not use OAuth scopes here.
                new string[] { }
            }
        });
    }
});

//Depandancy Injection practice
//------------------------------------//
builder.Services.AddScoped<IPersonRepo, PersonRepo>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IRoleRepo, RolesRepo>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITableRepo, TableRepo>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IItemRepo, ItemRepo>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentMethodRepo, PaymentMethodRepo>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
//------------------------------------//

builder.Services.AddCors(options =>
options.AddPolicy("RestaurantApiCorsPolicy", policy =>
policy.WithOrigins("https://localhost:7186",
                    "http://localhost:5074")
.AllowAnyHeader()
.AllowAnyMethod()
));

var app = builder.Build();
//builder.Configuration.GetConnectionString("DefaultConnection");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();