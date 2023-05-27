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
            // validate username
            if (UserByName(username) != null)
                throw new Exception("User already exists");

            // create a new user with default home directory and shell

            User u = new User();
            int userId = nextUserID++;

            u.userID = userId;
            u.userName = username;
            u.password = "";
            u.homeDirectory = "/users/" + username;
            u.shell = "pshell";
            usersById[userId] = u;

            if (filesystem != null)
            {
                // TODO: create user's home directory
            }

            // save the user to the password file

            // return user id
            return nextUserID;
        }

        public int UserID(string username)
        {
            // lookup user by username and return user id
            // validate username exists
            User u = UserByName(username) ?? throw new Exception("User doesn't exist by that username!");

            return u.userID;
        }

        public bool NeedsPassword(string username)
        { 
            // validate username exists
            User u = UserByName(username) ?? throw new Exception("User doesn't exist by that username!");


            return string.IsNullOrEmpty(u.password);
        }

        public void SetPassword(string username, string password)
        {
            // validate username exists
            User u = UserByName(username) ?? throw new Exception("User doesn't exist by that username!");


            // validate password meets rules
            if (string.IsNullOrWhiteSpace(password) || password.Length < 3)
                throw new Exception("Password must be at least 3 characters long!");

            u.password = password;

            //TODO: save it to the password file
        }

        public int Authenticate(string username, string password)
        {
            // authenticate user by username/password
            // return user id or throw an Exception if failed

            User u = UserByName(username) ?? throw new Exception("User doesn't exist by that username!");

            if (u.password != password)
                throw new Exception("Invalid password for user!");

            return u.userID;
        }

        public string UserName(int userID)
        {
            // lookup user by user id and return username
            // validate userID exists
            User u = usersById.ContainsKey(userID) ? usersById[userID] : throw new Exception("User doesn't exist by that ID!");

            return u.userName;
        }

        public string UserHomeDirectory(int userID)
        {
            // lookup user by user id and return home directory
            // TODO
            // validate userID exists
            User u = usersById.ContainsKey(userID) ? usersById[userID] : throw new Exception("User doesn't exist by that ID!");

            return u.homeDirectory;
        }

        public string UserPreferredShell(int userID)
        {
            // lookup user by user id and return shell name
            // validate userID exists
            User u = usersById.ContainsKey(userID) ? usersById[userID] : throw new Exception("User doesn't exist by that ID!");

            return u.shell;
        }
    }
}
