using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace kOS.Tools.Server
{
    public class CompletionHandler : ICompletionHandler
    {
        private ILanguageServer _server;
        private CompletionCapability _capability;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public CompletionHandler(ILanguageServer server)
        {
            _server = server;
        }

        public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CompletionList(
                new CompletionItem()
                {
                    InsertText = "sqrt",
                    Label = "sqrt function",
                    Documentation = "square root"
                },
                new CompletionItem()
                {
                    InsertText = "abs",
                    Label = "abs function",
                    Documentation = "absolute value"
                }
            ));
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                //ResolveProvider = true,
                TriggerCharacters = new Container<string>(":")
            };
        }

        public void SetCapability(CompletionCapability capability)
        {
            _capability = capability;
            _server.ServerSettings.Capabilities.CompletionProvider = new CompletionOptions()
            {
                TriggerCharacters = new Container<string>(":")
            };
        }
    }
}
