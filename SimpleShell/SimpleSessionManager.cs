// Assignment 4
// Pete Myers
// OIT, Spring 2018
// Handout

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFileSystem;

namespace SimpleShell
{
    public class SimpleSessionManager : SessionManager
    {
        private class SimpleSession : Session
        {
            private int userID;
            private SecuritySystem security;
            private FileSystem filesystem;
            private ShellFactory shells;
            private Directory homeDir;
            private Shell shell;
            private Terminal terminal;

            public SimpleSession(SecuritySystem security, FileSystem filesystem, ShellFactory shells, Terminal terminal, int userID)
            {
                this.security = security;
                this.filesystem = filesystem;
                this.shells = shells;
                this.terminal = terminal;
                this.userID = userID;

                // get user's home directory
                homeDir = filesystem.Find(security.UserHomeDirectory(userID)) as Directory;

                // identify user's shell
                shell = shells.CreateShell(security.UserPreferredShell(userID), this);

            }

            public int UserID => userID;
            public string Username => security.UserName(userID);
            public Terminal Terminal => terminal;
            public Shell Shell => shell;
            public Directory HomeDirectory => homeDir;
            public FileSystem FileSystem => filesystem;
            public SecuritySystem SecuritySystem => security;

            public void Run()
            {
                shell.Run(terminal);
            }

            public void Logout()
            {
                // TODO
            }
        }

        private int MAX_TRIES = 3;
        private SecuritySystem security;
        private FileSystem filesystem;
        private ShellFactory shells;

        public SimpleSessionManager(SecuritySystem security, FileSystem filesystem, ShellFactory shells)
        {
            this.security = security;
            this.filesystem = filesystem;
            this.shells = shells;
        }

        public Session NewSession(Terminal terminal)
        {
            // ask the user to login
            // give them 3 tries
            int tries = 0;
            while (tries < MAX_TRIES)
            {
                try
                {

                    // prompt for user name
                    terminal.Echo = true;
                    terminal.Write("username: ");
                    string username = terminal.ReadLine();

                    // determine if the user needs to set their password
                    if (security.NeedsPassword(username))
                    {
                        // prompt for new password
                        terminal.Write("new password: ");
                        terminal.Echo = false;
                        string password = terminal.ReadLine();
                        terminal.Echo = true;
                        terminal.WriteLine("");


                        // save the new password
                        security.SetPassword(username, password);

                        // return them to the login prompt to login with their new password
                    }
                    else
                    {
                        // prompt for new password
                        terminal.Write("password: ");
                        terminal.Echo = false;
                        string password = terminal.ReadLine();
                        terminal.Echo = true;
                        terminal.WriteLine("");

                        // authenticate user
                        int userID = security.Authenticate(username, password);

                        // create a new session and return it
                        return new SimpleSession(security, filesystem, shells, terminal, userID);
                    }
                }
                catch(Exception e)
                {
                    // use up a try
                    tries++;

                    // report the error
                    terminal.WriteLine(e.Message);

                    // give them another chance
                    if (tries < MAX_TRIES)
                    {
                        terminal.WriteLine("Try again...");
                    }
                    // too many tries, give up
                    else
                    {
                        terminal.WriteLine("Too many failed attempts, goodbye!");
                    }
                }
            }

            // user failed authentication too many times
            
            return null;
        }
    }
}
