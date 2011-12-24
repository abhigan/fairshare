Imports CG.FairShare.Globals
Imports CG.FairShare.Data
Imports System.IO

Module Module1

    Sub Main()

        Dim n As NetworkStateProvider = NetworkStateProvider.GetProvider
        n.init()

        Dim g As SelfGovernor = New SelfGovernor
        g.init()

        Dim l As LocalStateManager = LocalStateManager.getManager
        l.init()

        While True
            Threading.Thread.Sleep(1000)
            For Each stat As UsageStatistics In n.getNetworkState
                'Console.WriteLine("{2,-20}ROOF:{0,-8}USAGE:{1,-8}FREE:{4,-8}{3:hh:mm:ss}", _
                '                  Int(stat.Roof / 1024), _
                '                  Int(stat.Consumption / 1024), _
                '                  stat.User, _
                '                  stat.TimeStamp, _
                '                  Int(NetworkStateProvider.GetProvider.AvailableInNetwork / 1024))
            Next
            If Console.KeyAvailable AndAlso (Console.ReadKey).Key = ConsoleKey.Escape Then End
        End While
    End Sub


End Module
