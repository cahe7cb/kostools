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
        private readonly TextDocumentSyncKind Change = TextDocumentSyncKind.Incremental;

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

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine(request.Text ?? "(null)");
            return Unit.Task;
        }

        public TextDocumentSaveRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentSaveRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                IncludeText = true
            };
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
            _server.ServerSettings.Capabilities.TextDocumentSync = Change;
            _server.ServerSettings.Capabilities.TextDocumentSync.Options = new TextDocumentSyncOptions()
            {
                Change = Change,
                Save = new SaveOptions()
                {
                    IncludeText = true
                },
                OpenClose = true
            };
        }
    }
}
