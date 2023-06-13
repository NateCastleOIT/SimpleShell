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
            protected string MakeFullPath(string path)
            {
                if (path.StartsWith("/"))
                {
                    return path;
                }

                // modify the cwd using the path
                List<string> cwdParts = new List<string>(Shell.cwd.FullPathName.Split('/'));
                if (cwdParts.Count == 2 && cwdParts[1] == "")
                {
                    cwdParts.RemoveAt(1);
                }

                string[] pathParts = path.Split('/');


                try
                {
                    // add cwd parts
                    foreach (string part in pathParts)
                    {
                        if (part == ".")
                        {
                            continue;
                        }
                        else if (part == "..")
                        {
                            if (cwdParts.Count == 1)
                            {
                                throw new Exception("No parent directory!");
                            }
                            cwdParts.RemoveAt(cwdParts.Count - 1);
                        }
                        else
                        {
                            // descend
                            cwdParts.Add(part);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }

                if (cwdParts.Count == 0)
                {
                    return "/";
                }
                else if (cwdParts.Count == 1)
                {
                    return "/" + cwdParts[0];
                }
                else
                {
                    return string.Join("/", cwdParts);
                }
            }
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
            AddCmd(new HelpCmd(this));
            AddCmd(new MakeDirectoryCmd(this));
            AddCmd(new RemoveDirectoryCmd(this));
            AddCmd(new HeadCmd(this));
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
                        throw new Exception("Expect only 1 argument!");
                    }

                    string path = args[1];
                    
                    path = MakeFullPath(path);

                    // find the directory
                    FSEntry entry = FileSystem.Find(path);


                    // throw error if entry not found
                    if (entry == null)
                    {
                        throw new Exception("Directory not found: " + args[1]);
                    }
                    
                    // throw error if entry is not a directory
                    if (entry.IsFile)
                    {
                        throw new Exception("Path must be a directory: " + args[1]);
                    }

                    // change cwd
                    Shell.cwd = entry as Directory;
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
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
                
                try
                {
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
                        //IEnumerable<FSEntry> entries = (dir.GetSubDirectories() as IEnumerable<FSEntry>).Concat(dir.GetFiles());

                        //foreach (FSEntry entry in entries)
                        //{
                        //    Terminal.WriteLine(entry.Name);
                        //}

                        Terminal.WriteLine("");
                        foreach (Directory d in dir.GetSubDirectories())
                        {
                            Terminal.WriteLine('\t' + "/" + d.Name + "/");
                        }
                        Terminal.WriteLine("");
                        foreach (File f in dir.GetFiles())
                        {
                            Terminal.WriteLine('\t' + f.Name);
                        }
                        Terminal.WriteLine("");
                    }
                }
                catch(Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }
            }
            override public string HelpText { get { return "Lists the contents of a directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: ls <directory>");
            }
        }

        private class HelpCmd : Cmd
        {
            public HelpCmd(SimpleShell shell) : base("help", shell) { }
            public override void Execute(string[] args)
            {
                try
                {
                    // print help text for all commands
                    if (args.Length == 1)
                    {
                        // print all the commands
                        foreach (Cmd cmd in Shell.cmds.Values)
                        {
                            Terminal.WriteLine(cmd.Name + " - " + cmd.HelpText);
                        }
                    }
                    // print help text for a specific command
                    else if (args.Length == 2)
                    {
                        string cmdName = args[1];
                        if (Shell.cmds.ContainsKey(cmdName))
                        {
                            Terminal.WriteLine(Shell.cmds[cmdName].Name + " - " + Shell.cmds[cmdName].HelpText);
                            Terminal.WriteLine(Shell.cmds[cmdName].HelpText);
                        }
                        else
                        {
                            Terminal.WriteLine("Unknown command: " + cmdName);
                        }
                    }
                    // invalid number of arguments
                    else
                    {
                        throw new Exception("Expect only 1 argument!");
                    }
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }
            }
            override public string HelpText { get { return "Prints a list of the available shell commands."; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: help <cmd name>");
            }
        }

        private class MakeDirectoryCmd : Cmd
        {
            public MakeDirectoryCmd(SimpleShell shell) : base("mkdir", shell) { }
            public override void Execute(string[] args)
            {
                try
                {
                    if (args.Length != 2)
                    {
                        throw new Exception("Expect only 1 argument!");
                    }
                    string path = args[1];

                    // Can't use this because the FS throws an exception. FS expects relative path.
                    //path = MakeFullPath(path);

                    // find the directory
                    FSEntry entry = FileSystem.Find(path);

                    // throw error if entry not found
                    if (entry != null)
                    {
                        throw new Exception("Directory already exists: " + args[1]);
                    }

                    // create the directory
                    Directory dir = Shell.cwd.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }
            }
            override public string HelpText { get { return "Creates a new directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: mkdir <directory>");
            }
        }

        private class RemoveDirectoryCmd : Cmd
        {
            public RemoveDirectoryCmd(SimpleShell shell) : base("rmdir", shell) { }
            public override void Execute(string[] args)
            {
                // I am pretty sure this will leave floating data if the directory is not empty.
                // Seems to be fine for now. I delved into the session FS and the FS (this) immediately after the delete and it looks fine.
                try
                {
                    if (args.Length != 2)
                    {
                        throw new Exception("Expect only 1 argument!");
                    }
                    string path = args[1];

                    // get full path
                    path = MakeFullPath(path);

                    // find the directory
                    FSEntry entry = FileSystem.Find(path);
                    // throw error if entry not found
                    if (entry == null)
                    {
                        throw new Exception("Directory not found: " + args[1]);
                    }
                    // throw error if entry is not a directory
                    if (entry.IsFile)
                    {
                        throw new Exception("Path must be a directory: " + args[1]);
                    }

                    // remove the directory
                    entry.Delete();
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }
            }
            override public string HelpText { get { return "Removes a directory"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: rmdir <directory>");
            }
        }

        private class HeadCmd : Cmd
        {
            public HeadCmd(SimpleShell shell) : base("head", shell) { }
            public override void Execute(string[] args)
            {
                try
                {
                    if (args.Length == 1)
                    {
                        throw new Exception("Expect at least 1 argument!");
                    }
                    string path = args[1];
                    int readTo = 10;

                    if (args.Length == 3)
                    {
                        readTo = int.Parse(args[2]);
                    }

                    path = MakeFullPath(path);

                    // find the file
                    File file = FileSystem.Find(path) as File;

                    // throw error if entry not found
                    if (file == null)
                    {
                        throw new Exception("File not found: " + args[1]);
                    }
                    
                    FileStream fs = file.Open();

                    // print the first x chars
                    byte[] readBytes = fs.Read(0, readTo);
                    string readString = Encoding.ASCII.GetString(readBytes);
                    Terminal.WriteLine(readString);

                    fs.Close();
                }
                catch (Exception ex)
                {
                    Terminal.WriteLine("Error: " + ex.Message);
                    PrintUsage();
                }
            }
            override public string HelpText { get { return "Prints the first 10 lines of a file"; } }
            override public void PrintUsage()
            {
                Terminal.WriteLine("usage: head <file>");
            }
        }


        #endregion
    }
}
