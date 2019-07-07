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
        private BufferManager _bufferManager;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public DocumentSaveHandler(ILanguageServer server, BufferManager bufferManager)
        {
            _server = server;
            _bufferManager = bufferManager;
            Console.Error.WriteLine("DocumentSaveHandler: " + this.GetHashCode());
        }

        public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            if(request.Text != null)
            {
                _bufferManager.put(request.TextDocument.Uri, request.Text);
            }

            var script = new kOS.Tools.Script();

            var text = _bufferManager.get(request.TextDocument.Uri);
            var errors = script.Compile(null, 0, text, null, null);

            var diagnostics = new Diagnostic[errors.Count];
            if (errors.Count > 0)
            {
                for (int i = 0; i < errors.Count; i++)
                {
                    var error = errors[i];
                    diagnostics[i] = new Diagnostic()
                    {
                        Code = error.Code,
                        Message = error.Message,
                        Range = new Range(
                            new Position(error.Line-1, error.Column-1),
                            new Position(error.Line-1, (error.Column-1)+error.Length)
                        ),
                        Severity = DiagnosticSeverity.Error,
                        RelatedInformation = new Container<DiagnosticRelatedInformation>(),
                        Source = script.getSource()
                    };
                }
            }
            _server.Document.PublishDiagnostics(new PublishDiagnosticsParams()
            {
                Uri = request.TextDocument.Uri,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            });
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
