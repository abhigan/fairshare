Imports CG.FairShare.Globals
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary

Class Receiver

    Private WithEvents listner As New BackgroundWorker
    Public Event Received(ByVal RemoteStatistics As UsageStatistics)

    Public Sub StartReceiving()
        listner.RunWorkerAsync()
    End Sub

    Private Sub listner_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles listner.DoWork
        Dim endPoint As New IPEndPoint(IPAddress.Any, LISTNER_PORT)
        Dim udpListner As New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        udpListner.Bind(endPoint)

        While True
            Dim rcvData(100) As Byte
            Dim remoteStat As New UsageStatistics
            Dim remoteHost As New IPEndPoint(IPAddress.Any, 0)
            Dim l = udpListner.ReceiveFrom(rcvData, remoteHost)
            Dim msg As String = System.Text.Encoding.ASCII.GetString(rcvData, 0, l)
            Dim msg2() = msg.Split("#")
            remoteStat.Roof = msg2(0)
            remoteStat.Consumption = msg2(1)
            remoteStat.TimeStamp = Now
            remoteStat.User = remoteHost.Address.ToString

            RaiseEvent Received(remoteStat)
        End While
    End Sub
End Class
