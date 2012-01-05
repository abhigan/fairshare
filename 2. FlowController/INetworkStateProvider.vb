Imports CG.FairShare.Globals

Public Interface INetworkStateProvider
    Function getNetworkState() As UsageStatistics()
    Sub Shutdown()
End Interface
