using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeovimTerminalConsoleHarness
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var terminalTst = new NeovimTerminal.TerminalTst();
            for (var i = 0; i < 10000; i++)
            {
                terminalTst.PutText("a");
                terminalTst.SwapBuffers();
            }

            //Console.Read();
        }
    }
}
