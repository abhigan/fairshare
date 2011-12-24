Imports System.Runtime.Serialization
Imports System.Net


Public Class Globals
    Public Const PIPEWIDTH As Integer = (130 - 70) * 1024 ' bytes per second

    Public Const BAREMINIMUM As Integer = 10 * 1024 ' bytes per second
    Public Const HOARDING As Integer = 70 ' %
    Public Const MODERATE As Integer = 80 ' %
    Public Const TURBO As Integer = 90 ' %

    Public Const POLLING_INTERVAL As Integer = 10000 ' ms
    Public Const LOCAL_ANALYSIS_DURATION As Integer = 6 'sec

    Public Const LISTNER_PORT As Integer = 4039

    <Serializable()>
    Public Class UsageStatistics
        Public Property UserIP As String = String.Empty
        Public Property Roof As Integer = PIPEWIDTH
        Public Property Consumption As Integer = 0
        Public Property TimeStamp As Date = Now

        Public ReadOnly Property isMe As Boolean
            Get
                Dim myname = Dns.GetHostName
                Dim myentry = Dns.GetHostEntry(myname)
                For Each ipAddr In myentry.AddressList
                    If ipAddr.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                        If UserIP = ipAddr.ToString Then
                            Return True
                        End If
                    End If
                Next
                Return False
            End Get
        End Property
    End Class
End Class
