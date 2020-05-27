'Public Class PhasorS
'    Dim CS()() As Single
'    Public Shared dimension As Integer
'    Public Shared panels, panel As Integer
'    Public Shared W, H, D As Integer
'    Public Shared FDImage As StackImage
'    Public Shared Segmentation As SegmentationType
'    Public Shared Image As ImageStructure
'    Public Shared plot() As PlotStructure

'    Public Sub New(Win As Integer, Hin As Integer, Din As Integer, plotw As Integer, thbar As TrackBar)
'        W = Win
'        H = Hin
'        D = Din


'        ReDim CS(D)
'        ReDim CS(0)(D - 1)


'        For n = 1 To D - 1 Step 2
'            ReDim CS(n)(D - 1)
'            ReDim CS(n + 1)(D - 1)
'            For z = 0 To D - 1
'                CS(0)(z) = 1
'                CS(n)(z) = Math.Sin(2 * Math.PI * n / D * z)
'                CS(n + 1)(z) = Math.Cos(2 * Math.PI * n / D * z)

'            Next

'        Next


'        dimension = D - 1
'        If dimension > 6 Then dimension = 6

'        panels = factorial(dimension) / (2 * factorial(dimension - 2))
'        ReDim plot(panels - 1)

'        For i = 0 To panels - 1
'            plot(i) = New PlotStructure(plotw, i, thbar)
'        Next

'        Image = New ImageStructure(W, H, D)

'        Image.Bitdepth = 8

'        load_colormap()
'    End Sub
'    Public Sub Process(stack As Single()(,))

'        ConvertStacktoFD(stack, FDImage.data)
'        Image.Create()

'        For i = 0 To panels - 1
'            plot(i).MakePhasorHistogram()
'        Next

'    End Sub


'    Public Sub Process(Bytes As Byte())
'        Image = New ImageStructure(W, H, D)
'        ConvertBytestoImage(Bytes)
'        Image.Create()
'        plot(0).MakePhasorHistogram()
'    End Sub



'    Public Sub ConvertBytestoImage(Bytes As Byte())
'        Dim i As Integer
'        For y = 0 To H - 1
'            For x = 0 To W - 1


'                For j = 0 To 2
'                    Image.data(0)(x, y) += Bytes(i + j)
'                    Image.data(1)(x, y) += Bytes(i + j) * CS(1)(j)
'                    Image.data(2)(x, y) += Bytes(i + j) * CS(2)(j)
'                Next

'                Image.data(1)(x, y) = (Image.data(1)(x, y) / Image.data(0)(x, y) + 1) / 2 * plot(0).width
'                Image.data(2)(x, y) = (Image.data(2)(x, y) / Image.data(0)(x, y) + 1) / 2 * plot(0).width
'                i += 3
'            Next
'        Next
'    End Sub



'    Public Function ConvertStacktoFD(stack()(,) As Single, ByRef stackT()(,) As Single)

'        Dim W As Integer = stack(0).GetLength(0)
'        Dim H As Integer = stack(0).GetLength(1)
'        Dim D As Integer = stack.GetLength(0)

'        ' 'Here is the transform 

'        For i = 0 To dimension
'            ReDim stackT(i)(W - 1, H - 1)
'        Next

'        For i = 0 To dimension
'            For x = 0 To W - 1
'                For y = 0 To H - 1
'                    For z = 0 To D - 1

'                        stackT(i)(x, y) += stack(z)(x, y) * CS(i)(z)

'                    Next
'                Next
'            Next
'        Next



'        Return StackT

'    End Function



'    Public Sub PhasorToImage(p As Integer)
'        Dim U, V As Integer

'        For y = 0 To Image.Height - 1
'            For x = 0 To Image.Width - 1
'                U = plot(p).XYmap(x, y).Real
'                V = plot(p).XYmap(x, y).Imaginary
'                If plot(p).HighlightMap(U, V) > 0 Then
'                    'Image.HighlightMap(x, y) = plot(p).HighlightMap(U, V)
'                    Image.PaintMap(x, y) = plot(p).HighlightMap(U, V)
'                End If
'            Next
'        Next
'    End Sub
'    Public Sub ImageToPhasor(p As Integer, R As Rectangle, index As Integer)
'        Dim U, V As Integer

'        'R is to tell which region to look at
'        'This is to fix the over the range
'        If R.Right > Image.Width Then R = New Rectangle(R.Left, R.Top, Image.Width - R.Left, R.Height)
'        If R.Bottom > Image.Height Then R = New Rectangle(R.Left, R.Top, R.Height, Image.Height - R.Top)


'        For y = R.Top To R.Bottom - 1
'            For x = R.Left To R.Right - 1
'                U = plot(p).XYmap(x, y).Real
'                V = plot(p).XYmap(x, y).Imaginary
'                Image.HighlightMap(x, y) = index
'                plot(p).PaintMap(U, V) = Image.HighlightMap(x, y)


'            Next
'        Next


'    End Sub
'End Class
