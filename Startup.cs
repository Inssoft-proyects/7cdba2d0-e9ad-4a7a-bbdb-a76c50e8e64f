using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Utils;
using GuanajuatoAdminUsuarios.Framework;
using GuanajuatoAdminUsuarios.Models;
using SITTEG.APIClientHelper.Client;
using SITTEG.APIClientInfrastructure.Client;
using System.ComponentModel;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace GuanajuatoAdminUsuarios
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

            var mvcBuilder = services.AddControllersWithViews();

            #if DEBUG
            mvcBuilder.AddRazorRuntimeCompilation();
#endif
            //Connection Strings
            string connStringLicencias = Configuration.GetConnectionString("GUANAJUATO_ADMIN_USUARIOS_PRD");
            string connStringIncidencias = Configuration.GetConnectionString("GUANAJUATO_ADMIN_USUARIOS_PRD");

            services.AddDbContext<GuanajuatoLicenciasContext>(options => options.UseSqlServer(connStringLicencias));
            services.AddScoped<DbContext, GuanajuatoLicenciasContext>();

            services.AddDbContext<GuanajuatoIncidenciasMigContext>(options => options.UseSqlServer(connStringIncidencias));
            services.AddScoped<DbContext, GuanajuatoLicenciasContext>();

            // Configure cookie based authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        // Specify where to redirect un-authenticated users
                        options.LoginPath = "/";

                        // Specify the name of the auth cookie.
                        // ASP.NET picks a dumb name by default.
                        options.Cookie.Name = "gto_admin_auth_cookie";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                        options.SlidingExpiration = true;
                    });

            services.AddSession(options =>
            {
                options.Cookie.Name = "RiagS";
                options.IdleTimeout = TimeSpan.FromMinutes(40);
                options.Cookie.IsEssential = true;
            });
            services.AddHttpClient();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            

            services.AddRouting(setupAction =>
            {
                setupAction.LowercaseUrls = true;
            });
            // Add framework services.
            services
                .AddControllersWithViews()
                // Maintain property names during serialization. See:
                // https://github.com/aspnet/Announcements/issues/194
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            // Servicios KEndo Telerik
            services.AddKendo();

            //Added for session state
            services.AddDistributedMemoryCache();

            //configuracion de session
            services.AddSession();

            //Hacer accesible la cadena de conexion a la base desde la clase
            // services.AddTransient<ConexionBD>();

            //Servicios 
            services.AddScoped(typeof(IPdfGenerator), typeof(PdfGenerator));
            services.AddScoped<ISqlClientConnectionBD, SqlClientConnectionBD>();
            
            services.AddScoped<IMarcasVehiculos, MarcasVehiculoService>();
            services.AddScoped<ICatFormasTrasladoService, CatFormasTrasladoService>();
            services.AddScoped<ICatEntidadesService, CatEntidadesService>();
            services.AddScoped<ITiposCarga, TiposCargaService>();
            services.AddScoped<ISubmarcasVehiculos, SubmarcasVehiculosService>();
            services.AddScoped<ICatDelegacionesOficinasTransporteService, CatDelegacionesOficinasTransporteService>();
            services.AddScoped<IDependencias, DependenciasService>();
            services.AddScoped<IOficiales, OficialesService>();
            services.AddScoped<IPlacaServices, PlacaServices>();
            services.AddScoped<ILiberacionVehiculoService, LiberacionVehiculoService>();
            services.AddScoped<ITransitoTransporteService, TransitoTransporteService>();
            services.AddScoped<IGruasService, GruasService>();
            services.AddScoped<IPadronDepositosGruasService, PadronDepositosGruasService>();
            services.AddScoped<IMunicipiosService, MunicipiosService>();
            services.AddScoped<IConcesionariosService, ConcesionariosService>();
            services.AddScoped<IReporteAsignacionService, ReporteAsignacionService>();
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IPensionesService, PensionesService>();
            services.AddScoped<IDelegacionesService, DelegacionesService>();
            services.AddScoped<IResponsableService, ResponsableService>();
            services.AddScoped<IEstatusInfraccionService, EstatusInfraccionService>();
            services.AddScoped<ITipoCortesiaService, TipoCortesiaService>();
            services.AddScoped<IGarantiasService, GarantiasService>();
            services.AddScoped<IInfraccionesService, InfraccionesService>();
            services.AddScoped<IVehiculosService, VehiculosService>();
            services.AddScoped<IPersonasService, PersonasService>();
            services.AddScoped<ICatTipoLicenciasService, CatTipoLicenciasService>();

            services.AddScoped<ICatalogosService, CatalogosService>();
            services.AddScoped<ICatDictionary, CatDictionary>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddScoped<ICatFactoresAccidentesService, CatFactoresAccidentesService>();
            services.AddScoped<ICatFactoresOpcionesAccidentesService, CatFactoresOpcionesAccidentesService>();
            services.AddScoped<ICatCausasAccidentesService, CatCausasAccidentesService>();
            services.AddScoped<ICatHospitalesService, CatHospitalesService>();
            services.AddScoped<IDiasInhabiles, DiasInhabilesService>();

            services.AddScoped<ICatClasificacionAccidentes, CatClasificacionAccidentesService>();
            services.AddScoped<ICatMarcasVehiculosService, CatMarcasVehiculosService>();
            services.AddScoped<ICatSubmarcasVehiculosService, CatSubmarcasVehiculosService>();
            services.AddScoped<ICatMunicipiosService, CatMunicipiosService>();
            services.AddScoped<ICatTramosService, CatTramosService>();
            services.AddScoped<ICatCarreterasService, CatCarreterasService>();
            services.AddScoped<IRegistroReciboPagoService, RegistroReciboPagoService>();
            services.AddScoped<ICancelarInfraccionService, CancelarInfraccionService>();
            services.AddScoped<ICapturaAccidentesService, CapturaAccidentesService>();
            services.AddScoped<IEstadisticasService, EstadisticasService>();
            services.AddScoped<ICatTipoInvolucradoService, CatTipoInvolucradoService>();
            services.AddScoped<ICatEstadoVictimaService, CatEstadoVictimaService>();
            services.AddScoped<ICatInstitucionesTrasladoService, CatInstitucionesTrasladoService>();
            services.AddScoped<ICatAsientoService, CatAsientoService>();
            services.AddScoped<ICatCinturon, CatCinturonService>();
            services.AddScoped<ICatAutoridadesDisposicionService, CatAutoridadesDisposicionService>();
            services.AddScoped<ICatAutoridadesEntregaService, CatAutoridadesEntregaService>();
            services.AddScoped<ICatCiudadesService, CatCiudadesService>();
            services.AddScoped<ICatAgenciasMinisterioService, CatAgenciasMinisterioService>();
            services.AddScoped<IBusquedaAccidentesService, BusquedaAccidentesService>();
            services.AddScoped<IBitacoraService, BitacoraService>();

            services.AddScoped<IBusquedaEspecialAccidentesService, BusquedaEspecialAccidentesService>();
            services.AddScoped<IEnvioInfraccionesService, EnvioInfraccionesService>();
            services.AddScoped<ICatOficinasRentaService, CatOficinasRentaService>();
            services.AddScoped<ICatTiposVehiculosService, CatTiposVehiculosService>();
            services.AddScoped<ICatSubtipoServicio, CatSubtipoServicioService>();
            services.AddScoped<ICatResponsablesPensiones, CatResponsablesPensionesService>();
            services.AddScoped<ICatDescripcionesEventoService, DescripcionEvetosService>();
            services.AddScoped<ICatTipoMotivoAsignacionService, CatTipoMotivoAsignacionService>();
            services.AddScoped<ICatTipoUsuarioService, CatTipoUsuarioService>();
            services.AddScoped<IDepositosService, DepositosService>();
            services.AddScoped<IServiceAppSettingsService, ServiceAppSettingsService>();
            services.AddScoped(typeof(IRequestDynamic<,>), typeof(RequestDynamic<,>));
            services.AddScoped(typeof(IRequestXMLDynamic<>), typeof(RequestXMLDynamic<>));
            services.AddScoped<ICotejarDocumentosClientService, CotejarDocumentosClientService>();
            services.AddScoped<IConsultarDocumentoService, ConsultarDocumentoService>();
            services.AddScoped<IAnulacionDocumentoService, AnulacionDocumentoClientService>();
            services.AddScoped<IAsignacionGruasService, AsignacionGruasService>();
            services.AddScoped<IEstadisticasAccidentesService, EstadisticasAccidentesService>();
            services.AddScoped<ICrearMultasTransitoClientService, CrearMultasTransitoClientService>();
            services.AddScoped<ILicenciasService, LicenciasService>();
            services.AddScoped<IMotivoInfraccionService, MotivoInfraccionService>();
            services.AddScoped<ISalidaVehiculosService, SalidaVehiculosService>();
            services.AddScoped<IIngresarVehiculosService, IngresoVehiculosService>();
            services.AddScoped<IBusquedaDepositoService, BusquedaDepositoService>();
            services.AddScoped<ICortesiasNoAplicadas, CortesiasNoAplicadasService>();


            services.AddScoped<IColores, CatColoresService>();
            services.AddScoped<ICatEstatusReporteService, CatEstatusReporteService>();


            services.AddScoped<IAppSettingsService, AppSettingService>();
            services.AddScoped<IRepuveService, RepuveService>();
            services.AddScoped<IApiClientDatabaseService, ApiClientDatabaseService>();

            services.AddScoped<IColores, CatColoresService>();

            services.AddScoped<IComparativoInfraccionesService, ComparativoInfraccionesService>();

            services.AddScoped<IPagosInfraccionesService, PagosInfraccionesService>();
            services.AddScoped<ILogTraficoService, LogTraficoService>();

            services.AddScoped(typeof(IApiClient), typeof(ApiClient));
            services.AddScoped<IAccountClient, AccountClient>();
            services.AddScoped<IGenericClient, GenericClient>();

            services.AddScoped<ICatDependenciaEnviaService, CatDependenciaEnviaService>();
            services.AddScoped<ICatTipoMotivoIngresoService, CatTipoMotivoIngresoService>();
            services.AddScoped<IVehiculoPlataformaService, VehiculoPlataformaService>();


            services
               .AddControllersWithViews()
               .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddKendo();
            services.AddHttpContextAccessor();
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // implementa la validacion de sesion
            app.UseSession();
            app.UseRouting();
            

            //Autorizacion y autenticacion de usuario
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller=Inicio}/{action=Inicio}/{id?}");

                endpoints.MapControllerRoute(
           name: "servicio",
           pattern: "servicio/consumir",
           defaults: new { controller = "Login", action = "ConsumirServicio" });
            });
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "defaultSitteg",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Inicio}/{action=Inicio}/{id?}");
            });

        }
    }
}