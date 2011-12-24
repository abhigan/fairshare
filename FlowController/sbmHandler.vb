Imports CG.FairShare.Globals
Imports System.Text
Imports System.Net.Sockets
Imports System.Net



Class sbmHandler

    Shared Function getMyUsageStatistics() As UsageStatistics
        Dim command = GETRULESTATUS_COMMAND()
        Dim rawstats As String = communicate(command)

        Dim roof As Integer = getRoof(rawstats)
        If roof = 0 Then roof = PIPEWIDTH
        Dim avUsage As Integer = getUsageStat(rawstats)
        Dim myUsageStatistics = New UsageStatistics

        myUsageStatistics.Consumption = Math.Min(avUsage, roof)
        myUsageStatistics.Roof = roof
        myUsageStatistics.TimeStamp = Now
        myUsageStatistics.UserIP = "127.0.0.1"

        Return myUsageStatistics
    End Function

    Shared Sub setMyRoof(ByVal newRoof As Integer)
        Console.WriteLine("SETTING:" & Int(newRoof / 1024))
        Dim setRoofCommand As String = SETRULE_COMMAND(newRoof)
        communicate(setRoofCommand)
    End Sub

    Private Shared Function communicate(ByVal Command As String) As String
        Dim stb As New StringBuilder

        Using m_sock As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            m_sock.Connect("127.0.0.1", 8701)
            Dim sendData As String = getHTTPheader(Command) & Command
            Dim sendBytes() As Byte = System.Text.Encoding.ASCII.GetBytes(sendData)
            m_sock.Send(sendBytes)

            Dim recv(0) As Byte
            While m_sock.Receive(recv) > 0
                stb.Append(System.Text.Encoding.ASCII.GetChars(recv))
            End While
        End Using
        Dim recData = stb.ToString
        Return recData
    End Function

    Private Shared Function getHTTPheader(ByVal payload As String) As String
        Return "POST / HTTP/1.0" & vbCrLf & _
               "Content-Type: text/xml" & vbCrLf & _
               "Content-Length: " & payload.Length & vbCrLf & _
               "Authorization: Basic " & getAuth() & vbCrLf & _
               "" & vbCrLf
    End Function

    Private Shared Function GETRULE_COMMAND() As String
        Return "<?xml version=""1.0"" encoding=""windows-1252""?>" & vbCrLf & _
               "<request>" & vbCrLf & _
               "	<command>getrules</command>" & vbCrLf & _
               "</request>"
    End Function

    Private Shared Function GETRULESTATUS_COMMAND() As String
        Return "<?xml version=""1.0"" encoding=""windows-1252""?>" & vbCrLf & _
               "<request>" & vbCrLf & _
               "	<command>getrulestats</command>" & vbCrLf & _
               "	<rulename>limme</rulename>" & vbCrLf & _
               "</request>"
    End Function

    Private Shared Function SETRULE_COMMAND(ByVal roofRate As Integer)
        Dim value As String = My.Resources.setrule
        value = value.Replace("?RATE", roofRate.ToString)
        Return value
    End Function

    Private Shared Function getAuth() As String
        Dim uName = "admin"
        Dim pass = ""

        Dim auth = uName & ":" & pass
        Dim base64auth(100) As Char
        Dim l = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth))
        Return l
    End Function

    Private Shared Function getRoof(ByVal rulexml As String) As Integer
        Dim startPos As Integer = InStr(rulexml, "rate=""", CompareMethod.Text) + 5
        Dim endpos As Integer = InStr(startPos + 1, rulexml, """", CompareMethod.Text)
        Dim roofVal As Integer = Val(rulexml.Substring(startPos, endpos - startPos - 1))
        Return roofVal
    End Function

    Private Shared Function getUsageStat(ByVal rulexml As String) As Integer
        Dim startPos As Integer = InStr(rulexml, "<recv>", CompareMethod.Text) + 5
        Dim tempstr = rulexml.Substring(startPos)
        Dim endpos As Integer = InStr(startPos + 1, rulexml, "</recv>", CompareMethod.Text)
        Dim strusg As String = rulexml.Substring(startPos, endpos - startPos - 1)
        Dim usg() As String = strusg.Split("|")
        Dim usg2(LOCAL_ANALYSIS_DURATION - 1) As String
        Array.ConstrainedCopy(usg, usg.Length - LOCAL_ANALYSIS_DURATION, usg2, 0, LOCAL_ANALYSIS_DURATION)
        Dim avg As Integer = (From individualUsg As Integer In usg2 Select individualUsg).Average
        Return avg
    End Function
End Class