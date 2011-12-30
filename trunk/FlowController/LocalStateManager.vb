Imports System.ComponentModel
Imports CG.FairShare.Globals

Public Class LocalStateManager

    Private m_state As New UsageStatistics
    Private m_state_lock As New Object
    Private Shared m_instance As LocalStateManager = Nothing
    Private WithEvents m_sbmHandlerThread As New BackgroundWorker
    Private m_signature As Guid

    Private Sub New()
        'enforce singleton pattern
        m_signature = Guid.NewGuid
        SyncLock m_state_lock
            m_state = sbmHandler.getMyUsageStatistics()
            m_state.UserSignature = m_signature
        End SyncLock
    End Sub

    Shared Function getManager() As LocalStateManager
        If m_instance Is Nothing Then
            m_instance = New LocalStateManager
        End If
        Return m_instance
    End Function

    Public Sub init()
        m_sbmHandlerThread.RunWorkerAsync()
    End Sub

    ReadOnly Property Usage As UsageStatistics
        Get
            SyncLock m_state_lock
                Dim usg As New UsageStatistics
                usg.Consumption = m_state.Consumption
                usg.Roof = m_state.Roof
                usg.TimeStamp = m_state.TimeStamp
                usg.UserIP = m_state.UserIP
                usg.UserSignature = m_state.UserSignature
                If usg.Roof = 0 Then usg.Roof = PIPEWIDTH
                Return usg
            End SyncLock
        End Get
    End Property

    Sub SetRoof(ByVal newRoof As Integer)
        sbmHandler.setMyRoof(newRoof)
        SyncLock m_state_lock
            m_state.Roof = newRoof
        End SyncLock
    End Sub

    Function IsMyStatistcs(ByVal Stats As UsageStatistics) As Boolean
        Return Stats.UserSignature.Equals(m_signature)
    End Function

    Private Sub m_sbmHandler_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles m_sbmHandlerThread.DoWork
        While True
            Try
                SyncLock m_state_lock
                    m_state = sbmHandler.getMyUsageStatistics()
                    m_state.UserSignature = m_signature
                End SyncLock

            Catch ex As Exception
                Trace.WriteLine(ex.ToString)
            End Try
            Threading.Thread.Sleep(LOCAL_ANALYSIS_DURATION)
        End While
    End Sub
End Class
