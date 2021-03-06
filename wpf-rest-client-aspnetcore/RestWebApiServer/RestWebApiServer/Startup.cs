// -*- coding:utf-8-with-signature -*-
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestWebApiServer.Data;

namespace RestWebApiServer
{
// class Program から呼び出される.
// クラス名は "Startup" でなければならない。でないと scaffolding が失敗。
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services
    // to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore パッケージ
        // services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddControllers();
        services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestWebApiServer",
                                                     Version = "v1" });
                c.EnableAnnotations();
            });

        // データベースに接続する。
        services.AddDbContext<RestWebApiServerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RestWebApiServerContext")));
    }

    // This method gets called by the runtime. Use this method to configure the
    // HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) {
            // 例外の表示.
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestWebApiServer v1"));
        }

        // Middleware を並べることができる。
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
    }
} // class MyStartup

}
