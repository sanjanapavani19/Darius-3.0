'Imports System.Drawing.Imaging
'Imports System.Numerics


'Public Class PlotStructure

'    Public Histogram As StackImage
'    ' min and max of the phasor plot extention on 2D plot
'    Public min As Point
'    Public max As Point

'    Public zoomC As Single
'    Public zoomS As Single
'    Public OffC As Single
'    Public OffS As Single
'    Public NewZoom As Boolean
'    'Transformation parameteres
'    Public sx, sy As Single
'    Public Ax, Ay As Single
'    Public q As Single
'    Public ThresholdBar As TrackBar
'    Public width As Integer
'    Private panel As Integer
'    Public XYmap(,) As FFComplex
'    Public HighlightMap(,) As Integer
'    Public PaintMap(,) As Integer
'    Public UpdatePending As Boolean
'    'Public unmix As PhasorUnmixStructure

'    Structure FFComplex
'        Dim real As Single
'        Dim imaginary As Single
'        Public Sub New(rin As Single, Iin As Single)
'            real = rin
'            imaginary = Iin
'        End Sub
'    End Structure

'    Public Sub New(W As Integer, p As Integer, thBar As TrackBar)

'        panel = p
'        'If Phasor.type = Imagetype.FD Then width = W / 2 Else width = W
'        width = W
'        Histogram = New StackImage(W, W, 1, PixelFormat.Format32bppArgb)

'        zoomC = 1
'        zoomS = 1

'        ThresholdBar = thBar

'        ReDim XYmap(PhasorS.w, PhasorS.h)


'        ColorGenerator()
'    End Sub

'    Public Sub SomethingChanged()

'        UpdatePending = True

'    End Sub
'    Public Sub MakePhasorHistogram()

'        Dim U, V As Integer
'        Dim C, S As Integer

'        Dim x1, x2, y1, y2 As Integer

'        ReDim XYmap(PhasorS.Image.Width, PhasorS.Image.Height)

'        x1 = PhasorS.Image.Bitmp(0).ROI(0).Left
'        x2 = x1 + PhasorS.Image.Bitmp(0).ROI(0).Width - 1

'        y1 = PhasorS.Image.Bitmp(0).ROI(0).Top
'        y2 = y1 + PhasorS.Image.Bitmp(0).ROI(0).Height - 1

'        Axisfinder(panel, C, S)




'        If NewZoom Then

'            OffC = OffC + (Histogram.Bitmp(0).ROI(0).Left) / zoomC
'            OffS = OffS + (Histogram.Bitmp(0).ROI(0).Top) / zoomS

'            zoomC *= Me.width / Histogram.Bitmp(0).ROI(0).Width
'            zoomS *= Me.width / Histogram.Bitmp(0).ROI(0).Height
'        End If

'        If zoomC = Single.PositiveInfinity Or zoomS = Single.PositiveInfinity Then Exit Sub
'        ReDim Histogram.data(0)(Me.width - 1, Me.width - 1)

'        ReDim HighlightMap(Me.width - 1, Me.width - 1)
'        ReDim PaintMap(width - 1, width - 1)




'        For x = x1 To x2
'            For y = y1 To y2

'                If PhasorS.Image.data(0)(x, y) > PhasorS.Image.threshold(0) Then




'                    U = CInt((PhasorS.Image.data(C)(x, y) - OffC) * zoomC)
'                    V = CInt((PhasorS.Image.data(S)(x, y) - OffS) * zoomS)


'                If U < Me.width - 1 And U > 0 And V < Me.width - 1 And V > 0 Then
'                    Histogram.data(0)(U, V) = 255
'                    XYmap(x, y) = New FFComplex(U, V)

'                End If

'                      End If
'            Next
'        Next


'        'For U = 0 To Me.width - 1
'        '    For V = 0 To Me.width - 1

'        '        If Histogram.data(0)(U, V) > 0 Then
'        '            Histogram.data(0)(U, V) = (Math.Sqrt(Histogram.data(0)(U, V)))

'        '        End If
'        '    Next
'        'Next

'        'Histogram.data(0) = Bin2DArray(2, Histogram.data(0))
'        Histogram.Max(0) = 1
'        Histogram.Min(0) = 0

'        FindMinMax()

'        UpdatePending = False
'        PaintPhasorHistogram(Colortype.RGB, Histogram.Min(0), Histogram.Max(0))
'    End Sub
'    Public Sub ExpandPhasor()
'        'first, lets find the center of mass

'        Dim mean As Point
'        Dim Ave As Single

'        For U = 0 To Me.width - 1
'            For V = 0 To Me.width - 1
'                Ave += Histogram.data(0)(U, V)
'            Next
'        Next


'        For U = 0 To Me.width - 1
'            For V = 0 To Me.width - 1

'                If Histogram.data(0)(U, V) > 0 Then
'                    mean.X += Histogram.data(0)(U, V) * V

'                End If
'            Next
'        Next


'        mean.X = mean.X / Ave
'        mean.Y = mean.Y / Ave

'        Dim H(width - 1, width - 1) As Single
'        Dim R, Q As Single
'        Dim X, Y As Single
'        Dim U2 As Single

'        For U = 0 To Me.width - 1
'            For V = 0 To Me.width - 1
'                U2 = (PhasorS.Image.data(3)(X, Y) + width / 2 - OffC)

'                If Histogram.data(0)(U, V) > 0 Then
'                    R = Math.Sqrt((U - mean.X) ^ 2 + (V - mean.Y) ^ 2)
'                    Q = Math.Atan2((V - mean.Y), (U - mean.X))
'                    X = R * Math.Cos(Q) + mean.X
'                    Y = R * Math.Sin(Q) + mean.Y
'                    H(X, Y) = Histogram.data(0)(U, V)
'                End If
'            Next
'        Next
'        ReDim Histogram.data(0)(width - 1, width - 1)

'        Histogram.data(0) = H

'        FindMinMax()

'        PaintPhasorHistogram(Colortype.Grey, Histogram.Min(0), Histogram.Max(0))



'    End Sub
'    Public Sub FindMinMax()
'        'finding Max min
'        Histogram.Max(0) = 0
'        Histogram.Min(0) = 0
'        'Minimum is always zero. No need to find it


'        For U = 0 To Me.width - 1
'            For V = 0 To Me.width - 1

'                If Histogram.data(0)(U, V) > 0 Then

'                    If Histogram.data(0)(U, V) > Histogram.Max(0) Then Histogram.Max(0) = Histogram.data(0)(U, V)
'                    If Histogram.data(0)(U, V) < Histogram.Min(0) Then Histogram.Min(0) = Histogram.data(0)(U, V)
'                End If
'            Next
'        Next


'        ThresholdBar.Value = 0
'    End Sub

'    Public Sub PaintPhasorHistogram(type As Colortype, min As Single, max As Single)

'        ReDim Histogram.Bitmp(0).bytes(Me.width * Me.width * 4 - 1)

'        Dim p As Integer = 0
'        Dim T As Single = 0
'        If min = 0 Then min = 1
'        If max = min Then min = max - 1

'        min = Math.Min(Histogram.threshold(0), min)

'        If type = Colortype.RGB Then
'            For j = 0 To Me.width - 1
'                For i = 0 To Me.width - 1

'                    If Histogram.data(0)(i, j) > Histogram.threshold(0) Then

'                        T = (Histogram.data(0)(i, j) - min) / (max - min)
'                        If T < 0 Then T = 0



'                        Histogram.Bitmp(0).bytes(p) = rr(T * 1000)
'                        Histogram.Bitmp(0).bytes(p + 1) = gg(T * 1000)
'                        Histogram.Bitmp(0).bytes(p + 2) = bb(T * 1000)
'                        Histogram.Bitmp(0).bytes(p + 3) = 255

'                    End If
'                    p += 4
'                Next
'            Next



'        Else

'            For j = 0 To Me.width - 1
'                For i = 0 To Me.width - 1


'                    Histogram.Bitmp(0).bytes(p) = 0
'                    Histogram.Bitmp(0).bytes(p + 1) = Histogram.data(0)(i, j)
'                    Histogram.Bitmp(0).bytes(p + 2) = Histogram.data(0)(i, j)
'                    Histogram.Bitmp(0).bytes(p + 3) = 255

'                    p += 4
'                Next
'            Next

'        End If
'        Histogram.Bitmp(0).MakeNewFromBytes(Me.width, Me.width, PixelFormat.Format32bppArgb)


'    End Sub

'    Public Sub ResetZoom()

'        ' I kept the panel intentionally P because resetting the zoom factors doesn't 
'        'change the active panel number
'        OffC = 0
'        OffS = 0

'        zoomC = 1
'        zoomS = 1

'        Histogram.Bitmp(0).ResetROI()
'    End Sub

'    Public Sub resetROI()

'        Histogram.Bitmp(0).ResetROI()

'        Histogram.Bitmp(0).Reset()
'        ReDim HighlightMap(Me.width, Me.width)
'        PhasorS.Image.ResetHighlightMap()
'        PhasorS.Image.ResetPaintMap()


'        'T.Clickindex = 0
'    End Sub


'    Public Sub ResetPaintMap()
'        ReDim PaintMap(Me.width, Me.width)
'    End Sub



'    Public Sub Axisfinder(panel As Integer, ByRef C As Integer, ByRef S As Integer)
'        'Counting to find out the couples that are supposed to be calculated . This is based on the panel number. For example panel 1 is  1,2. Panel 2  is 1,3 (if exist)

'        Dim PanelCounter As Integer
'        For C = 1 To PhasorS.dimension
'            For S = 1 To PhasorS.dimension

'                If S > C Then
'                    PanelCounter += 1
'                    If PanelCounter > panel Then Exit For
'                End If
'            Next
'            If PanelCounter > panel Then Exit For
'        Next

'1:

'    End Sub


'    Public Sub Highlight(R As Rectangle)
'        'this is with rectangle

'        For u = 0 To Me.width - 1
'            For v = 0 To Me.width - 1
'                If Histogram.data(0)(u, v) > Histogram.threshold(0) Then
'                    If R.Contains(u, v) Then
'                        HighlightMap(u, v) = Histogram.Bitmp(0).ROIindex + 1
'                    End If
'                End If
'            Next
'        Next

'    End Sub




'    Public Sub Refresh(ByRef pbox As PictureBox)

'        If UpdatePending Then MakePhasorHistogram()
'        NewZoom = False
'        pbox.Image = Histogram.Bitmp(0).bmp

'    End Sub

'    Public Sub FindEdges()
'        min = New Point(width, width)

'        For u = 0 To width - 1
'            For v = 0 To width - 1
'                If PhasorS.plot(panel).Histogram.data(0)(u, v) > 1 Then
'                    If u < min.X Then min.X = u
'                    If v < min.Y Then min.Y = v

'                    If u > max.X Then max.X = u
'                    If v > max.Y Then max.Y = v

'                End If

'            Next
'        Next

'    End Sub

'    Public Sub PaintHighlighted()
'        Histogram.Bitmp(0).Reset()
'        Histogram.Bitmp(0).Unlock()
'        Dim K As Integer

'        For u = 0 To width - 1
'            For v = 0 To width - 1
'                If PaintMap(u, v) > K Then K = PaintMap(u, v)

'            Next
'        Next

'        Dim colors(K) As Color

'        For kk = 0 To K
'            'colors(kk) = HSBtoARGB(255, kk * 330 / K, 1, 1)
'            colors(kk) = GetColor(255, kk)
'        Next

'        Dim I As Integer

'        For u = 0 To width - 1
'            For v = 0 To width - 1
'                If PaintMap(u, v) > 0 Then
'                    I = Histogram.data(0)(u, v) / Histogram.Max(0) * 255
'                    Histogram.Bitmp(0).SetPixel(u, v, colors(PaintMap(u, v)).R, colors(PaintMap(u, v)).G, colors(PaintMap(u, v)).B, 255)
'                End If
'            Next
'        Next
'        Histogram.Bitmp(0).lock()
'    End Sub
'End Class