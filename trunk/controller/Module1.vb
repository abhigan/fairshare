Imports CG.FairShare.Globals
Imports CG.FairShare.Data
Imports System.IO

Module Module1
    Sub Main()
        Dim listener = New TextWriterTraceListener("trace.log")
        Trace.AutoFlush = True
        Trace.Listeners.Add(listener)

        Dim n As NetworkStateProvider = NetworkStateProvider.GetProvider
        n.init(True, True)

        Dim g As SelfGovernor = New SelfGovernor
        g.init()

        Dim l As LocalStateManager = LocalStateManager.getManager
        l.init()

        While True
            Threading.Thread.Sleep(POLLING_INTERVAL)
            For Each stat As UsageStatistics In n.getNetworkState
                Console.WriteLine("USER:{2,-20} ROOF:{0,-8} USAGE:{1,-8} {3,hh:mm:ss}", _
                                  Int(stat.Roof / 1024), _
                                  Int(stat.Consumption / 1024), _
                                  stat.UserIP, _
                                  stat.TimeStamp)
            Next
            Console.WriteLine(n.getNetworkState.Count & "----------------------------")
            If Console.KeyAvailable AndAlso (Console.ReadKey).Key = ConsoleKey.Escape Then End
        End While
    End Sub


End Module
