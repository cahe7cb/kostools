using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Server;
using Microsoft.Extensions.Logging;
using kOS.Tools.Server;
using kOS.Safe.Exceptions;
using kOS.Safe.Encapsulation;
using kOS.Suffixed;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using kOS.Safe.Utilities;
using kOS.Tools.Execution;
using System.IO;

namespace kOS.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            SafeHouse.Init(new Execution.Config(), new VersionInfo(0, 0, 0, 0), "", false, Directory.GetCurrentDirectory());
            SafeHouse.Logger = new NoopLogger();

            try
            {
                AssemblyWalkAttribute.Walk();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                throw;
            }

            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            // while (!System.Diagnostics.Debugger.IsAttached)
            // {
            //    await Task.Delay(100);
            // }

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    .WithMinimumLogLevel(LogLevel.Trace)
                    .WithHandler<DocumentChangeHandler>()
                    .WithHandler<DocumentSaveHandler>()
                    .WithHandler<DocumentOpenHandler>()
                    .WithHandler<CompletionHandler>()
                    .WithHandler<DocumentCloseHandler>()
                    .WithServices((IServiceCollection services) =>
                    {
                        services.AddSingleton<BufferManager>();
                    })
                );

            await server.WaitForExit;
        }
    }
}
