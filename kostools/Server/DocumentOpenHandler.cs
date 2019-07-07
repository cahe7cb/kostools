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
    public class DocumentOpenHandler : IDidOpenTextDocumentHandler
    {
        private ILanguageServer _server;
        private SynchronizationCapability _capability;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public DocumentOpenHandler(ILanguageServer server)
        {
            _server = server;
        }

        public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            Console.Error.WriteLine("This shit did open");
            return Unit.Task;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions()
            {
                DocumentSelector = _documentSelector
            };
        }

        public void SetCapability(SynchronizationCapability capability)
        {
            _capability = capability;
        }
    }
}
