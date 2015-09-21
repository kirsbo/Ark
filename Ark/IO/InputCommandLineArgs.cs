using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.IO
{
    public class InputCommandLineArgs : ioInput
    {
        public CommandlineDropDestinationEnum CommandLineDropDestination;

        public enum CommandlineDropDestinationEnum
        {
            ExistingFolder,
            NewFolder
        }
        public InputCommandLineArgs(string[] commandLineArgs)
        {
            List<string> fsoPaths = commandLineArgs.Skip(1).ToList(); //## Might need removing in live since the skip is most likely done due to vshost.exe adding itself as parameter
            base.initFSOPath(fsoPaths);
        }
    }
}
