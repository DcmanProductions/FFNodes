# Design

This will outline the projects code structure

## Table of Contents

1.

## Namespaces

| Namespace | Description                                                                            | Type                |
| --------- | -------------------------------------------------------------------------------------- | ------------------- |
| Core      | `House's the shared information that both client and server will need`                 | **Library**         |
| Server    | `Handles all the server specific functions`                                            | **Library**         |
| Rest      | `The rest server that the clients will communicate with`                               | **ASP.NET Kestrel** |
| Web       | `A pretty interface for the layman to interact with`                                   | **ASP.NET Kestrel** |
| Client    | `The client node will sit on the users computer and do the work expected from it`      | **Console**         |
| Installer | `The installer will, you guessed it, install the program and provide any needed setup` | **Console**         |

## Class Types

| Type        | Description                                                | Base               |
| ----------- | ---------------------------------------------------------- | ------------------ |
| Models      | `Models are objects that hold information to be used`      | **STRUCT**         |
| Collections | `Collections create, hold, and manipulate lists of models` | **ABSTRACT CLASS** |
| Utilities   | `Utility classes are a series of useful functions`         | **STATIC CLASS**   |
