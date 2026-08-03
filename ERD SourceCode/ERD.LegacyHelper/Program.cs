using System;
using System.Diagnostics;
using System.Linq;

namespace ERD.LegacyHelper
{
    internal class Program
    {
        static int Main(string[] args)
        {
            args = CheckDebug(args);

            try
            {
                string flag = args.Length > 0 ? args[0] : string.Empty;
                args = RemoveArgsFalgs(args, flag);

                if (flag == Flags.UnzipFileBinaryFormatter)
                {
                    UnzipFileBinaryFormatter unzipper = new UnzipFileBinaryFormatter();
                    return unzipper.UnzipFile(args);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex.ToString());
                return 1;
            }
        }

        internal static string[] CheckDebug(string[] args)
        {
#if DEBUG

            if (args.Any(s => s.Equals(Flags.Debug, StringComparison.OrdinalIgnoreCase))
             || args.Any(s => s.Equals(Flags.D, StringComparison.OrdinalIgnoreCase)))
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }

                Debugger.Break();

                args = RemoveArgsFalgs(args, Flags.Debug, Flags.D);
            }
#endif

            return args;
        }

        internal static string[] RemoveArgsFalgs(string[] args, params string[] flags)
        {
            foreach (string flag in flags)
            {
                int debugIndex = Array.FindIndex(args, s => s.Equals(flag, StringComparison.OrdinalIgnoreCase));

                if (debugIndex >= 0)
                {
                    args = args.Where((s, index) => index != debugIndex).ToArray();

                    return args;
                }
            }
            
            return args;
        }
    }
}
