using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace test_web_dotnet5
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                policy  =>
                                {
                                    policy.WithOrigins("http://localhost:8000");
                                });
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/healthcheck", async context =>
                {

                    await context.Response.WriteAsJsonAsync(JsonConvert.SerializeObject(new {
                        message = "Hello World!",
                    }));
                });
                
                endpoints.MapPost("/user", async context =>
                {
                    User user;
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        System.Console.WriteLine(body);
                        user = JsonConvert.DeserializeObject<User>(body);
                    }
                    user.Name = user.Name + " Alguma coisa";

                    await context.Response.WriteAsJsonAsync(JsonConvert.SerializeObject(user));
                });
            });
        }
    }
}
