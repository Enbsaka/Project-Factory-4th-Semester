using Dunder_Store.Data.Repositories;
using Dunder_Store.Database;
using Dunder_Store.E_commerce.Business.Entities;
using Dunder_Store.E_commerce.Business.Interfaces.IRepositories;
using Dunder_Store.E_commerce.Business.Interfaces.IServices;
using Dunder_Store.E_commerce.Business.Services;
using Dunder_Store.E_commerce.Data.Repositories;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;
using Dunder_Store.Repositories;
using Dunder_Store.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace Dunder_Store
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API E-commerce", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira 'Bearer {seu token JWT}' para autenticar."
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


            const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",   // Frontend Vue (padrão)
                            "http://localhost:5174",   // Frontend Vue (teste cliente)
                            "http://localhost:3000",   // Frontend Vue (teste completo)
                            "http://localhost:5212",   // Backend HTTP
                            "https://localhost:7136"   // Backend HTTPS
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });


            if (builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<ProdutosDbContext>(options =>
                    options.UseInMemoryDatabase("ProdutosTests"));
            }
            else
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<ProdutosDbContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            }


            builder.Services.AddScoped<IProdutoService, ProdutoService>();
            builder.Services.AddScoped<IPedidoService, PedidoService>();
            builder.Services.AddScoped<IClienteService, ClienteService>();
            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<ICupomService, CupomService>();


            builder.Services.AddHttpClient<IViaCepService, ViaCepService>();


            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
            builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
            builder.Services.AddScoped<IPedidoProdutoRepository, PedidoProdutoRepository>();
            builder.Services.AddScoped<ICupomRepository, CupomRepository>();
            builder.Services.AddScoped<IPrecoRegiaoRepository, PrecoRegiaoRepository>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();


            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection.GetValue<string>("Key");
            var jwtIssuer = jwtSection.GetValue<string>("Issuer");
            var jwtAudience = jwtSection.GetValue<string>("Audience");

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("Configuração JWT inválida: 'Jwt:Key' não definida.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services
                .AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));


            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 7136;
            });


            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ProdutosDbContext>();

                    try
                    {
                        db.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DEV] Falha ao aplicar migrações: {ex.Message}");
                    }


                    var devAdminEmail = app.Configuration.GetValue<string>("DevAdminEmail");
                    if (!string.IsNullOrWhiteSpace(devAdminEmail))
                    {
                        var admin = db.Clientes.FirstOrDefault(c => c.Email == devAdminEmail);
                        if (admin != null && !admin.IsAdmin)
                        {
                            admin.IsAdmin = true;
                            db.SaveChanges();
                            Console.WriteLine($"[DEV] Usuário '{devAdminEmail}' promovido a Admin.");
                        }
                    }
                }
            }

            app.UseCors(MyAllowSpecificOrigins);

            if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
            {
                app.UseHttpsRedirection();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            var produtosPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "produtos");
            Directory.CreateDirectory(produtosPath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(produtosPath),
                RequestPath = "/produtos"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");
            app.Run();
        }
    }
}
