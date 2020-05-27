Public Class LinearUnmixingStructure
    Dim A(2) As Single

    Public Sub New(Ain As Single())
        Array.Copy(Ain, A, Ain.GetLength(0))
        load_colormap()
    End Sub

    Public Sub Analyze(Imagein As FastBMP, ByRef Imageout As FastBMP)
        Imageout = New FastBMP(Imagein.width, Imagein.height, Imaging.PixelFormat.Format24bppRgb)
        Dim offset As Integer = Imagein.Stride - Imagein.width * 3
        Dim u As Integer
        Dim p As Integer
        For y = 0 To Imagein.height - 1
            For x = 0 To Imagein.width - 1

                u = (A(0) * Imagein.bytes(p) + A(1) * Imagein.bytes(p + 1) + A(2) * Imagein.bytes(p + 2))
                If u > 0 And u < 255 Then
                    Imageout.FillOriginalPixel(x, y, u, u, u)
                Else
                    Imageout.FillOriginalPixel(x, y, 0, 0, 0)
                End If
                p += 3
            Next
            p += offset
        Next
        Imageout.Unlock()
        Imageout.lock()
    End Sub



End Class
