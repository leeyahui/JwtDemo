using System;
using System.IO;
using System.Reflection;
using System.Text;
using JwtDemo.Interface;
using JwtDemo.Models;
using JwtDemo.Services;
using JwtDemo.SwaggerFile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace JwtDemo
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
            services.AddControllers();
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {                  
                    ValidateIssuer = true, //是否验证发行人
                    ValidIssuer = token.Issuer,//发行人
                                          
                    ValidateAudience = true,//是否验证受众人
                    ValidAudience = token.Audience,//受众人
                                             
                    ValidateIssuerSigningKey = true,//是否验证密钥
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),

                    ValidateLifetime = true, //验证生命周期
                    RequireExpirationTime = true, //过期时间
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "测试JWT接口",
                    Description = "测试接口"
                });
                // 为 Swagger 设置xml文档注释路径
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                a.IncludeXmlComments(xmlPath);
                a.DocInclusionPredicate((docName, description) => true);
                //添加对控制器的标签(描述)
                a.DocumentFilter<ApplyTagDescription>(); //显示类名
                a.CustomSchemaIds(type => type.FullName); // 可以解决相同类名会报错的问题

                #region 启用swagger验证功能

                a.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                });
                a.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] { }
                    }
                });

                #endregion
            });

            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            //启用jwt认证
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web App v1");
                c.RoutePrefix = "doc"; //设置根节点访问
                //c.DocExpansion(DocExpansion.None);//折叠
                c.DefaultModelsExpandDepth(-1); //不显示Schemas
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}