using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using kOS.Tools;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace kOS.Tools
{
    class CompletionHandler : ICompletionHandler
    {
        private const string PackageReferenceElement = "PackageReference";
        private const string IncludeAttribute = "Include";
        private const string VersionAttribute = "Version";

        private readonly ILanguageServer _router;
        private readonly ServerLogger _logger;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Pattern = "**/*.ks"
            }
        );

        private CompletionCapability _capability;

        public CompletionHandler(ILanguageServer router, ServerLogger logger)
        {
            _router = router;
            _logger = logger;

            Console.Error.WriteLine("CompletionHandler: " + this.GetHashCode());
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            Console.Error.WriteLine("GetRegistrationOptions: CompletionHandler");
            return new CompletionRegistrationOptions
            {
                DocumentSelector = _documentSelector,
                ResolveProvider = false,
                TriggerCharacters = new Container<string>(":"),
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToString();

            var items = new List<CompletionItem>();
            var abs = new CompletionItem
            {
                InsertText = "abs",
                Documentation = "Returns absolute value of input:",
                Detail = "builtin function",
            };

            items.Add(abs);

            Console.Error.WriteLine("Handle: " + request);
            //Console.Error.WriteLine("\tDocument: " + request.TextDocument.Uri.ToString());
            //Console.Error.WriteLine("\tPosition: " + request.Position.Line + "," + request.Position.Character);

            return new CompletionList(items);
        }

        public void SetCapability(CompletionCapability capability)
        {
            Console.Error.WriteLine("SetCapability: " + capability);
            Console.Error.WriteLine("\tDynamicRegistration: " + capability.DynamicRegistration);
            Console.Error.WriteLine("\tContextSupport: " + capability.ContextSupport);
            //Console.Error.WriteLine("\tCompletionItem.CommitCharactersSupport: " + capability.CompletionItem.CommitCharactersSupport);
            //Console.Error.WriteLine("\tCompletionItem.DeprecatedSupport: " + capability.CompletionItem.DeprecatedSupport);
            //Console.Error.WriteLine("\tCompletionItem.PreselectSupport: " + capability.CompletionItem.PreselectSupport);
            //Console.Error.WriteLine("\tCompletionItem.SnippetSupport: " + capability.CompletionItem.SnippetSupport);
            //Console.Error.WriteLine("\tCompletionItem.DocumentationFormat: " + capability.CompletionItem.DocumentationFormat.ToString());
            _capability = capability;
        }
    }
}
