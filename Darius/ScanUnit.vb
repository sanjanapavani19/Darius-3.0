Imports System.IO

Public Class ScanUnit

    Public Zscan As ZstackStructure
    Dim done As Boolean
    Dim X, Y As Integer
    Dim FileX, FileY As Integer
    Dim W, H As Integer
    Dim Range, stepsize As Single
    Dim scale As Integer
    Dim Address As String

    Dim bmp As Bitmap
    Dim Dehaze As DehazeClass
    Dim Directory As String
    Dim FileName As String


    Public Sub New(W As Integer, H As Integer, Range As Single, Stepsize As Single, scale As Integer)
        Me.Range = Range
        Me.stepsize = Stepsize
        Me.scale = scale
        done = True
        bmp = New Bitmap(W, H, Imaging.PixelFormat.Format24bppRgb)
        Zscan = New ZstackStructure(W, H, Range, Stepsize, scale)
        Dehaze = New DehazeClass(W, H, 0.008, 0.5)
    End Sub
    Public Sub InputSettings(X As Integer, Y As Integer, Directory As String, Filename As String)
        Me.X = X
        Me.Y = Y
        Me.Address = Address
        Me.Directory = Directory
        Me.FileName = Filename
    End Sub
    Public Sub Acquire(loop_x As Integer, loop_y As Integer, Hdirection As Integer, Vdirection As Integer, EDOF As Boolean)
        Do Until done
            Application.DoEvents()
        Loop
        done = False
        If Hdirection = 1 Then FileY = loop_y - 1 Else FileY = Y - loop_y
        FileX = loop_x - 1
        'Threading.Thread.Sleep(100)


        If EDOF Then
            Zscan.Acquire(False, Vdirection)
        Else

            ReDim Zscan.OutputBytes(Camera.W * Camera.H * 3 - 1)
            Camera.Capture(Zscan.OutputBytes)
            Zscan.WrapUpDone = True
        End If

        'Save()

        Dim SaveThread As New System.Threading.Thread(AddressOf Save)
        SaveThread.Start()
        'Stage.WaitUntilIdle(Stage.Zaxe)
    End Sub

    Public Sub Save()
        Do Until Zscan.WrapUpDone
            Application.DoEvents()
        Loop
        Dehaze.Apply(Zscan.OutputBytes)
        byteToBitmap(Zscan.OutputBytes, bmp)
        bmp.Save(Path.Combine(Directory, FileName + " Y" + FileY.ToString("D5") + " X" + FileX.ToString("D5") + ".bmp"))
        done = True
        Application.DoEvents()
    End Sub

End Class
