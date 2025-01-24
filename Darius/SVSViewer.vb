Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports OpenSlideNET
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class SVSViewer
    Inherits UserControl

    Private pictureBox As PictureBox

    Public Sub New(pictureBox As PictureBox)
        InitializeComponent()
        Me.pictureBox = pictureBox
    End Sub

    Public Sub LoadSVSImage(filePath As String)
        Using slide As OpenSlideImage = OpenSlideImage.Open(filePath)
            ' Get the dimensions of the highest resolution level
            Dim dimensions = slide.Dimensions

            ' Read the entire image at the highest resolution level
            Dim regionBytes As Byte() = slide.ReadRegion(0, 0, 0, dimensions.Width, dimensions.Height)

            ' Convert the byte array to a Bitmap
            Dim regionBitmap As Bitmap = ByteArrayToBitmap(regionBytes, dimensions.Width, dimensions.Height)

            ' fill the main picturebox0 with the image without changing the size of picturebox0
            pictureBox.Image = regionBitmap
        End Using
    End Sub

    Private Function ByteArrayToBitmap(bytes As Byte(), width As Integer, height As Integer) As Bitmap
        Dim bitmap As New Bitmap(width, height, PixelFormat.Format32bppArgb)
        Dim bitmapData As BitmapData = bitmap.LockBits(New Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Marshal.Copy(bytes, 0, bitmapData.Scan0, bytes.Length)
        bitmap.UnlockBits(bitmapData)
        Return bitmap
    End Function

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        ' 
        ' SVSViewer
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Name = "SVSViewer"
        Me.ResumeLayout(False)

    End Sub
End Class