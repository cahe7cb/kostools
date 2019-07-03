using kOS.Safe.Exceptions;
using kOS.Safe.Compilation;
using kOS.Safe.Compilation.KS;
using kOS.Safe.Persistence;

using System;
using System.Collections.Generic;

namespace kOS.Tools
{
    public class Script
    {
        private readonly Scanner scanner;
        private readonly Parser parser;

        public Script()
        {
            scanner = new Scanner();
            parser = new Parser(scanner);
        }

        public ParseErrors Compile(GlobalPath filePath, int startLineNum, string scriptText, string contextId, CompilerOptions options)
        {
            return parser.Parse(scriptText).Errors;
        }
    }
}