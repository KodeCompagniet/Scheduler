using System.Runtime.CompilerServices;

// As of version 2.0, all logging types have been consolidated into DemiCode.Shared.
// These type forwarders ensure binary compatibility for consumers that referenced DemiCode.Logging in 1.x.

// Core interfaces
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.ILogger))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.ILoggerFactory))]

// Concrete implementations
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.ConsoleLogger))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.ConsoleLoggerFactory))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.NilLogger))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.NilLoggerFactory))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.LoggerFactoryCollection))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.SimpleNDC))]

// Configuration types
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.Configuration.LoggerElementBase))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.Configuration.LoggerElement))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.Configuration.ConsoleLoggerElement))]
[assembly: TypeForwardedTo(typeof(DemiCode.Logging.Configuration.LoggerElementCollection<>))]
