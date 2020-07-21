Public Class FocusStructure
    Public Range As Single
    Public Nimg As Single
    Public bin As Integer
    Dim Steps, MicroSteps As Single
    'focus point
    Public Z0 As Single
    Dim readout As Single
    Dim exp As Single

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
        For zz = 0 To Nimg - 1
            If zz > 0 Then Stage.MoveRelative(Stage.Zaxe, steps) ' The first time should be acquired with no movement 
            Pos(zz) = Stage.Z
            Camera.Capture(BinnedImage(0))
            CM(zz) = FT.FindCenterOfMass(BinnedImage(0))
            'SaveSinglePageTiff("C:\temp\POS- " + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(0), Camera.Wbinned, Camera.Hbinned)
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * steps) * 1000), CM(zz))
            Application.DoEvents()
            If CM(zz) = CM.Max Then decline = 0
            If CM(zz) < CM.Max Then decline += 1
            If decline = 10 Then Exit For

        Next

        ' For some stupid reason camera captures the previous frame? So at focous point it delivers an image from ther previous position!
        ' That is why I have +2 instead of +1
        'Igo one step further to start the focus from there.

        Stage.MoveRelative(Stage.Zaxe, -Steps * (decline + 1) * 1.2)
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
            CM(zz) = FT.FindCenterOfMass(BinnedImage(0))
            '   SaveSinglePageTiff("C:\temp\2nd\POS- " + zz.ToString + Pos(zz).ToString + "CM- " + CM(zz).ToString + ".tif", BinnedImage(0), Camera.Wbinned, Camera.Hbinned)
            Form1.Chart1.Series(1).Points.AddXY(Int((zz * MicroSteps) * 1000), CM(zz))
            Application.DoEvents()
            If CM(zz) = CM.Max Then decline = 0
            If CM(zz) < CM.Max Then decline += 1
            If decline = 5 Then Exit For
        Next

        Form1.Chart1.Series(0).Points.Clear()
        Form1.Chart1.Series(1).Points.Clear()

        Stage.MoveRelative(Stage.Zaxe, -MicroSteps * (decline + 1))
        focus = Stage.Z


        Return focus
    End Function



    Public Sub Release()
        ' If Camera.FFsetup Then Camera.Flatfield(1)
        Camera.SetExposure(exp, False)
        Camera.SetBinning(False, bin)
    End Sub



End Class
