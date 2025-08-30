Imports System
Imports System.Diagnostics
Imports DemiCode.Logging
Imports DemiCode.Scheduling.Tasks

Namespace Tasks
    ''' <summary>
    ''' The ApplicationTask executes an application. 
    ''' </summary>
    Public Class ApplicationTask
        Implements ITask

        ''' <summary>
        ''' The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        ''' or the path to an executable file.
        ''' </summary>
        Public Path As String

        ''' <summary>
        ''' The strings in <see cref="Arguments"/> will be passed to the application as command-line arguments.
        ''' </summary>
        Public Arguments As String()

        ''' <summary>
        ''' Construct an <see cref="ApplicationTask"/> instance.
        ''' </summary>
        ''' <param name="pathValue">The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        ''' or the path to an executable file.</param>
        ''' <param name="argumentsValue">The strings in <paramref name="argumentsValue"/> will be passed to the application as command-line arguments.</param>
        Public Sub New(ByVal pathValue As String, ByVal argumentsValue As String)

            If String.IsNullOrEmpty(pathValue) Then
                Throw New ArgumentException("Path cannot be an empty string", "pathValue")
            End If

            If argumentsValue Is Nothing Then
                Throw New ArgumentNullException("argumentsValue")
            End If

            Path = pathValue
            Arguments = New String(){ argumentsValue }
        End Sub

        ''' <summary>
        ''' Execute the application specified in <see cref="Path"/>.
        ''' </summary>
        ''' <remarks>
        ''' The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        ''' or the path to an executable file.
        ''' The strings in <see cref="Arguments"/> will be passed to the application as command-line arguments.
        ''' <br/>
        ''' <see cref="Execute"/> will return immediately after the application have started and will not wait for the process to stop.
        ''' </remarks>
        ''' <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        Public Sub Execute(ByVal loggerFactory As ILoggerFactory) Implements ITask.Execute

            Dim logger As ILogger = loggerFactory.CreateLogger(GetType(ApplicationTask))

            Using process As Process = New Process()

                process.StartInfo.Arguments = ArgumentsToString(Arguments)
                process.StartInfo.UseShellExecute = True
                process.StartInfo.FileName = Path

                Try
                    process.Start()
                Catch e As Exception
                    logger.Error("Failed to start process {0}", e, Path)
                End Try
            End Using
        End Sub

        Private Shared Function ArgumentsToString(ByVal arguments As String()) As String
            Dim args As String = ""
            For Each argument As String In arguments

                If Not (argument.StartsWith("""") And argument.EndsWith("""")) Then
                    args += """" + argument + """ "
                Else
                    args += argument + " "
                End If
            Next
            Return args
        End Function

    End Class
End Namespace