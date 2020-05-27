Imports AForge.Math
Imports AForge.Imaging.Filters
Imports AForge.Video.DirectShow



Public Class PreviewStructure
    Dim V As AForge.Controls.VideoSourcePlayer
    Public Width, Height As Integer
    Public Bmp, BmpLabel As Bitmap
    Public CameraAttached As Boolean
    Public X, Y, W As Single
    Dim videoDevices As New FilterInfoCollection(FilterCategory.VideoInputDevice)
    ' create video source


    Dim videoSource As Object

    Public Sub New()

        If videoDevices.Count > 0 Then
            videoSource = New VideoCaptureDevice(videoDevices(0).MonikerString)
        V = New AForge.Controls.VideoSourcePlayer With {
            .VideoSource = videoSource
        }
        videoSource.Start()
            CameraAttached = True
        End If




    End Sub

    Public Function Capture() As Bitmap
        V.Start()

        Dim Bmpf = New Bitmap(V.GetCurrentVideoFrame)
        Bmp = New Bitmap(Bmpf.Width, Bmpf.Height, Imaging.PixelFormat.Format24bppRgb)
        Bmp = Bmpf.Clone(New Rectangle(0, 0, Bmpf.Width, Bmpf.Height), Imaging.PixelFormat.Format24bppRgb)

        Width = Bmp.Width
        Height = Bmp.Height
        'BmpTissue = New FastBMP(CInt(Width * 2 / 3), Height, Imaging.PixelFormat.Format32bppArgb)

        'Dim Cropfilter As New Crop(New Rectangle(0, Height / 6, Width, Height * (2 / 3)))
        'BmpTissue = New FastBMP(Cropfilter.Apply(Bmp))

        'X = (1.4 / 5.5) * BmpTissue.width
        'W = (2.5 / 5.5) * BmpTissue.width

        'BmpTissue.GR.DrawRectangle(New Pen(Color.DarkBlue), New Rectangle(X, 0, W, W))


        Dim Cropfilter As New Crop(New Rectangle(246, 174, 350, 170))
        Dim FlipFilter As New Mirror(True, False)

        Bmp = FlipFilter.Apply(Cropfilter.Apply(Bmp))



        'Bmp.Save("C:\bmp.png")
        'BmpTissue.Save("C:\bmptissue.png")
        Return Bmp

    End Function


    Public Sub StopPreview()
        Try
            videoSource.Stop()
            V.Stop()
        Catch ex As Exception

        End Try



    End Sub





End Class
