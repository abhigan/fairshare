Imports System.ComponentModel
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports CG.FairShare.Data
Imports System.Net.Sockets
Imports System.Runtime.Remoting

Public Class ConnSettingUpDialogue

    Dim targetMachine As String = "localhost"
    Dim WithEvents channelSetup As BackgroundWorker
    Dim caller As Form1 = Nothing

    Private Sub ConnSettingUpDialogue_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        channelSetup = New BackgroundWorker
        channelSetup.WorkerReportsProgress = True
        Me.Button1.Text = "Start connecting"
        channelSetup.RunWorkerAsync(Form1)
    End Sub

    Private Sub channelSetup_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles channelSetup.ProgressChanged
        Me.Button1.Text = e.UserState.ToString
    End Sub

    Private Sub channelSetup_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles channelSetup.RunWorkerCompleted
        Form1.ns = e.Result
        Form1.Timer1.Enabled = True
        Me.Button1.Text = "done..."
        Me.Close()
    End Sub

    Private Sub channelSetup_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles channelSetup.DoWork
        Try
            caller.ns.Shutdown()
        Catch
        End Try
        Dim bw As BackgroundWorker = sender
        Try
            bw.ReportProgress(25, "Connecting to " & Me.targetMachine & "...")
            Dim h As RemoteNetworkStateProvider = Activator.GetObject(GetType(RemoteNetworkStateProvider),
                                                "tcp://" & Me.targetMachine & ":4949/RemoteNetworkStateProvider")
            Dim n = h.getNetworkState()
            'it is working
            e.Result = h

        Catch ex As SocketException
            bw.ReportProgress(50, "No server at " & Me.targetMachine & vbCrLf & "Installing receiver...")
            Dim h As NetworkStateProvider
            h = NetworkStateProvider.GetProvider
            h.init(False, True)
            h.getNetworkState()
            'it is working
            e.Result = h
        End Try
    End Sub

    Shadows Sub ShowDialog(ByVal sender As Form, ByVal intendedServer As String)
        Me.targetMachine = intendedServer
        Me.caller = sender
        MyBase.ShowDialog()
    End Sub
End Class