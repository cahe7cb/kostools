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
    class DocumentChangeHandler : IDidChangeTextDocumentHandler
    {
        public TextDocumentSyncKind Change => TextDocumentSyncKind.Incremental;

        public SynchronizationCapability _capability;
        public ILanguageServer _server;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public DocumentChangeHandler(ILanguageServer server)
        {
            _server = server;
        }

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentChangeRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                SyncKind = Change
            };
        }

        public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine("This shit did change");
            return Unit.Task;
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }
    }
}
