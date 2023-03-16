
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional Namespaces
using ChinookSystem.BLL;
using ChinookSystem.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace ChinookSystem
{
    //your clas needs to be public so it can be used outside
    //  of this  project
    //this class also needs to be statid
    public static class ChinookExtensions
    {
        //method name can be anything, it must match
        // the builder.Services.xxxxx(options => ....
        // statement in your Program.cs

        //the first parameter in the method signature is the class
        //  that you are attempting to extend

        //the second parameter in the method signature is the options
        //  value in your calling statement
        //it is receiving the connectionstring for your application

        public static void ChinookSystemDependencies(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            //registing of the DbContext class and any service classes

            //register the DbContext class with the service collection
            services.AddDbContext<ChinookContext>(options);

            //add any services that you create in the class library
            // are registered in the IServiceCollection using the
            //    .AddTransient<T>(....) where T is the classname
            // where your service method exist

            services.AddTransient<TrackServices>((serviceProvider) =>
                {
                    var context = serviceProvider.GetRequiredService<ChinookContext>();
                    return new TrackServices(context);
                });

            services.AddTransient<PlaylistTrackServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                return new PlaylistTrackServices(context);
            });

        }
    }
}
