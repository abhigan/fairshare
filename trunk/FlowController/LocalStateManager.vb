Imports System.ComponentModel
Imports CG.FairShare.Globals

Public Class LocalStateManager

    Private m_state As New UsageStatistics
    Private m_state_lock As New Object
    Private Shared m_instance As New LocalStateManager
    Private WithEvents m_sbmHandlerThread As New BackgroundWorker

    Private Sub New()
        'enforce singleton pattern
        SyncLock m_state_lock
            m_state = sbmHandler.getMyUsageStatistics()
        End SyncLock
    End Sub

    Shared Function getManager() As LocalStateManager
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

    Private Sub m_sbmHandler_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles m_sbmHandlerThread.DoWork
        While True
            SyncLock m_state_lock
                m_state = sbmHandler.getMyUsageStatistics()
            End SyncLock
            Threading.Thread.Sleep(LOCAL_ANALYSIS_DURATION)
        End While
    End Sub
End Class
