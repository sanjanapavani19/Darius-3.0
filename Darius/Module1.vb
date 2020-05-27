'Imports System.ComponentModel
'Imports System.IO
'Imports AForge.Imaging.Filters
'Imports Microsoft.VisualBasic.Devices
'Imports AForge.Imaging

'Public Class Form1

'    Public Display As ImageDisplay

'    Public LEDcontroller As Relay

'    Dim IsDragging As Boolean
'    Dim CollagenImage As StackImage
'    Dim Concatenate As Single(,,)
'    Dim Imagetype As ImagetypeEnum
'    Dim AutoFocus As FocusStructure
'    Dim Slideloaded As Boolean
'    Dim LinearUnmixing As LinearUnmixingStructure
'    Dim cropX As Integer
'    Dim cropY As Integer
'    Public cropPen = New Pen(Brushes.Blue, 2.9)
'    Dim Count As Integer = 0
'    Dim cropWidth As Integer
'    Dim cropHeight As Integer
'    Dim CropRect As New Rectangle
'    Dim rects(2) As Rectangle
'    Dim index As Integer = 0
'    Dim Variable As Integer = 0
'    Dim FocusPoints(2) As Single

'    Dim panel As Integer

'    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'        LEDcontroller = New Relay
'        LEDcontroller.SetRelays(1, True)
'        LEDcontroller.SetRelays(2, False)


'        Preview = New PreviewStructure
'        Imagetype = ImagetypeEnum.Brightfield
'        Camera = New XimeaColor


'        If Camera.status Then
'            Textbox_exposure.Text = Camera.exp
'            AutoFocus = New FocusStructure(0.2, 20, 4)

'            Display = New ImageDisplay(Camera.Dim_X, Camera.Dim_Y, 2)

'        End If

'        stage = New Zaber(5, Setting.Gett("FOVX"), Setting.Gett("FOVX") * Camera.Dim_Y / Camera.Dim_X)
'        stage.correct = True

'        TextBoxGain.Text = Setting.Gett("Gain")
'        Textbox_exposure.Text = Setting.Gett("exposure")


'        If Camera.status Then
'            CollagenImage = New StackImage(Camera.Dim_X, Camera.Dim_Y, 4, Imaging.PixelFormat.Format32bppArgb)
'            ReDim Concatenate(Camera.Dim_X, Camera.Dim_Y, 5)
'            ArrangeControls(10)
'            GoLive()
'        End If

'        Tracking = New TrackingStructure(PictureBox_Preview)
'    End Sub


'    Sub ArrangeControls(d As Integer)
'        Dim scale As Single = 0.5



'        PictureBox0.Width = Display.BmpPreview.width * scale
'        PictureBox0.Height = Display.BmpPreview.height * scale
'        PictureBox0.SizeMode = PictureBoxSizeMode.Zoom

'        PictureBox1.Width = Display.BmpPreview.width * scale
'        PictureBox1.Height = Display.BmpPreview.height * scale
'        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom

'        PictureBox2.Width = Display.BmpPreview.width * scale
'        PictureBox2.Height = Display.BmpPreview.height * scale
'        PictureBox2.SizeMode = PictureBoxSizeMode.Zoom

'        PictureBox3.Width = Display.BmpPreview.width * scale
'        PictureBox3.Height = Display.BmpPreview.height * scale
'        PictureBox3.SizeMode = PictureBoxSizeMode.Zoom

'        TabControl1.Width = Display.BmpPreview.width * (0.1 + scale)
'        TabControl1.Height = Display.BmpPreview.width * (0.1 + scale)




'        PictureBox_Phasor.Left = TabControl1.Left + TabControl1.Width + d
'        PictureBox_Phasor.Top = TabControl1.Top
'        HScrollBar_PhasorPanels.Left = PictureBox_Phasor.Left
'        HScrollBar_PhasorPanels.Top = PictureBox_Phasor.Top + PictureBox_Phasor.Height + d

'        GroupBox3.Left = PictureBox_Phasor.Left
'        GroupBox3.Top = HScrollBar_PhasorPanels.Top + HScrollBar_PhasorPanels.Height + d
'        TabControl_Settings.Top = GroupBox3.Top + GroupBox3.Height + d
'        TabControl_Settings.Left = GroupBox3.Left
'        TextBoxGain.Text = Setting.Gett("Gain")
'        TextBox_GainB.Text = Setting.Gett("GainB")
'        TextBox_GainG.Text = Setting.Gett("GainG")
'        TextBox_GainR.Text = Setting.Gett("GainR")

'    End Sub



'    Private Sub Textbox_exposure_KeyDown(sender As Object, e As KeyEventArgs) Handles Textbox_exposure.KeyDown
'        If e.KeyCode = Keys.Return Then
'            ChangeExposure()

'        End If
'    End Sub

'    Public Sub ChangeExposure()

'        Camera.exp = Val(Textbox_exposure.Text)
'        Camera.ExposureChanged = True
'        Setting.Sett("EXPOSURE", Camera.exp)

'        'Do Until Camera.ExposureChanged = False

'        'Loop
'        'Display.AdjustBrightness()
'    End Sub



'    Public Sub GoLive()
'        Dim Thread1 As New System.Threading.Thread(AddressOf Live)
'        Thread1.Start()
'    End Sub



'    Public Sub Live()
'        '' Setting the waithandle to false because the initial setting was true.
'        ''WaitHandle_LiveReturned.Reset()
'        Do
'            Camera.busy = True
'            If Camera.Dostop Then Exit Do
'            Camera.capture()
'            Application.DoEvents()
'            If Camera.ExposureChanged Then Camera.SetExposure() : Camera.ExposureChanged = False


'            If Imagetype = ImagetypeEnum.Brightfield Then
'                Display.Preview(Camera.frame, True)
'                PictureBox0.Image = Display.BmpPreview.bmp
'            End If

'            If Imagetype = ImagetypeEnum.Fluorescence Then
'                Display.Preview(Camera.frame, False)
'                PictureBox1.Image = Display.BmpPreview.bmp
'            End If


'        Loop
'        Camera.busy = False

'    End Sub

'    Public Sub ExitLive()
'        If Camera.status = False Then Exit Sub
'        Camera.Dostop = True
'        Do Until Camera.busy = False

'        Loop
'        Camera.Dostop = False
'    End Sub

'    Private Sub Button_right_Click(sender As Object, e As EventArgs) Handles Button_right.Click
'        If Not Camera.busy Then GoLive()
'        stage.Move_r(stage.Xport, -stage.FovX)
'    End Sub

'    Private Sub Button_left_Click(sender As Object, e As EventArgs) Handles Button_left.Click
'        If Not Camera.busy Then GoLive()
'        stage.Move_r(stage.Xport, stage.FovX)
'    End Sub

'    Private Sub Button_top_Click(sender As Object, e As EventArgs) Handles Button_top.Click
'        If Not Camera.busy Then GoLive()
'        stage.Move_r(stage.Yport, -stage.FovY)
'    End Sub

'    Private Sub Button_bottom_Click(sender As Object, e As EventArgs) Handles Button_bottom.Click
'        If Not Camera.busy Then GoLive()
'        stage.Move_r(stage.Yport, stage.FovY)
'    End Sub

'    Private Sub Button_adjustBrightness_Click(sender As Object, e As EventArgs) Handles Button_adjustBrightness.Click
'        Display.AdjustBrightness()
'        Display.PlotHistogram(Chart1)

'    End Sub

'    Private Sub Form1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
'        If Not Camera.busy Then GoLive()
'        Dim speed As Single
'        If Control.ModifierKeys = Keys.Control Then speed = 10 Else speed = 1

'        'If XYZ.name = "NewPort" Then
'        If e.Delta > 0 Then
'            stage.Move_r(stage.Zport, speed * 0.001 * Math.Abs(e.Delta) / 120)
'        Else
'            stage.Move_r(stage.Zport, speed * -0.001 * Math.Abs(e.Delta) / 120)
'        End If

'    End Sub

'    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
'        If Camera.status = True Then
'            ExitLive()

'        End If
'        Preview.StopPreview()
'        LEDcontroller.LED_OFF()
'    End Sub

'    Private Sub RadioButton_zoom_in_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_zoom_in.CheckedChanged
'        If RadioButton_zoom_in.Checked Then
'            Display.zoom = True
'        Else
'            Display.zoom = False
'        End If
'    End Sub

'    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

'    End Sub

'    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_r(stage.Zport, Val(TextBox3.Text))
'        End If
'    End Sub



'    Private Sub TextBox_GainR_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainR.KeyDown
'        If e.KeyCode = Keys.Return Then
'            Try
'                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))

'            Catch ex As Exception

'            End Try

'        End If
'    End Sub

'    Private Sub TextBox_GainG_TextChanged(sender As Object, e As EventArgs) Handles TextBox_GainG.TextChanged

'    End Sub

'    Private Sub TextBox_GainG_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainG.KeyDown
'        If e.KeyCode = Keys.Return Then
'            Try
'                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))
'            Catch ex As Exception

'            End Try

'        End If
'    End Sub



'    Private Sub TextBox_GainB_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox_GainB.KeyDown
'        If e.KeyCode = Keys.Return Then
'            Try
'                Display.SetColorGain(Val(TextBox_GainR.Text), Val(TextBox_GainG.Text), Val(TextBox_GainB.Text))
'            Catch ex As Exception

'            End Try

'        End If
'    End Sub

'    Private Sub TextBoxGain_TextChanged(sender As Object, e As EventArgs) Handles TextBoxGain.TextChanged

'    End Sub

'    Private Sub TextBoxGain_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBoxGain.KeyDown
'        If e.KeyCode = Keys.Return Then
'            Camera.setGain(Val(TextBoxGain.Text))
'        End If
'    End Sub



'    Private Sub Button_Preview_Click(sender As Object, e As EventArgs) Handles Button_Preview.Click
'        PictureBox_Preview.Refresh()
'        ReDim rects(2)
'        ReDim FocusPoints(2)
'        Count = 0
'        Variable = 0

'        stage.SetSpeed(stage.Zport, 500000)
'        stage.Move_A(stage.Zport, 0)
'        stage.Move_A(stage.Yport, 12.5)
'        stage.Move_A(stage.Xport, 0)
'        MsgBox("Load the slide and then hit  OK.")
'        Preview.Capture()
'        stage.GoZero(1)
'        stage.SetSpeed(stage.Zport, 100000)
'        'stage.Move_A(stage.Xport, 0)
'        stage.Go_Middle()
'        Tracking.UpdateBmp(Preview.Bmp)
'        PictureBox_Preview.Image = Tracking.bmp.bmp

'        'If SaveFileDialog1.ShowDialog() <> DialogResult.Cancel Then
'        '    Tracking.bmp.bmp.Save(SaveFileDialog1.FileName + ".jpg")
'        'End If

'        Slideloaded = True
'        Button_Scan.Enabled = True
'    End Sub

'    Private Sub Button_Home_Click(sender As Object, e As EventArgs) Handles Button_Home.Click
'        stage.Move_A(stage.Xport, 0)
'        stage.Move_A(stage.Yport, 0)
'    End Sub



'    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_r(stage.Xport, TextBox1.Text)
'        End If

'    End Sub

'    Private Sub TextBox2_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_r(stage.Yport, TextBox2.Text)
'        End If
'    End Sub

'    Private Sub Button_Scan_Click(sender As Object, e As EventArgs) Handles Button_Scan.Click
'        ExitLive()
'        stage.correct = True

'        Dim X0, Y0, Z0 As Single
'        Dim Correction As Single
'        Z0 = stage.Z
'        X0 = stage.X
'        Y0 = stage.Y

'        If Tracking.ROImade Then
'            Tracking.MovetoROIEdge()
'            Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'            stage.Move_A(stage.Zport, Correction)
'        End If


'        ' Exit Sub 
'        If SaveFileDialog1.ShowDialog = DialogResult.Cancel Then Exit Sub


'        Dim Outputfile As New BinaryFileStructure(SaveFileDialog1.FileName, FileMode.Append)

'        Camera.SetDataMode(Colortype.Grey)

'        Dim X, Y As Integer
'        Y = TextBoxY.Text
'        X = TextBoxX.Text
'        Dim filen As Integer
'        Dim direction As Integer
'        Dim AdjustedStepX, AdjustedStepY As Single
'        AdjustedStepX = stage.FovX
'        AdjustedStepY = stage.FovY
'        Dim bmp(X * Y - 1) As Bitmap
'        ProgressBar_Mosaic.Maximum = X * Y
'        direction = 1
'        'Camera.capture()

'        'Dim Resize As New ResizeBilinear(Camera.Dim_X * 2, Camera.Dim_Y * 2)
'        'Resize.Apply(Camera.BmpRef)

'        'writes the version
'        Outputfile.write(2)

'        'The number of frmaes at both directions 
'        Outputfile.write(X)
'        Outputfile.write(Y)
'        'bitmap width  and height 
'        Outputfile.write(Camera.Dim_X)
'        Outputfile.write(Camera.Dim_Y)



'        'now writing the preview bitmap
'        Outputfile.write(Tracking.bmp.width)
'        Outputfile.write(Tracking.bmp.bmp.Height)
'        Outputfile.write(Tracking.bmp.bytes.GetLength(0))
'        Outputfile.write(Tracking.bmp.bytes, Tracking.bmp.bytes.Length)

'        'Writing the ROI properties 
'        If Tracking.ROImade Then
'            ' to show that ROI is there 
'            Outputfile.write(1)
'            Outputfile.write(Tracking.bmp.ROI(0).Width)
'            Outputfile.write(Tracking.bmp.ROI(0).Height)
'            Outputfile.write(Tracking.bmp.ROI(0).Top)
'            Outputfile.write(Tracking.bmp.ROI(0).Left)
'        Else
'            ' to show that ROI is not there
'            Outputfile.write(0)

'            Outputfile.write(0)
'            Outputfile.write(0)
'            Outputfile.write(0)
'            Outputfile.write(0)

'        End If
'        ' version 2 adds a byte to indicate the type of scan
'        Dim ScanType As Byte
'        If RadioButton1.Checked Then ScanType = 1
'        If RadioButton2.Checked Then ScanType = 2
'        If RadioButton3.Checked Then ScanType = 3
'        Outputfile.write(ScanType)
'        ' now preserve the space for future developments. 
'        Dim bytes(999) As Byte
'        Outputfile.write(bytes, 999)
'        'The returned frmae from the camera is 1 byte longer!
'        Dim framelength = Camera.Dim_X * Camera.Dim_Y



'        Select Case ScanType
'            Case 1
'                For loop_y = 1 To Y
'                    If loop_y > 1 Then
'                        stage.Move_r(stage.Yport, AdjustedStepY)
'                        Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                        stage.Move_A(stage.Zport, Correction)
'                        filen = filen + X + direction
'                        direction = direction * -1
'                    End If
'                    For loop_x = 1 To X
'                        If loop_x > 1 Then
'                            stage.Move_r(stage.Xport, -AdjustedStepX * direction)
'                            Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                            stage.Move_A(stage.Zport, Correction)
'                        End If
'                        ProgressBar_Mosaic.Increment(1)
'                        LEDcontroller.SetRelays(1, True)
'                        Camera.capture()
'                        LEDcontroller.SetRelays(1, False)
'                        Display.ApplyColorGain(Camera.frame, Camera.Dim_X, Camera.Dim_Y)
'                        Outputfile.write(Camera.frame, framelength)

'                        'If Camera.exp < 0.1 Then Threading.Thread.Sleep(100)
'                        LEDcontroller.SetRelays(2, True)
'                        Camera.capture()
'                        LEDcontroller.SetRelays(2, False)
'                        Outputfile.write(Camera.frame, framelength)
'                        Application.DoEvents()
'                    Next
'                Next
'            Case 2
'                LEDcontroller.SetRelays(1, True)
'                LEDcontroller.SetRelays(2, False)

'                For loop_y = 1 To Y
'                    If loop_y > 1 Then
'                        stage.Move_r(stage.Yport, AdjustedStepY)
'                        Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                        stage.Move_A(stage.Zport, Correction)
'                        filen = filen + X + direction
'                        direction = direction * -1
'                    End If
'                    For loop_x = 1 To X
'                        If loop_x > 1 Then
'                            stage.Move_r(stage.Xport, -AdjustedStepX * direction)
'                            Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                            stage.Move_A(stage.Zport, Correction)
'                        End If
'                        ProgressBar_Mosaic.Increment(1)
'                        Camera.capture()
'                        Display.ApplyColorGain(Camera.frame, Camera.Dim_X, Camera.Dim_Y)
'                        Outputfile.write(Camera.frame, framelength)
'                        Application.DoEvents()
'                    Next
'                Next
'            Case 3
'                LEDcontroller.SetRelays(1, False)
'                LEDcontroller.SetRelays(2, True)
'                For loop_y = 1 To Y
'                    If loop_y > 1 Then
'                        stage.Move_r(stage.Yport, AdjustedStepY)
'                        Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                        stage.Move_A(stage.Zport, Correction)
'                        filen = filen + X + direction
'                        direction = direction * -1
'                    End If
'                    For loop_x = 1 To X
'                        If loop_x > 1 Then
'                            stage.Move_r(stage.Xport, -AdjustedStepX * direction)
'                            Correction = Z0 / stage.ZMMtoSteps + (X0 - stage.X) / stage.MMtoSteps * stage.Xcorrection + (Y0 - stage.Y) / stage.MMtoSteps * stage.YCorrection
'                            stage.Move_A(stage.Zport, Correction)
'                        End If
'                        ProgressBar_Mosaic.Increment(1)
'                        Camera.capture()
'                        Outputfile.write(Camera.frame, framelength)
'                        Application.DoEvents()
'                    Next
'                Next
'        End Select



'        LEDcontroller.SetRelays(1, True)
'        LEDcontroller.SetRelays(2, False)

'        'stage.Move_A(stage.Xport, 0)
'        'stage.Move_A(stage.Yport, 0)

'        ProgressBar_Mosaic.Value = 0
'        'MakeMontage(bmp, X, Y)
'        Outputfile.CLOSE()
'        GoLive()
'        Button_Scan.Enabled = False


'    End Sub

'    Public Sub MakeMontage(bmp() As Bitmap, x As Integer, y As Integer)

'        Dim width As Integer = bmp(0).Width
'        Dim height As Integer = bmp(0).Height
'        Dim BmpMontage = New Bitmap(x * width, y * height)

'        Dim gr As Graphics = Graphics.FromImage(BmpMontage)
'        Dim i As Integer

'        ProgressBar_Mosaic.Value = 0
'        Application.DoEvents()
'        For loop_y = 0 To y - 1
'            For loop_x = 0 To x - 1
'                gr.DrawImage(bmp(i), loop_x * width, loop_y * height, width, height)
'                ProgressBar_Mosaic.Increment(1)
'                Application.DoEvents()
'                i = i + 1
'            Next
'        Next
'        ProgressBar_Mosaic.Value = 0
'        BmpMontage.Save("d:\1.png")
'    End Sub



'    Private Sub PictureBox_Preview_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox_Preview.MouseDown


'        'Tracking.IsDragging = True

'        'Tracking.bmp.ROI(0).X = e.X
'        'Tracking.bmp.ROI(0).Y = e.Y
'        'Cursor = Cursors.Cross
'        Try
'            If e.Button = Windows.Forms.MouseButtons.Left Then
'                Tracking.IsDragging = True
'                Tracking.bmp.ROI(0).X = e.X
'                Tracking.bmp.ROI(0).Y = e.Y
'                Cursor = Cursors.Cross
'                'cropX = e.X
'                'cropY = e.Y
'                cropPen = New Pen(Brushes.Blue, 2.9)
'                Cursor = Cursors.Cross

'            End If
'        Catch exc As Exception
'        End Try

'    End Sub


'    Private Sub PictureBox_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox_Preview.MouseMove 'moves little red rectangles
'        Dim i As Integer
'        Dim myPen As Pen
'        myPen = New Pen(Brushes.Red, 2.9)
'        If e.Button = Windows.Forms.MouseButtons.Left Then
'            PictureBox_Preview.Refresh()
'            For i = 0 To rects.Length - 1

'                If rects(i).Contains(e.Location) = True Then
'                    rects(i).X = e.X - rects(i).Width / 2
'                    rects(i).Y = e.Y - rects(i).Height / 2

'                End If
'                PictureBox_Preview.CreateGraphics.DrawRectangle(myPen, rects(i))
'                'PictureBox_Preview.CreateGraphics.DrawRectangle(cropPen, Tracking.bmp.ROI(0))
'            Next
'        End If
'    End Sub




'    Private Sub createDot(ByVal x As Integer, ByVal y As Integer)

'        Dim i As Integer = 0
'        Dim myGraphics As Graphics = Me.CreateGraphics
'        Dim myPen As Pen
'        myPen = New Pen(Brushes.Red, 2.9)
'        ReDim Preserve rects(index)
'        rects(index) = New Rectangle(x, y, 10, 10)
'        index += 1
'        PictureBox_Preview.CreateGraphics.DrawRectangle(myPen, New Rectangle(x, y, 10, 10))
'        ' Count += 1

'    End Sub


'    Private Sub PictureBox_Preview_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox_Preview.MouseMove 'draws main rectangle to enclose the area
'        'Dim i As Integer
'        'If Tracking.IsDragging And Count = 0 Then
'        '    Tracking.AnalyzeROI((e.X - Tracking.bmp.ROI(0).X), (e.Y - Tracking.bmp.ROI(0).Y)) 'sets height and width 
'        '    Tracking.bmp.RefreshROI()
'        '    Reset()
'        '    For i = 0 To Tracking.bmp.ROI.GetUpperBound(0)
'        '        PictureBox_Preview.CreateGraphics.DrawRectangle(New Pen(Color.Blue), Tracking.bmp.ROI(i))
'        '    Next

'        '    Tracking.Pbox.Image = Tracking.bmp.bmp

'        '    If Tracking.ROIX > 0 And Tracking.ROIY > 0 Then
'        '        TextBoxX.Text = Tracking.ROIX
'        '        TextBoxY.Text = Tracking.ROIY
'        '    End If
'        'End If

'        Try
'            If PictureBox_Preview.Image Is Nothing Then Exit Sub
'            If e.Button = Windows.Forms.MouseButtons.Left And Count = 0 Then
'                Tracking.AnalyzeROI((e.X - Tracking.bmp.ROI(0).X), (e.Y - Tracking.bmp.ROI(0).Y)) 'sets height and width 
'                PictureBox_Preview.Refresh()
'                cropWidth = e.X - Tracking.bmp.ROI(0).X
'                cropHeight = e.Y - Tracking.bmp.ROI(0).Y
'                CropRect = New Rectangle(Tracking.bmp.ROI(0).X, Tracking.bmp.ROI(0).Y, cropWidth, cropHeight)
'                PictureBox_Preview.CreateGraphics.DrawRectangle(cropPen, CropRect)
'                If Tracking.ROIX > 0 And Tracking.ROIY > 0 Then
'                    TextBoxX.Text = Tracking.ROIX
'                    TextBoxY.Text = Tracking.ROIY
'                End If
'            End If
'        Catch exc As Exception
'            If Err.Number = 5 Then Exit Sub
'        End Try

'    End Sub

'    Private Sub PictureBox_Preview_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox_Preview.MouseUp
'        If e.Button = MouseButtons.Left And Tracking.IsDragging Then
'            If Tracking.ROIX > 0 And Tracking.ROIY > 0 Then Tracking.ROImade = True
'            If Slideloaded Then Button_Scan.Enabled = True

'        End If

'        If e.Button = MouseButtons.Right Then
'            Tracking.RemoveROI()
'            Exit Sub
'        End If
'        Tracking.IsDragging = False

'        If Variable = 0 Then
'            createDot(CropRect.Right - CropRect.Width / 2, CropRect.Bottom - CropRect.Height / 2)
'            createDot(CropRect.Right - CropRect.Width / 3, CropRect.Bottom - CropRect.Height / 3)
'            createDot(CropRect.Right - CropRect.Width * 2 / 3, CropRect.Bottom - CropRect.Height * 2 / 3)
'            Count += 1
'            Variable += 1

'        End If

'        Dim x As Integer
'        For x = 0 To rects.Length - 1
'            Dim myPen As Pen
'            myPen = New Pen(Brushes.Red, 2.9)
'            PictureBox_Preview.CreateGraphics.DrawRectangle(myPen, rects(x))
'            PictureBox_Preview.CreateGraphics.DrawRectangle(cropPen, CropRect)
'            Count += 1
'        Next

'    End Sub

'    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
'        ExitLive()
'        OpenFileDialog1.ShowDialog()

'        Dim inputFile = IO.File.Open("c:\1.sci", IO.FileMode.Open)
'        Dim bytes(9163872 - 1) As Byte
'        inputFile.Read(bytes, 0, 9163872)
'        Dim bitmap As New Bitmap(Camera.Dim_X, Camera.Dim_Y, Imaging.PixelFormat.Format24bppRgb)
'        Dim bitmap8bit As New Bitmap(Camera.Dim_X, Camera.Dim_Y, Imaging.PixelFormat.Format8bppIndexed)
'        byteToBitmap(bytes, bitmap8bit)

'        Dim Bayer = New Filters.BayerFilter
'        Dim Pattern(1, 1) As Integer
'        Pattern = {{RGB.R, RGB.G}, {RGB.G, RGB.B}}
'        Bayer.BayerPattern = Pattern

'        bitmap = Bayer.Apply(bitmap8bit)
'        bitmap.Save("c:\1.png")
'        inputFile.Close()



'        'Dim bmp As New Bitmap(OpenFileDialog1.FileName)

'        'CollagenImage = New StackImage(bmp.Width, bmp.Height, 4, Imaging.PixelFormat.Format24bppRgb)
'        'Display = New ImageDisplay(bmp.Width, bmp.Height, 2)
'        'ArrangeControls(10)
'        'CollagenImage.Bitmp(0) = New FastBMP(bmp)
'        'OpenFileDialog1.ShowDialog()
'        'bmp = New Bitmap(OpenFileDialog1.FileName)
'        'CollagenImage.Bitmp(1) = New FastBMP(bmp)
'        'LinearUnmixing = New LinearUnmixingStructure(New Single() {1.1 * 4, 0.85 * 4, -2 * 4})
'        'LinearUnmixing.Analyze(CollagenImage.Bitmp(1), CollagenImage.Bitmp(2))
'        'UpdateCollagen()
'    End Sub

'    Private Sub Button_Refresh_Click(sender As Object, e As EventArgs)

'    End Sub

'    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
'        'LinearUnmixing = New LinearUnmixingStructure(New Single() {1.7 * 6, 0.9 * 6, -2.2 * 6})
'        LinearUnmixing = New LinearUnmixingStructure(New Single() {1.1 * 4, 0.85 * 4, -2 * 4})
'        LinearUnmixing.Analyze(CollagenImage.Bitmp(1), CollagenImage.Bitmp(2))
'        UpdateCollagen()
'    End Sub



'    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
'        Dim watch As New Stopwatch
'        watch.Start()

'        For i = 1 To 10
'            stage.Move_r(stage.Xport, stage.FovX)
'        Next
'        watch.Stop()
'        MsgBox(watch.ElapsedMilliseconds)
'    End Sub



'    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
'        DoAutoFocus(2)
'    End Sub



'    Private Sub TextBox4_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox4.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_A(stage.Xport, TextBox4.Text)
'        End If
'    End Sub


'    Private Sub TextBox5_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox5.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_A(stage.Yport, TextBox5.Text)
'        End If
'    End Sub

'    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

'    End Sub

'    Private Sub TextBox6_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox6.KeyDown
'        If e.KeyCode = Keys.Return Then
'            stage.Move_A(stage.Zport, TextBox6.Text)
'        End If
'    End Sub

'    Private Sub RadioButton_Conversion_CheckedChanged(sender As Object, e As EventArgs)

'    End Sub




'    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

'        stage.Move_A(stage.Xport, 2)
'        stage.Move_A(stage.Yport, 1.5)
'        DoAutoFocus(2)
'        stage.Z1 = stage.GetPosition(stage.Zport)

'        MsgBox(stage.Z1)

'        stage.Move_A(stage.Yport, 23)
'        DoAutoFocus(2)
'        stage.Z2 = stage.GetPosition(stage.Zport)

'        MsgBox(stage.Z2)

'        stage.Move_A(stage.Xport, 50)
'        DoAutoFocus(2)
'        stage.Z3 = stage.GetPosition(stage.Zport)

'        MsgBox(stage.Z3)

'        stage.Xcorrection = (stage.Z2 - stage.Z3) / 48
'        stage.YCorrection = (stage.Z1 - stage.Z2) / (23 - 1.5)
'        'GoLive()
'        Setting.Sett("XCORRECTION", stage.Xcorrection)
'        Setting.Sett("YCORRECTION", stage.YCorrection)
'        stage.Go_Middle()

'    End Sub

'    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
'        stage.correct = False


'    End Sub

'    Private Sub PictureBox_Preview_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox_Preview.DoubleClick

'    End Sub

'    Private Sub PictureBox_Preview_Click(sender As Object, e As EventArgs) Handles PictureBox_Preview.Click

'    End Sub

'    Private Sub TabPage4_Click(sender As Object, e As EventArgs) Handles TabPage4.Click

'    End Sub

'    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
'        ExitLive()
'        Camera.SetDataMode(Colortype.RGB)
'        'go home 
'        'stage.Move_A(1, 0)
'        'stage.Move_A(2, 0)
'        Dim X, Y As Integer
'        Y = TextBoxY.Text
'        X = TextBoxX.Text
'        Dim filen As Integer
'        Dim direction As Integer
'        Dim AdjustedStepX, AdjustedStepY As Single
'        AdjustedStepX = stage.FovX
'        AdjustedStepY = stage.FovY
'        Dim bmp(X * Y - 1) As Bitmap
'        ProgressBar_Mosaic.Maximum = X * Y
'        direction = 1

'        Dim Resize As New ResizeNearestNeighbor(Camera.Dim_X / 10, Camera.Dim_Y / 10)

'        For loop_y = 1 To Y

'            If loop_y > 1 Then
'                stage.Move_r(stage.Yport, AdjustedStepY)

'                filen = filen + X + direction
'                direction = direction * -1
'            End If


'            For loop_x = 1 To X
'                If loop_x > 1 Then stage.Move_r(stage.Xport, -AdjustedStepX * direction)
'                ProgressBar_Mosaic.Increment(1)

'                Camera.captureBmp()
'                filen = filen + direction

'                bmp(filen - 1) = New Bitmap(Camera.BmpRef)
'                PictureBox1.Image = bmp(filen - 1)


'            Next
'        Next

'        'stage.Move_A(stage.Xport, 0)
'        'stage.Move_A(stage.Yport, 0)

'        ProgressBar_Mosaic.Value = 0
'        MakeMontage(bmp, X, Y)
'        Camera.SetDataMode(Colortype.Grey)
'        GoLive()



'    End Sub



'    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click



'        Dim Grey As New Grayscale(0.2126, 0.7152, 0.0722)
'        Dim image As New Bitmap(Tracking.bmp.bmp.Width, Tracking.bmp.bmp.Height)
'        image = Grey.Apply(Tracking.bmp.bmp)

'        Dim INV As New Invert
'        INV.ApplyInPlace(image)

'        Dim th As New Threshold(20)
'        th.ApplyInPlace(image)

'        ' locating objects
'        Dim blobCounter As New AForge.Imaging.BlobCounter

'        blobCounter.FilterBlobs = True
'        blobCounter.MinHeight = 50
'        blobCounter.MinWidth = 50

'        blobCounter.ProcessImage(image)

'        Dim blobs As Blob() = blobCounter.GetObjectsInformation()
'        Dim i As Integer = 0
'        Dim R(blobs.GetUpperBound(0)) As Rectangle
'        For Each blob In blobs
'            Dim g As Graphics = Graphics.FromImage(Tracking.bmp.bmp)
'            g.DrawRectangle(New Pen(Brushes.Blue, 0.5), blob.Rectangle)
'            R(i) = blob.Rectangle
'            i += 1
'        Next

'        MsgBox("Items found :" + i.ToString)
'        PictureBox_Preview.Image = Tracking.bmp.bmp

'    End Sub

'    Private Sub RadioButton_zoom_out_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_zoom_out.CheckedChanged

'    End Sub

'    Private Sub PictureBox_Phasor_Click(sender As Object, e As EventArgs) Handles PictureBox_Phasor.Click

'    End Sub

'    Private Sub PictureBox0_Click(sender As Object, e As EventArgs) Handles PictureBox0.Click

'    End Sub

'    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
'        ReDim rects(2)
'        Count = 0
'        Variable = 0

'    End Sub

'    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click

'        stage.Move_A(stage.Xport, Tracking.ConvertPixeltoCoordinateX(rects(0).X + rects(0).Width / 2))
'        stage.Move_A(stage.Yport, Tracking.ConvertPixeltoCoordinateY(rects(0).Y + rects(0).Height / 2))
'        FocusPoints(0) = DoAutoFocus(1)


'        stage.Move_A(stage.Xport, Tracking.ConvertPixeltoCoordinateX(rects(1).X + rects(1).Width / 2))
'        stage.Move_A(stage.Yport, Tracking.ConvertPixeltoCoordinateY(rects(1).Y + rects(1).Height / 2))
'        FocusPoints(1) = DoAutoFocus(1)

'        stage.Move_A(stage.Xport, Tracking.ConvertPixeltoCoordinateX(rects(2).X + rects(2).Width / 2))
'        stage.Move_A(stage.Yport, Tracking.ConvertPixeltoCoordinateY(rects(2).Y + rects(2).Height / 2))
'        FocusPoints(2) = DoAutoFocus(1)


'    End Sub
'End Class

