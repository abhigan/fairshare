Imports CG.FairShare.Globals
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp


Public Class NetworkStateProvider
    Implements INetworkStateProvider

    Private m_networkState As New SortedDictionary(Of Guid, UsageStatistics)
    Private m_networkState_lock As New Object
    Private Shared m_instance As New NetworkStateProvider
    Dim WithEvents m_receiver As New Receiver
    Dim m_transmitter As New Transmitter

    Private Sub New()
        'enforce singleton pattern
        RemoteNetworkStateProvider._actualProvider = Me
    End Sub

    Public Sub init(ByVal transmit As Boolean, ByVal receive As Boolean)
        If transmit Then m_transmitter.StartTransmitting()
        If receive Then m_receiver.StartReceiving()

        If ChannelServices.RegisteredChannels.Count = 0 Then
            Dim tc As New TcpChannel(4949)
            ChannelServices.RegisterChannel(tc, False)
        End If
        RemotingConfiguration.RegisterWellKnownServiceType(GetType(RemoteNetworkStateProvider),
                                                           "RemoteNetworkStateProvider",
                                                           WellKnownObjectMode.Singleton)
    End Sub

    Public Sub Shutdown() Implements INetworkStateProvider.Shutdown
        Console.WriteLine("shutting down")
        m_receiver.stopReceiving()
        m_transmitter.StopTransmitting()
    End Sub

    Public Shared Function GetProvider() As NetworkStateProvider
        Return m_instance
    End Function

    Public Function getNetworkState() As UsageStatistics() Implements INetworkStateProvider.getNetworkState
        SyncLock m_networkState_lock
            Dim expired As New List(Of Guid)
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
            If m_networkState.ContainsKey(RemoteStatistics.UserSignature) Then
                'we replace the existing
                m_networkState.Item(RemoteStatistics.UserSignature) = RemoteStatistics

            Else ' we add this user
                m_networkState.Add(RemoteStatistics.UserSignature, RemoteStatistics)
            End If
        End SyncLock
    End Sub

End Class

Public Class RemoteNetworkStateProvider
    Inherits MarshalByRefObject
    Implements INetworkStateProvider

    Friend Shared _actualProvider As NetworkStateProvider

    Public Sub New()
        _actualProvider = NetworkStateProvider.GetProvider
    End Sub

    Function getNetworkState() As UsageStatistics() Implements INetworkStateProvider.getNetworkState
        Return _actualProvider.getNetworkState
    End Function

    Sub Shutdown() Implements INetworkStateProvider.Shutdown
        Throw New InvalidOperationException("Remote server cannot be shut down")
    End Sub

End Class