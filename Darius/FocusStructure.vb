Imports MathNet.Numerics
Public Class FocusStructure
    Public Range As Single
    Dim MicroRange As Single
    Public Nimg As Single
    Public bin As Integer
    Dim Steps, MicroSteps As Single
    ' For speed calibration of the stage 

    'focus point
    Public Z0 As Single
    Dim readout As Single
    Public Zacceleration As Single = 3000
    Dim exp As Single
    'One is for camera Cpline  one is for stage spline
    Dim Sx(19), sy(19) As Double
    Dim MicroSx(19), MicroSy(19) As Double
    Dim Cx(), Cy() As Double
    Dim MicroSpline, Spline, Cpline As Interpolation.LinearSpline
    Dim BinnedImage()() As Byte
    Dim FT As ExtendedDepth5
    Public Sub New(Range As Single, Steps As Single, bin As Integer)

        Me.Range = Range
        Me.Steps = Steps

        MicroRange = Steps * 2
        MicroSteps = Steps / 10
        Nimg = 100
        Me.bin = bin

        Camera.SetBinning(True, bin)
        FT = New ExtendedDepth5(Camera.Wbinned, Camera.Hbinned, 0, False)
        Camera.SetBinning(False, bin)
        Select Case bin
            Case 4
                readout = 27
            Case 8
                readout = 12
        End Select
        Try
            ReadS()
            ReadC()
            ReadS2()
        Catch ex As Exception

        End Try



    End Sub

    Public Sub Initialize()

        Camera.Flatfield(0)
        exp = Camera.exp
        Camera.SetDataMode(Colortype.Grey)
        Camera.SetExposure(exp / bin, False)
        Camera.SetBinning(True, bin)
        ReDim BinnedImage(Nimg - 1)
        Stage.SetAcceleration(Stage.Zaxe, Zacceleration)

        'To use only integer steps 
        '      Range = Int(Range / Nimg * stage.ZMMtoSteps) * Nimg / stage.ZMMtoSteps

    End Sub



    Public Sub Calibrate(ByRef pbar As ProgressBar)
        pbar.Maximum = 19
        Initialize()
        For s = 0 To 19
            Stage.SetSpeed(Stage.Zaxe, (s + 1) / 4)
            sy(s) = (s + 1) / 4
            Dim watch As New Stopwatch
            watch.Start()
            Stage.MoveRelative(Stage.Zaxe, Range)
            watch.Stop()
            Sx(s) = watch.ElapsedMilliseconds
            Stage.MoveRelative(Stage.Zaxe, -Range)
            pbar.Value = s
            Application.DoEvents()
        Next
        Spline = Interpolate.Linear(Sx, sy)
        WriteS()
        '-------------------------------------Microsteps=--------------------------------------

        For s = 0 To 19
            Stage.SetSpeed(Stage.Zaxe, (s + 1) / 10)
            Microsy(s) = (s + 1) / 10
            Dim watch As New Stopwatch
            watch.Start()
            Stage.MoveRelative(Stage.Zaxe, MicroRange)
            watch.Stop()
            MicroSx(s) = watch.ElapsedMilliseconds
            Stage.MoveRelative(Stage.Zaxe, -MicroRange)
            pbar.Value = s
            Application.DoEvents()
        Next
        MicroSpline = Interpolate.Linear(MicroSx, Microsy)
        WriteS2()
        '----------------------------------Camera--------------------------------------------------
        ReDim BinnedImage(0)(Camera.Wbinned * Camera.Hbinned - 1)
        Dim tc As Integer = 0
        pbar.Maximum = 500
        For t = 1 To 500 Step 10
            Camera.SetExposure(t / bin, False)
            ReDim Preserve Cx(tc)
            Cx(tc) = t
            Dim watch As New Stopwatch
            watch.Start()
            For i = 1 To 5
                Camera.Capture(BinnedImage(0))
                'FT.FindCenterOfMass2(BinnedImage(0))
                Stage.UpdateZPositions()
            Next
            watch.Stop()
            ReDim Preserve Cy(tc)
            Cy(tc) = watch.ElapsedMilliseconds / 5
            tc += 1
            pbar.Value = t
            Application.DoEvents()
        Next
        Cpline = Interpolate.Linear(Cx, Cy)

        WriteC()
        pbar.Value = 0
        Release()
    End Sub
    Sub WriteS2()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "MicroStage.txt", OpenMode.Output)
        For i = 0 To sy.GetUpperBound(0)
            PrintLine(fn, MicroSx(i), Microsy(i))
        Next
        FileClose(fn)

    End Sub
    Sub WriteS()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "stage.txt", OpenMode.Output)
        For i = 0 To sy.GetUpperBound(0)
            PrintLine(fn, Sx(i), sy(i))
        Next
        FileClose(fn)
    End Sub

    Sub WriteC()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "Camera.txt", OpenMode.Output)
        For i = 0 To Cy.GetUpperBound(0)
            PrintLine(fn, Cx(i), Cy(i))
        Next
        FileClose(fn)
    End Sub

    Sub ReadC()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "Camera.txt", OpenMode.Input)
        Dim i As Integer
        Do Until (EOF(fn))
            ReDim Preserve Cx(i)
            ReDim Preserve Cy(i)
            Input(fn, Cx(i))
            Input(fn, Cy(i))
            i += 1
        Loop
        FileClose(fn)
        Cpline = Interpolate.Linear(Cx, Cy)
    End Sub


    Sub ReadS()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "Stage.txt", OpenMode.Input)
        Dim i As Integer
        Do Until (EOF(fn))
            ReDim Preserve Sx(i)
            ReDim Preserve sy(i)
            Input(fn, Sx(i))
            Input(fn, sy(i))
            i += 1
        Loop
        FileClose(fn)
        Spline = Interpolate.Linear(Sx, sy)
    End Sub

    Sub ReadS2()
        Dim fn As Integer = FreeFile()
        FileOpen(fn, "MicroStage.txt", OpenMode.Input)
        Dim i As Integer
        Do Until (EOF(fn))
            ReDim Preserve MicroSx(i)
            ReDim Preserve Microsy(i)
            Input(fn, MicroSx(i))
            Input(fn, Microsy(i))
            i += 1
        Loop
        FileClose(fn)
        MicroSpline = Interpolate.Linear(MicroSx, Microsy)
    End Sub


    Public Function Analyze() As Single


        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        Nimg = Range / Steps

        For n = 0 To Nimg - 1
            ReDim BinnedImage(n)(Camera.Wbinned * Camera.Hbinned - 1)
        Next


        Dim CmXMax As Single
        Dim Startpoint As Single
        Dim focus As Single

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()
        Camera.Capture(BinnedImage(0))
        Camera.Capture(BinnedImage(0))

        Startpoint = Stage.Z


        Dim CM(Nimg - 1) As Single
        'Dim Pos1(Nimg - 1) As Single
        Dim Pos(Nimg - 1) As Single
        Dim decline As Integer

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()


        Dim Speed As Double = Spline.Interpolate(Cpline.Interpolate(Camera.exp) * Nimg)
        Stage.SetSpeed(Stage.Zaxe, Speed)
        Stage.MoveRelativeAsync(Stage.Zaxe, Range)


        For zz = 0 To Nimg - 1

            Camera.Capture(BinnedImage(zz))
            Stage.UpdateZPositions()
            Pos(zz) = Stage.Z

        Next


        For zz = 0 To Nimg - 1
            CM(zz) = FT.FindCenterOfMass3(BinnedImage(zz))
            '  SaveSinglePageTiff("C:\test\POS- " + zz.ToString + "-" + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(zz), Camera.Wbinned, Camera.Hbinned)
        Next


        CmXMax = CM.Max
        Dim zzMax As Integer
        For zz = 0 To Nimg - 1
            If CM(zz) = CmXMax Then focus = Pos(zz) : zzMax = zz
            Form1.Chart1.Series(1).Points.AddXY(Pos(zz), CM(zz))
            Application.DoEvents()
        Next
        'SaveSinglePageTiff("C:\temp\focus1.tif", BinnedImage(zzMax), Camera.Wbinned, Camera.Hbinned)
        Stage.SetSpeed(Stage.Zaxe, 48)
        ' For some stupid reason camera captures the previous frame? So at focous point it delivers an image from ther previous position!
        ' That is why I have +2 instead of +1
        'Igo one step further to start the focus from there.

        'Stage.MoveRelative(Stage.Zaxe, -Steps * (decline + 2))
        Stage.MoveAbsolute(Stage.Zaxe, focus - 2 * Steps)
        Camera.Capture(BinnedImage(0))
        Camera.Capture(BinnedImage(0))


        Nimg = MicroRange / MicroSteps

        ReDim CM(Nimg - 1)
        ReDim Pos(Nimg - 1)
        decline = 0
        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        Speed = MicroSpline.Interpolate(Cpline.Interpolate(Camera.exp) * Nimg)
        Stage.SetSpeed(Stage.Zaxe, Speed)
        Stage.MoveRelativeAsync(Stage.Zaxe, MicroRange)


        For zz = 0 To Nimg - 1

            Camera.Capture(BinnedImage(zz))
            Stage.UpdateZPositions()
            Pos(zz) = Stage.Z

        Next
        Stage.SetSpeed(Stage.Zaxe, 48)

        For zz = 0 To Nimg - 1
            CM(zz) = FT.FindCenterOfMass3(BinnedImage(zz))
            '    SaveSinglePageTiff("C:\test\2nd\POS- " + zz.ToString + "-" + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(zz), Camera.Wbinned, Camera.Hbinned)
        Next


        CmXMax = CM.Max

        For zz = 0 To Nimg - 1
            If CM(zz) = CmXMax Then focus = Pos(zz) : zzMax = zz
            Form1.Chart1.Series(1).Points.AddXY(Pos(zz), CM(zz))
            Application.DoEvents()
        Next
        Stage.MoveAbsolute(Stage.Zaxe, focus - MicroSteps)

        'For zz = 0 To Nimg - 1
        '    If zz > 0 Then Stage.MoveRelative(Stage.Zaxe, MicroSteps) ' The first time should be acquired with no movement 
        '    Pos(zz) = Stage.Z
        '    Camera.Capture(BinnedImage(0))
        '    CM(zz) = FT.FindCenterOfMass3(BinnedImage(0))
        '    '  SaveSinglePageTiff("C:\temp\2nd\POS- " + zz.ToString + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(0), Camera.Wbinned, Camera.Hbinned)
        '    Form1.Chart1.Series(1).Points.AddXY(Int((zz * MicroSteps) * 1000), CM(zz))
        '    Application.DoEvents()
        '    If zz > 0 Then
        '        If CM(zz) > CM(zz - 1) Then decline = 0
        '    End If
        '    If CM(zz) < CM.Max Then decline += 1
        '    If decline = 5 Then Exit For
        'Next

        'Form1.Chart1.Series(0).Points.Clear()
        'Form1.Chart1.Series(1).Points.Clear()

        'CmXMax = CM.Max
        'For zz = 0 To Nimg - 1
        '    If CM(zz) = CmXMax Then focus = Pos(zz)
        'Next
        'Stage.MoveAbsolute(Stage.Zaxe, focus)
        focus = Stage.Z


        Return focus
    End Function



    Public Sub Release()
        Camera.SetDataMode(Colortype.RGB)
        If Camera.FFsetup Then Camera.Flatfield(1)
        Camera.SetExposure(exp, False)
        Camera.SetBinning(False, bin)
        Stage.SetAcceleration(Stage.Zaxe, Stage.Zacc)
    End Sub



End Class
