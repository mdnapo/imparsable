# Imparsable

## Motivation

Imparsable is a project to excercise my development skills by writing a parser, an LSP and a runtime for a toy language.
Over the years I've written several toy languages, but I've never implemented decent editor support. I've added syntax highlighting in Monaco before and also some custom error reporting on the client side, but it
always felt a bit hacky. So I've finally challenged myself to put my skills to the test and build a parsing toolkit with
decent LSP support.

## Stack

The backend is built using .NET 10 and the frontend is built on Angular 22.

## Parsing

The parsing infrastructure is contained in the [Imparsable.Parsing](./Imparsable.Parsing) library, which is strongly
inspired by the book [Crafting Interpreters by Bob Nystrom](https://craftinginterpreters.com/). The project
[Imparsable.Tool.Calculator](./Imparsable.Tool.Calculator) is used to demonstrate how to build and use a parser based on
the parsing library.

## Language Server Protocol

The project [Imparsable.LSP.Protocol](./Imparsable.LSP.Protocol) contains a custom LSP server implementation. At the
time of writing (august 2026) there are no well maintainted libraries that support websockets and integrate nicely with
ASP.NET Core, so the choice was made to implement a custom solution. Given that LSP is a JSON-RPC based protocol, the
implementation can be kept quite simple. The JSON-RPC connection is provided by
the [SteamJsonRpc](https://www.nuget.org/packages/StreamJsonRpc/) library, while the LSP types are pulled
from [OmniSharp.Extensions.LanguageServer.Shared](https://www.nuget.org/packages/OmniSharp.Extensions.LanguageServer.Shared).
The project [Imparsable.LSP.Server.Calculator](./Imparsable.LSP.Server.Calculator) demonstrates the usage of the
library.

## Calculator

The calculator project is meant to provide a concrete end to end example of a parser and LSP implementation. It's meant
to be simple enough to implement in a relatively short period of time, but complete enough to demonstrate decently
complex parser and compiler implementations. 

