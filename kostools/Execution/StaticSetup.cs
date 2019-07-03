using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Utilities;

namespace kOS.Tools.Execution
{
    public class StaticSetup
    {
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
        }
    }
}
