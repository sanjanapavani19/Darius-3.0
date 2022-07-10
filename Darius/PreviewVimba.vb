
Imports AForge.Imaging.Filters
Imports AForge.Video.DirectShow
Imports AForge.Imaging
Imports AForge.Math.Geometry
Imports AVT.VmbAPINET
Imports System.Drawing.Imaging


Public Class PreviewVimba

    Public Width, Height As Integer
    Public Bmp, BmpVimba, bmpf As Bitmap
    Public ZmapBmp As FastBMP
    Public Exposure As Single
    Public R As Rectangle
    Dim mySys As Vimba
    Dim mCameras As CameraCollection
    Dim mCamera As Camera
    Dim frame As Frame
    Dim features As FeatureCollection
    Dim Deivative As CentralDerivitavie
    Dim BLure As FFTW_VB_Real
    Dim expFeature As Feature
    Dim Preview_Z As Single
    Dim Z As Integer
    Dim Zsteps As Single
    Dim Pbar As ProgressBar
    Dim MaxMap() As Single
    Dim Zmmap(,) As Single
    Dim C(), S() As Single
    Dim X0, Y0, W, H As Integer
    Dim GreyEdge()() As Single

    ' create video source
    Public Sub New(Z As Integer, Zsteps As Single, Pbar As ProgressBar)
        mySys = New Vimba
        mySys.Startup()
        mCameras = mySys.Cameras
        mCamera = mCameras(0)
        mCamera.Open(VmbAccessModeType.VmbAccessModeFull)
        features = mCamera.Features

        X0 = Setting.Gett("Preview_X")
        Y0 = Setting.Gett("Preview_Y")
        W = Setting.Gett("Preview_W")
        H = Setting.Gett("Preview_H")
        R = New Rectangle(X0, Y0, W, H)

        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)

        Width = BmpVimba.Width
        Height = BmpVimba.Height


        Deivative = New CentralDerivitavie(Width, Height)
        BLure = New FFTW_VB_Real(Height, Width)
        BLure.MakeGaussianReal(0.005, BLure.MTF, 2)
        Preview_Z = Setting.Gett("Preview_Z")
        Me.Z = Z
        Me.Zsteps = Zsteps
        Me.Pbar = Pbar
        Pbar.Maximum = Z
        ReDim GreyEdge(Z - 1)
        ReDim C(Z - 1)
        ReDim S(Z - 1)
        For zz = 0 To Z - 1
            ReDim GreyEdge(zz)(Width * Height - 1)
            C(zz) = Math.Cos(2 * Math.PI / Z * zz)
            S(zz) = Math.Sin(2 * Math.PI / Z * zz)
        Next
        'load_colormap()
    End Sub

    Public Function Capture(exposure As Integer, focus As Integer) As Bitmap



        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)


        Width = BmpVimba.Width
        Height = BmpVimba.Height

        Dim CropFilter As New Crop(R)
        Bmp = CropFilter.Apply(BmpVimba)

        Dim Flip As Mirror = New Mirror(False, True)
        Flip.ApplyInPlace(Bmp)

        Return Bmp

    End Function
    Public Function CaptureWhole(exposure As Integer, focus As Integer) As Bitmap

        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)
        Bmp = BmpVimba.Clone()
        Return Bmp
    End Function
    Public Function CaptureROI(exposure As Integer, focus As Integer) As Bitmap

        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)


        Dim Bmpf = New FastBMP(BmpVimba)
        Bmp = New Bitmap(Bmpf.width, Bmpf.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        Bmp = Bmpf.bmp.Clone(New Rectangle(0, 0, Bmpf.width, Bmpf.height), System.Drawing.Imaging.PixelFormat.Format24bppRgb)

        Width = Bmp.Width
        Height = Bmp.Height
        'trying the quickPhasor
        Dim Phasor As New QuickPhasor(400, Width, Height)
        Phasor.MakeHistogram(Bmpf, True)
        Phasor.CreateMask(0, 175, 175)
        Dim segmented As New FastBMP(Bmp.Width, Bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        Phasor.Segment(Bmpf, segmented)
        ' now detecting the rectangle
        Dim blobCounter As New AForge.Imaging.BlobCounter

        blobCounter.FilterBlobs = True
        blobCounter.MinHeight = 300
        blobCounter.MinWidth = 500

        blobCounter.ProcessImage(segmented.bmp)

        Dim blobs As Blob() = blobCounter.GetObjectsInformation()

        Dim g As Graphics = Graphics.FromImage(segmented.bmp)
        g.DrawRectangle(New Pen(Brushes.Red, 0.5), blobs(0).Rectangle)
        R = blobs(0).Rectangle

        Dim CropFilter As New Crop(R)
        Bmp = CropFilter.Apply(segmented.bmp)


        Setting.Sett("Preview_X", R.X)
        Setting.Sett("Preview_Y", R.Y)
        Setting.Sett("Preview_W", R.Width)
        Setting.Sett("Preview_H", R.Height)


        segmented.bmp.Save("C:\test\segmented.jpg")
        Phasor.Plot.bmp.Save("C:\test\PhasorPlot.png")
        Bmp.Save("C:\test\bmptissue.png")

        Return Bmp
        'Form1.PictureBox_Phasor.Image = Phasor.Plot.bmp

    End Function
    Public Sub StopPreview()
        mCamera.Close()
        mySys.Shutdown()

    End Sub

    Public Sub SetExposure(exp As Single)
        expFeature = features("ExposureTime")
        expFeature.FloatValue = exp * 1000

    End Sub

    Public Sub MovetoPreview()
        Stage.MoveAbsolute(Stage.Zaxe, Preview_Z)
        Stage.MoveAbsolute(Stage.Yaxe, 7)
        Stage.MoveAbsolute(Stage.Xaxe, 0)

    End Sub

    Public Sub EstimateProfile()
        Dim i As Integer
        Dim BmpVimbaFast As New FastBMP(W, H, PixelFormat.Format24bppRgb)

        Dim Edge As HomogenityEdgeDetector


        Pbar.Value = 0
        Stage.MoveAbsolute(Stage.Zaxe, Preview_Z, False)
        For zz = 0 To Z - 1


            mCamera.AcquireSingleImage(frame, 500)
            frame.Fill(BmpVimba)
            If zz > 0 Then Stage.MoveRelativeAsync(Stage.Zaxe, Zsteps, False)



            BmpVimbaFast = New FastBMP(BmpVimba)
            BmpVimbaFast.GetGraysacleArray()

            Deivative.AnalyzeX(BmpVimbaFast.Greyimage, GreyEdge(zz))
            BLure.UpLoad(GreyEdge(zz))
            BLure.Process_FT_MTF()
            BLure.DownLoad(GreyEdge(zz))

            'BmpVimbaFast.bmp.Save("c:\temp\" + zz.ToString("D4") + ".jpg")
            'saveSinglePage32("c:\temp\" + zz.ToString("D4") + ".tif", GreyEdge(zz), W, H)
            Pbar.Increment(1)
            Application.DoEvents()
        Next


        Dim max, maxZ As Single
        ReDim MaxMap(W * H - 1)
        Dim MaxmapPhasor(W * H - 1) As Single
        Dim maxi As Integer = W * H - 1
        Dim Cs, Ss As Single
        For i = 0 To maxi
            max = 0
            Cs = 0
            Ss = 0
            For Zi = 0 To Z - 1
                If GreyEdge(Zi)(i) > max Then max = GreyEdge(Zi)(i) : maxZ = Zi : MaxMap(i) = maxZ
                Cs += C(Zi) * GreyEdge(Zi)(i)
                Ss += S(Zi) * GreyEdge(Zi)(i)
            Next
            MaxmapPhasor(i) = (Math.Atan2(Ss, Cs))
            If Ss < 0 Then
                MaxmapPhasor(i) = Math.PI * 2 + (Math.Atan2(Ss, Cs))
            End If
        Next

        Dim Zmap(W - 1, H - 1) As Single
        Dim MAxmap2D(Width - 1, Height - 1) As Single
        Buffer.BlockCopy(MaxmapPhasor, 0, MAxmap2D, 0, maxi)


        For yy = 0 To H - 1
            For xx = 0 To W - 1
                Zmap(xx, yy) = MAxmap2D(xx + X0, yy + Y0)
            Next
        Next



        ZmapBmp = New FastBMP(W, H, Imaging.PixelFormat.Format24bppRgb)

        i = 0
        Dim p As Integer = 0
        Dim r, g, b As Byte
        For yy = 0 To H - 1
            For xx = 0 To W - 1
                r = rr(MaxmapPhasor(i) / (Math.PI * 2) * 1000)
                g = gg(MaxmapPhasor(i) / (Math.PI * 2) * 1000)
                b = bb(MaxmapPhasor(i) / (Math.PI * 2) * 1000)
                ZmapBmp.FillOriginalPixel(xx, yy, r, g, b)

                p += 3
                i += 1
            Next
            p += ZmapBmp.offset
        Next
        ZmapBmp.lock()


        saveSinglePage32("c:\temp\MaxMap.tif", MaxMap, W, H)
        saveSinglePage32("c:\temp\MaxMapPhasor.tif", MaxmapPhasor, W, H)
        Pbar.Value = 0
    End Sub


End Class
