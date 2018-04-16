using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using WebApp.Model;

namespace WebApp
{
    public class Startup
    {
        /// <summary>
        /// 10 KB
        /// </summary>
        public const int RECEIVE_BUFFER_SIZE = 10 * 1024;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseWebSockets(new WebSocketOptions()
            {
                ReceiveBufferSize = RECEIVE_BUFFER_SIZE
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/wsMothership" || context.Request.Path.ToString().StartsWith("/wsClient/"))
                {
                    if (!context.WebSockets.IsWebSocketRequest)
                    {
                        context.Response.StatusCode = 400;
                        return;
                    }

                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    if (context.Request.Path == "/wsMothership")
                    {
                        var mothership = AllMothershipsModel.CreateNewMothership(webSocket);
                        await mothership.RunReceiveLoopAsync();
                    }
                    else
                    {
                        string path = context.Request.Path.ToString();
                        string mothershipName = path.Substring("/wsClient/".Length).TrimEnd('/');

                        var client = AllMothershipsModel.TryCreateClient(webSocket, mothershipName);
                        if (client == null)
                        {
                            context.Response.StatusCode = 400;
                            return;
                        }
                        await client.RunReceiveLoopAsync();
                    }
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
