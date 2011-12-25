﻿Imports CG.FairShare.Globals
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp


Public Class NetworkStateProvider

    Private m_networkState As New SortedDictionary(Of String, UsageStatistics)
    Private m_networkState_lock As New Object
    Private Shared m_instance As New NetworkStateProvider
    Dim WithEvents m_receiver As New Receiver
    Dim m_transmitter As New Transmitter

    Dim inited As Boolean = False

    Private Sub New()
        'enforce singleton pattern
        RemoteNetworkStateProvider._actualProvider = Me
    End Sub

    Public Sub init()
        m_transmitter.StartTransmitting()
        m_receiver.StartReceiving()

        Dim tc As New TcpChannel(4949)
        ChannelServices.RegisterChannel(tc, False)
        RemotingConfiguration.RegisterWellKnownServiceType(GetType(RemoteNetworkStateProvider),
                                                           "RemoteNetworkStateProvider",
                                                           WellKnownObjectMode.Singleton)

        inited = True
    End Sub

    Public Shared Function GetProvider() As NetworkStateProvider
        Return m_instance
    End Function

    Function getNetworkState() As UsageStatistics()
        SyncLock m_networkState_lock
            Dim expired As New List(Of String)
            For Each usr In m_networkState
                Dim interval As Integer = (Now - usr.Value.TimeStamp).TotalMilliseconds
                If interval >= 2 * POLLING_INTERVAL Then
                    expired.Add(usr.Key)
                End If
            Next
            For Each expireditem In expired
                m_networkState.Remove(expireditem)
            Next
            Return m_networkState.Values.ToArray
        End SyncLock
    End Function

    Private Sub receiver_Received(ByVal RemoteStatistics As UsageStatistics) Handles m_receiver.Received
        SyncLock m_networkState_lock
            If m_networkState.ContainsKey(RemoteStatistics.UserIP.ToLower) Then
                'we replace the existing
                m_networkState.Item(RemoteStatistics.UserIP.ToLower) = RemoteStatistics

            Else ' we add this user
                m_networkState.Add(RemoteStatistics.UserIP.ToLower, RemoteStatistics)
            End If
        End SyncLock
    End Sub

End Class

Public Class RemoteNetworkStateProvider
    Inherits MarshalByRefObject

    Friend Shared _actualProvider As NetworkStateProvider

    Public Sub New()
        _actualProvider = NetworkStateProvider.GetProvider
    End Sub

    Function getNetworkState() As UsageStatistics()
        Return _actualProvider.getNetworkState
    End Function

End Class