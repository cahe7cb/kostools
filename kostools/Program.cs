using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Server;
using Microsoft.Extensions.Logging;
using kOS.Tools.Server;
using kOS.Safe.Exceptions;

namespace kOS.Tools
{
    class Program
    {
        public static void Main(string[] args)
        {
            SyntaxVerifier verifier = new SyntaxVerifier();

            verifier.Setup();

            try
            {
                verifier.RunScript(args[0]);
            }
            catch (KOSParseException e)
            {
                Console.Error.WriteLine("Parse Error");
                Console.WriteLine(e.VerboseMessage);
            }
            catch (KOSCompileException e)
            {
                Console.Error.WriteLine("Compiling Error");
                Console.WriteLine(e.VerboseMessage);
            }
        }

        //static void Main(string[] args)
        //{
        //    MainAsync(args).Wait();
        //}

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
                );

            await server.WaitForExit;
        }
    }
}
