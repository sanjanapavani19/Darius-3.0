Imports System.ComponentModel
Imports System.IO
Imports AForge.Imaging.Filters
Imports Microsoft.VisualBasic.Devices
Imports AForge.Imaging
Imports System.Threading
Imports System.Windows.Forms.DataVisualization.Charting
Imports MathNet.Numerics

Public Class Form1

    Public Display As ImageDisplay
    Public LEDcontroller As Relay
    Dim IsDragging As Boolean

    Dim AutoFocus As FocusStructure
    Dim Slideloaded As Boolean

    Dim StopAlign As Boolean
    Dim panel As Integer
    Dim Focusing As Boolean
    Dim Filenames() As String
    Dim Scanning As Boolean
    Dim fileN As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Preview = New PreviewStructure
        TextBox_PrevieEXp.Text = Setting.Gett("PREVIEWEXP")
        TextBox_PreviewFocus.Text = Setting.Gett("PREVIEWFOCUS")


        Camera = New XimeaColor
        EDF = New ExtendedDepth5(Camera.W, Camera.H, 0.25, False)
        ZEDOF = New ZstackStructure(Camera.W, Camera.H, Setting.Gett("ZSTACRRANGE"), Setting.Gett("ZSTACKSTEPS"))
        TextBox21.Text = Setting.Gett("ZSTACRRANGE")
        TextBox22.Text = Setting.Gett("ZSTACKSTEPS")
        'Triangle = New TriangulationStructure(340, 1078, 2600, 700)
        If Camera.status Then
            TextBox_exposure.Text = Camera.exp
            AutoFocus = New FocusStructure(2, 0.1, 4)
            Display = New ImageDisplay(Camera.W, Camera.H, Chart1)
            Display.imagetype = ImagetypeEnum.Brightfield
        End If

        Stage = New ZaberNew(Setting.Gett("FOVX"), Setting.Gett("FOVY"))
        TextBox_FOVX.Text = Stage.FOVX
        TextBox_FOVY.Text = Stage.FOVY

        'Piezo = New EO(10)


        TextBoxGain.Text = Setting.Gett("Gain")
        TextBox_GainB.Text = Setting.Gett("GainB")
        TextBox_GainG.Text = Setting.Gett("GainG")
        TextBox_GainR.Text = Setting.Gett("GainR")

        TextBox_exposure.Text = Setting.Gett("exposure")


        If Camera.status Then


            Camera.SetFlatField("ff.tif", "dark.tif")

            GoLive()
            LEDcontroller = New Relay
            LEDcontroller.SetRelays(1, True)
            LEDcontroller.SetRelays(2, False)

            ArrangeControls(10)

        End If

        GetPreview(True)
    End Sub


    Sub ArrangeControls(d As Integer)
        Dim scale As Single = 0.34 * 2708 / 2048

        PictureBox0.Width = Display.Width * scale
        PictureBox0.Height = Display.Height * scale
        PictureBox0.SizeMode = PictureBoxSizeMode.Zoom

        PictureBox1.Width = Display.Width * scale
        PictureBox1.Height = Display.Height * scale
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom

        PictureBox0.Top = d
        PictureBox0.Left = d

        PictureBox1.Top = d
        PictureBox1.Left = d


        PictureBox2.Width = Display.Width * scale
        PictureBox2.Height = Display.Height * scale
        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom

        PictureBox2.Top = d
        PictureBox2.Left = d

        TabControl1.Width = Display.Width * scale + 2 * d
        TabControl1.Height = Display.Height * scale + 2 * d

        TabControl2.Left = TabControl1.Width + d
        TabControl2.Width = Me.ClientSize.Width - TabControl1.Width - d
        TabControl2.Height = TabControl1.Height
        PictureBox_Preview.Left = d
        PictureBox_Preview.Top = d
        PictureBox_Preview.Width = TabControl2.Width - 2 * d - GroupBox3.Height * 1.5

        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()

        GroupBox3.Left = PictureBox_Preview.Left
        GroupBox3.Top = PictureBox_Preview.Top + PictureBox_Preview.Height + d
        TabControl_Settings.Top = d
        TabControl_Settings.Left = d + d
        Chart1.Left = GroupBox3.Left + GroupBox3.Width + d
        Chart1.Top = GroupBox3.Top
        Chart1.Height = GroupBox3.Height

        ListBox1.Left = Chart1.Left + Chart1.Width + d
        ListBox1.Top = GroupBox3.Top
        ListBox1.Height = GroupBox3.Height
        Button_GIMP.Left = ListBox1.Left + ListBox1.Width + d
        Button_GIMP.Top = ListBox1.Top
        Button_Luigi.Left = Button_GIMP.Left
        Button_Luigi.Top = Button_GIMP.Top + Button_GIMP.Height

        Button_Sedeen.Left = Button_GIMP.Left
        Button_Sedeen.Top = Button_Luigi.Top + Button_Luigi.Height
    End Sub

    Public Sub ChangeExposure()



        Camera.exp = Val(TextBox_exposure.Text)

        Select Case Display.imagetype
            Case ImagetypeEnum.Brightfield
                Setting.Sett("EXPOSUREB", Camera.exp)
            Case ImagetypeEnum.Fluorescence
                Setting.Sett("EXPOSUREF", Camera.exp)
        End Select
        Setting.Sett("EXPOSURE", Camera.exp)
        Timer1.Interval = 1
        If Camera.exp > 1 Then Timer1.Interval = Camera.exp
        Camera.ExposureChanged = 0

        'Do Until Camera.ExposureChanged = False

        'Loop
        'Display.AdjustBrightness()
    End Sub



    Public Sub GoLive()
        Timer1.Interval = 1
        If Camera.exp > 1 Then Timer1.Interval = Camera.exp
        Timer1.Enabled = True

        Dim Thread1 As New System.Threading.Thread(AddressOf Live)
        Thread1.Start()


    End Sub

    Public Sub ExitLive()
        If Camera.status = False Then Exit Sub
        Camera.Dostop = True
        Do Until Camera.busy = False

        Loop
        Camera.Dostop = False
        Timer1.Enabled = False
    End Sub

    Private Sub Button_right_Click(sender As Object, e As EventArgs) Handles Button_right.Click

        Stage.MoveRelative(Stage.Xaxe, -Stage.FOVX)
        ExitEDOf()
    End Sub

    Private Sub Button_left_Click(sender As Object, e As EventArgs) Handles Button_left.Click

        Stage.MoveRelative(Stage.Xaxe, Stage.FOVX)
        ExitEDOf()
    End Sub

    Private Sub Button_top_Click(sender As Object, e As EventArgs) Handles Button_top.Click

        Stage.MoveRelative(Stage.Yaxe, -Stage.FOVY)
        ExitEDOf()
    End Sub

    Private Sub Button_bottom_Click(sender As Object, e As EventArgs) Handles Button_bottom.Click

        Stage.MoveRelative(Stage.Yaxe, Stage.FOVY)
        ExitEDOf()
    End Sub

    Private Sub Button_adjustBrightness_Click(sender As Object, e As EventArgs) Handles Button_adjustBrightness.Click

        Display.AdjustBrightness()


    End Sub

    Public Sub ExitEDOf()

        If Display.imagetype = ImagetypeEnum.EDF_Brightfield Then TabControl1.SelectedIndex = 0
        If Display.imagetype = ImagetypeEnum.EDF_Fluorescence Then TabControl1.SelectedIndex = 1
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Camera.status = True Then
            ExitLive()

        End If
        Preview.StopPreview()
        LEDcontroller.LED_OFF()
    End Sub

    Private Sub RadioButton_zoom_in_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_zoom_in.CheckedChanged
        If RadioButton_zoom_in.Checked Then
            Display.zoom = True
            PictureBox0.SizeMode = PictureBoxSizeMode.CenterImage
            PictureBox1.SizeMode = PictureBoxSizeMode.CenterImage
            PictureBox2.SizeMode = PictureBoxSizeMode.CenterImage
        Else
            PictureBox0.SizeMode = PictureBoxSizeMode.Zoom
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
            PictureBox2.SizeMode = PictureBoxSizeMode.Zoom
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = Keys.Return Then

            Stage.MoveRelative(Stage.Zaxe, Val(TextBox3.Text))

        End If
    End Sub



    Private Sub TextBox_GainR_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainR.KeyDown
        If e.KeyCode = Keys.Return Then
            Try
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text), Display.imagetype)
            Catch ex As Exception

            End Try

        End If
    End Sub

    Private Sub TextBox_GainG_TextChanged(sender As Object, e As EventArgs) Handles TextBox_GainG.TextChanged

    End Sub

    Private Sub TextBox_GainG_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainG.KeyDown
        If e.KeyCode = Keys.Return Then
            Try
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text), Display.imagetype)
            Catch ex As Exception

            End Try

        End If
    End Sub



    Private Sub TextBox_GainB_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainB.KeyDown
        If e.KeyCode = Keys.Return Then
            Try
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text), Display.imagetype)
            Catch ex As Exception

            End Try

        End If
    End Sub

    Private Sub TextBoxGain_TextChanged(sender As Object, e As EventArgs) Handles TextBoxGain.TextChanged

    End Sub

    Private Sub TextBoxGain_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxGain.KeyDown
        If e.KeyCode = Keys.Return Then
            Camera.setGain(Val(TextBoxGain.Text))
        End If
    End Sub


    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs)

    End Sub


    Public Sub Live()
        Dim Charttest As New Chart
        Charttest = Chart1
        Dim BmpPreview As New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
        Do
            Camera.busy = True
            If Camera.Dostop Then Exit Do

            If Camera.ExposureChanged = 0 Then Camera.SetExposure() : Display.AdjustBrightness() : Camera.ExposureChanged = 1
            If Display.RequestIbIc = 0 Then Camera.ResetMatrix() : Display.RequestIbIc = 1
            Camera.Capture()

            If Display.imagetype = ImagetypeEnum.Brightfield Then PictureBox0.Image = Display.Preview(Camera.Bytes, True)
            If Display.imagetype = ImagetypeEnum.Fluorescence Then PictureBox1.Image = Display.Preview(Camera.Bytes, True)

            Application.DoEvents()
        Loop
        Camera.busy = False

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If Not Camera.busy Then Exit Sub

        CheckBoxLED.Checked = True

        If TabControl1.SelectedIndex = 0 Then

            LEDcontroller.SetRelays(2, False)
            LEDcontroller.SetRelays(1, True)
            If Display.imagetype = ImagetypeEnum.EDF_Fluorescence Or Display.imagetype = ImagetypeEnum.Fluorescence Then
                Display.imagetype = ImagetypeEnum.Brightfield
                Camera.SetFlatField("ff.tif", "dark.tif")

                TextBox_exposure.Text = Setting.Gett("Exposureb")
                TextBox_GainB.Text = Setting.Gett("GainB")
                TextBox_GainG.Text = Setting.Gett("GainG")
                TextBox_GainR.Text = Setting.Gett("GainR")
                Display.SetColorGain(Setting.Gett("GainR"), Setting.Gett("GainG"), Setting.Gett("GainB"), ImagetypeEnum.Brightfield)
                ChangeExposure()
                'Display.AdjustBrightness()

            End If
            Display.imagetype = ImagetypeEnum.Brightfield


        End If

        If TabControl1.SelectedIndex = 1 Then

            LEDcontroller.SetRelays(1, False)
            LEDcontroller.SetRelays(2, True)

            If Display.imagetype = ImagetypeEnum.EDF_Brightfield Or Display.imagetype = ImagetypeEnum.Brightfield Then
                Display.imagetype = ImagetypeEnum.Fluorescence
                Camera.SetFlatField("ff_FiBi.tif", "dark.tif")


                TextBox_exposure.Text = Setting.Gett("Exposuref")
                TextBox_GainB.Text = Setting.Gett("GainB_FiBi")
                TextBox_GainG.Text = Setting.Gett("GainG_FiBi")
                TextBox_GainR.Text = Setting.Gett("GainR_FiBi")
                Display.SetColorGain(Setting.Gett("GainR_FiBi"), Setting.Gett("GainG_FiBi"), Setting.Gett("GainB_FiBi"), ImagetypeEnum.Fluorescence)
                ChangeExposure()
                'Display.AdjustBrightness()

            End If
            Display.imagetype = ImagetypeEnum.Fluorescence


        End If

        If TabControl1.SelectedIndex = 2 Then

            If Display.imagetype = ImagetypeEnum.Fluorescence Then Display.imagetype = ImagetypeEnum.EDF_Fluorescence : PictureBox2.Image = PictureBox1.Image
            If Display.imagetype = ImagetypeEnum.Brightfield Then Display.imagetype = ImagetypeEnum.EDF_Brightfield : PictureBox2.Image = PictureBox0.Image
            Dim ccMatrix As Single = Camera.CCMAtrix
            ExitLive() : Camera.ResetMatrix()


            ZEDOF.AcquireThreaded(True)
            'ZEDOF.Acquire()
            Dim bmp As New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
            'byteToBitmap(ZEDOF.OutputBytes, bmp)

            Display.ApplyBrightness(ZEDOF.OutputBytes, ccMatrix, bmp)
            PictureBox2.Image = bmp
            CheckBoxLED.Checked = False
            GoLive()


        End If
    End Sub

    Private Sub Button_Brightfield_Acquire_Click(sender As Object, e As EventArgs) Handles Button_Brightfield_Acquire.Click
        Acquire()
    End Sub

    Public Sub Acquire()
        ExitLive() : Camera.ResetMatrix()
        Thread.Sleep(500)

        Dim bmp As New Bitmap(Camera.captureBmp)
        SaveFileDialog1.DefaultExt = ".jpg"
        If SaveFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub



        Select Case Display.imagetype
            Case ImagetypeEnum.Brightfield


                bmp.Save(SaveFileDialog1.FileName)
                'Display.MakeFullsizeImage.Save(SaveFileDialog1.FileName + "_WD.jpg")
                ReDim Preserve Filenames(fileN)
                Filenames(fileN) = SaveFileDialog1.FileName
                fileN += 1
                ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
            Case ImagetypeEnum.Fluorescence


                bmp.Save(SaveFileDialog1.FileName)

                ReDim Preserve Filenames(fileN)
                Filenames(fileN) = SaveFileDialog1.FileName
                fileN += 1
                ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
            Case ImagetypeEnum.EDF_Brightfield

                ReDim Preserve Filenames(fileN)
                bmp = New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
                byteToBitmap(ZEDOF.OutputBytes, bmp)
                bmp.Save(SaveFileDialog1.FileName)
                ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
                Filenames(fileN) = SaveFileDialog1.FileName
                '  GoLive()
            Case ImagetypeEnum.EDF_Fluorescence
                ReDim Preserve Filenames(fileN)
                bmp = New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
                byteToBitmap(ZEDOF.OutputBytes, bmp)
                bmp.Save(SaveFileDialog1.FileName)
                Filenames(fileN) = SaveFileDialog1.FileName
                ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
        End Select



        GoLive()
        Display.AdjustBrightness()

    End Sub



    Private Sub PictureBox0_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseDown, PictureBox1.MouseDown, PictureBox2.MouseDown
        Stage.xp = e.X
        Stage.yp = e.Y

    End Sub

    Private Sub PictureBox0_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseUp, PictureBox1.MouseUp, PictureBox2.MouseDown

        Dim Z As Integer = 1
        '   If Display.zoom Then Z = Display.sampeling Else Z = 1

        Stage.MoveRelativeAsync(Stage.Xaxe, (e.X - Stage.xp) * Stage.FOVX * (1 / Z) / PictureBox0.Width)
        Stage.MoveRelativeAsync(Stage.Yaxe, -(e.Y - Stage.yp) * Stage.FOVY * (1 / Z) / PictureBox0.Height)
        ExitEDOf()

    End Sub

    Public Function DoAutoFocus(DoInitialize As Boolean, DoRelease As Boolean) As Single
        Stage.GoZero(Stage.Zaxe, block)

        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True


        Dim focus As Single
        If DoInitialize Then AutoFocus.Initialize()
        focus = AutoFocus.Analyze()
        If DoRelease Then AutoFocus.Release()

        'if camera is stopped because  of this sub then it resumes the live.
        If WasLive Then GoLive()

        Return focus
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        DoAutoFocus(True, True)
    End Sub



    Private Sub Button_Home_Click(sender As Object, e As EventArgs) Handles Button_Home.Click
        Stage.Go_Middle()
    End Sub



    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.MoveRelative(Stage.Xaxe, TextBox1.Text)
        End If

    End Sub

    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.MoveRelative(Stage.Yaxe, TextBox2.Text)
        End If
    End Sub
    Public Sub SetScan()
        If Slideloaded Then Button_Scan.Enabled = True
        TextBoxX.Text = Tracking.ROIX
        TextBoxY.Text = Tracking.ROIY

    End Sub



    Private Sub Button_Scan_Click(sender As Object, e As EventArgs) Handles Button_Scan.Click
        If Scanning Then Scanning = False : Button_Scan.Text = "Scan" : Exit Sub
        SaveFileDialog1.DefaultExt = ".tif"
        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub
        SaveFileDialog1.AddExtension = True

        Dim watch As Stopwatch
        watch = New Stopwatch
        watch.Start()
        CheckBoxLED.Checked = True
        Scanning = True
        Button_Scan.Text = "Cancel"
        FastScan(TextBoxX.Text, TextBoxY.Text, 0.00, SaveFileDialog1.FileName)
        If Scanning = True Then CheckBoxLED.Checked = False Else CheckBoxLED.Checked = True
        If Scanning = False Then GoTo 2
        watch.Stop()

        MsgBox("Scanned in " + (watch.ElapsedMilliseconds / 1000).ToString + " s")

        ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
        ReDim Preserve Filenames(fileN)
        Filenames(fileN) = SaveFileDialog1.FileName
        fileN += 1
2:

        Scanning = False
        Button_Scan.Text = "Scan"
    End Sub

    Public Sub FastScan(X As Integer, y As Integer, overlap As Single, Address As String)

        If Camera.busy Then ExitLive() : Camera.ResetMatrix()

        'Camera.SetPolicyToSafe()



        Select Case Display.imagetype
            Case ImagetypeEnum.Brightfield
                Camera.SetFlatField("ff.tif", "dark.tif")

            Case ImagetypeEnum.Fluorescence
                Camera.SetFlatField("ff_FiBi.tif", "dark.tif")

        End Select

        Dim Filen As Integer = 1
        Dim direction As Integer = 1

        ' Creating overlap to enhance the stitching with ICE
        Dim AdjustedStepX As Single = Stage.FOVX * (1 - overlap)
        Dim AdjustedStepY As Single = Stage.FOVY * (1 - overlap)

        Dim cx, cy, cz As Single
        Stage.UpdatePositions()
        cx = Stage.X
        cy = Stage.Y
        cz = Stage.Z


        Pbar.Visible = True
        Pbar.Maximum = X * y

        Dim Pattern(X * y - 1) As Integer
        Dim FrameIndex As Integer = 0

        Dim Axis As String = ""
        Dim watch As New Stopwatch

        Dim Pyramid(3) As Pyramids

        Pyramid(0) = New Pyramids(X, y, Camera.W, Camera.H, 4, 0, Address, 100)
        If Pyramid(0).TiffisOpen Then MsgBox("tHE FILE IS ALREADY OPEN SOMEWHERE ELSE. pLEASE CLOSE IT FIRSt. ") : Scanning = False : GoTo 1
        Pyramid(1) = New Pyramids(X, y, Camera.W, Camera.H, 4, 1, Address + "1", 100)
        Pyramid(2) = New Pyramids(X, y, Camera.W, Camera.H, 4, 2, Address + "2", 100)
        Pyramid(3) = New Pyramids(X, y, Camera.W, Camera.H, 4, 3, Address + "3", 100)

        Dim ColorBytes(Camera.W * Camera.H * 3 - 1)

        'Dim Fit As New Polynomial_fit
        Dim A(4) As Double



        'If Tracking.ROI.IsMade And Not CheckBox2.Checked Then


        '    Dim vx(Tracking.ROI.numDots * 2 - 1) As Single
        '    Dim vy(Tracking.ROI.numDots - 1) As Single

        '    AutoFocus.Initialize()
        '    For i = 0 To Tracking.ROI.numDots - 1
        '        Tracking.MovetoDots(i)

        '        vy(i) = DoAutoFocus(False, False)


        '        vx(i * 2) = Stage.X
        '        vx(i * 2 + 1) = Stage.Y
        '        Dim imgtest(Camera.Wbinned * Camera.Hbinned - 1) As Byte
        '        Camera.Capture()
        '        SaveSinglePageTiff("c:\temp\" + i.ToString + ".tif", Camera.Bytes, Camera.Wbinned, Camera.Hbinned)
        '        If Scanning = False Then AutoFocus.Release() : GoTo 1

        '    Next
        '    AutoFocus.Release()
        '    A = Fit.Main(vx, vy, 1000, 0.0000001)
        '    If A.Sum = 0 Then
        '        MsgBox("Predicytive focus couldn't be estimated. Try moving your points inside the tissue.")
        '        Scanning = False : GoTo 1
        '    End If

        '    Tracking.MovetoROIEdge()
        '    Dim X0 As Single = Stage.X
        '    Dim Y0 As Single = Stage.Y

        'End If


        'Stage.SetAcceleration(Stage.Zaxe, AutoFocus.Zacceleration)
        'If Tracking.ROI.IsMade Then
        '    Tracking.MovetoROIEdge()
        '    If Not CheckBox2.Checked Then Stage.MoveAbsolute(Stage.Zaxe, Fit.ComputeE({Stage.X, Stage.Y}, A))


        'End If

        Tracking.MovetoROIEdge()
        ' Dim BytesExport(Camera.Bytes.GetUpperBound(0)) As Byte
        For loop_y = 1 To y
            For loop_x = 1 To X
                Pbar.Increment(1)
                If Scanning = False Then GoTo 1
                If CheckBox2.Checked Then
                    ZEDOF.AcquireThreaded(False, False)

                Else

                    'Camera.Capture_Threaded()
                    'Thread.Sleep(Camera.exp * 1.2)

                    Camera.Capture()
                End If

                'Moves while it generates the preview and others.
                If loop_x < X Then
                    If CheckBox2.Checked Then

                        Stage.MoveRelativeAsync(Stage.Xaxe, -AdjustedStepX * direction, False) : Axis = "X"
                    Else
                        Stage.MoveRelative(Stage.Xaxe, -AdjustedStepX * direction, False) : Axis = "X"
                    End If


                    Stage.X += -AdjustedStepX * direction


                Else
                    If loop_y < y Then
                        If CheckBox2.Checked Then
                            Stage.MoveRelativeAsync(Stage.Yaxe, AdjustedStepY, False) : Axis = "y"
                        Else
                            Stage.MoveRelative(Stage.Yaxe, AdjustedStepY, False) : Axis = "y"
                        End If


                        Stage.Y += AdjustedStepY


                    End If
                End If
                If CheckBox2.Checked Then ZEDOF.Wrapup()
                If Tracking.ROI.IsMade And Not CheckBox2.Checked Then
                    'Stage.MoveAbsolute(Stage.Zaxe, Fit.ComputeE({Stage.X, Stage.Y}, A), False)

                    Do Until Camera.ready

                    Loop


                End If

                Do Until Pyramid(0).Ready And Pyramid(1).Ready And Pyramid(2).Ready And Pyramid(3).Ready

                Loop



                'If direction > 0 Then
                '    Tiles.SaveTile(loop_x - 1, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'Else
                '    Tiles.SaveTile(X - loop_x, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'End If


                If direction > 0 Then
                    For i = 0 To Pyramid(0).pages - 1

                        If CheckBox2.Checked Then
                            Pyramid(i).SaveTile(loop_x - 1, loop_y - 1, (ZEDOF.OutputBytes))
                        Else
                            Pyramid(i).SaveTile(loop_x - 1, loop_y - 1, (Camera.Bytes))
                        End If

                    Next
                Else
                    For i = 0 To Pyramid(0).pages - 1

                        If CheckBox2.Checked Then
                            Pyramid(i).SaveTile(X - loop_x, loop_y - 1, (ZEDOF.OutputBytes))
                        Else
                            Pyramid(i).SaveTile(X - loop_x, loop_y - 1, (Camera.Bytes))
                        End If



                    Next
                End If



                If Axis = "X" Then
                    Filen += direction
                    FrameIndex += 1
                Else
                    Filen += X
                    direction = direction * -1

                End If
                Application.DoEvents()

            Next

        Next


        Do Until Pyramid(0).Ready And Pyramid(1).Ready And Pyramid(2).Ready And Pyramid(3).Ready

        Loop
        For i = 1 To Pyramid(0).pages - 1
            Pyramid(i).Close()
        Next


        Pyramid(0).AssemblePyramid({Pyramid(0), Pyramid(1), Pyramid(2), Pyramid(3)})


        For i = 1 To Pyramid(0).pages - 1
            If File.Exists(Pyramid(i).address) Then Kill(Pyramid(i).address)
        Next



1:
        Stage.MoveAbsoluteAsync(Stage.Xaxe, cx)
        Stage.MoveAbsoluteAsync(Stage.Yaxe, cy)
        Stage.MoveAbsoluteAsync(Stage.Zaxe, cz)
        ZEDOF.direction = 1
        Pbar.Value = 0
        'Camera.SetPolicyToUNSafe()
        'Camera.Flatfield(0)
        'Camera.SetDataMode(Colortype.Grey)
        'Stage.SetAcceleration(Stage.Zaxe, Stage.Zacc)
        GoLive()
        Display.AdjustBrightness()


    End Sub


    Public Sub MakeMontage(bmp() As Bitmap, x As Integer, y As Integer)

        Dim width As Integer = bmp(0).Width
        Dim height As Integer = bmp(0).Height
        Dim BmpMontage = New Bitmap(x * width, y * height)

        Dim gr As Graphics = Graphics.FromImage(BmpMontage)
        Dim i As Integer

        Pbar.Value = 0
        Application.DoEvents()
        For loop_y = 0 To y - 1
            For loop_x = 0 To x - 1
                gr.DrawImage(bmp(i), loop_x * width, loop_y * height, width, height)
                Pbar.Increment(1)
                Application.DoEvents()
                i = i + 1
            Next
        Next
        Pbar.Value = 0
        BmpMontage.Save("d:\1.png")
    End Sub




    Private Sub TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.MoveAbsolute(Stage.Xaxe, TextBox4.Text)
        End If
    End Sub


    Private Sub TextBox5_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox5.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.MoveAbsolute(Stage.Yaxe, TextBox5.Text)
        End If
    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

    End Sub

    Private Sub TextBox6_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox6.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.MoveAbsolute(Stage.Zaxe, TextBox6.Text)
        End If
    End Sub

    Private Sub RadioButton_Conversion_CheckedChanged(sender As Object, e As EventArgs)

    End Sub




    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Stage.CalibrateZoffset(AutoFocus.Range)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Tracking.clear()
    End Sub

    Private Sub RadioButton_zoom_out_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_zoom_out.CheckedChanged

    End Sub

    Private Sub PictureBox_Preview_Click(sender As Object, e As EventArgs) Handles PictureBox_Preview.Click

    End Sub

    Private Sub Textbox_exposure_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True

        Camera.Capture()

        Camera.Flatfield(0)
        Camera.SetROI()
        Camera.SetDataMode(Colortype.Grey)
        Camera.SetROI()

        CheckBoxLED.Checked = False
        Thread.Sleep(500)
        Camera.Capture()
        SaveSinglePageTiff16("dark.tif", Camera.Bytes, Camera.W, Camera.H)
        CheckBoxLED.Checked = True
        Thread.Sleep(500)

        Dim Flatfield(Camera.W * Camera.H - 1) As Single
        Dim Flatfieldbytes(Camera.W * Camera.H - 1) As Byte
        Dim direction As Integer = 1
        For y = 1 To 5
            For x = 1 To 5
                'Stage.MoveRelative(Stage.Xaxe, direction * Stage.FOVX / 10)
                Camera.Capture()
                For i = 0 To Camera.W * Camera.H - 1
                    Flatfield(i) += Camera.Bytes(i)
                Next
            Next
            'Stage.MoveRelative(Stage.Yaxe, Stage.FOVY / 10)
            direction *= -1
        Next
        'Stage.MoveRelative(Stage.Yaxe, -5 * Stage.FOVY / 10)
        'Stage.MoveRelative(Stage.Xaxe, -5 * Stage.FOVX / 10)


        Dim BLure = New FFTW_VB_Real(Camera.W, Camera.H)
        BLure.MakeGaussianReal(0.1, BLure.MTF, 2)
        BLure.UpLoad(Flatfield)
        BLure.Process_FT_MTF()
        BLure.DownLoad(Flatfield)


        'For i = 0 To Camera.W * Camera.H - 1
        '    If Flatfield(i) > 255 Then Flatfield(i) = 255
        '    Flatfieldbytes(i) = Flatfield(i)
        'Next

        Select Case Display.imagetype
            Case ImagetypeEnum.Brightfield
                SaveSinglePageTiff16("ff.tif", Flatfield, Camera.W, Camera.H)
            Case ImagetypeEnum.Fluorescence
                SaveSinglePageTiff16("ff_FiBi.tif", Flatfield, Camera.W, Camera.H)
        End Select



        Camera.SetDataMode(Colortype.RGB)
        Camera.SetROI()
        Camera.SetDataMode(Colortype.RGB)
        Camera.Capture()


        Select Case Display.imagetype
            Case ImagetypeEnum.Brightfield
                Camera.SetFlatField("ff.tif", "dark.tif")

            Case ImagetypeEnum.Fluorescence
                Camera.SetFlatField("ff_FiBi.tif", "dark.tif")

        End Select



        Camera.Capture()
        If WasLive Then GoLive()
    End Sub



    Private Sub Button_Acquire_fLUORESCENT_Click(sender As Object, e As EventArgs)
        Acquire()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        GetPreview()

    End Sub
    Public Sub GetPreview(Optional wait As Boolean = True)
        Stage.MoveAbsolute(Stage.Zaxe, 0)
        Stage.MoveAbsolute(Stage.Yaxe, 0)



        If wait Then
            Stage.MoveAbsolute(Stage.Xaxe, 0)
            If MsgBox("Load the sample. Is this a block?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then block = True Else block = False

        End If
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 20)
        Tracking.UpdateBmp(Preview.Capture(Val(TextBox_PrevieEXp.Text), Val(TextBox_PreviewFocus.Text)))

        Stage.MoveAbsolute(Stage.Zaxe, 0)
        Stage.Go_Middle()
        'stage.MoveAbsolute(stage.Zaxe, lastZ)

        Tracking.Pbox.Image = Tracking.bmp.bmp

        Slideloaded = True
        Button_Scan.Enabled = True

        Stage.GoToFocus(block)
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Stage.GoToFocus(block)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxLED.CheckedChanged

        UpdateLED(CheckBoxLED.Checked)

    End Sub

    Public Sub UpdateLED(status As Boolean)

        If Display IsNot Nothing Then
            If status Then
                If Display.imagetype = ImagetypeEnum.Brightfield Then

                    LEDcontroller.SetRelays(2, False)
                    LEDcontroller.SetRelays(1, True)
                Else

                    LEDcontroller.SetRelays(1, False)
                    LEDcontroller.SetRelays(2, True)
                End If

            Else
                LEDcontroller.SetRelays(1, False)
                LEDcontroller.SetRelays(2, False)

            End If
        End If
    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button_GIMP.Click


        Try
            Process.Start("C:\Program Files\GIMP 2\bin\gimp-2.10.exe ", Chr(34) + Filenames(ListBox1.SelectedIndex) + Chr(34))
        Catch
            MsgBox("No image file is found", MsgBoxStyle.Critical)
        End Try


    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        Stage.MoveAbsolute(Stage.Zaxe, 0)
        Stage.MoveAbsolute(Stage.Yaxe, 0)
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 20)

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Stage.MoveAbsolute(Stage.Zaxe, 0)
        Stage.Go_Middle()
        Stage.GoToFocus(block)
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Preview.CaptureWhole(Val(TextBox_PrevieEXp.Text), Val(TextBox_PreviewFocus.Text)).Save("c:\temp\whole.jpg")

        Tracking.UpdateBmp(Preview.Capture(Val(TextBox_PrevieEXp.Text), Val(TextBox_PreviewFocus.Text)))
        Preview.bmpf.Save("C:\temp\preview.jpg")
        PictureBox_Preview.Image = Tracking.bmp.bmp

    End Sub

    Private Sub TextBox_PrevieEXp_TextChanged(sender As Object, e As EventArgs) Handles TextBox_PrevieEXp.TextChanged

    End Sub

    Private Sub TextBox_PrevieEXp_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_PrevieEXp.KeyDown
        If e.KeyCode = Keys.Return Then
            Setting.Sett("PREVIEWEXP", TextBox_PrevieEXp.Text)
        End If

    End Sub
    Private Sub TextBox_PreviewFocus_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_PreviewFocus.KeyDown
        If e.KeyCode = Keys.Return Then
            Setting.Sett("PREVIEWFOCUS", TextBox_PreviewFocus.Text)
        End If

    End Sub

    Private Sub PictureBox0_Click(sender As Object, e As EventArgs) Handles PictureBox0.Click

    End Sub

    Private Sub PictureBox0_KeyDown(sender As Object, e As KeyEventArgs) Handles PictureBox0.KeyDown

    End Sub

    Private Sub PictureBox0_KeyUp(sender As Object, e As KeyEventArgs) Handles PictureBox0.KeyUp

    End Sub

    Private Sub TextBox7_TextChanged(sender As Object, e As EventArgs) Handles TextBox7.TextChanged

    End Sub

    Private Sub TextBox7_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox7.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetSpeed(Stage.Xaxe, TextBox7.Text)
        End If
    End Sub



    Private Sub TextBox8_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox8.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetSpeed(Stage.Yaxe, TextBox8.Text)
        End If
    End Sub


    Private Sub TextBox9_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox9.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetSpeed(Stage.Zaxe, TextBox9.Text)
        End If
    End Sub



    Private Sub TextBox10_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox10.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetAcceleration(Stage.Xaxe, TextBox10.Text)
        End If
    End Sub

    Private Sub TextBox11_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox11.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetAcceleration(Stage.Yaxe, TextBox11.Text)
        End If
    End Sub


    Private Sub TextBox12_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox12.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetAcceleration(Stage.Zaxe, TextBox12.Text)
        End If
    End Sub

    Private Sub TextBox_GainR_TextChanged(sender As Object, e As EventArgs) Handles TextBox_GainR.TextChanged

    End Sub





    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click

        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True

        AutoFocus.Calibrate(Pbar)

        'if camera is stopped because  of this sub then it resumes the live.
        If WasLive Then GoLive()

    End Sub




    Private Sub TextBox13_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_exposure.KeyDown
        If e.KeyCode = Keys.Return Then
            ChangeExposure()

        End If
    End Sub




    Private Sub PictureBox_MouseWheel(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseWheel, PictureBox1.MouseWheel, PictureBox2.MouseWheel
        If Focusing = True Then Exit Sub
        Focusing = True
        ExitEDOf()
        Dim speed As Single
        If System.Windows.Forms.Control.ModifierKeys = Keys.Control Then speed = 20 Else speed = 5

        'If XYZ.name = "NewPort" Then
        If e.Delta > 0 Then
            Stage.MoveRelative(Stage.Zaxe, speed * 0.001 * Math.Abs(e.Delta) / 120)
        Else
            Stage.MoveRelative(Stage.Zaxe, speed * -0.001 * Math.Abs(e.Delta) / 120)
        End If
        Focusing = False
    End Sub

    Private Sub TextBox15_TextChanged(sender As Object, e As EventArgs) Handles TextBox15.TextChanged

    End Sub

    Private Sub TextBox15_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox15.KeyDown
        If e.KeyCode = Keys.Return Then
            Camera.SetMatrix(TextBox15.Text)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Display.MakeHistogram()
        Display.PlotHistogram()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button_Luigi.Click
        'Try
        '    Dim viewer As New LuigiViewer.DisplayForm(Filenames(ListBox1.SelectedIndex))
        '    viewer.Show()

        'Catch
        '    MsgBox("No image file is found", MsgBoxStyle.Critical)
        'End Try

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button_Sedeen_Click(sender As Object, e As EventArgs) Handles Button_Sedeen.Click
        Try
            Process.Start("C:\Program Files\Sedeen Viewer\sedeen.exe", Chr(34) + Filenames(ListBox1.SelectedIndex) + Chr(34))
        Catch
            MsgBox("No image file is found", MsgBoxStyle.Critical)
        End Try

    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click

        Tracking.MovetoNextDots()

    End Sub

    Private Sub TextBox_FOVX_TextChanged(sender As Object, e As EventArgs) Handles TextBox_FOVX.TextChanged

    End Sub

    Private Sub TextBox_FOVX_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_FOVX.KeyDown, TextBox_FOVY.KeyDown
        If e.KeyCode = Keys.Return Then
            Stage.SetFOV(TextBox_FOVX.Text, TextBox_FOVY.Text)
        End If
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        If Camera.busy Then ExitLive()
        StopAlign = False
        Dim Montage As New Bitmap(Camera.W * 4, Camera.H * 4)
        Dim BMParray(3) As Bitmap
        For i = 0 To 3
            BMParray(i) = New Bitmap(Camera.W, Camera.H)
        Next

        Dim croppedsize As Integer = TextBox16.Text

        Dim cropped As New Bitmap(croppedsize, croppedsize)
        Dim g As Graphics
        g = Graphics.FromImage(Montage)


        For i = 0 To 3
            If i = 1 Then Stage.MoveRelative(Stage.Xaxe, -Stage.FOVX)
            If i = 2 Then Stage.MoveRelative(Stage.Yaxe, Stage.FOVY)
            If i = 3 Then Stage.MoveRelative(Stage.Xaxe, Stage.FOVX)


            BMParray(i) = New Bitmap(Camera.captureBmp)

            If i = 0 Then g.DrawImage(BMParray(i), New Point(0, 0))
            If i = 1 Then g.DrawImage(BMParray(i), New Point(Camera.W, 0))
            If i = 2 Then g.DrawImage(BMParray(i), New Point(Camera.W, Camera.H))
            If i = 3 Then g.DrawImage(BMParray(i), New Point(0, Camera.H))
            Application.DoEvents()

        Next
        Stage.MoveRelative(Stage.Yaxe, -Stage.FOVY)
        cropped = Montage.Clone(New Rectangle(Camera.W - croppedsize / 2, Camera.H - croppedsize / 2, croppedsize, croppedsize), Imaging.PixelFormat.Format24bppRgb)
        PictureBox0.Image = cropped
        cropped.Save("c:\temp\cropped.jpg")
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        StopAlign = True
        If Not Camera.busy Then GoLive()
    End Sub

    Private Sub TabPage6_Click(sender As Object, e As EventArgs) Handles TabPage6.Click

    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        'stage.SetSpeed(stage.Zport, 500000)
        If MsgBox("Load the Calibration slide and hit OK.", MsgBoxStyle.OkCancel) = MsgBoxResult.Cancel Then Exit Sub
        Stage.MoveAbsolute(Stage.Zaxe, 0)
        Stage.MoveAbsolute(Stage.Yaxe, 0)
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 20)
        'MsgBox("Load the sample and then hit OK.")
        Tracking.UpdateBmp(Preview.CaptureROI(TextBox_PrevieEXp.Text, TextBox_PreviewFocus.Text))

        Stage.Go_Middle()
        'stage.MoveAbsolute(stage.Zaxe, lastZ)

        Tracking.Pbox.Image = Tracking.bmp.bmp

        Slideloaded = True
        Button_Scan.Enabled = True

        Stage.GoToFocus(block)



    End Sub

    Private Sub TextBox_PreviewFocus_TextChanged(sender As Object, e As EventArgs) Handles TextBox_PreviewFocus.TextChanged

    End Sub

    Private Sub Button18_Click_1(sender As Object, e As EventArgs) Handles Button18.Click
        'TabControl1.SelectedIndex = 0
        'TabControl1.SelectedIndex = 0

        ''EO.Move_A(0)
        'Ximea.exposure = Val(TextBox_exposure.Text)
        'Ximea.SetExposure(Val(TextBox_exposure.Text))
        'ExitLive()
        ''Dim SuperFrmeStack As New Stack
        '' ReDim SuperFrmeStack.bmp(1)
        ''SuperFrmeStack.bmp(0) = New Bitmap(Ximea.bmpRef)

        'Ximea.SetImagingFormat(8)
        'Ximea.SetExposure(Val(TextBox_exposure.Text))
        'Ximea.TRG_MODE = 3
        'Ximea.StartAcquisition()
        'EO.initialDelay = 0
        'EO.setSleep()
        'EO.retrn = True
        'Dim Thread2 As New System.Threading.Thread(AddressOf EO.PiezoScan)
        'Thread2.Start()
        'Ximea.capture()
        'Dim watch As New Stopwatch
        'watch.Start()
        'Ximea.bmpRef = EDF.analyze(Ximea.bmpRef)
        'watch.Stop()
        ''MsgBox(watch.ElapsedMilliseconds)

        ''SuperFrmeStack.bmp(1) = EDF.SuperFrame.bmpRGB
        ''SuperFrmeStack.bmp(1) = New Bitmap(Ximea.bmpRef)
        ''SuperFrmeStack.bmp(3) = New Bitmap(Ximea.bmpRef.Width, Ximea.bmpRef.Height)

        'Ximea.SetImagingFormat(24)
        ''CoolBright()
        'Ximea.StopAcquisition()
        'Muse.Display.imagetype = " <b> DuperFrame </b>, CutOff=  " + EDF.CutOff.ToString
        'SaveFrame()

        ''SuperFrmeStack.MakeMontage(2, 1)
        ''SuperFrmeStack.SaveMontage(2, 1, False)
        '' Ximea.Start()
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click





        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub

        ExitLive()

        Camera.SetROI()
        Camera.SetDataMode(Colortype.Grey)
        Camera.SetROI()




        Dim numZ As Integer = 20

        Dim ZupperFrame(numZ - 1)() As Byte

        Dim watch As New Stopwatch

        Pbar.Maximum = 11
        For loop_Z = 0 To numZ - 1
            ReDim ZupperFrame(loop_Z)(Camera.W * Camera.H - 1)
            Camera.Capture()

            Buffer.BlockCopy(Camera.Bytes, 0, ZupperFrame(loop_Z), 0, Camera.Bytes.Length)
            Pbar.Increment(1)
            Application.DoEvents()
            Stage.MoveRelative(Stage.Zaxe, 0.002, False)
        Next
        Stage.MoveRelativeAsync(Stage.Zaxe, -0.002 * numZ, False)
        SaveJaggedArray(ZupperFrame, Camera.W, Camera.H, SaveFileDialog1.FileName + ".tif")

        Pbar.Value = 0

        Camera.SetDataMode(Colortype.RGB)
        Camera.SetROI()
        Camera.SetDataMode(Colortype.RGB)

        'Display.BayerInterpolate(EDF.AnalyzeZuper(ZupperFrame), Camera.BmpRef)
        'Camera.BmpRef.Save(SaveFileDialog1.FileName + ".jpg")


        GoLive()
    End Sub



    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Setting.Sett("Xmin", Stage.X)
        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Setting.Sett("ymin", Stage.Y)
        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()
    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        Setting.Sett("Xmax", Stage.X)
        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        Setting.Sett("ymax", Stage.Y)
        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()
    End Sub

    Private Sub TextBox18_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox18.KeyDown
        If e.KeyCode = Keys.Return Then
            Piezo.MoveRelative(TextBox18.Text)
        End If
    End Sub

    Private Sub TextBox17_TextChanged(sender As Object, e As EventArgs) Handles TextBox17.TextChanged

    End Sub

    Private Sub TextBox17_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox17.KeyDown
        If e.KeyCode = Keys.Return Then
            Piezo.MoveAbsolute(TextBox17.Text)
        End If
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click

        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub

        ExitLive()
        Camera.StopAcqusition()
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        Camera.StartAcqusition()

        Piezo.MoveAbsolute(0)
        Camera.Capture()

        Piezo.setSleep(Camera.exp)

        Piezo.MakeDelay()
        Dim Thread2 As New System.Threading.Thread(AddressOf Piezo.Scan)
        Thread2.Start()
        'Piezo.Scan()
        'Camera.Capture_Threaded()
        Camera.Capture()

        SaveSinglePageTiff(SaveFileDialog1.FileName + ".tif", Camera.Bytes, Camera.W, Camera.H)
        Piezo.MoveAbsolute(0)

        'Dim BMP As New Bitmap(Camera.W, Camera.H)
        'Display.BayerInterpolate(Camera.Bytes, BMP)
        'PictureBox1.Image = BMP

        Camera.SetDataMode(Colortype.RGB)
        GoLive()

    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        ExitLive()
        Camera.StopAcqusition()
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        Camera.StartAcqusition()

        Piezo.MoveAbsolute(0)

        Piezo.Scan()
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        Camera.SetDataMode(Colortype.RGB)
        GoLive()
    End Sub

    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True

        Piezo.Calibrate(Pbar)

        'if camera is stopped because  of this sub then it resumes the live.
        If WasLive Then GoLive()

    End Sub

    Private Sub CheckBox1_CheckedChanged_1(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            LEDcontroller.SetRelays(3, True)
        Else
            LEDcontroller.SetRelays(3, False)
        End If
    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        ExitLive()
        Pbar.Maximum = 200
        For i = 0 To 200
            Camera.captureBmp()
            Camera.BmpRef.Save("C:\temp\Laser line generator Triangulation\" + i.ToString + ".jpg")
            Stage.MoveRelative(Stage.Zaxe, 0.001)
            Pbar.Increment(1)
            Application.DoEvents()
        Next
        GoLive()
        Pbar.Value = 0
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        ExitLive()
        Triangle.Initialize()
        Triangle.Capture(TextBox20.Text, TextBox19.Text)
        Triangle.release()
        GoLive()

    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBoxY_TextChanged(sender As Object, e As EventArgs) Handles TextBoxY.TextChanged

    End Sub



    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        ExitLive() : Camera.ResetMatrix()
    End Sub

    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click


        ZEDOF.Acquire 
        Dim bmp As New Bitmap(Camera.W, Camera.H, Imaging.PixelFormat.Format24bppRgb)
        byteToBitmap(ZEDOF.OutputBytes, bmp)
        PictureBox0.Image = bmp


    End Sub

    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        GoLive()
    End Sub



    Private Sub TextBox21_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox21.KeyDown, TextBox22.KeyDown
        If e.KeyCode = Keys.Return Then
            ZEDOF = New ZstackStructure(Camera.W, Camera.H, TextBox21.Text, TextBox22.Text)
            Setting.Sett("ZSTACRRANGE", TextBox21.Text)
            Setting.Sett("ZSTACKSTEPS", TextBox22.Text)
        End If
    End Sub


    Private Sub TextBox21_TextChanged(sender As Object, e As EventArgs) Handles TextBox21.TextChanged

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Camera.Flatfield(1)
    End Sub

    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        Camera.Flatfield(0)
    End Sub
End Class


