Imports CG.FairShare.Data
Imports CG.FairShare.Globals
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Net.Sockets

Public Class Form1
    Dim ns As RemoteNetworkStateProvider

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Threading.Thread.Sleep(5)
        ChannelServices.RegisterChannel(New TcpClientChannel, False)
        Dim h As RemoteNetworkStateProvider = Activator.GetObject(GetType(RemoteNetworkStateProvider),
                                            "tcp://localhost:4949/RemoteNetworkStateProvider")
        ns = h
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim NetState() As UsageStatistics
        Static connectionFailCounter As Integer = 0
        Try
            NetState = ns.getNetworkState
            connectionFailCounter = 0
        Catch ex As SocketException
            If ex.Message = "An existing connection was forcibly closed by the remote host" Then
                End

            ElseIf ex.Message.Contains("No connection could be made because the target machine actively refused it") Then
                connectionFailCounter += 1
                If connectionFailCounter > 5 Then End
                Exit Sub

            Else
                Throw
            End If
        End Try

        TextBox1.Text = ""
        Dim users As New ArrayList
        Dim usage As New ArrayList
        Dim used As Integer = 0
        For Each usr In NetState
            TextBox1.Text = TextBox1.Text & String.Format("{0,-15}{1,6}", usr.UserIP, usr.Consumption) & vbCrLf
            used += usr.Consumption
            users.Add(usr.UserIP)
            usage.Add(usr.Consumption)
        Next

        users.Add("Remaining")
        usage.Add(PIPEWIDTH - used)

        Chart1.Series(0).Points.DataBindXY(users.ToArray, usage.ToArray)

    End Sub

End Class
