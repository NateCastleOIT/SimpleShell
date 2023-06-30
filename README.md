# SimpleShell Project

## Overview

The SimpleShell project is a basic shell implementation in C#. It provides a command-line interface for interacting with a file system. The project is divided into several classes, each with its own responsibilities. The main classes are `SimpleShell`, `SimpleSecurity`, and `SimpleSessionManager`.

## SimpleShell

The `SimpleShell` class is the core of the shell implementation. It provides a command-line interface for the user to interact with the file system. It supports a variety of commands such as `exit`, `pwd`, `cd`, `ls`, `help`, `mkdir`, `rmdir`, `head`, `tail`, `wc`, and `mv`.

Each command is implemented as a nested class within `SimpleShell` that extends the abstract `Cmd` class. The `Cmd` class provides a common interface for all commands, including methods for executing the command and displaying help text.

The `SimpleShell` class also maintains the current working directory and a dictionary of commands for quick lookup.

## SimpleSecurity

The `SimpleSecurity` class is an implementation of a security system. It manages user accounts, including user IDs, usernames, passwords, home directories, and preferred shells. It provides methods for adding users, setting passwords, and authenticating users.

User information is stored in a password file in the file system. The `SimpleSecurity` class provides methods for loading and saving this password file.

## SimpleSessionManager

The `SimpleSessionManager` class manages user sessions. It provides a method for creating a new session, which involves prompting the user to log in and authenticating their credentials. If the user successfully logs in, a new `Session` object is created and returned.

The `Session` class represents a user session. It provides access to the user's terminal, shell, home directory, file system, and security system. It also provides methods for running the shell and logging out.

## Usage

To use the SimpleShell, you first need to create an instance of `SimpleSessionManager`, passing in the necessary dependencies. You can then call the `NewSession` method to create a new user session. Once you have a session, you can call the `Run` method to start the shell.

Here's an example:

```csharp
var security = new SimpleSecurity();
var filesystem = new SimpleFileSystem();
var shells = new ShellFactory();
var terminal = new Terminal();

var sessionManager = new SimpleSessionManager(security, filesystem, shells);
var session = sessionManager.NewSession(terminal);

session.Run();
```

This will start the shell and allow the user to interact with the file system.
