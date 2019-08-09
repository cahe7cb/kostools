using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using kOS.Safe;
using kOS.Safe.Compilation.KS;
using kOS.Safe.Encapsulation;
using kOS.Suffixed;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;
using kOS.Communication;
using kOS.Control;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Suffixed.Part;

namespace kOS.Tools.Server
{
    public class CompletionHandler : ICompletionHandler
    {
        private ILanguageServer _server;
        private CompletionCapability _capability;
        private BufferManager _bufferManager;
        private readonly ConcurrentDictionary<Type, Structure> _mocks;
        private readonly ConcurrentDictionary<string, Type> _bindings;

        private readonly DocumentSelector _documentSelector = new DocumentSelector(
            new DocumentFilter()
            {
                Language = "kerboscript",
                Pattern = "**/*.ks"
            }
        );

        public CompletionHandler(ILanguageServer server, BufferManager bufferManager)
        {
            _server = server;
            _bufferManager = bufferManager;
            _mocks = new ConcurrentDictionary<Type, Structure>();
            _bindings = new ConcurrentDictionary<string, Type>();

            Console.Error.WriteLine("CompletionHandler: " + this.GetHashCode());

            InitializeMockObjects();
            InitializeBindings();
        }

        public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var items = new CompletionList();

            if(request.Context != null)
            {
                if (request.Context.TriggerKind == CompletionTriggerKind.TriggerCharacter)
                {
                    if (request.Context.TriggerCharacter == ":")
                    {
                        items = StructureMemberComplete(request);
                    }
                }
            }
            else
            {
                items = StructureMemberComplete(request);
            }

            return Task.FromResult(items);
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions()
            {
                DocumentSelector = _documentSelector,
                TriggerCharacters = new Container<string>(":"),
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

        private void InitializeMockObjects()
        {
            _mocks.GetOrAdd(typeof(VesselTarget), (type) =>
            {
                var constructor = type.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(Vessel), typeof(SharedObjects) },
                    null);
                Structure obj = (Structure)constructor.Invoke(new object[] { null, new SharedObjects() });
                if (obj == null)
                    Console.Error.WriteLine("Failed to create mock object for " + type.ToString());
                else
                    obj.HasSuffix("TYPENAME");
                return obj;
            });

            _mocks.GetOrAdd(typeof(BodyTarget), (type) =>
            {
                var constructor = type.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(CelestialBody), typeof(SharedObjects) },
                    null);
                Structure obj = (Structure)constructor.Invoke(new object[] { null, new SharedObjects() });
                if (obj == null)
                    Console.Error.WriteLine("Failed to create mock for " + type.ToString());
                return obj;
            });

            _mocks.GetOrAdd(typeof(kOS.Suffixed.Vector), (type) =>
            {
                var obj = new kOS.Suffixed.Vector();
                obj.HasSuffix("TYPENAME"); // force suffix initialization
                return obj;
            });

            _mocks.GetOrAdd(typeof(OrbitableVelocity), (type) =>
            {
                return new OrbitableVelocity((Vector)null, (Vector)null);
            });

            _mocks.GetOrAdd(typeof(KUniverseValue), (type) =>
            {
                return new KUniverseValue(new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(OrbitInfo), (type) =>
            {
                return new OrbitInfo();
            });

            _mocks.GetOrAdd(typeof(StringValue), (type) =>
            {
                var obj = new StringValue("");
                obj.HasSuffix("TYPENAME"); // force suffix initialization
                return obj;
            });

            _mocks.GetOrAdd(typeof(VesselAlt), (type) =>
            {
                return new VesselAlt(new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(Node), (type) =>
            {
                return new Node(0.0, 0.0, 0.0, 0.0, new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(Suffixed.TimeSpan), (type) =>
            {
                return new Suffixed.TimeSpan(0.0);
            });

            _mocks.GetOrAdd(typeof(VesselEta), (type) =>
            {
                return new VesselEta(new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(kOS.Core), (type) =>
            {
                return new Core(null, new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(HomeConnection), (type) =>
            {
                return new HomeConnection(new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(ControlConnection), (type) =>
            {
                return new ControlConnection(new SharedObjects());
            });

            _mocks.GetOrAdd(typeof(Direction), (type) =>
            {
                return new Direction();
            });

            _mocks.GetOrAdd(typeof(PIDLoop), (type) =>
            {
                return new PIDLoop();
            });

            _mocks.GetOrAdd(typeof(SteeringManager), (type) =>
            {
                return new SteeringManager((Vessel)null);
            });

            _mocks.GetOrAdd(typeof(GeoCoordinates), (type) =>
            {
                var constructor = type.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[]{},
                    null);
                Structure obj = (Structure)constructor.Invoke(new object[]{});
                if (obj == null)
                    Console.Error.WriteLine("Failed to create mock for " + type.ToString());
                return obj;
            });

            _mocks.GetOrAdd(typeof(FlightControl), (type) =>
            {
                return new FlightControl((Vessel)null);
            });

            _mocks.GetOrAdd(typeof(ListValue), (type) =>
            {
                var obj = new ListValue();
                obj.HasSuffix("TYPENAME");
                return obj;
            });

            _mocks.GetOrAdd(typeof(ListValue<Structure>), (type) =>
            {
                var obj = new ListValue<Structure>();
                obj.HasSuffix("TYPENAME");
                return obj;
            });

            _mocks.GetOrAdd(typeof(PartValue), (type) =>
            {
                var constructor = type.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(SharedObjects), typeof(global::Part), typeof(PartValue), typeof(DecouplerValue) },
                    null);
                Structure obj = (Structure)constructor.Invoke(new object[] { new SharedObjects(), null, null, null });
                if (obj == null)
                    Console.Error.WriteLine("Failed to create mock for " + type.ToString());
                else
                    obj.HasSuffix("TYPENAME");
                return obj;
            });

            _mocks.GetOrAdd(typeof(kOS.Safe.Persistence.Volume), (type) =>
            {
                return new Persistence.MockVolume();
            });

            _mocks.GetOrAdd(typeof(kOS.Safe.Encapsulation.Lexicon), (type) =>
            {
                return new kOS.Safe.Encapsulation.Lexicon();
            });

            _mocks.GetOrAdd(typeof(ConstantValue), (type) =>
            {
                return new ConstantValue();
            });

            _mocks.GetOrAdd(typeof(VesselAlt), (type) =>
            {
                return new VesselAlt((SharedObjects)null);
            });

            _mocks.GetOrAdd(typeof(VesselEta), (type) =>
            {
                return new VesselEta((SharedObjects)null);
            });
        }

        private void InitializeBindings()
        {
            _bindings.TryAdd("CORE", typeof(kOS.Core));
            _bindings.TryAdd("SHIP", typeof(VesselTarget));
            _bindings.TryAdd("KUNIVERSE", typeof(KUniverseValue));
            _bindings.TryAdd("BODY", typeof(BodyTarget));
            _bindings.TryAdd("ORBIT", typeof(OrbitInfo));
            _bindings.TryAdd("TIME", typeof(Suffixed.TimeSpan));
            _bindings.TryAdd("ACTIVESHIP", typeof(VesselTarget));
            _bindings.TryAdd("NEXTNODE", typeof(Node));
            _bindings.TryAdd("TARGET", typeof(VesselTarget));
            _bindings.TryAdd("HOMECONNECTION", typeof(HomeConnection));
            _bindings.TryAdd("CONTROLCONNECTION", typeof(ControlConnection));
            _bindings.TryAdd("SOLARPRIMEVECTOR", typeof(Vector));
            _bindings.TryAdd("STEERINGMANAGER", typeof(SteeringManager));
            //_bindings.TryAdd("CONSTANT", typeof(ConstantValue));
            _bindings.TryAdd("ARCHIVE", typeof(kOS.Safe.Persistence.Volume));
            _bindings.TryAdd("ANGULARVELOCITY", typeof(Vector));
            _bindings.TryAdd("ENCOUNTER", typeof(OrbitInfo));
            _bindings.TryAdd("ETA", typeof(VesselEta));
        }

        private CompletionList StructureMemberComplete(CompletionParams request)
        {
            var tree = _bufferManager.GetParseTree(request.TextDocument.Uri);
            var symbol = GetSymbolUnderCursor(tree, request.Position);
            var items = new List<CompletionItem>();

            if (symbol != null && symbol != tree)
            {
                var suffixes = GetSymbolSuffixes(symbol);
                var trailer = GetSuffixTrailers(symbol);

                if (trailer.Count > 0)
                {
                    suffixes = GetTrailerSuffixes(trailer);
                }

                if (suffixes != null)
                {
                    foreach (var suffix in suffixes)
                    {
                        items.Add(GetSuffixCompletionItem(suffix.Key, suffix.Value));
                    }
                }
            }
            return new CompletionList(items);
        }

        private CompletionItem GetSuffixCompletionItem(string name, ISuffix suffix)
        {
            var type = suffix.GetType();
            var detail = "";

            try
            {
                var infoGetter = typeof(SuffixBase).GetMethod(
                    "DelegateInfo",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var info = (DelegateInfo)infoGetter.Invoke(suffix, null);
                if (info != null)
                {
                    detail += "( ";
                    foreach (var arg in info.Parameters)
                    {
                        detail += arg.ParameterType.Name + " ";
                    }
                    detail += ") -> " + info.ReturnType.Name;
                }
            }
            catch (TargetInvocationException)
            {
                detail = type.GenericTypeArguments?[0].Name;
            }

            return new CompletionItem()
            {
                Label = name,
                Detail = detail,
                Documentation = suffix.Description
            };
        }

        private IDictionary<string, ISuffix> GetTrailerSuffixes(List<ParseNode> trailer)
        {
            var suffixes = GetSymbolSuffixes(trailer[0]);
            for (int i = 1; i < trailer.Count && suffixes != null; i++)
            {
                if (suffixes.TryGetValue(trailer[i].Token.Text, out ISuffix suffix))
                {
                    var type = suffix.GetType().GenericTypeArguments?[0];
                    if (type != null)
                    {
                        if (_mocks.TryGetValue(type, out Structure mock))
                        {
                            suffixes = GetSuffixes(mock);
                        }
                        else
                        {
                            suffixes = null;
                        }
                    }
                }
            }
            return suffixes;
        }

        private IDictionary<string, ISuffix> GetSuffixes(Structure mock)
        {
            FieldInfo info = typeof(Structure).GetField(
                "instanceSuffixes",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (IDictionary<string, ISuffix>)info.GetValue(mock);
        }

        private Structure GetMockForSymbol(ParseNode node)
        {
            Structure mock = null;
            if (_bindings.TryGetValue(node.Token.Text.ToUpper(), out Type mockType))
            {
                if (_mocks.TryGetValue(mockType, out Structure obj))
                {
                    mock = obj;
                }
            }
            return mock;
        }

        private IDictionary<string, ISuffix> GetSymbolSuffixes(ParseNode symbol)
        {
            var mock = GetMockForSymbol(symbol);
            if (mock == null)
                return null;
            return GetSuffixes(mock);
        }

        private List<ParseNode> GetSuffixTrailers(ParseNode node)
        {
            var suffix = node.Parent?.Parent?.Parent?.Parent;
            var chain = new List<ParseNode>();
            if(suffix != null && suffix.Token.Type == TokenType.suffix)
            {
                foreach (var child in suffix.Nodes)
                {
                    ParseNode identifier = null;
                    try
                    {
                        if (child.Token.Type == TokenType.suffixterm)
                        {
                            identifier = child.Nodes[0]?.Nodes[0];
                        }
                        else if(child.Token.Type == TokenType.suffix_trailer)
                        {
                            identifier = child.Nodes[1]?.Nodes[0]?.Nodes[0];
                        }
                        chain.Add(identifier);
                    }
                    catch (ArgumentOutOfRangeException)
                    {}
                }
            }
            return chain;
        }

        private ParseNode GetSymbolUnderCursor(ParseNode node, Position position)
        {
            ParseNode symbol = node;
            foreach (var child in node.Nodes)
            {
                var childSymbol = GetSymbolUnderCursor(child, position);
                // match cursor line
                if(childSymbol.Token.Line == position.Line + 1
                // match identifier range
                && position.Character - 1 >= childSymbol.Token.Column
                && position.Character - 1 <= childSymbol.Token.Column + childSymbol.Token.Length
                // only value identifiers
                && childSymbol.Token.Type == TokenType.IDENTIFIER)
                {
                    symbol = childSymbol;
                }
            }
            return symbol;
        }
    }
}
