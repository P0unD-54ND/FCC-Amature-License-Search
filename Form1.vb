Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Security
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MsgBox("this will take a minute to two")
        ' Get the latest Amature radio database from the FCC
        Dim extractPath As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim remoteUri As String = "https://data.fcc.gov/download/pub/uls/complete/l_amat.zip"
        Dim fileName As String = "l_amat.zip"
        Dim password As String = "..."
        Dim username As String = "..."
        Using client As New WebClient()
            client.Credentials = New NetworkCredential(username, password)
            client.DownloadFile(remoteUri, extractPath & "\" & fileName)
        End Using

        ' Before extracting the database, delete the old files if they exists.

        If File.Exists((extractPath & "\AM.dat")) Then Kill(extractPath & "\AM.dat")
        If File.Exists((extractPath & "\CO.dat")) Then Kill(extractPath & "\CO.dat")
        If File.Exists((extractPath & "\counts")) Then Kill(extractPath & "\counts")
        If File.Exists((extractPath & "\EN.dat")) Then Kill(extractPath & "\EN.dat")
        If File.Exists((extractPath & "\HD.dat")) Then Kill(extractPath & "\HD.dat")
        If File.Exists((extractPath & "\HS.dat")) Then Kill(extractPath & "\HS.dat")
        If File.Exists((extractPath & "\LA.dat")) Then Kill(extractPath & "\LA.dat")
        If File.Exists((extractPath & "\SC.dat")) Then Kill(extractPath & "\SC.dat")
        If File.Exists((extractPath & "\SF.dat")) Then Kill(extractPath & "\SF.dat")

        ZipFile.ExtractToDirectory(fileName, extractPath)
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click



        Dim readfilename As String = "EN.dat" 'text data file to be read by StreamReader. The file must be in the same folder at this application.
        Dim textreader As New StreamReader(readfilename, Encoding.Default) 'object to read the text data file
        Dim stringline As String = ""
        Dim gridrow As Integer = 0
        Dim callsignsearchstring As String = TextBox1.Text.ToUpper()
        Dim statesearchstring As String = TextBox2.Text.ToUpper()
        DataGridView1.Rows.Clear()
        Do
            stringline = textreader.ReadLine() ' read a line from the text file
            If stringline Is Nothing Then Exit Do ' stop the program if there are no more lines
            Dim words() As String = stringline.Split("|") ' parce the line into a string array
            Dim regex1 As New Regex(callsignsearchstring) 'set up regex for call sign match looking at the right side of the call sign
            Dim regex2 As New Regex(statesearchstring) ' setup regex for state match, two characters required for each state and each state seperated by a pipe character
            Dim match1 As Match = regex1.Match(words(4)) ' does the string from the input box 1 make a match using regular expressions (boolean) 
            Dim match2 As Match = regex2.Match(words(17)) ' does the string from the input box 2 make a match using regular expressions (boolean) 
            If match1.Success And match2.Success Then ' get the rest of the line if there is a match
                DataGridView1.Rows.Add() ' add a row to the grid
                DataGridView1.Rows(gridrow).Cells(0).Value = words(4) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(1).Value = words(8) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(2).Value = words(9) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(3).Value = words(10) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(4).Value = words(16) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(5).Value = words(17) ' paste into the grid
                DataGridView1.Rows(gridrow).Cells(6).Value = MakePhoneticList(words(4)) ' paste into the grid
                gridrow = gridrow + 1
            End If
        Loop
        textreader.Close()
    End Sub



    Private Function MakePhoneticList(callsign As String)
        Dim phoneticDictionary As New Dictionary(Of Char, String)()
        phoneticDictionary.Add("a", "Alfa")
        phoneticDictionary.Add("b", "Bravo")
        phoneticDictionary.Add("c", "Charlie")
        phoneticDictionary.Add("d", "Delta")
        phoneticDictionary.Add("e", "Echo")
        phoneticDictionary.Add("f", "Foxtrot")
        phoneticDictionary.Add("g", "Golf")
        phoneticDictionary.Add("h", "Hotel")
        phoneticDictionary.Add("i", "India")
        phoneticDictionary.Add("j", "Juliet")
        phoneticDictionary.Add("k", "Kilo")
        phoneticDictionary.Add("l", "Lima")
        phoneticDictionary.Add("m", "Mike")
        phoneticDictionary.Add("n", "November")
        phoneticDictionary.Add("o", "Oscar")
        phoneticDictionary.Add("p", "Papa")
        phoneticDictionary.Add("q", "Quebec")
        phoneticDictionary.Add("r", "Romeo")
        phoneticDictionary.Add("s", "Sierra")
        phoneticDictionary.Add("t", "Tango")
        phoneticDictionary.Add("u", "Uniform")
        phoneticDictionary.Add("v", "Victor")
        phoneticDictionary.Add("w", "Whiskey")
        phoneticDictionary.Add("x", "X-ray")
        phoneticDictionary.Add("y", "Yankee")
        phoneticDictionary.Add("z", "Zulu")
        phoneticDictionary.Add("0", "Zero")
        phoneticDictionary.Add("1", "Wun")
        phoneticDictionary.Add("2", "Too")
        phoneticDictionary.Add("3", "Tree")
        phoneticDictionary.Add("4", "FOW-er")
        phoneticDictionary.Add("5", "Fife")
        phoneticDictionary.Add("6", "Six")
        phoneticDictionary.Add("7", "Sev-in")
        phoneticDictionary.Add("8", "Ait")
        phoneticDictionary.Add("9", "Nin-er")
        Dim phoneticString As New StringBuilder()
        For Each c As Char In callsign.ToLower()
            If phoneticDictionary.ContainsKey(c) Then
                phoneticString.Append(phoneticDictionary(c) & " ")
            Else
                phoneticString.Append(c)
            End If
        Next
        Return phoneticString
    End Function


    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        DataGridView1.Rows(e.RowIndex).Selected = True
    End Sub


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        On Error GoTo errormsxbox 'handle the error with a message box
        Dim selectedRowIndex As Integer = DataGridView1.SelectedRows(0).Index ' Get the index of the first selected row
        Dim row As DataGridViewRow = DataGridView1.Rows(selectedRowIndex) ' Replace rowIndex with the actual index

        Dim stringArray As String() = New String(row.Cells.Count - 1) {} ' Create array with appropriate size
        For i As Integer = 0 To row.Cells.Count - 1
            stringArray(i) = If(row.Cells(i).Value IsNot Nothing, row.Cells(i).Value.ToString(), String.Empty) ' Handle null values
        Next
        DataGridView2.Rows.Add() ' add a row to the grid
        Dim firstEmptyRow As Integer
        firstEmptyRow = DataGridView2.Rows.Count - 1
        DataGridView2.Rows(firstEmptyRow).Cells(0).Value = stringArray(0) ' paste into the grid
        DataGridView2.Rows(firstEmptyRow).Cells(1).Value = stringArray(1) ' paste into the grid
        DataGridView2.Rows(firstEmptyRow).Cells(2).Value = stringArray(2) ' paste into the grid
        DataGridView2.Rows(firstEmptyRow).Cells(3).Value = stringArray(3) ' paste into the grid
        DataGridView2.Rows(firstEmptyRow).Cells(4).Value = stringArray(4) ' paste into the grid
        DataGridView2.Rows(firstEmptyRow).Cells(5).Value = stringArray(5) ' paste into the grid
        GoTo allgood
errormsxbox:
        MsgBox("please click on a row before clicking on the 'Copy Selected Row' button")
allgood:
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub


End Class
