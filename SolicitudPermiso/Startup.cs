using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SolicitudPermiso.Datos;

namespace SolicitudPermiso
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
            // *** Para variables de sesión ***
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // *** Para variables de sesión ***

            //services.AddMvc()
            //    .AddMvcOptions(options =>
            //    {
            //        options.Filters.Add(new PerfilamientoActionFilter());
            //    });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Solicitud Permisos Calle Larga", Version = "v1" });
            });

            var cadenaConexion = Configuration.GetConnectionString("BD_Muni");
            services.AddTransient(provider => new ProveedorConexion(cadenaConexion));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddTransient<ComunasServicio>();
            //services.AddTransient<PagoWebServicio>();
            services.AddTransient<PermisosCirculacionServicios>();
            services.AddTransient<PermisosCirculacionSolicitudServicio>();
            services.AddTransient<TiposVehiculosServicio>();
            services.AddTransient<MarcasServicio>();
            services.AddTransient<TiposCarroceriasServicio>();
            services.AddTransient<TiposSellosServicio>();
            services.AddTransient<TiposCombustionesServicio>();
            services.AddTransient<TiposTransmisionesServicio>();
            services.AddTransient<TiposEquipamientosServicio>();
            services.AddTransient<CalidadJuridicaServicio>();
            services.AddTransient<PlantasRevisionTecnicaServicio>();
            services.AddTransient<CompaniasAseguradorasServicio>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Inicio/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication21 v1"));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // *** Para variables de sesión ***
            app.UseSession();
            // *** Para variables de sesión ***

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
