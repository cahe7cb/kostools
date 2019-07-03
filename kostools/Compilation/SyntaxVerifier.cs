using System;
using System.Collections.Generic;
using System.IO;
using kOS.Safe;
using kOS.Safe.Compilation;
using kOS.Safe.Compilation.KS;
using kOS.Safe.Encapsulation;
using kOS.Safe.Exceptions;
using kOS.Safe.Execution;
using kOS.Safe.Function;
using kOS.Safe.Persistence;
using kOS.Safe.Utilities;
using kOS.Tools.Execution;

namespace kOS.Tools
{
    public class SyntaxVerifier
    {
        private ICpu cpu;
        private SafeSharedObjects shared;
        private Execution.Screen screen;
        private string baseDir;

        private string FindKerboscriptTests()
        {
            var currentDir = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(currentDir, "kerboscript_tests")))
            {
                currentDir = Directory.GetParent(currentDir).FullName;
            }

            return Path.Combine(currentDir, "kerboscript_tests");
        }

        public void Setup()
        {
            SafeHouse.Init(new Config(), new VersionInfo(0, 0, 0, 0), "", false, "");
            SafeHouse.Logger = new NoopLogger();

            try
            {
                AssemblyWalkAttribute.Walk();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                throw;
            }

            baseDir = Directory.GetCurrentDirectory();

            screen = new Execution.Screen();

            shared = new SafeSharedObjects();
            shared.FunctionManager = new FunctionManager(shared);
            shared.GameEventDispatchManager = new NoopGameEventDispatchManager();
            shared.Processor = new NoopProcessor();
            shared.ScriptHandler = new KSScript();
            shared.Screen = screen;
            shared.UpdateHandler = new UpdateHandler();
            shared.VolumeMgr = new VolumeManager();
            shared.Logger = new TerminalLogger();

            shared.FunctionManager.Load();

            Archive archive = new Archive(baseDir);
            shared.VolumeMgr.Add(archive);
            shared.VolumeMgr.SwitchTo(archive);

            cpu = new CPU(shared);
        }

        public void RunScript(string fileName)
        {
            string contents = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));
            GlobalPath path = shared.VolumeMgr.GlobalPathFromObject("0:/" + fileName);

            var compiled = shared.ScriptHandler.Compile(path, 1, contents, "test", new CompilerOptions()
            {
                LoadProgramsInSameAddressSpace = true,
                IsCalledFromRun = false,
                FuncManager = shared.FunctionManager
            });
            cpu.Boot();

            screen.ClearOutput();

            cpu.GetCurrentContext().AddParts(compiled);

            foreach (var part in compiled)
            {
                Console.WriteLine(part.ToString() + ":" + part.GetHashCode());
                hugeDump(part.MergeSections());
            }
        }

        protected void RunSingleStep()
        {
            shared.UpdateHandler.UpdateFixedObservers(0.01);

            if (cpu.InstructionsThisUpdate == SafeHouse.Config.InstructionsPerUpdate)
            {
                throw new Exception("Script did not finish");
            }
        }

        protected void AssertOutput(params string[] expected)
        {
            screen.AssertOutput(expected);
        }

        void hugeDump(List<Opcode> part)
        {
            foreach (var op in part)
            {
                string dump = op.ToString();
                dump += " " + op.SourceLine;
                dump += ":" + op.SourceColumn;
                //dump += " " + op.SourcePath;
                Console.WriteLine("\t" + dump);
            }
        }
    }
}
