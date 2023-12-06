using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace AuthCraft.Api.Test
{
    public class TestStartup
    {
        //public IServiceProvider ConfigureTestServices(IServiceCollection services)
        //{
        //    services.AddMvc();

        //    var containerBuilder = new ContainerBuilder();
        //    containerBuilder.RegisterModule<AutofacTestModule>();

        //    containerBuilder.Populate(services);
        //    var container = containerBuilder.Build();

        //    return new AutofacServiceProvider(container);
        //}

        public void ConfigureTest(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
            // loggerFactory.AddSerilog();
        }
    }
}
