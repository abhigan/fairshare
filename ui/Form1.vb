Imports CG.FairShare.Data
Imports CG.FairShare.Globals
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Net.Sockets
Imports System.Net
Imports System.ComponentModel
Imports System.Runtime.Remoting

Public Class Form1
    Friend ns As INetworkStateProvider

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Show()
        Me.Refresh()
        Application.DoEvents()
        RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off
        ConnSettingUpDialogue.ShowDialog(Me, "localhost")
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim NetState() As UsageStatistics
        Static connectionFailCounter As Integer = 0
        Try
            NetState = ns.getNetworkState
            connectionFailCounter = 0
        Catch ex As SocketException
            If ex.Message = "An existing connection was forcibly closed by the remote host" Then
                Timer1.Enabled = False
                ConnSettingUpDialogue.ShowDialog(Me, "localhost")

            ElseIf ex.Message.Contains("No connection could be made because the target machine actively refused it") Then
                connectionFailCounter += 1
                If connectionFailCounter > 5 Then End

            Else
                Throw
            End If
            Exit Sub
        End Try

        TextBox1.Text = ""
        Dim users As New ArrayList
        Dim usage As New ArrayList
        Dim used As Integer = 0
        For Each usr In NetState
            TextBox1.Text = TextBox1.Text & String.Format("{0,-12} ({2,-14}) {1,6}", usr.UserIP, usr.Consumption, resolveIP(usr.UserIP)) & vbCrLf
            used += usr.Consumption
            users.Add(resolveIP(usr.UserIP))
            usage.Add(usr.Consumption)
        Next

        users.Add("Remaining")
        usage.Add(PIPEWIDTH - used)

        Chart1.Series(0).Points.DataBindXY(users.ToArray, usage.ToArray)
        ComboBox1.DataSource = users.ToArray

    End Sub

    Function resolveIP(ByVal ip As String) As String
        Static knownHosts As New Dictionary(Of String, String)
        Dim resolvedName As String = ""
        If Not knownHosts.TryGetValue(ip, resolvedName) Then
            Try
                resolvedName = Dns.GetHostEntry(ip).HostName
                knownHosts.Add(ip, resolvedName)
            Catch ex As Exception
                resolvedName = ip
            End Try
        End If
        Return resolvedName
    End Function

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        SplitContainer2.SplitterDistance = 25
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        MsgBox(My.Resources.About)
    End Sub

    Private Sub ComboBox1_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectionChangeCommitted
        Timer1.Enabled = False
        ConnSettingUpDialogue.ShowDialog(Me, CType(sender, ComboBox).SelectedValue)
    End Sub
End Class
