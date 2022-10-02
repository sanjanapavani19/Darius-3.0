Imports System.IO

Module SuperScan
    Dim bmp As Bitmap
    Dim Scanning As Boolean
    Dim StopCrazyCamera As Boolean = False
    Public Activeb As Integer
    Dim watch As Stopwatch
    Dim MonitorJ(100) As Single
    Dim MonitorTimeStamp(100) As Single
    Dim MonitorActiveb(100) As Single
    Dim MonitorloopZ(100) As Single
    Dim Bufferbytes() As Byte
    Public Captured As Boolean
    Dim Hdirection As Integer = 1
    Dim Vdirection As Integer = 1
    Dim loop_x, loop_y As Integer


    Public Sub FastScan2(X As Integer, Y As Integer, ScanOverlap As Integer, Pbar As ProgressBar, Address As String, AcqusitionType As AcqusitionTypeEnum)
        bmp = New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
        ReDim Bufferbytes(Camera.W * Camera.H * 3 - 1)

        Select Case AcqusitionType
            Case AcqusitionTypeEnum.WhiteDwarf
                FlatField = ReadSinglePage32bit("ff.tif")
            Case AcqusitionTypeEnum.FiBi
                FlatField = ReadSinglePage32bit("ff_fibi.tif")
        End Select



        watch = New Stopwatch
        Activeb = 0
        Dim CrazyCameraThread As New System.Threading.Thread(AddressOf CrazyCamera)
        watch.Start()
        CrazyCameraThread.Start()




        Form1.CheckBoxLED.Checked = True
        Scanning = True
        Form1.Button_Scan.Text = "Cancel"

        Camera.ResetMatrix()

        Camera.Flatfield(0)

        ' Creating overlap to enhance the stitching with ICE
        Dim AdjustedStepX As Single = Stage.FOVX * (1 - ScanOverlap / Camera.W)
        Dim AdjustedStepY As Single = Stage.FOVY * (1 - ScanOverlap / Camera.H)

        Dim cx, cy, cz As Single
        Stage.UpdatePositions()
        cx = Stage.X
        cy = Stage.Y
        cz = Stage.Z

        Form1.Pbar.Visible = True
        Form1.Pbar.Maximum = X * Y

        Dim Axis As String = ""


        If Tracking.ROI.IsMade Then
            Tracking.MovetoROIEdge()
        End If

        Dim b As Integer = 0




        Dim FileName As String = Path.GetFileNameWithoutExtension(Address)
        Dim Dir As String = Path.Combine(Path.GetDirectoryName(Address), FileName)
        Dim OUTPUT As String = Path.GetDirectoryName(Address) + "\" + FileName + "Stitched.svs"
        Directory.CreateDirectory(Dir)

        Dim Stitcher As New MVStitchintLibrary.StitcherClass
        Dim InputDirectory As New IO.DirectoryInfo(Dir)
        Form1.Pbar.Maximum = X * Y



        For b = 0 To ScanBufferSize - 1

            ScanUnits(b).InputSettings(X, Y, Dir, FileName)
        Next




        For loop_x = 1 To X
            For loop_y = 1 To Y

                Form1.Pbar.Increment(1)
                If Scanning = False Then GoTo 1


                If b = ScanBufferSize Then b = 0

                Stage.WaitUntilIdle(Stage.Zaxe)
                ScanUnits(b).PrepareAcquire(loop_x, loop_y, Hdirection, Vdirection, False, b)
                b += 1

                If loop_y < Y Then

                    Stage.MoveRelative(Stage.Yaxe, -AdjustedStepY * Hdirection, False)
                    Stage.Y += AdjustedStepY
                    Vdirection = Vdirection * -1

                Else
                    If loop_x < X Then

                        Stage.MoveRelative(Stage.Xaxe, -AdjustedStepX, False)
                        Stage.X += -AdjustedStepX * Hdirection
                        Vdirection = Vdirection * -1
                        Hdirection = Hdirection * -1
                    End If
                End If



                Application.DoEvents()
            Next
        Next


        Form1.CheckBoxLED.Checked = False
        Dim Alldone As Boolean = False
        Do Until Alldone = 0
            For b = 0 To ScanBufferSize - 1
                Alldone *= ScanUnits(b).ready
            Next
        Loop

        StopCrazyCamera = True
        Form1.Pbar.Maximum = 100

1:
        Stage.MoveAbsoluteAsync(Stage.Xaxe, cx)
        Stage.MoveAbsoluteAsync(Stage.Yaxe, cy)
        Stage.MoveAbsoluteAsync(Stage.Zaxe, cz)
        ZEDOF.Vdirection = 1
        Form1.Pbar.Value = 0


        watch.Stop()

        MsgBox("Scanned in " + (watch.ElapsedMilliseconds / 1000).ToString + " s")

        Stitcher.Process(Pbar, 2048 - ScanOverlap, 2048 - ScanOverlap, ScanOverlap, InputDirectory, OUTPUT)

        'ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
        'ReDim Preserve Filenames(fileN)
        'Filenames(fileN) = SaveFileDialog1.FileName
        'Form1.fileN += 1
2:

        Scanning = False
        Form1.Button_Scan.Text = "Scan"
        Form1.GoLive()


    End Sub
    Sub NextField()

    End Sub
    Sub CrazyCamera()
        StopCrazyCamera = False

        'Warmup!
        For i = 1 To 20
            Camera.capture(False)
        Next
        Do Until StopCrazyCamera
            'For j = 0 To 100
            Captured = False


            If ScanUnits(Activeb).ActivelyCapturing Then


                ScanUnits(Activeb).ImageBeingCaptured = True
                Camera.Capture(ScanUnits(Activeb).bytes(ScanUnits(Activeb).loopZ))
                ScanUnits(Activeb).Imagecreated(ScanUnits(Activeb).loopZ) = 1
                Captured = True

                If ScanUnits(Activeb).loopZ < ScanUnits(Activeb).Z - 1 Then
                    ScanUnits(Activeb).loopZ += 1
                Else
                    ScanUnits(Activeb).ActivelyCapturing = False
                End If

                'MonitorloopZ(j) = ScanUnits(Activeb).Zscan.loopZ
                'MonitorActiveb(j) = Activeb
                'MonitorTimeStamp(j) = watch.ElapsedMilliseconds
            End If

            If Captured = False Then
                Camera.Capture(Bufferbytes)
            End If

            'Next

        Loop

    End Sub

End Module
