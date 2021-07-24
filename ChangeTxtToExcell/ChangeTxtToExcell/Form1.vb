Imports System.IO
Imports System.Text

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then         ' 파일선택 다이얼로그 호출 및 선택하면
            Dim ChoiceFileName = OpenFileDialog1.FileName              ' 파일명 가져오고
            Label2.Text = ChoiceFileName
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim f = New FileStream(Label2.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        Dim TxtFile = New StreamReader(f, Encoding.Default)
        Dim sReadLine As String
        Dim sTemp = ""

        ' Dim fcvs = New FileInfo(Label2.Text + ".csv")
        Dim CsvFile = New StreamWriter(Label2.Text + ".csv", FileMode.Append, Encoding.Default)

        While True
            sReadLine = TxtFile.ReadLine()                         ' 읽고
            If IsNothing(sReadLine) Then Exit While               ' 파일끝인지
            ListBox1.Items.Add(sReadLine)
            Dim aa = Split(sReadLine)   ' 읽은줄을 배열로 바꿔
            Select Case aa(0)
                Case "경기도육아종합지원센터"
                Case "시설유형"
                    sTemp = sTemp + "," + Strings.Mid(sReadLine, 8)
                Case "홈페이지"
                    'sTemp = sTemp + "," + Strings.Mid(sReadLine, 8)
                Case "전화번호"
                    sTemp = sTemp + "," + Strings.Mid(sReadLine, 8)
                Case "주소"
                    sTemp = sTemp + "," + Strings.Mid(sReadLine, 6)
                    ListBox2.Items.Add(sTemp)
                    csvfile.WriteLine(sTemp)
                    sTemp = ""
                Case Else
                    If Len(aa(0)) > 2 Then     ' 2자이상이면
                        sTemp = sReadLine
                    End If
            End Select

        End While
        TxtFile.Close()                                                     ' 닫는다
        CsvFile.Close()
        Label3.Text = Label2.Text + ".csv"
    End Sub
End Class
