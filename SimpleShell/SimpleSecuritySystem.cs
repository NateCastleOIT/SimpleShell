// Assignment 4
// Pete Myers
// OIT, Spring 2018
// Handout

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using SimpleFileSystem;


namespace SimpleShell
{
    public class SimpleSecurity : SecuritySystem
    {
        private class User
        {
            public int userID;
            public string userName;
            public string password;
            public string homeDirectory;
            public string shell;
        }

        private int nextUserID;
        private Dictionary<int, User> usersById;        // userID -> User

        private FileSystem filesystem;
        private string passwordFileName;
        
        public SimpleSecurity()
        {
            nextUserID = 1;
            usersById = new Dictionary<int, User>();
        }

        public SimpleSecurity(FileSystem filesystem, string passwordFileName)
        {
            nextUserID = 1;
            usersById = new Dictionary<int, User>();
            this.filesystem = filesystem;
            this.passwordFileName = passwordFileName;

            LoadPasswordFile();
        }

        private void LoadPasswordFile()
        {
            // Read all users from the password file
            // userID;username;password;homedir;shell
            // TODO
        }

        private void SavePasswordFile()
        {
            // Save all users to the password file
            // userID;username;password;homedir;shell
            // TODO
        }

        private User UserByName(string username)
        {
            return usersById.Values.FirstOrDefault(u => u.userName == username);
        }

        public int AddUser(string username)
        {
            // create a new user with default home directory and shell
            // initially empty password
            // create user's home directory if needed
            // return user id
            // save the user to the password file
            // TODO

            return 0;
        }

        public int UserID(string username)
        {
            // lookup user by username and return user id
            // TODO
            return 0;
        }

        public bool NeedsPassword(string username)
        {
            // return true if user needs a password set
            // TODO
            return false;
        }

        public void SetPassword(string username, string password)
        {
            // set user's password
            // validate it meets any rules
            // save it to the password file
            // TODO
        }

        public int Authenticate(string username, string password)
        {
            // authenticate user by username/password
            // return user id
            // TODO

            return 0;
        }

        public string UserName(int userID)
        {
            // lookup user by user id and return username
            // TODO
            return null;
        }

        public string UserHomeDirectory(int userID)
        {
            // lookup user by user id and return home directory
            // TODO

            return null;
        }

        public string UserPreferredShell(int userID)
        {
            // lookup user by user id and return shell name
            // TODO

            return null;
        }
    }
}
