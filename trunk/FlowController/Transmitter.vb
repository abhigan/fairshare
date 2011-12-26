Imports CG.FairShare.Globals
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Transmitter
    Dim udpSendClients As New List(Of UdpClient)
    Private WithEvents sender As New BackgroundWorker

    Public Sub StartTransmitting()
        Dim myHostName As String = Dns.GetHostName
        Dim myDnsEntry = Dns.GetHostEntry(myHostName)
        udpSendClients.Clear()
        For Each myIp In myDnsEntry.AddressList
            If myIp.AddressFamily = AddressFamily.InterNetwork Then
                Dim localendpoint As New IPEndPoint(myIp, 4040)
                Dim aUDPSender = New UdpClient(localendpoint)
                udpSendClients.Add(aUDPSender)
            End If
        Next

        sender.WorkerSupportsCancellation = True
        sender.RunWorkerAsync()
    End Sub

    Sub StopTransmitting()
        For Each udpsender In udpSendClients
            udpsender.Close()
        Next
        sender.CancelAsync()
    End Sub

    Private Sub sender_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles sender.DoWork
        Dim remoteEndPoint As New IPEndPoint(IPAddress.Broadcast, LISTNER_PORT)
        While True
            Dim myStats As UsageStatistics = LocalStateManager.getManager.Usage
            Dim msg = myStats.Roof & "#" & myStats.Consumption & "#" & myStats.UserSignature.ToString
            Dim txData() As Byte = System.Text.Encoding.ASCII.GetBytes(msg)
            For Each udpsender In udpSendClients
                udpsender.Send(txData, txData.Length, remoteEndPoint)
            Next
            If CType(sender, BackgroundWorker).CancellationPending Then Exit While
            Threading.Thread.Sleep(POLLING_INTERVAL)
        End While
    End Sub
End Class
