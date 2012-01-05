Imports CG.FairShare.Globals
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary

Class Receiver

    Private WithEvents listner As BackgroundWorker
    Public Event Received(ByVal RemoteStatistics As UsageStatistics)

    Public Sub StartReceiving()
        listner = New BackgroundWorker
        listner.WorkerSupportsCancellation = True
        listner.RunWorkerAsync()
    End Sub

    Public Sub stopReceiving()
        listner.CancelAsync()
    End Sub

    Private Sub listner_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles listner.DoWork
        Dim udpListner As Socket
        Dim endPoint As New IPEndPoint(IPAddress.Any, LISTNER_PORT)
        udpListner = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        udpListner.Bind(endPoint)

        While True
            Try
                Dim rcvData(100) As Byte
                Dim remoteStat As New UsageStatistics
                Dim remoteHost As New IPEndPoint(IPAddress.Any, 0)
                Dim l = udpListner.ReceiveFrom(rcvData, remoteHost)
                Dim msg As String = System.Text.Encoding.ASCII.GetString(rcvData, 0, l)
                Dim msg2() = msg.Split("#")
                remoteStat.Roof = msg2(0)
                remoteStat.Consumption = msg2(1)
                remoteStat.UserSignature = Guid.Parse(msg2(2))
                remoteStat.TimeStamp = Now
                remoteStat.UserIP = remoteHost.Address.ToString
                RaiseEvent Received(remoteStat)

                If CType(sender, BackgroundWorker).CancellationPending Then
                    'udpListner.Disconnect(True)
                    udpListner.Dispose()
                    Exit While
                End If

            Catch ex As Exception
                Trace.WriteLine(ex.ToString)
            End Try
            Threading.Thread.Sleep(100)
        End While
    End Sub
End Class
