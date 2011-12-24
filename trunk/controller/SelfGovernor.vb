Imports CG.FairShare.Globals
Imports CG.FairShare.Data
Imports System.ComponentModel

Class SelfGovernor

    WithEvents governor As BackgroundWorker

    Sub init()
        governor = New BackgroundWorker
        governor.RunWorkerAsync()
    End Sub

    Private Sub governor_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles governor.DoWork
        Dim netStateProv As NetworkStateProvider = NetworkStateProvider.GetProvider

        While True
            Dim expectedRoof As Integer
            Dim myUsageStatistics = LocalStateManager.getManager.Usage
            expectedRoof = getExpectedRoof(netStateProv.getNetworkState, myUsageStatistics)

            'self correcting routing when my roof is illigally large
            If myUsageStatistics.Roof > PIPEWIDTH - BAREMINIMUM Then
                expectedRoof = PIPEWIDTH - BAREMINIMUM
            End If

            'resist small changes
            If Math.Abs(expectedRoof - myUsageStatistics.Roof) / 1024 > 1 Then
                LocalStateManager.getManager.SetRoof(expectedRoof)
            End If

            Threading.Thread.Sleep(POLLING_INTERVAL)
        End While
    End Sub

    Function getExpectedRoof(ByVal networkState As UsageStatistics(), ByVal myUsageStatistics As UsageStatistics) As Integer
        Dim netStateProv As NetworkStateProvider = NetworkStateProvider.GetProvider
        Dim myLegalShare As Integer = netStateProv.getMyLegalSshare

        Dim availableInNetwork As Integer = netStateProv.AvailableInNetwork
        Dim currentStat As UsageStatistics = myUsageStatistics
        Dim percentageUsage As Integer = (currentStat.Consumption * 100) / currentStat.Roof

        Console.WriteLine(vbCrLf & "roof:{0,-8} legal:{3,-8} usg:{1,-8} available:{2,-8} pipe:{4,-8}",
                         Int(currentStat.Roof / 1024), Int(currentStat.Consumption / 1024), Int(availableInNetwork / 1024), Int(myLegalShare / 1024), Int(PIPEWIDTH / 1024))

        Return myUsageStatistics.Roof ' same as that was before

    End Function


End Class
