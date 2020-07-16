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
    Public Sub New(Range As Single, Nimg As Integer, bin As Integer)

        Me.Range = Range
        Me.Nimg = Nimg
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
        Camera.SetExposure(exp / bin ^ 2, False)
        Camera.SetBinning(True, bin)
        ReDim BinnedImage(Nimg - 1)
        'To use only integer steps 
        '      Range = Int(Range / Nimg * stage.ZMMtoSteps) * Nimg / stage.ZMMtoSteps

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
        Dim Startpoint As Single
        Dim direction As Integer = 1
        Startpoint = Stage.Z
            Form1.Chart1.Series(0).Points.Clear()
            Form1.Chart1.Series(1).Points.Clear()
            For zz = 0 To Nimg - 1
                Camera.Capture(BinnedImage(zz))
                If zz < (Nimg - 1) Then Stage.MoveRelative(Stage.Zaxe, (Srange) / Nimg) ' The lastimage should be acquired with no movement afterwards
                CM(zz) = FT.FindCenterOfMass(BinnedImage(zz))
                Form1.Chart1.Series(1).Points.AddXY(Int((zz * Srange / Nimg) * 1000), CM(zz))
                If CM(zz) = CM.Max Then CmXMax = zz
                Application.DoEvents()

            Next

        If CmXMax = 0 Then direction = -1
        If CmXMax = Nimg - 1 Then direction = 1



        Startpoint = Stage.Z

        ReDim CM(Nimg * 10 - 1)
        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()
        For zz = 0 To Nimg * 10 - 1
            Camera.Capture(BinnedImage(0))
            If zz < (Nimg * 10 - 1) Then Stage.MoveRelative(Stage.Zaxe, (Srange) / Nimg * direction) ' The lastimage should be acquired with no movement afterwards
            CM(zz) = FT.FindCenterOfMass(BinnedImage(0))
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * Srange / Nimg) * 1000), CM(zz))
            If CM(zz) = CM.Max Then CmXMax = zz
            Application.DoEvents()

        Next



        Dim focus As Single = (CmXMax - 1) * (Srange / Nimg)

        If direction = 1 Then Stage.MoveAbsolute(Stage.Zaxe, Startpoint + focus)
        If direction = -1 Then Stage.MoveAbsolute(Stage.Zaxe, Startpoint - focus)


        For j = 0 To 2
            Srange = Srange / 2

            Startpoint = Stage.Z
            Form1.Chart1.Series(0).Points.Clear()
            Form1.Chart1.Series(1).Points.Clear()
            For zz = 0 To Nimg - 1
                Camera.Capture(BinnedImage(zz))
                If zz < (Nimg - 1) Then Stage.MoveRelative(Stage.Zaxe, (Srange) / Nimg) ' The lastimage should be acquired with no movement afterwards

                CM(zz) = FT.FindCenterOfMass(BinnedImage(zz))
                Form1.Chart1.Series(1).Points.AddXY(Int((zz * Srange / Nimg) * 1000), CM(zz))
                If CM(zz) = CM.Max Then CmXMax = zz
                Application.DoEvents()

            Next

            focus = (CmXMax - 1) * (Srange / Nimg)
            Stage.MoveAbsolute(Stage.Zaxe, Startpoint + focus)

        Next

        Stage.MoveRelative(Stage.Zaxe, (Srange / Nimg))
        focus = Stage.Z


        'Z0 = stage.Z
        Return focus
    End Function

    Private Function SacnFocus(R As Single) As Integer

    End Function


    Public Sub Release()
        If Camera.FFsetup Then Camera.Flatfield(1)
        Camera.SetExposure(exp, False)
        Camera.SetBinning(False, bin)
    End Sub



End Class
