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
    public class DocumentSaveHandler : IDidSaveTextDocumentHandler
    {
        private readonly TextDocumentSyncKind Change = TextDocumentSyncKind.Full;

        private ILanguageServer _server;
        private SynchronizationCapability _capability;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public DocumentSaveHandler(ILanguageServer server)
        {
            _server = server;
        }

        public TextDocumentSaveRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine("This shit did save");
            return Unit.Task;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
            _server.ServerSettings.Capabilities.TextDocumentSync = Change;
        }
    }
}
