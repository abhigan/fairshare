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

        'letzus start here
        Dim totalBanwidth = PIPEWIDTH
        Dim usableBandwidth = PIPEWIDTH - BAREMINIMUM 'let some remain in pipe

        '---------------------------------------------------------------
        '=====================      I D L E     ========================
        '---------------------------------------------------------------
        'the IDLErs are free to consume that what is left in pipe (BAREMINIMUM)
        Dim limitOfLaziness As Integer = (9 * BAREMINIMUM) / 10 ' that is 90% of BAREMINIMUM
        If myUsageStatistics.Consumption < limitOfLaziness Then Return myUsageStatistics.Roof
        Dim idleUsers = From netstate As UsageStatistics In networkState Where netstate.Consumption < limitOfLaziness Select netstate
        Dim activeUsers = networkState.Except(idleUsers)



        '---------------------------------------------------------------
        '=================      M O D E R A T E     ====================
        '---------------------------------------------------------------
        'this usabe bandwidth should be shared among all NON-IDLE or better ACTIVE users
        Dim legalShare As Integer = usableBandwidth / activeUsers.Count  '  <<<<<<<<<<<<----------watch-out for devide by zero

        'let us now find all the--
        Dim moderateUses As New List(Of UsageStatistics)
        'out of all the NON-IDLE or in other words ACTIVE users,
        For Each user As UsageStatistics In activeUsers
            'not everyone will be consuming whole of their legal share. 
            'there will be some leftover bandwidth
            If user.Consumption < legalShare Then
                moderateUses.Add(user)
                Dim allotted As Integer = 0
                Dim leftover As Integer = legalShare - user.Consumption
                'now the downloaders can scrape-out some of this
                'but only --
                If leftover > BAREMINIMUM Then '                                       \
                    Dim contributableBandwidth As Integer = leftover - BAREMINIMUM '   |
                    'so that user will be alloted:                                     |     allotted 
                    allotted = legalShare - contributableBandwidth '                    >       =              consumption
                Else ' there is very little leftover                                   |   MIN ( legalShare,        +        )
                    'no contribution to the heavy downloaders                          |                       BAREMINIMUM
                    allotted = legalShare '                                            /
                End If
                usableBandwidth -= allotted
                If LocalStateManager.getManager.IsMyStatistcs(user) Then Return legalShare
            End If
        Next





        '---------------------------------------------------------------
        '==============      D O W N L O A D E R     ===================
        '---------------------------------------------------------------
        'finally we come for the --
        Dim heavyDownloaders = activeUsers.Except(moderateUses)
        'all of the usabeBandwidth will be distributed evenly among these heavyDownloaders
        Dim lionsShare As Integer = usableBandwidth / heavyDownloaders.Count  '  <<<<<<<<<<<<----------watch-out for devide by zero
        Console.WriteLine("heavy downloaders " & heavyDownloaders.Count)
        If (From user As UsageStatistics In heavyDownloaders Where LocalStateManager.getManager.IsMyStatistcs(user) = True).Count = 1 Then Return lionsShare

        Throw New ApplicationException("how come i am nowhere")


    End Function


End Class
