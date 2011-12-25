Imports System.Runtime.Serialization
Imports System.Net


Public Class Globals
    Public Const PIPEWIDTH As Integer = (130) * 1024 ' bytes per second

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
        Public Property UserSignature As Guid

        Public ReadOnly Property ConsumptionPercent As Integer
            Get
                Return (Consumption * 100) / Roof
            End Get
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not TypeOf (obj) Is UsageStatistics Then
                Return MyBase.Equals(obj)
            Else
                Dim second As UsageStatistics = obj
                Return second.UserIP.Equals(Me.UserIP)
            End If
        End Function


    End Class

End Class
