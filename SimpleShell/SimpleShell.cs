// Assignment 4
// Pete Myers
// OIT, Spring 2018
// Handout

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleFileSystem;

namespace SimpleShell
{
    public class SimpleShell : Shell
    {
        private abstract class Cmd
        {
            private string name;
            private SimpleShell shell;

            public Cmd(string name, SimpleShell shell) { this.name = name; this.shell = shell; }

            public string Name => name;
            public SimpleShell Shell => shell;
            public Session Session => shell.session;
            public Terminal Terminal => shell.session.Terminal;
            public FileSystem FileSystem => shell.session.FileSystem;
            public SecuritySystem SecuritySystem => shell.session.SecuritySystem;

            abstract public void Execute(string[] args);
            virtual public string HelpText { get { return ""; } }
            virtual public void PrintUsage() { Terminal.WriteLine("Help not available for this command"); }
        }

        private Session session;
        private Directory cwd;
        private Dictionary<string, Cmd> cmds;   // name -> Cmd
        private bool running;

        public SimpleShell(Session session)
        {
            this.session = session;
            cwd = null;
            cmds = new Dictionary<string, Cmd>();
            running = false;

            AddCmd(new ExitCmd(this));
            AddCmd(new PrintWorkingDirectoryCmd(this));
            AddCmd(new ChangeDirectoryCmd(this));
            AddCmd(new ListDirectoryCmd(this));
        }

        private void AddCmd(Cmd c) { cmds[c.Name] = c; }

        public void Run(Terminal terminal)
        {
            // NOTE: takes over the current thread, returns only when shell exits
            // expects terminal to already be connected

            // set the initial current working directory
            cwd = session.HomeDirectory;

            // main loop...
            running = true;
            while (running)
            {
                // print command prompt
                terminal.Write(cwd.FullPathName + ">");

                // get command line
                string cmdline = terminal.ReadLine().Trim();

                // identify and execute command
                string[] args = cmdline.Split(' ');

                if (cmds.ContainsKey(args[0]))
                {
                    cmds[args[0]].Execute(args);
                }
                else
                {
                    // invalid command
                    terminal.WriteLine("Unknown command: " + args[0]);
                }
            }

        }

        #region commands

        // example command: exit
        private class ExitCmd : Cmd
        {
            public ExitCmd(SimpleShell shell) : base("exit", shell) { }

            public override void Execute(string[] args)
            {
                Terminal.WriteLine("Bye!");
                Shell.running = false;
            }

            override public string HelpText { get { return "Exits shell"; } }

            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: exit");
            }
        }

        private class PrintWorkingDirectoryCmd : Cmd
        {
            public PrintWorkingDirectoryCmd(SimpleShell shell) : base("pwd", shell) { }
            public override void Execute(string[] args)
            {
                Terminal.WriteLine(Shell.cwd.FullPathName);
            }
            override public string HelpText { get { return "Prints the current working directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: pwd");
            }
        }

        private class ChangeDirectoryCmd : Cmd
        {
            public ChangeDirectoryCmd(SimpleShell shell) : base("cd", shell) { }
            public override void Execute(string[] args)
            {
                try
                {
                    if (args.Length != 2)
                    {
                        PrintUsage();
                    }

                    string path = args[1];
                    // TODO: qualitfy partial paths
                    // TODO: what if path is a file?
                    FSEntry entry = FileSystem.Find(path);
                    if (entry == null)
                    {
                        throw new Exception("Directory not found: " + args[1]);
                    }
                        
                    if (entry.IsFile)
                    {
                        throw new Exception("Path must be a directory: " + args[1]);
                    }
                    Shell.cwd = entry as Directory;
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                }
            }
            override public string HelpText { get { return "Changes the current working directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: cd <directory>");
            }
        }

        private class ListDirectoryCmd : Cmd
        {
            public ListDirectoryCmd(SimpleShell shell) : base("ls", shell) { }
            public override void Execute(string[] args)
            {
                Directory dir;
                
                if (args.Length == 1)
                {
                    dir = FileSystem.Find(Shell.cwd.FullPathName) as Directory;
                }
                else
                {
                    dir = FileSystem.Find(args[1]) as Directory;
                }

                if (dir == null)
                {
                    Terminal.WriteLine("Directory not found: " + args[1]);
                }
                else
                {
                    // flatten files and subdirectories into a single list
                    IEnumerable<FSEntry> entries = (dir.GetSubDirectories() as IEnumerable<FSEntry>).Concat(dir.GetFiles());

                    foreach (FSEntry entry in entries)
                    {
                        Terminal.WriteLine(entry.Name);
                    }
                }
            }
            override public string HelpText { get { return "Lists the contents of a directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: ls <directory>");
            }
        }
        #endregion
    }
}
