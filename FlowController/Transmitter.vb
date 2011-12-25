Imports CG.FairShare.Globals
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Transmitter
    Dim udpSender As UdpClient
    Private WithEvents sender As New BackgroundWorker

    Public Sub StartTransmitting()
        Dim localendpoint As New IPEndPoint(IPAddress.Parse("192.168.1.5"), 4040)
        udpSender = New UdpClient(localendpoint)
        sender.WorkerSupportsCancellation = True
        sender.RunWorkerAsync()
    End Sub

    Sub StopTransmitting()
        udpSender.Close()
        sender.CancelAsync()
    End Sub

    Private Sub sender_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles sender.DoWork
        Dim endPoint As New IPEndPoint(IPAddress.Broadcast, LISTNER_PORT)
        While True
            Dim myStats As UsageStatistics = LocalStateManager.getManager.Usage
            Dim msg = myStats.Roof & "#" & myStats.Consumption & "#" & myStats.UserSignature.ToString
            Dim txData() As Byte = System.Text.Encoding.ASCII.GetBytes(msg)
            udpSender.Send(txData, txData.Length, endPoint)

            If CType(sender, BackgroundWorker).CancellationPending Then Exit While
            Threading.Thread.Sleep(POLLING_INTERVAL)
        End While
    End Sub
End Class
