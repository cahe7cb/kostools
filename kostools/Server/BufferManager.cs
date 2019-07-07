using System;
using System.Collections.Concurrent;
using System.Text;
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

        public void put(System.Uri uri, string text)
        {
            var buffer = _buffers.GetOrAdd(uri.ToString(), (key) => new StringBuilder());
            buffer.Clear();
            buffer.Append(text);
        }

        public string get(System.Uri uri)
        {
            var buffer = _buffers.GetOrAdd(uri.ToString(), (key) => new StringBuilder());
            return buffer.ToString();
        }

        public void patch(System.Uri uri, TextDocumentContentChangeEvent change)
        {
            if(change.Range is null)
            {
                this.put(uri, change.Text);
                return;
            }

        }
    }
}
