Imports System.ComponentModel
Imports System.IO
Imports AForge.Imaging.Filters
Imports Microsoft.VisualBasic.Devices
Imports AForge.Imaging
Imports System.Threading
Imports System.Windows.Forms.DataVisualization.Charting

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
    Dim Focusing As Boolean
    Dim Filenames() As String
    Dim fileN As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim vx() As Single = {17.036356, 31.4184856, 12.8087683, 31.370985, 17.0838566, 35.74095, 12.90377, 35.8359528, 14.5188036, 33.9834137}
        'Dim vy() As Single = {10.8271627, 10.9771814, 10.72715, 10.7571545, 10.7371511}
        'Dim fit As New Polynomial_fit
        'Dim A(4) As Double
        'A = fit.Main(vx, vy, 1000, 0.001)

        Preview = New PreviewStructure
        TextBox_PrevieEXp.Text = Setting.Gett("PREVIEWEXP")
        TextBox_PreviewFocus.Text = Setting.Gett("PREVIEWFOCUS")

        Imagetype = ImagetypeEnum.Brightfield
        Camera = New XimeaColor

        If Camera.status Then
            TextBox_exposure.Text = Camera.exp
            AutoFocus = New FocusStructure(2, 0.1, 4)
            Display = New ImageDisplay(Camera.W, Camera.H, Chart1)
        End If
        Stage = New ZaberNew(Setting.Gett("FOVX"), Setting.Gett("FOVX") * Camera.H / Camera.W)


        TextBoxGain.Text = Setting.Gett("Gain")
        TextBox_exposure.Text = Setting.Gett("exposure")


        If Camera.status Then
            CollagenImage = New StackImage(Camera.W, Camera.H, 4, Imaging.PixelFormat.Format32bppArgb)
            ReDim Concatenate(Camera.W, Camera.H, 5)

            Camera.SetFlatField("ff.tif", "dark.tif")

            GoLive()
            LEDcontroller = New Relay
            LEDcontroller.SetRelays(1, True)
            LEDcontroller.SetRelays(2, False)

            Tracking = New TrackingStructure(PictureBox_Preview)
            Tracking.Update()
            ArrangeControls(10)

        End If

        GetPreview()
    End Sub


    Sub ArrangeControls(d As Integer)
        Dim scale As Single = 0.33

        PictureBox0.Width = Display.Width * scale
        PictureBox0.Height = Display.Height * scale
        PictureBox0.SizeMode = PictureBoxSizeMode.Zoom

        PictureBox1.Width = Display.Width * scale
        PictureBox1.Height = Display.Height * scale
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom


        TabControl1.Width = Display.Width * scale + 2 * d
        TabControl1.Height = Display.Height * scale + 2 * d


        PictureBox_Preview.Left = TabControl1.Left + TabControl1.Width + d
        PictureBox_Preview.Top = TabControl1.Top


        GroupBox3.Left = PictureBox_Preview.Left
        GroupBox3.Top = PictureBox_Preview.Top + PictureBox_Preview.Height + d
        TabControl_Settings.Top = GroupBox3.Top
        TabControl_Settings.Left = GroupBox3.Left + GroupBox3.Width + d
        Chart1.Left = GroupBox3.Left
        Chart1.Top = GroupBox3.Top + GroupBox3.Height + d
        TextBoxGain.Text = Setting.Gett("Gain")
        TextBox_GainB.Text = Setting.Gett("GainB")
        TextBox_GainG.Text = Setting.Gett("GainG")
        TextBox_GainR.Text = Setting.Gett("GainR")
        ListBox1.Left = PictureBox_Preview.Width + PictureBox_Preview.Left + d
    End Sub

    Public Sub ChangeExposure()

        Camera.ExposureChanged = True
        Camera.exp = Val(TextBox_exposure.Text)

        Select Case Imagetype
            Case ImagetypeEnum.Brightfield
                Setting.Sett("EXPOSUREB", Camera.exp)
            Case ImagetypeEnum.Fluorescence
                Setting.Sett("EXPOSUREF", Camera.exp)
        End Select
        Setting.Sett("EXPOSURE", Camera.exp)
        Timer1.Interval = Camera.exp


        'Do Until Camera.ExposureChanged = False

        'Loop
        'Display.AdjustBrightness()
    End Sub



    Public Sub GoLive()
        Timer1.Enabled = True
        Timer1.Interval = Camera.exp
        Dim Thread1 As New System.Threading.Thread(AddressOf Live)
        Thread1.Start()


    End Sub



    Public Sub Live()
        '' Setting the waithandle to false because the initial setting was true.
        ''WaitHandle_LiveReturned.Reset()



        Do
            Camera.busy = True
            If Camera.Dostop Then Exit Do

            Camera.Capture()
            If Camera.ExposureChanged Then Camera.SetExposure() : Camera.ExposureChanged = False : Display.RequestIbIc = True

            If Imagetype = ImagetypeEnum.Brightfield Then PictureBox0.Image = Display.Preview(Camera.Bytes, True)
            If Imagetype = ImagetypeEnum.Fluorescence Then PictureBox1.Image = Display.Preview(Camera.Bytes, True)

            Application.DoEvents()
        Loop
        Camera.busy = False


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
        Else
            PictureBox0.SizeMode = PictureBoxSizeMode.Zoom
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        End If
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = Keys.Return Then
            Dim watch As New Stopwatch
            watch.Start()
            Stage.MoveRelative(Stage.Zaxe, Val(TextBox3.Text))
            watch.Stop()
            ' MsgBox(watch.ElapsedMilliseconds)
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
        CheckBoxLED.Checked = True
        If TabControl1.SelectedIndex = 0 Then

            'ExitLive()
            Imagetype = ImagetypeEnum.Brightfield
            '  Camera.setGain(0)
            ' TextBoxGain.Text = Setting.Gett("Gain")

            'Textbox_exposure.Text = get
            LEDcontroller.SetRelays(2, False)
            LEDcontroller.SetRelays(1, True)
            Button_adjustBrightness.PerformClick()
            TextBox_exposure.Text = Setting.Gett("Exposureb")
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
            TextBox_exposure.Text = Setting.Gett("Exposuref")
            ChangeExposure()

            'TextBoxGain.Text = Setting.Gett("Gain")
            'GoLive()
        End If

    End Sub

    Private Sub Button_Brightfield_Acquire_Click(sender As Object, e As EventArgs) Handles Button_Brightfield_Acquire.Click
        Acquire()
    End Sub

    Public Sub Acquire()
        ExitLive()

        Dim bmp As New Bitmap(Camera.captureBmp)
        SaveFileDialog1.DefaultExt = ".jpg"
        If SaveFileDialog1.ShowDialog() = DialogResult.Cancel Then Exit Sub
        If Imagetype = ImagetypeEnum.Brightfield Then

            bmp.Save(SaveFileDialog1.FileName + "_WD.jpg")
            'Display.MakeFullsizeImage.Save(SaveFileDialog1.FileName + "_WD.jpg")
            ReDim Preserve Filenames(fileN)
            Filenames(fileN) = SaveFileDialog1.FileName + "_WD.jpg"
            fileN += 1
            ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName + "_WD.jpg"))
        ElseIf Imagetype = ImagetypeEnum.Fluorescence Then


            bmp.Save(SaveFileDialog1.FileName + "_FiBi.jpg")

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

        Dim Z As Integer = 1
        '   If Display.zoom Then Z = Display.sampeling Else Z = 1
        Stage.MoveRelativeAsync(Stage.Xaxe, (e.X - Stage.xp) * Stage.FOVX * (1 / Z) / PictureBox0.Width)
        Stage.MoveRelativeAsync(Stage.Yaxe, -(e.Y - Stage.yp) * Stage.FOVY * (1 / Z) / PictureBox0.Height)


    End Sub

    Public Function DoAutoFocus(position As Integer) As Single
        Stage.GoZero(Stage.Zaxe, 1)

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
        'SaveFileDialog1.DefaultExt = ".tif"
        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub
        '  SaveFileDialog1.AddExtension = True

        Dim watch As Stopwatch
        watch = New Stopwatch
        watch.Start()
        CheckBoxLED.Checked = True
        FastScan(TextBoxX.Text, TextBoxY.Text, 0.00, SaveFileDialog1.FileName)
        CheckBoxLED.Checked = False
        watch.Stop()
        MsgBox("Scanned in " + (watch.ElapsedMilliseconds / 1000).ToString + " s")

        ListBox1.Items.Add(Path.GetFileName(SaveFileDialog1.FileName))
        ReDim Preserve Filenames(fileN)
        Filenames(fileN) = SaveFileDialog1.FileName
        fileN += 1
    End Sub

    Public Sub FastScan(X As Integer, y As Integer, overlap As Single, Address As String)
        If Camera.busy Then ExitLive()

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

        Dim Pyramid(3) As Pyramids

        Pyramid(0) = New Pyramids(X, y, Camera.W, Camera.H, 4, 0, Address, 100)
        Pyramid(1) = New Pyramids(X, y, Camera.W, Camera.H, 4, 1, Address + "1", 100)
        Pyramid(2) = New Pyramids(X, y, Camera.W, Camera.H, 4, 2, Address + "2", 100)
        Pyramid(3) = New Pyramids(X, y, Camera.W, Camera.H, 4, 3, Address + "3", 100)

        Dim ColorBytes(Camera.W * Camera.H * 3 - 1)

        Dim Fit As New Plannar_fit
        Dim A(4) As Double
        ReDim FocusMap(X, y)
        If Tracking.ROI.IsMade Then


            Dim vx(Tracking.ROI.numDots * 2 - 1) As Single
            Dim vy(Tracking.ROI.numDots - 1) As Single


            For i = 0 To Tracking.ROI.numDots - 1
                Tracking.MovetoDots(i)
                vy(i) = DoAutoFocus(1)
                vx(i * 2) = Stage.X
                vx(i * 2 + 1) = Stage.Y
                Dim imgtest(Camera.Wbinned * Camera.Hbinned - 1) As Byte
                Camera.Capture()
                SaveSinglePageTiff("c:\temp\" + i.ToString + ".tif", Camera.Bytes, Camera.W, Camera.H)


            Next
            A = Fit.Main(vx, vy, 1000, 0.0001)


            Tracking.MovetoROIEdge()
            Dim X0 As Single = Stage.X
            Dim Y0 As Single = Stage.Y

            For j = 1 To y
                For i = 1 To X
                    If j Mod 2 = 0 Then

                        ' FocusMap(i, j) = -(A * (X0 + (i - 1) * -AdjustedStepX) + B * (Y0 + (j - 1) * AdjustedStepY) + D) / C
                        FocusMap(i, j) = Fit.ComputeE({(X0 + (i - 1) * -AdjustedStepX), (Y0 + (j - 1) * AdjustedStepY)}, A)
                    Else
                        'FocusMap(i, j) = -(A * (X0 + (X - i) * -AdjustedStepX) + B * (Y0 + (j - 1) * AdjustedStepY) + D) / C
                        FocusMap(i, j) = Fit.ComputeE({X0 + (X - i - 1) * -AdjustedStepX, Y0 + (j - 1) * AdjustedStepY}, A)
                    End If

                Next
            Next

        End If


        Stage.SetAcceleration(Stage.Zaxe, AutoFocus.Zacceleration)
        If Tracking.ROI.IsMade Then Tracking.MovetoROIEdge()
        Stage.MoveAbsolute(Stage.Zaxe, Fit.ComputeE({Stage.X, Stage.Y}, A))




        ' Dim BytesExport(Camera.Bytes.GetUpperBound(0)) As Byte
        For loop_y = 1 To y
            For loop_x = 1 To X
                Pbar.Increment(1)

                Camera.Capture_Threaded()
                Thread.Sleep(Camera.exp * 1.2)


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

                Stage.MoveAbsolute(Stage.Zaxe, Fit.ComputeE({Stage.X, Stage.Y}, A))
                Do Until Camera.ready

                Loop
                Do Until Pyramid(0).Ready And Pyramid(1).Ready And Pyramid(2).Ready And Pyramid(3).Ready

                Loop



                'If direction > 0 Then
                '    Tiles.SaveTile(loop_x - 1, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'Else
                '    Tiles.SaveTile(X - loop_x, loop_y - 1, Display.MakeFullsizeImage(Camera.Bytes))
                'End If


                If direction > 0 Then
                    For i = 0 To Pyramid(0).pages - 1
                        Pyramid(i).SaveTile(loop_x - 1, loop_y - 1, (Camera.Bytes))
                    Next
                Else
                    For i = 0 To Pyramid(0).pages - 1
                        Pyramid(i).SaveTile(X - loop_x, loop_y - 1, (Camera.Bytes))
                    Next
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
        Do Until Pyramid(0).Ready And Pyramid(1).Ready And Pyramid(2).Ready And Pyramid(3).Ready

        Loop
        For i = 1 To Pyramid(0).pages - 1
            Pyramid(i).Close()
        Next


        Pyramid(0).AssemblePyramid({Pyramid(0), Pyramid(1), Pyramid(2), Pyramid(3)})

        'MakeMontage(X, Y, Bmp, True)
1:
        Pbar.Value = 0
        'Camera.SetPolicyToUNSafe()
        'Camera.Flatfield(0)
        'Camera.SetDataMode(Colortype.Grey)
        Stage.SetAcceleration(Stage.Zaxe, Stage.Zacc)
        GoLive()



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

    Private Sub Textbox_exposure_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim WasLive As Boolean
        If Camera.busy Then ExitLive() : WasLive = True
        Camera.Capture()
        Camera.Flatfield(0)
        Camera.SetDataMode(Colortype.Grey)
        CheckBoxLED.Checked = False
        Thread.Sleep(500)
        Camera.Capture()
        SaveSinglePageTiff16("dark.tif", Camera.Bytes, Camera.W, Camera.H)
        CheckBoxLED.Checked = True
        Thread.Sleep(500)

        'Dim Flatfield(Camera.Dim_X * Camera.Dim_Y - 1) As Single
        'Dim Flatfieldbytes(Camera.Dim_X * Camera.Dim_Y - 1) As Byte
        'Dim direction As Integer = 1
        'For y = 1 To 5
        '    For x = 1 To 5
        '        Stage.MoveRelative(Stage.Xaxe, direction * Stage.FOVX / 10)
        '        Camera.Capture()
        '        For i = 0 To Camera.Dim_X * Camera.Dim_Y - 1
        '            Flatfield(i) += Camera.Bytes(i)
        '        Next
        '    Next
        '    Stage.MoveRelative(Stage.Yaxe, direction * Stage.FOVY / 10)
        '    direction *= -1
        'Next


        'For i = 0 To Camera.Dim_X * Camera.Dim_Y - 1
        '    Flatfieldbytes(i) = Flatfield(i) / 25
        'Next


        SaveSinglePageTiff16("ff.tif", Camera.Bytes, Camera.W, Camera.H)
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
        GetPreview()

    End Sub
    Public Sub GetPreview()
        Stage.MoveAbsolute(Stage.Yaxe, 0)
        Stage.MoveAbsolute(Stage.Xaxe, 12.5)
        Stage.MoveAbsolute(Stage.Zaxe, 0)
        'MsgBox("Load the sample and then hit OK.")

        Tracking.UpdateBmp(Preview.Capture(Val(TextBox_PrevieEXp.Text), Val(TextBox_PreviewFocus.Text)))


        Stage.Go_Middle()
        'stage.MoveAbsolute(stage.Zaxe, lastZ)

        Tracking.Pbox.Image = Tracking.bmp.bmp

        Slideloaded = True
        Button_Scan.Enabled = True

        Stage.GoToFocus()
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Stage.GoToFocus()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxLED.CheckedChanged

        UpdateLED(CheckBoxLED.Checked)

    End Sub

    Public Sub UpdateLED(status As Boolean)

        Try
            If status Then
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

        Stage.Go_Middle()
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

    Private Sub TextBox_GainR_TextChanged(sender As Object, e As EventArgs) Handles TextBox_GainR.TextChanged

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

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        Tracking = New TrackingStructure(PreScan.PictureBox1)
        Tracking.Update()
        Tracking.UpdateBmp(Preview.Bmp)
        PreScan.ShowDialog()
        Tracking = New TrackingStructure(PictureBox_Preview)
        Tracking.Update()
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

    Private Sub TextBox_exposure_TextChanged(sender As Object, e As EventArgs) Handles TextBox_exposure.TextChanged

    End Sub

    Private Sub TextBox_exposure_MouseWheel(sender As Object, e As MouseEventArgs) Handles TextBox_exposure.MouseWheel
        If Camera.ExposureChanged Then Exit Sub

        TextBox_exposure.Text += e.Delta / 120
        If TextBox_exposure.Text < 1 Then TextBox_exposure.Text = 1
        ChangeExposure()
    End Sub

    Private Sub PictureBox_MouseWheel(sender As Object, e As MouseEventArgs) Handles PictureBox0.MouseWheel, PictureBox1.MouseWheel
        If Focusing = True Then Exit Sub
        Focusing = True
        Dim speed As Single
        If Control.ModifierKeys = Keys.Control Then speed = 50 Else speed = 5

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
        Display.MakeHistogram()
        Display.PlotHistogram()
    End Sub
End Class

