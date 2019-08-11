using System;
using System.Collections.Concurrent;
using System.Text;
using kOS.Safe.Compilation.KS;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace kOS.Tools.Server
{
    public class BufferManager
    {
        private readonly ConcurrentDictionary<string, StringBuilder> _buffers = new ConcurrentDictionary<string, StringBuilder>();

        public BufferManager()
        {
            Console.Error.WriteLine("BufferManager: " + this.GetHashCode());
        }

        public void Put(System.Uri uri, string text)
        {
            var buffer = _buffers.GetOrAdd(uri.ToString(), (key) => new StringBuilder());
            buffer.Clear();
            buffer.Append(text);
        }

        public string Get(System.Uri uri)
        {
            var buffer = _buffers.GetOrAdd(uri.ToString(), (key) => new StringBuilder());
            return buffer.ToString();
        }

        public void Patch(System.Uri uri, TextDocumentContentChangeEvent change)
        {
            if(change.Range is null)
            {
                // non-incremental/full buffer synchronization
                this.Put(uri, change.Text);
                return;
            }

            var buffer = _buffers.GetOrAdd(uri.ToString(), (key) => new StringBuilder());
            var text = buffer.ToString();
            int start = BufferPosition(text, change.Range.Start);

            if(start != -1)
            {
                buffer.Remove(start, change.RangeLength);
                buffer.Insert(start, change.Text);
            }
        }

        public ParseTree GetParseTree(System.Uri uri)
        {
            var scanner = new Scanner();
            var parser = new Parser(scanner);
            return parser.Parse(Get(uri), uri.ToString());
        }

        protected int BufferPosition(string text, Position position)
        {
            int pos = 0;

            for (int i = 0; i < position.Line; i++)
            {
                var p = pos;
                if ((p = text.IndexOf('\n', p)) != -1)
                    pos = p + 1;
            }

            if (pos != -1)
                pos += (int)position.Character;

            if (!(pos <= text.Length))
                pos = -1;

            return pos;
        }
    }
}
