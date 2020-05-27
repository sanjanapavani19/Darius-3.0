Public Class FocusStructure
    Public Range As Single
    Public Nimg As Single
    Public bin As Integer
    'focus point
    Public Z0 As Single
    Dim readout As Single
    Dim exp As Single
    Dim BinnedImage()() As Byte
    Dim FT As ExtendedDepth5
    Public Sub New(R As Single, N As Integer, b As Integer)

        Range = R
        Nimg = N
        bin = b

        camera.SetBinning(True, bin)
        FT = New ExtendedDepth5(camera.Wbinned, camera.Hbinned, 0, False)
        camera.SetBinning(False, bin)
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
        Camera.SetExposure(exp / bin ^ 2, False)
        Camera.SetBinning(True, bin)
        ReDim BinnedImage(Nimg - 1)
        'To use only integer steps 
        Range = Int(Range / Nimg * stage.ZMMtoSteps) * Nimg / stage.ZMMtoSteps

    End Sub

    Public Function Analyze() As Single


        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        For n = 0 To Nimg - 1
            ReDim BinnedImage(n)(Camera.Wbinned * Camera.Hbinned - 1)
        Next

        Dim Srange As Single = Range

        Dim CM(Nimg - 1) As Single
        Dim CmXMax As Single
        For zz = 0 To Nimg - 1
            Camera.capture_binned(BinnedImage(zz))
            If zz < (Nimg - 1) Then stage.Move_r(stage.Zport, (Range) / Nimg) ' The lastimage should be acquired with no movement afterwards
            CM(zz) = FT.FindCenterOfMass(BinnedImage(zz))
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * Range / Nimg) * 1000), CM(zz))
            If CM(zz) = CM.Max Then CmXMax = zz

        Next
        'MsgBox("")
        Application.DoEvents()

        ''finds center of mass of focus 
        Dim focus As Single = (Nimg - CmXMax) * (Range / Nimg)
        stage.Move_r(stage.Zport, -focus)

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        ReDim CM(Nimg - 1)

        Srange = Range / Nimg * 2
        Srange = Int(Srange / Nimg * stage.ZMMtoSteps) * Nimg / stage.ZMMtoSteps

        For zz = 0 To Nimg - 1
            Camera.capture_binned(BinnedImage(zz))
            If zz < (Nimg - 1) Then  stage.Move_r(stage.Zport, Srange / Nimg)
            CM(zz) = FT.FindCenterOfMass(BinnedImage(zz))
            Form1.Chart1.Series(1).Points.AddXY(zz, CM(zz))
            If CM(zz) = CM.Max Then CmXMax = zz
        Next


        focus = (Nimg - CmXMax) * (Srange / Nimg)
        stage.Move_r(stage.Zport, -focus)


        Z0 = stage.Z
        Return focus
    End Function

    Public Sub Release()
        If Camera.FFsetup Then Camera.Flatfield(1)
        Camera.SetExposure(exp, False)
        Camera.SetBinning(False, bin)
    End Sub



End Class
