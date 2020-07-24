# SimpleC

A compiler, written using C#, for a C-like language that attempts to be simpler and cleaner to use. It might even be a bit pretty.

Examples of SimpleC source can be found in the directory, TestFiles.sc. These will be used for testing the compiler and vary from as basic as possible to - eventually
covering all common uses.

The frontend compiler creates llvm ir which can be used with llvm to run immediately or compile to native code, this allows for use with varied architectures and
utilising builtin optimisation by llvm.
