'Imports System.Drawing.Imaging
'Public Class ImageStructure
'    Inherits StackImage
'    'This is converted and processed image

'    Public IsDragging As Boolean
'    Public Intensity8bit(,) As Byte

'    Public HighlightMap(,) As Integer
'    Public PaintMap(,) As Integer
'    Dim MaxPaintMap As Integer
'    Public Sub New(width As Integer, height As Integer, depth As Integer)
'        MyBase.New(width, height, depth, PixelFormat.Format32bppArgb)
'        ReDim Intensity8bit(width - 1, height - 1)
'        ReDim HighlightMap(width - 1, height - 1)
'        ReDim PaintMap(width - 1, height - 1)

'    End Sub

'    Public Sub ResetHighlightMap()
'        ReDim HighlightMap(Width - 1, Height - 1)
'        Bitmp(0).Reset()
'    End Sub

'    Public Sub ResetPaintMap()
'        ReDim PaintMap(Width - 1, Height - 1)
'    End Sub





'    Public Sub ShowPaintMapd()
'        'MaxPaintMap = 0
'        'For y = 0 To Height - 1
'        '    For x = 0 To Width - 1
'        '        If PaintMap(x, y) > MaxPaintMap Then MaxPaintMap = PaintMap(x, y)
'        '    Next
'        'Next
'        Bitmp(0).Unlock()
'        Dim colors As Color
'        For y = 0 To Height - 1
'            For x = 0 To Width - 1
'                If PaintMap(x, y) > 0 Then

'                    colors = GetColor(Intensity8bit(x, y), PaintMap(x, y))
'                    '  colors = GetColor(255, PaintMap(x, y))
'                    'colors = HSBtoARGB(255, (PaintMap(x, y)) * 300 / MaxPaintMap, 1, 1)
'                    ' colors = Color.FromArgb(255, Intensity8bit(x, y), Intensity8bit(x, y), Intensity8bit(x, y))
'                    'If PaintMap(x, y) = 0 Then colors = Color.Black
'                    Bitmp(0).SetPixel(x, y, colors)
'                End If
'            Next
'        Next
'        Bitmp(0).lock()
'    End Sub
'    Public Sub Create()

'        ReDim Bitmp(0).bytes(Width * Height * 4 - 1)
'        'Here is filling the byte . 32 bits is better, first because  of stride problem,
'        'seecond because we might want to have transparency as intentsity, and keep the rest for the color

'        Dim T, P As Integer
'        For y = 0 To Height - 1
'            For x = 0 To Width - 1

'                If data(0)(x, y) > threshold(0) Then
'                    T = CInt(data(0)(x, y) / 3)
'                    Bitmp(0).bytes(P) = T
'                    Bitmp(0).bytes(P + 1) = T
'                    Bitmp(0).bytes(P + 2) = T
'                    Bitmp(0).bytes(P + 3) = 255
'                    Intensity8bit(x, y) = T
'                End If
'                P += 4

'            Next
'        Next

'        Bitmp(0).MakeNewFromBytes(Width, Height, PixelFormat.Format32bppArgb)
'    End Sub






'End Class