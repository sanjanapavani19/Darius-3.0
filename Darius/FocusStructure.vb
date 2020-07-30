Imports MathNet.Numerics
Public Class FocusStructure
    Public Range As Single
    Public Nimg As Single
    Public bin As Integer
    Dim Steps, MicroSteps As Single
    ' For speed calibration of the stage 
    Dim Sx(19), sy(19) As Double
    Dim Cx(), Cy() As Double
    'focus point
    Public Z0 As Single
    Dim readout As Single
    Dim exp As Single
    'One is for camera Cpline  one is for stage spline
    Dim Spline, Cpline As Interpolation.LinearSpline
    Dim BinnedImage()() As Byte
    Dim FT As ExtendedDepth5
    Public Sub New(Range As Single, Steps As Single, bin As Integer)

        Me.Range = Range
        Me.Steps = Steps
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

    End Sub

    Public Sub Initialize()

        Camera.Flatfield(0)
        exp = Camera.exp
        Camera.SetExposure(exp / bin, False)
        Camera.SetBinning(True, bin)
        ReDim BinnedImage(Nimg - 1)
        'To use only integer steps 
        '      Range = Int(Range / Nimg * stage.ZMMtoSteps) * Nimg / stage.ZMMtoSteps

    End Sub
    Public Sub Calibrate(ByRef pbar As ProgressBar)
        pbar.Maximum = 19

        For s = 0 To 19
            Stage.SetSpeed(Stage.Zaxe, (s + 1) / 2)
            Sx(s) = s + 1
            Dim watch As New Stopwatch
            watch.Start()
            Stage.MoveRelative(Stage.Zaxe, Range)
            watch.Stop()
            sy(s) = watch.ElapsedMilliseconds
            Stage.MoveRelative(Stage.Zaxe, -Range)
            pbar.Value = s
            Application.DoEvents()
        Next
        Spline = Interpolate.Linear(Sx, sy)



        Dim Binnedimage(Camera.Wbinned * Camera.Hbinned - 1) As Byte
        Camera.SetBinning(True, bin)
        Camera.Flatfield(0)

        Dim tc As Integer = 0
        pbar.Maximum = 300
        For t = 1 To 300 Step 10
            Camera.SetExposure(t / bin, False)
            ReDim Preserve Cx(tc)
            Cx(tc) = tc
            Dim watch As New Stopwatch
            watch.Start()
            For i = 1 To 5
                Camera.Capture(Binnedimage)
                FT.FindCenterOfMass2(Binnedimage)
            Next
            watch.Stop()
            ReDim Preserve Cy(tc)
            Cy(tc) = watch.ElapsedMilliseconds / 5
            tc += 1
            pbar.Value = t
            Application.DoEvents()
        Next
        Cpline = Interpolate.Linear(Cx, Cy)
        pbar.Value = 0

    End Sub


    Public Function Analyze() As Single


        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()


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
        Dim Pos(Nimg - 1) As Single
        Dim decline As Integer

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()
        'Stage.SetSpeed(Stage.Zaxe, 2)
        'Stage.MoveRelativeAsync(Stage.Zaxe, 2)

        For zz = 0 To Nimg - 1
            If zz > 0 Then Stage.MoveRelative(Stage.Zaxe, Steps) ' The first time should be acquired with no movement 
            Stage.UpdateZPositions()
            Pos(zz) = Stage.Z
            Camera.Capture(BinnedImage(0))
            CM(zz) = FT.FindCenterOfMass2(BinnedImage(0))
            ' SaveSinglePageTiff("C:\temp\POS- " + zz.ToString + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(0), Camera.Wbinned, Camera.Hbinned)
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * Steps) * 1000), CM(zz))
            Application.DoEvents()
            If zz > 0 Then
                If CM(zz) > CM(zz - 1) Then decline = 0
            End If
            If CM(zz) < CM.Max Then decline += 1
            If decline = 5 Then Exit For

        Next


        CmXMax = CM.Max
        For zz = 0 To Nimg - 1
            If CM(zz) = CmXMax Then focus = Pos(zz)
        Next

        'Stage.SetSpeed(Stage.Zaxe, 48)
        ' For some stupid reason camera captures the previous frame? So at focous point it delivers an image from ther previous position!
        ' That is why I have +2 instead of +1
        'Igo one step further to start the focus from there.

        'Stage.MoveRelative(Stage.Zaxe, -Steps * (decline + 2))
        Stage.MoveAbsolute(Stage.Zaxe, focus - Steps)
        Camera.Capture(BinnedImage(0))
        Camera.Capture(BinnedImage(0))

        MicroSteps = Steps / 10
        ReDim CM(Nimg - 1)
        ReDim Pos(Nimg - 1)
        decline = 0
        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()
        For zz = 0 To Nimg - 1
            If zz > 0 Then Stage.MoveRelative(Stage.Zaxe, MicroSteps) ' The first time should be acquired with no movement 
            Pos(zz) = Stage.Z
            Camera.Capture(BinnedImage(0))
            CM(zz) = FT.FindCenterOfMass2(BinnedImage(0))
            '  SaveSinglePageTiff("C:\temp\2nd\POS- " + zz.ToString + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(0), Camera.Wbinned, Camera.Hbinned)
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * MicroSteps) * 1000), CM(zz))
            Application.DoEvents()
            If zz > 0 Then
                If CM(zz) > CM(zz - 1) Then decline = 0
            End If
            If CM(zz) < CM.Max Then decline += 1
            If decline = 5 Then Exit For
        Next

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        CmXMax = CM.Max
        For zz = 0 To Nimg - 1
            If CM(zz) = CmXMax Then focus = Pos(zz)
        Next
        Stage.MoveAbsolute(Stage.Zaxe, focus - MicroSteps)
        focus = Stage.Z


        Return focus
    End Function



    Public Sub Release()
        ' If Camera.FFsetup Then Camera.Flatfield(1)
        Camera.SetExposure(exp, False)
        Camera.SetBinning(False, bin)
    End Sub



End Class
