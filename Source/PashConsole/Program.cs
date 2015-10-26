// Copyright (C) Pash Contributors. License: GPL/BSD. See https://github.com/Pash-Project/Pash/
using System;
using System.Text;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace Pash
{
    // TODO: fix all the assembly attributes
    internal class Program
    {
        static int Main(string[] args)
        {
            var interactive = true; // interactive by default
            var file = false; // interactive by default
            StringBuilder commands = new StringBuilder();
	    string cmd;
            int startCommandAt = 0;

            // check first arg for "-noexit"
            if (args.Length > 0)
            {
                // no interactive shell if we have commands given but not this parameter
                interactive = args[0].Equals("-noexit");
                file = args[0].Equals("-f");
                if (interactive)
                {
                    // ignore first arg as it is no command
                    startCommandAt = 1;
                }
                if (file)
                {
                    interactive = false;
                }

            }

            // other args are interpreted as commands to be executed
            if(!file){
	    	for (int i = startCommandAt; i < args.Length; i++)
            	{	
                	commands.Append(args[i]);
	                commands.Append("; ");
            	}
            	FullHost p = new FullHost(interactive);
	        return p.Run(commands.ToString());
	    } else {
		cmd = System.IO.File.ReadAllText(args[1]);
            	FullHost p = new FullHost(interactive);
            	return p.Run(cmd);
    	    }

        }
    }
}
