Imports CG.FairShare.Globals
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Transmitter

    Private WithEvents sender As New BackgroundWorker

    Public Sub StartTransmitting()
        sender.RunWorkerAsync()
    End Sub

    Private Sub sender_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles sender.DoWork
        Dim endPoint As New IPEndPoint(IPAddress.Broadcast, LISTNER_PORT)
        Dim localendpoint As New IPEndPoint(IPAddress.Parse("192.168.1.5"), 4040)
        Dim udpSender As New UdpClient(localendpoint)
        While True
            Dim myStats As UsageStatistics = LocalStateManager.getManager.Usage
            Dim msg = myStats.Roof & "#" & myStats.Consumption & "#" & myStats.UserSignature.ToString
            Dim txData() As Byte = System.Text.Encoding.ASCII.GetBytes(msg)
            udpSender.Send(txData, txData.Length, endPoint)
            Threading.Thread.Sleep(POLLING_INTERVAL)
        End While
    End Sub
End Class
