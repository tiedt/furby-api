using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.Domain.Identity;
using Project.EmailService;
using Project.EmailService.CustomTokenProviders;
using Project.Repository;
using Project.Respository;
using Swashbuckle.AspNetCore.Filters;

namespace Project.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProjectContext>(x => x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddCors();
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddControllers();

            IdentityBuilder builder = services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false; //números especiais
                options.Password.RequireNonAlphanumeric = false; // nada de alfanúmerico
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<ProjectContext>()
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation");

            services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(3));

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddControllersWithViews();

            services.AddRazorPages();

            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling =
                 Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAutoMapper(typeof(Startup));

            // Configurando o serviço de documentação do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FURBY", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                      {
                        new OpenApiSecurityScheme
                        {
                          Reference = new OpenApiReference
                            {
                              Type = ReferenceType.SecurityScheme,
                              Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                          },
                          new List<string>()
                        }
                      });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            CreateRoles(serviceProvider).Wait();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            /* app.UseEndpoints(endpoints =>
           {
               endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
           }); */
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { "Administrator", "Store-Manager", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                // ensure that the role does not exist
                if (!roleExist)
                {

                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(CreateNewRole(roleName));
                }
            }

            // find the user with the admin email 
            var _user = await UserManager.FindByEmailAsync("lpa@furb.br");

            // check if the user exists
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app
                var poweruser = new User
                {
                    UserName = "luciana",
                    Email = "lpa@furb.br",
                    EmailConfirmed = true,
                    isAdmin = true
                };
                string adminPassword = "123456";

                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Administrator");

                }
            }
        }

        private Role CreateNewRole(string roleName)
        {
            var role = new Role();
            role.Name = roleName;
            return role;
        }
    }
}