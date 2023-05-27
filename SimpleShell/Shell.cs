// Assignment 4
// Pete Myers
// OIT, Spring 2018
// Handout

using System;

namespace SimpleShell
{
    public interface Shell
    {
        void Run(Terminal terminal);
    }

    public interface ShellFactory
    {
        Shell CreateShell(string name, Session session);
    }
}
