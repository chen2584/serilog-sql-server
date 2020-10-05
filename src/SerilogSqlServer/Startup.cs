using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

namespace SerilogSqlServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var setting = Configuration.Get<AppSetting>();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: setting.Logging.ConnectionString,
                    sinkOptions: new SinkOptions
                    {
                        TableName = setting.Logging.TableName,
                        AutoCreateSqlTable = true
                    },
                    columnOptions: GetColumnOptions(),
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();

            // Serilog.Debugging.SelfLog.Enable(message =>
            // {
            //     Log.Logger.Information(message);
            // });
        }

        public IConfiguration Configuration { get; }

        #region Private Method
        private ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);

            columnOptions.AdditionalColumns = new List<SqlColumn>
            {
                new SqlColumn
                {
                    DataType = SqlDbType.NVarChar,
                    ColumnName = "RequestedBy"
                },
                new SqlColumn
                {
                    DataType = SqlDbType.NVarChar,
                    ColumnName = "RequestedIpAddress"
                },
                new SqlColumn
                {
                    DataType = SqlDbType.NVarChar,
                    ColumnName = "HostIpAddress"
                },
                new SqlColumn
                {
                    DataType = SqlDbType.DateTime,
                    ColumnName = "Date"
                }
            };

            return columnOptions;
        }
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
