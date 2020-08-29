Imports System.Windows.Forms.DataVisualization.Charting
Imports AForge.Imaging



Public Class ImageDisplay


    Public Width, Height As Integer


    Dim rawImage As ByteImage
    Public zoom As Boolean
    Public BmpPreview(10) As FastBMP
    Dim f As Integer
    Public GainR, GainG, GainB As Single
    Public busy As Boolean
    Public HistBin As Integer = 255
    Public ib, ic As Single
    Public RequestIbIc As Boolean
    Dim HistoChart As Chart
    Public Histogram() As Integer

    Public Sub New(W As Integer, H As Integer, ByRef HistoChart As Chart)
        Me.Width = W
        Me.Height = H
        Me.HistoChart = HistoChart
        rawImage.Width = W
        rawImage.Height = H
        ' Get Raw Data
        rawImage.Size = rawImage.Width * rawImage.Height
        ReDim rawImage.data(rawImage.Size - 1)
        For i = 0 To 10
            BmpPreview(i) = New FastBMP(W, H, Imaging.PixelFormat.Format24bppRgb)
        Next
        SetColorGain(Setting.Gett("GainR"), Setting.Gett("GainG"), Setting.Gett("GainB"))

    End Sub
    Public Sub AdjustBrightness()
        RequestIbIc = True
    End Sub

    Public Sub PlotHistogram()

        HistoChart.Series(0).Points.Clear()

        For j = 0 To HistBin
            HistoChart.Series(0).Points.AddXY(j + 1, Histogram(j))
        Next

    End Sub

    Public Function Preview(rawin As Byte(), Gained As Boolean)
        rawImage.data = rawin
        f += 1
        If f = 11 Then f = 1
        BmpPreview(f).MakeFromBytes(rawin)
        If RequestIbIc Then SetIbIc() : RequestIbIc = False
        'MakeHistogram()
        'PlotHistogram(HistoChart)

        Return BmpPreview(f).bmp
    End Function



    Public Sub SetColorGain(R As Single, G As Single, B As Single)
        GainR = R
        GainB = B
        GainG = G

        Setting.Sett("GainB", B)
        Setting.Sett("GainG", G)
        Setting.Sett("GainR", R)

        Camera.SetColorGain(R, G, B)
    End Sub



    Public Sub MakeHistogram()
        ReDim Histogram(HistBin)


        For i = 0 To rawImage.Size - 1 Step 4


            Histogram(rawImage.data(i) / Camera.CCMAtrix) += 1

        Next

        ' We never have zero intensity  this make  it possible  to find the min on the SetIbIc using the min function
        Histogram(0) = Histogram(1)
    End Sub
    Public Sub SetIbIc()


        MakeHistogram()
        Dim Hmax As Integer = 0
        Dim HCum(HistBin) As Integer
        Dim HCumMax, HCumMin As Integer
        'Accumulates the histogram bins
        Dim i As Integer
        For i = 0 To HistBin
            For ii = 0 To i
                HCum(i) += Histogram(ii) / HistBin
            Next
        Next

        HCumMax = HCum.Max
        HCumMin = HCum.Min

        For i = HistBin To 0 Step -1
            If Histogram(i) > HCumMax Then Exit For
        Next
        ic = i * 1.5
        If ic > HistBin Then ic = HistBin


        For i = 0 To ic
            If Histogram(i) > HCumMin Then Exit For
        Next
        ib = i


        Camera.SetMatrix(255 / ic)


    End Sub





End Class
