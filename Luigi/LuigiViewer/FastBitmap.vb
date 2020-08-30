Imports System.Drawing.Imaging

Public Class FastBMP
    Public bmp As Bitmap
    Public width As Integer
    Public height As Integer
    Public bytes() As Byte
    Private byteCopy() As Byte
    Public GR As Graphics
    Public ROIColor As Color
    Public ROI() As Rectangle
    Public ROIindex As Integer

    Public Sub MakeNew(W As Integer, H As Integer, fmt As PixelFormat)
        bmp = New Bitmap(W, H, fmt)
        width = W
        height = H
        GR = Graphics.FromImage(bmp)
        ReDim bytes(W * H * 4 - 1)
        ReDim byteCopy(W * H * 4 - 1)
        ROIColor = Color.White
        ResetROI()
    End Sub

    Public Sub MakeNewfromBmp(b As Bitmap)
        bmp = New Bitmap(b)
        width = bmp.Width
        height = bmp.Height
        GR = Graphics.FromImage(bmp)
        ReDim bytes(width * height * 4 - 1)
        Unlock()
        ROIColor = Color.White
    End Sub


    Public Sub MakeNewFromBytes(W As Integer, H As Integer, fmt As PixelFormat)
        bmp = New Bitmap(W, H, fmt)
        width = W
        height = H
        byteToBitmap(bytes, bmp)
        GR = Graphics.FromImage(bmp)
        ROIColor = Color.White
    End Sub

    Public Sub RefreshROI()
        Reset()
        For i = 0 To ROI.GetUpperBound(0)
            GR.DrawRectangle(New Pen(ROIColor), ROI(i))
        Next
    End Sub


    Public Sub ResetROI()
        ReDim ROI(0)
        ROI(0) = New Rectangle(0, 0, width, height)
        ROIindex = 0
        Reset()
    End Sub
    Public Sub Reset()
        byteToBitmap(bytes, bmp)
    End Sub

    Public Sub Unlock()
        BitmapToBytes(bmp, bytes)
    End Sub

    Public Sub SetPixel(x As Integer, y As Integer, clr As Color)
        Dim index As Integer = (y * width + x) * 4
        byteCopy(index) = clr.R
        byteCopy(index + 1) = clr.G
        byteCopy(index + 2) = clr.B
        byteCopy(index + 3) = clr.A
    End Sub

    Public Sub SetPixel(x As Integer, y As Integer, byteR As Byte, byteG As Byte, byteB As Byte, byteA As Byte)
        Dim index As Integer = (y * width + x) * 4
        byteCopy(index) = byteR
        byteCopy(index + 1) = byteG
        byteCopy(index + 2) = byteB
        byteCopy(index + 3) = byteA
    End Sub

    Public Sub FillOriginalPixel(x As Integer, y As Integer, byteR As Byte, byteG As Byte, byteB As Byte, byteA As Byte)
        Dim index As Integer = (y * width + x) * 4
        bytes(index) = byteR
        bytes(index + 1) = byteG
        bytes(index + 2) = byteB
        bytes(index + 3) = byteA
    End Sub




    Public Sub lock()
        byteToBitmap(bytes, bmp)
    End Sub



End Class





Module Bitmaps
    Public Sub byteToBitmap(bytes() As Byte, bmp As Bitmap)
        Try
            Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)
            ' Copy the RGB values back to the bitmap
            Runtime.InteropServices.Marshal.Copy(bytes, 0, bmpData.Scan0, bmpData.Stride * bmp.Height)
            'Runtime.InteropServices.Marshal.Copy(bytes, 0, bmpData.Scan0, bmp.Width * bmp.Height * 3)
            ' Unlock the bits.
            bmp.UnlockBits(bmpData)
            If bmp.PixelFormat = PixelFormat.Format8bppIndexed Then
                bmp.Palette = GetGrayScalePalette()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Function BitmapToBytes(bmp As Bitmap, bytes() As Byte)
        Dim rect As Rectangle = New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)
        ' Copy the RGB values back to the bitmap
        Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, bytes, 0, bmpData.Stride * bmp.Height)
        ' Unlock the bits.
        bmp.UnlockBits(bmpData)
        Return bmpData.Stride
    End Function



    Public Sub SetPixelByte32(ByRef bytes() As Byte, X As Integer, Y As Integer, width As Integer, clr As Color)
        Dim index As Integer = (Y * width + X) * 4
        bytes(index) = clr.B
        bytes(index + 1) = clr.G
        bytes(index + 2) = clr.R
        bytes(index + 3) = clr.A

    End Sub

    Public Sub SetPixelByte32Linear(ByRef bytes() As Byte, index As Integer, clr As Color)
        bytes(index) = clr.B
        bytes(index + 1) = clr.G
        bytes(index + 2) = clr.R
        bytes(index + 3) = clr.A
    End Sub
    Private Function GetGrayScalePalette() As ColorPalette
        Dim bmp As Bitmap = New Bitmap(1, 1, Imaging.PixelFormat.Format8bppIndexed)
        Dim monoPalette As ColorPalette = bmp.Palette
        Dim entries() As Color = monoPalette.Entries

        Dim i As Integer
        For i = 0 To 256 - 1 Step i + 1
            entries(i) = Color.FromArgb(i, i, i)
        Next

        Return monoPalette
    End Function





End Module
