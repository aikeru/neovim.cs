# neovim.cs

This is a fork of [Pireax/neovim.cs](https://github.com/Pireax/neovim.cs).

### Why the fork?

This fork fixes the build issue and the memory leak, although performance is pretty painful. This project is in need of a good terminal to render the vims on. I hope to get the changes merged back into Pireax's repo or possibly use something other than .NET Bitmaps which are leaky.

### Original Description

A C# client for talking to [Neovim.](https://github.com/neovim/neovim)
This also includes a WPF terminal (wip)

To use the client simply create a new NeovimClient instance with the path to the neovim executable as argument.
This creates a new neovim process and takes over it's Input and Output.
You can then use all function bindings from the NeovimClient class instance to communicate with neovim.
The Notifications neovim sends are currently in [MessagePackObject](https://github.com/msgpack/msgpack-cli/wiki/Messagepackobject) format from Messagepack-CLI.
