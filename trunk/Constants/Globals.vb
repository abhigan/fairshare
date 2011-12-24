Imports System.Runtime.Serialization

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
        Public User As String
        Public Roof As Integer
        Public Consumption As Integer
        Public TimeStamp As DateTime
    End Class
End Class
