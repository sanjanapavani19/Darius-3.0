'Imports System.Numerics

'Public Class PhasorUnmixStructure

'    Public Structure Triangle
'        Public edges() As Point
'        Public Clickindex As Byte
'        Public changed As Boolean


'        Public Function Area() As Single
'            Dim A As Single = Math.Abs((1 / 2) * (edges(0).X * edges(1).Y - edges(0).Y * edges(1).X - edges(0).X * edges(2).Y + edges(0).Y * edges(2).X + edges(1).X * edges(2).Y - edges(1).Y * edges(2).X))
'            Return A
'        End Function

'    End Structure

'    Public Image As StackImage
'    Public W, H As Integer
'    Public done As Boolean
'    Public components As Integer
'    Public T As Triangle

'    Private Panel As Integer

'    Public Sub New(width As Integer, height As Integer, c As Integer)
'        W = width
'        H = height
'        components = c
'        Image = New StackImage(W, H, components, Imaging.PixelFormat.Format24bppRgb)
'        Panel = PhasorS.panel
'        PhasorS.plot(Panel).resetROI()

'        ReDim T.edges(components - 1)

'        ReDim PhasorS.plot(Panel).Histogram.Bitmp(0).ROI(components - 1)
'        ReDim PhasorS.plot(Panel).Histogram.Bitmp(0).ROISelected(components - 1)


'        'Finding the edges of the phasor to assign the vertex randomly
'        'Dim min As Point
'        'Dim max As Point

'        'min = New Point(PhasorS.plot(Panel).width, PhasorS.plot(Panel).width)

'        'For u = 0 To PhasorS.plot(Panel).width - 1
'        '    For v = 0 To PhasorS.plot(Panel).width - 1
'        '        If PhasorS.plot(Panel).Histogram.data(0)(u, v) > 1 Then
'        '            If u < min.X Then min.X = u
'        '            If v < min.Y Then min.Y = v

'        '            If u > max.X Then max.X = u
'        '            If v > max.Y Then max.Y = v

'        '        End If

'        '    Next
'        'Next

'        'PhasorS.plot(Panel).FindEdges()


'        'T.edges(0) = PhasorS.plot(Panel).min
'        'T.edges(1) = PhasorS.plot(Panel).max


'        'If components = 3 Then
'        '    T.edges(2) = New Point(min.X, max.Y)
'        'End If


'        ReadVertex()

'        Analyze()
'    End Sub

'    Public Sub ReadVertex()
'        T.edges(0).X = Setting.Gett("VX1")
'        T.edges(0).Y = Setting.Gett("VY1")

'        T.edges(1).X = Setting.Gett("VX2")
'        T.edges(1).Y = Setting.Gett("VY2")

'        T.edges(2).X = Setting.Gett("VX3")
'        T.edges(2).Y = Setting.Gett("VY3")

'    End Sub


'    Public Sub Save_VerteX()
'        Setting.Sett("VX1", T.edges(0).X)
'        Setting.Sett("Vy1", T.edges(0).Y)

'        Setting.Sett("VX2", T.edges(1).X)
'        Setting.Sett("Vy2", T.edges(1).Y)

'        Setting.Sett("VX3", T.edges(2).X)
'        Setting.Sett("Vy3", T.edges(2).Y)

'    End Sub



'    Public Sub Vertex_Moved(x As Integer, y As Integer)


'        Panel = PhasorS.panel
'        For i = 0 To PhasorS.plot(Panel).Histogram.Bitmp(0).ROI.GetUpperBound(0)

'            If PhasorS.plot(Panel).Histogram.Bitmp(0).ROI(i).Contains(x, y) Then
'                PhasorS.plot(Panel).Histogram.Bitmp(0).ROISelected(i) = True
'                T.edges(i).X = x
'                T.edges(i).Y = y
'                T.changed = True
'                Save_VerteX()
'                DrawTriangleVertex()
'            End If
'        Next


'    End Sub


'    Public Sub Vertex_OnClick()
'        If T.Clickindex < 3 Then Exit Sub
'        Panel = PhasorS.panel

'        For i = 0 To PhasorS.plot(Panel).Histogram.Bitmp(0).ROI.GetUpperBound(0)
'            PhasorS.plot(Panel).Histogram.Bitmp(0).ROISelected(i) = False
'        Next
'    End Sub


'    Public Sub DrawTriangleVertex()

'        Panel = PhasorS.panel

'        For i = 0 To components - 1
'            PhasorS.plot(Panel).Histogram.Bitmp(0).ROI(i) = New Rectangle(T.edges(i).X - 10, T.edges(i).Y - 10, 20, 20)


'        Next
'        PhasorS.plot(Panel).Histogram.Bitmp(0).RefreshROI()
'        PhasorS.plot(Panel).Histogram.Bitmp(0).GR.DrawPolygon(New Pen(Color.White), T.edges)

'    End Sub

'    Public Sub Analyze()

'        DrawTriangleVertex()

'        Dim U, V As Single

'        'Making new triangles to compute the fractional intensities
'        Dim Tuv(2) As Triangle
'        ReDim Tuv(0).edges(2)
'        ReDim Tuv(1).edges(2)
'        ReDim Tuv(2).edges(2)

'        'The coordinates are copied to the new triangles because two vertex are common.
'        For i = 0 To 2
'            Tuv(0).edges(i).X = T.edges(i).X
'            Tuv(1).edges(i).X = T.edges(i).X
'            Tuv(2).edges(i).X = T.edges(i).X

'            Tuv(0).edges(i).Y = T.edges(i).Y
'            Tuv(1).edges(i).Y = T.edges(i).Y
'            Tuv(2).edges(i).Y = T.edges(i).Y
'        Next

'        PhasorS.Image.Bitmp(0).Unlock()
'        Dim TArea As Single = T.Area
'        Dim Fraction(components - 1) As Single
'        For x = 0 To W - 1
'            For y = 0 To H - 1

'                U = PhasorS.plot(Panel).XYmap(x, y).real
'                V = PhasorS.plot(Panel).XYmap(x, y).imaginary
'                'U = 10
'                'V = 20
'                If U > 0 And V > 0 And U < PhasorS.plot(0).width And V < PhasorS.plot(0).width Then
'                    For i = 1 To 1
'                        'to replace the  common vertics for each component
'                        Tuv(i).edges(i).X = U
'                        Tuv(i).edges(i).Y = V

'                        Fraction(i) = Tuv(i).Area / TArea
'                        If Fraction(i) > 1 Then Fraction(i) = 1
'                        If Fraction(i) < 0 Then Fraction(i) = 0

'                        'till I find how to save 32 bit image.
'                        'Image.data(i)(x, y) = Fraction(i) * Phasor.Image.data(0)(x, y)

'                        Image.data(i)(x, y) = Fraction(i) * PhasorS.Image.Intensity8bit(x, y)

'                        Fraction(i) *= 255
'                        Image.Bitmp(i).FillOriginalPixel(x, y, 0, Image.data(i)(x, y), 0)
'                    Next




'                End If



'                'PhasorS.Image.Bitmp(0).SetPixel(x, y, 0, Fraction(1), 0, PhasorS.Image.Intensity8bit(x, y))
'            Next
'        Next


'        Image.Bitmp(0).MakeNewFromBytes(W, H, Imaging.PixelFormat.Format24bppRgb)
'        Image.Bitmp(1).MakeNewFromBytes(W, H, Imaging.PixelFormat.Format24bppRgb)
'        Image.Bitmp(2).MakeNewFromBytes(W, H, Imaging.PixelFormat.Format24bppRgb)
'        PhasorS.Segmentation = SegmentationType.Unmixed
'        done = True


'        ' checking Manders

'        'Dim RG As Single
'        'Dim R2 As Single
'        'Dim G2 As Single





'        'For x = 0 To W - 1
'        '    For y = 0 To H - 1

'        '        RG += Image.data(0)(x, y) * Image.data(1)(x, y)
'        '        R2 += (Image.data(0)(x, y)) ^ 2
'        '        G2 += (Image.data(1)(x, y)) ^ 2
'        '    Next
'        'Next

'        'Form1.Label7.Text = (RG / Math.Sqrt(R2 * G2)).ToString










'    End Sub



'End Class