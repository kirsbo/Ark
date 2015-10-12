using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.IO
{
    class InputMock : ioInput
    {
        public InputMock()
        {
            List<string> fsoPaths = new List<string>();
            fsoPaths.Add(@"c:\input\test1");
            fsoPaths.Add(@"c:\input\test2");
            base.initFSOPath(fsoPaths);

        }
    }
}
