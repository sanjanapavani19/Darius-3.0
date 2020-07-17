Imports System.ComponentModel
Imports System.IO
Imports AForge.Imaging.Filters
Imports Microsoft.VisualBasic.Devices
Imports AForge.Imaging
Imports System.Threading

Public Class Form1

    Public Display As ImageDisplay

    Public LEDcontroller As Relay

    Dim IsDragging As Boolean
    Dim CollagenImage As StackImage
    Dim Concatenate As Single(,,)
    Dim Imagetype As ImagetypeEnum
    Dim AutoFocus As FocusStructure
    Dim Slideloaded As Boolean
    Dim LinearUnmixing As LinearUnmixingStructure
    Dim FocusMap(,) As Single
    Dim panel As Integer
    Dim Filenames() As String
    Dim fileN As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        Preview = New PreviewStructure
        TextBox_PrevieEXp.Text = Setting.Gett("PREVIEWEXP")
        TextBox_PreviewFocus.Text = Setting.Gett("PREVIEWFOCUS")


        Imagetype = ImagetypeEnum.Brightfield
        Camera = New XimeaColor


        If Camera.status Then
            Textbox_exposure.Text = Camera.exp
            AutoFocus = New FocusStructure(0.5, 5, 4)

            Display = New ImageDisplay(Camera.Dim_X, Camera.Dim_Y, 2)

        End If
        Stage = New ZaberNew(Setting.Gett("FOVX"), Setting.Gett("FOVX") * Camera.Dim_Y / Camera.Dim_X)




        TextBoxGain.Text = Setting.Gett("Gain")
        Textbox_exposure.Text = Setting.Gett("exposure")


        If Camera.status Then
            CollagenImage = New StackImage(Camera.Dim_X, Camera.Dim_Y, 4, Imaging.PixelFormat.Format32bppArgb)
            ReDim Concatenate(Camera.Dim_X, Camera.Dim_Y, 5)

            Camera.Flatfield(0)
            'Camera.SetFlatField("ff.tif")
            GoLive()
            LEDcontroller = New Relay
            LEDcontroller.SetRelays(1, True)
            LEDcontroller.SetRelays(2, False)

            Tracking = New TrackingStructure(PictureBox_Preview)
            Tracking.Update()
            ArrangeControls(10)

        End If
        'comes down to focus
        Stage.MoveRelative(Stage.Zaxe, AutoFocus.Range / 2)
    End Sub


    Sub ArrangeControls(d As Integer)
        Dim scale As Single = 0.65



        PictureBox0.Width = Display.BmpPreview.width * scale
        PictureBox0.Height = Display.BmpPreview.height * scale
        PictureBox0.SizeMode = PictureBoxSizeMode.Zoom

        PictureBox1.Width = Display.BmpPreview.width * scale
        PictureBox1.Height = Display.BmpPreview.height * scale
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom


        TabControl1.Width = Display.BmpPreview.width * (0.1 + scale)
        TabControl1.Height = Display.BmpPreview.width * (0.1 + scale)


        PictureBox_Preview.Left = TabControl1.Left + TabControl1.Width + d
        PictureBox_Preview.Top = TabControl1.Top


        GroupBox3.Left = PictureBox_Preview.Left
        GroupBox3.Top = PictureBox_Preview.Top + PictureBox_Preview.Height + d
        TabControl_Settings.Top = GroupBox3.Top + GroupBox3.Height + d
        TabControl_Settings.Left = GroupBox3.Left
        TextBoxGain.Text = Setting.Gett("Gain")
        TextBox_GainB.Text = Setting.Gett("GainB")
        TextBox_GainG.Text = Setting.Gett("GainG")
        TextBox_GainR.Text = Setting.Gett("GainR")
        ListBox1.Left = PictureBox_Preview.Width + PictureBox_Preview.Left + d
    End Sub



    Private Sub Textbox_exposure_KeyDown(sender As Object, e As KeyEventArgs) Handles Textbox_exposure.KeyDown
        If e.KeyCode = Keys.Return Then
            ChangeExposure()

        End If
    End Sub

    Public Sub ChangeExposure()


        Camera.exp = Val(Textbox_exposure.Text)
        Camera.ExposureChanged = True

        Select Case Imagetype
            Case ImagetypeEnum.Brightfield
                Setting.Sett("EXPOSUREB", Camera.exp)
            Case ImagetypeEnum.Fluorescence
                Setting.Sett("EXPOSUREF", Camera.exp)
        End Select
        Setting.Sett("EXPOSURE", Camera.exp)



        'Do Until Camera.ExposureChanged = False

        'Loop
        'Display.AdjustBrightness()
    End Sub



    Public Sub GoLive()
        Dim Thread1 As New System.Threading.Thread(AddressOf Live)
        Thread1.Start()
        '    Live()
    End Sub



    Public Sub Live()
        '' Setting the waithandle to false because the initial setting was true.
        ''WaitHandle_LiveReturned.Reset()

        Do
            Camera.busy = True
            If Camera.Dostop Then Exit Do

            Camera.Capture()
            Application.DoEvents()
            If Camera.ExposureChanged Then Camera.SetExposure() : Camera.ExposureChanged = False


            If Imagetype = ImagetypeEnum.Brightfield Then
                Display.Preview(Camera.Bytes, True)
                PictureBox0.Image = Display.BmpPreview.bmp
            End If

            If Imagetype = ImagetypeEnum.Fluorescence Then
                Display.Preview(Camera.Bytes, True)
                PictureBox1.Image = Display.BmpPreview.bmp
            End If


        Loop
        Camera.busy = False


    End Sub

    Public Sub ExitLive()
        If Camera.status = False Then Exit Sub
        Camera.Dostop = True
        Do Until Camera.busy = False

        Loop
        Camera.Dostop = False
    End Sub

    Private Sub Button_right_Click(sender As Object, e As EventArgs) Handles Button_right.Click
        If Not Camera.busy Then GoLive()
        Stage.MoveRelative(Stage.Xaxe, -Stage.FOVX)
    End Sub

    Private Sub Button_left_Click(sender As Object, e As EventArgs) Handles Button_left.Click
        If Not Camera.busy Then GoLive()
        Stage.MoveRelative(Stage.Xaxe, Stage.FOVX)
    End Sub

    Private Sub Button_top_Click(sender As Object, e As EventArgs) Handles Button_top.Click
        If Not Camera.busy Then GoLive()
        Stage.MoveRelative(Stage.Yaxe, -Stage.FOVY)
    End Sub

    Private Sub Button_bottom_Click(sender As Object, e As EventArgs) Handles Button_bottom.Click
        If Not Camera.busy Then GoLive()
        Stage.MoveRelative(Stage.Yaxe, Stage.FOVY)
    End Sub

    Private Sub Button_adjustBrightness_Click(sender As Object, e As EventArgs) Handles Button_adjustBrightness.Click
        Display.AdjustBrightness()
        Display.PlotHistogram(Chart1)

    End Sub

    Private Sub Form1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        If Not Camera.busy Then GoLive()
        Dim speed As Single
        If Control.ModifierKeys = Keys.Control Then speed = 10 Else speed = 1

        'If XYZ.name = "NewPort" Then
        If e.Delta > 0 Then
            Stage.MoveRelative(Stage.Zaxe, speed * 0.001 * Math.Abs(e.Delta) / 120)
        Else
            Stage.MoveRelative(Stage.Zaxe, speed * -0.001 * Math.Abs(e.Delta) / 120)
        End If

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
        Else
            Display.zoom = False
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
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))
            Catch ex As Exception

            End Try

        End If
    End Sub

    Private Sub TextBox_GainG_TextChanged(sender As Object, e As EventArgs) Handles TextBox_GainG.TextChanged

    End Sub

    Private Sub TextBox_GainG_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainG.KeyDown
        If e.KeyCode = Keys.Return Then
            Try
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))
            Catch ex As Exception

            End Try

        End If
    End Sub



    Private Sub TextBox_GainB_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainB.KeyDown
        If e.KeyCode = Keys.Return Then
            Try
                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))
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



    Private Sub Button_Save_Click(sender As Object, e As EventArgs)
        If SaveFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub

        Dim filename As String = Path.GetFileName(SaveFileDialog1.FileName)
        Dim Dirname As String = SaveFileDialog1.FileName
        Directory.CreateDirectory(Dirname)
        CollagenImage.Bitmp(0).bmp.Save(Dirname & "\Brighfield-" & filename & ".jpg")
        CollagenImage.Bitmp(1).bmp.Save(Dirname & "\Fluorescent-" & filename & ".jpg")
        ' it resets the unmixing to original ....
        'CollagenImage.Bitmp(2).Reset()
        'CollagenImage.Bitmp(2).bmp.Save(Dirname & "\Unmixed-" & filename & ".jpg")
        'CollagenImage.Bitmp(3).bmp.Save(Dirname & "\Overlaid-" & filename & ".jpg")

        'SaveMultipageTiff(Dirname & "\Concatenate-" & filename & ".tif", Concatenate)

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs)

    End Sub


    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) _
         Handles TabControl1.SelectedIndexChanged
        If Not Camera.busy Then Exit Sub
        CheckBox1.Checked = True
        If TabControl1.SelectedIndex = 0 Then

            'ExitLive()
            Imagetype = ImagetypeEnum.Brightfield
            '  Camera.setGain(0)
            ' TextBoxGain.Text = Setting.Gett("Gain")

            'Textbox_exposure.Text = get
            LEDcontroller.SetRelays(2, False)
            LEDcontroller.SetRelays(1, True)
            Button_adjustBrightness.PerformClick()
            Textbox_exposure.Text = Setting.Gett("Exposureb")
            ChangeExposure()
            'GoLive()

        End If

        If TabControl1.SelectedIndex = 1 Then

            'ExitLive()
            Imagetype = ImagetypeEnum.Fluorescence
            'Textbox_exposure.Text = 0.1
            LEDcontroller.SetRelays(1, False)
            LEDcontroller.SetRelays(2, True)
            Button_adjustBrightness.PerformClick()
            'Camera.setGain(30)
            Textbox_exposure.Text = Setting.Gett("Exposuref")
            ChangeExposure()

            'TextBoxGain.Text = Setting.Gett("Gain")
            'GoLive()
        End If

    End Sub

    Private Sub Button_Brightfield_Acquire_Click(sender As Object, e As EventArgs) Handles Button_Brightfield_Acquire.Click
        Acquire()
    End Sub

    Public Sub Acquire()
        If SaveFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub
        If Camera.busy Then ExitLive()
        If Imagetype = ImagetypeEnum.Brightfield Then
            Camera.SetDataMode(Colortype.RGB)
            Camera.captureBmp.Save(SaveFileDialog1.FileName + "_WD.jpg")
            Camera.SetDataMode(Colortype.Grey)
            'Display.MakeFullsizeImage.Save(SaveFileDialog1.FileName + "_WD.jpg")
            ReDim Preserve Filenames(fileN)
            Filenames(fileN) = SaveFileDialog1.FileName + "_WD.jpg"
            fileN += 1
            ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName + "_WD.jpg"))
        ElseIf Imagetype = ImagetypeEnum.Fluorescence Then
            Camera.SetDataMode(Colortype.RGB)
            Camera.captureBmp.Save(SaveFileDialog1.FileName + "_FiBi.jpg")
            Camera.SetDataMode(Colortype.Grey)
            ReDim Preserve Filenames(fileN)
            Filenames(fileN) = SaveFileDialog1.FileName + "_FiBi.jpg"
            fileN += 1
            ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName + "_FiBi.jpg"))
        End If

        GoLive()

    End Sub



    Private Sub PictureBox0_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseDown, PictureBox1.MouseDown
        Stage.xp = e.X
        Stage.yp = e.Y

    End Sub

    Private Sub PictureBox0_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseUp, PictureBox1.MouseUp

        Dim Z As Integer
        If Display.zoom Then Z = Display.sampeling Else Z = 1
        Stage.MoveRelative(Stage.Xaxe, (e.X - Stage.xp) * Stage.FOVX * (1 / Z) / PictureBox0.Width)
        Stage.MoveRelative(Stage.Yaxe, -(e.Y - Stage.yp) * Stage.FOVY * (1 / Z) / PictureBox0.Height)


    End Sub

    Public Function DoAutoFocus(position As Integer)
        'Stage.GoZero(Stage.Zaxe, 1)

        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True

        Dim focus As Single
        AutoFocus.Initialize()
        focus = AutoFocus.Analyze()
        AutoFocus.Release()

        'if camera is stopped because  of this sub then it resumes the live.
        If WasLive Then GoLive()

        Return focus
    End Function

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        DoAutoFocus(1)
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
        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub
        Dim watch As Stopwatch
        watch = New Stopwatch
        watch.Start()
        FastScan(TextBoxX.Text, TextBoxY.Text, 0.05, SaveFileDialog1.FileName)
        watch.Stop()
        MsgBox("Scanned in " + (watch.ElapsedMilliseconds / 1000).ToString + " s")

        ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
        ReDim Preserve Filenames(fileN)
        Filenames(fileN) = SaveFileDialog1.FileName
        fileN += 1
    End Sub

    Public Sub FastScan(X As Integer, y As Integer, overlap As Single, Address As String)
        If Camera.busy Then ExitLive()


        Camera.SetDataMode(Colortype.RGB)
        Camera.SetFlatField("ff.tif", "dark.tif")


        'Camera.SetPolicyToSafe()
        Dim TravelX, TravelY As Single
        Dim Filen As Integer = 1
        Dim direction As Integer = 1

        ' Creating overlap to enhance the stitching with ICE
        Dim AdjustedStepX As Single = Stage.FOVX * (1 - overlap)
        Dim AdjustedStepY As Single = Stage.FOVY * (1 - overlap)

        Pbar.Visible = True
        Pbar.Maximum = X * y

        Dim Pattern(X * y - 1) As Integer
        Dim FrameIndex As Integer = 0

        Dim Axis As String = ""
        Dim watch As New Stopwatch


        Dim Tiles As New TileStructure(X, y, Camera.Dim_X, Camera.Dim_Y, 1, Address, 100)
        Dim ColorBytes(Camera.Dim_X * Camera.Dim_Y * 3 - 1)
        ' Dim BytesExport(Camera.Bytes.GetUpperBound(0)) As Byte
        For loop_y = 1 To y
            For loop_x = 1 To X
                Pbar.Increment(1)

                Camera.Capture_Threaded()
                Thread.Sleep(Camera.exp * 1200)


                'Moves while it generates the preview and others.
                If loop_x < X Then

                    Stage.MoveRelative(Stage.Xaxe, -AdjustedStepX * direction) : Axis = "X"

                    TravelX += AdjustedStepX * direction
                Else
                    If loop_y < y Then
                        Stage.MoveRelative(Stage.Yaxe, AdjustedStepY) : Axis = "y"
                        TravelY += AdjustedStepY
                    End If
                End If

                Do Until Camera.ready

                Loop
                Do Until Tiles.Ready

                Loop



                'If direction > 0 Then
                '    Tiles.SaveTile(loop_x - 1, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'Else
                '    Tiles.SaveTile(X - loop_x, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'End If


                If direction > 0 Then
                    Tiles.SaveTile(loop_x - 1, loop_y - 1, (Camera.Bytes))
                Else
                    Tiles.SaveTile(X - loop_x, loop_y - 1, (Camera.Bytes))
                End If
                'byteToBitmap(Camera.Bytes, Display.Bmp)
                'Array.Copy(Camera.GetBytes, BytesExport, BytesExport.GetLength(0))


                'Display.Bmp.Save("C:\temp\" + Filen.ToString + "RGB.bmp")
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

        Stage.MoveRelative(Stage.Xaxe, TravelX)
        Stage.MoveRelative(Stage.Yaxe, -TravelY)
        Do Until Tiles.Ready

        Loop
        Tiles.Close()
        'MakeMontage(X, Y, Bmp, True)

        Pbar.Value = 0
        'Camera.SetPolicyToUNSafe()
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        GoLive()



    End Sub

    Public Sub SciScan()
        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub

        ExitLive()

        Dim X, Y As Integer
        Y = TextBoxY.Text
        X = TextBoxX.Text
        Dim filen As Integer
        Dim direction As Integer
        Dim AdjustedStepX, AdjustedStepY As Single
        AdjustedStepX = Stage.FOVX
        AdjustedStepY = Stage.FOVY
        Dim bmp(X * Y - 1) As Bitmap
        Pbar.Maximum = X * Y
        direction = 1
        Tracking.ROI.IsMade = False
        ReDim FocusMap(X, Y)
        If Tracking.ROI.IsMade Then


            Dim A, B, C, D As Single
            Dim X0, X1, X2, X3 As Single
            Dim Y0, Y1, Y2, Y3 As Single
            Dim Z1, Z2, Z3 As Single


            Tracking.MovetoDots(0)
            DoAutoFocus(0)
            Z1 = Stage.Z
            X1 = Stage.X
            Y1 = Stage.Y

            Tracking.MovetoDots(1)
            DoAutoFocus(0)
            Z2 = Stage.Z
            X2 = Stage.X
            Y2 = Stage.Y


            Tracking.MovetoDots(2)
            DoAutoFocus(0)
            Z3 = Stage.Z
            X3 = Stage.X
            Y3 = Stage.Y



            A = (Y2 - Y1) * (Z3 - Z1) - (Y3 - Y1) * (Z2 - Z1)
            B = (Z2 - Z1) * (X3 - X1) - (Z3 - Z1) * (X2 - X1)
            C = (X2 - X1) * (Y3 - Y1) - (X3 - X1) * (Y2 - Y1)
            D = -(A * X1 + B * Y1 + C * Z1)

            Tracking.MovetoROIEdge()
            X0 = Stage.X
            Y0 = Stage.Y

            For j = 1 To Y
                For i = 1 To X
                    If j Mod 2 = 0 Then
                        FocusMap(i, j) = -(A * (X0 + (X - i) * -AdjustedStepX) + B * (Y0 + (j - 1) * AdjustedStepY) + D) / C
                    Else

                        FocusMap(i, j) = -(A * (X0 + (i - 1) * -AdjustedStepX) + B * (Y0 + (j - 1) * AdjustedStepY) + D) / C
                    End If

                Next
            Next


        End If
        Preview.Bmp = New Bitmap(256, 256, Imaging.PixelFormat.Format24bppRgb)
        Tracking.UpdateBmp(Preview.Bmp)
        Dim Outputfile As New BinaryFileStructure(SaveFileDialog1.FileName, FileMode.Append)
        Dim FocusmapFile As System.IO.StreamWriter = New StreamWriter("C:\temp\focusmap.text")
        Camera.SetDataMode(Colortype.Grey)


        Camera.Capture()

        'Dim Resize As New ResizeBilinear(Camera.Dim_X * 2, Camera.Dim_Y * 2)
        'Resize.Apply(Camera.BmpRef)

        'writes the version
        Outputfile.write(2)

        'The number of frmaes at both directions 
        Outputfile.write(X)
        Outputfile.write(Y)
        'bitmap width  and height 
        Outputfile.write(Camera.Dim_X)
        Outputfile.write(Camera.Dim_Y)



        'now writing the preview bitmap
        Outputfile.write(Tracking.bmp.width)
        Outputfile.write(Tracking.bmp.bmp.Height)
        Outputfile.write(Tracking.bmp.bytes.GetLength(0))
        Outputfile.write(Tracking.bmp.bytes, Tracking.bmp.bytes.Length)

        'Writing the ROI properties 
        If Tracking.ROI.IsMade Then
            ' to show that ROI is there 
            Outputfile.write(1)
            Outputfile.write(Tracking.ROI.Rect.Width)
            Outputfile.write(Tracking.ROI.Rect.Height)
            Outputfile.write(Tracking.ROI.Rect.Y)
            Outputfile.write(Tracking.ROI.Rect.X)
        Else
            ' to show that ROI is not there
            Outputfile.write(0)

            Outputfile.write(0)
            Outputfile.write(0)
            Outputfile.write(0)
            Outputfile.write(0)

        End If
        ' version 2 adds a byte to indicate the type of scan
        Dim ScanType As Byte
        If RadioButton1.Checked Then ScanType = 1
        If RadioButton2.Checked Then ScanType = 2
        If RadioButton3.Checked Then ScanType = 3
        Outputfile.write(ScanType)
        ' now preserve the space for future developments. 
        Dim bytes(999) As Byte
        Outputfile.write(bytes, 999)
        'The returned frmae from the camera is 1 byte longer!
        Dim framelength = Camera.Dim_X * Camera.Dim_Y


        Dim loop_x, loop_y As Integer

        Dim Fx, Fy As Integer

        For loop_y = 1 To Y

            If loop_y > 1 Then
                Stage.MoveRelative(Stage.Yaxe, AdjustedStepY)

                ' stage.MoveAbsolute(stage.Zaxe, FocusMap(X, loop_y))
                filen = filen + X + direction
                direction = direction * -1
                Fy += 1
                If direction < 0 Then Fx = X - 1 Else Fx = 0
            End If

            For loop_x = 1 To X
                '  stage.MoveAbsolute(stage.Zaxe, FocusMap(loop_x, loop_y))
                If loop_x > 1 Then
                    Stage.MoveRelative(Stage.Xaxe, -AdjustedStepX * direction)
                End If


                Pbar.Increment(1)
                'LEDcontroller.SetRelays(1, True)
                FocusMap(loop_x - 1, loop_y - 1) = DoAutoFocus(0)
                Camera.Capture()
                '  LEDcontroller.SetRelays(1, False)
                Display.ApplyColorGain(Camera.Bytes, Camera.Dim_X, Camera.Dim_Y)
                Outputfile.write(Camera.Bytes, framelength)

                'If Camera.exp < 0.1 Then Threading.Thread.Sleep(100)
                ' LEDcontroller.SetRelays(2, True)
                'Camera.capture()
                'LEDcontroller.SetRelays(2, False)
                'Outputfile.write(Camera.frame, framelength)
                Application.DoEvents()

                If direction < 0 Then
                    Fx -= 1
                Else
                    Fx += 1
                End If


            Next
        Next

        Dim Outputstring As String
        For j = 0 To Y - 1
            Outputstring = ""
            For i = 0 To X - 1
                If i < X - 1 Then Outputstring += FocusMap(i, j).ToString + "," Else Outputstring += FocusMap(i, j).ToString
            Next
            FocusmapFile.WriteLine(Outputstring)
        Next
        FocusmapFile.Close()


        LEDcontroller.SetRelays(1, True)
        LEDcontroller.SetRelays(2, False)

        'stage.MoveAbsolute(stage.Xaxe, 0)
        'stage.MoveAbsolute(stage.Yaxe, 0)

        Pbar.Value = 0
        'MakeMontage(bmp, X, Y)
        Outputfile.CLOSE()
        GoLive()
        Button_Scan.Enabled = False
        TextBoxX.Text = 0
        TextBoxY.Text = 0
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



    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        DoAutoFocus(0)
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
        Stage.MoveRelative(Stage.Zaxe, -AutoFocus.Range / 2)
        Dim ZZ As Single = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("ZOFFSET", ZZ)
        Stage.StorePosition(Stage.Zaxe, 1)
        Stage.MoveRelative(Stage.Zaxe, AutoFocus.Range / 2)
        ZZ = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("Focus", ZZ)
        Stage.StorePosition(Stage.Zaxe, 2)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Tracking.clear()
    End Sub

    Private Sub RadioButton_zoom_out_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_zoom_out.CheckedChanged

    End Sub

    Private Sub PictureBox_Preview_Click(sender As Object, e As EventArgs) Handles PictureBox_Preview.Click

    End Sub

    Private Sub Textbox_exposure_Click(sender As Object, e As EventArgs) Handles Textbox_exposure.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True
        Camera.Capture()
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        CheckBox1.Checked = False
        Thread.Sleep(500)
        Camera.Capture()
        SaveSinglePageTiff16("dark.tif", Camera.Bytes, Camera.Dim_X, Camera.Dim_Y)
        CheckBox1.Checked = True
        Thread.Sleep(500)
        Camera.Capture()
        SaveSinglePageTiff16("ff.tif", Camera.Bytes, Camera.Dim_X, Camera.Dim_Y)
        Camera.SetFlatField("ff.tif", "dark.tif")
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        If WasLive Then GoLive()
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click

        Camera.SetFlatField("ff.tif", "dark.tif")

    End Sub


    Private Sub Button_Acquire_fLUORESCENT_Click(sender As Object, e As EventArgs)
        Acquire()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        Stage.MoveAbsolute(Stage.Yaxe, 0)
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 0)
        MsgBox("Load the sample and then hit OK.")

        Tracking.UpdateBmp(Preview.Capture(Val(TextBox_PrevieEXp.Text), Val(TextBox_PreviewFocus.Text)))

        Stage.Go_Middle()
        'stage.MoveAbsolute(stage.Zaxe, lastZ)

        PictureBox_Preview.Image = Tracking.bmp.bmp

        Slideloaded = True
        Button_Scan.Enabled = True

        Stage.GoToFocus()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Stage.GoZero(Stage.Zaxe, 1)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        Try
            If CheckBox1.Checked Then
                If Imagetype = ImagetypeEnum.Brightfield Then

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
        Catch ex As Exception

        End Try








    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click


        Try
            Process.Start("C:\Program Files\GIMP 2\bin\gimp-2.10.exe ", Chr(34) + Filenames(ListBox1.SelectedIndex) + Chr(34))
        Catch
            MsgBox("No image file is found", MsgBoxStyle.Critical)
        End Try


    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        Stage.MoveAbsolute(Stage.Yaxe, 0)
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 0)

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click

        stage.Go_Middle()
        Stage.GoToFocus()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
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
End Class

