
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
    Public Z As Integer
    Dim Zsteps As Single
    Dim Pbar As ProgressBar
    Dim MaxMap() As Single
    Public Zx() As Single
    Public Zmap(,) As Single
    Dim C(), S() As Single
    Dim Flip As Mirror
    Dim Cropfilter As Crop
    Public X0, Y0, W, H As Integer
    Public GreyEdge()() As Single
    Public GreyEdge2D()(,) As Single

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

        Flip = New Mirror(False, True)
        Cropfilter = New Crop(R)

        Deivative = New CentralDerivitavie(W, H)
        BLure = New FFTW_VB_Real(H, W)
        BLure.MakeGaussianReal(0.008, BLure.MTF, 2)
        Preview_Z = Setting.Gett("Preview_Z")
        Me.Z = Z
        Me.Zsteps = Zsteps
        Me.Pbar = Pbar
        Pbar.Maximum = Z
        ReDim GreyEdge(Z - 1)
        ReDim GreyEdge2D(Z - 1)
        ReDim C(Z - 1)
        ReDim S(Z - 1)
        ReDim Zx(Z - 1)
        For zz = 0 To Z - 1
            ReDim GreyEdge(zz)(W * H - 1)
            ReDim GreyEdge2D(zz)(H - 1, W - 1)
            C(zz) = Math.Cos(2 * Math.PI / Z * zz)
            S(zz) = Math.Sin(2 * Math.PI / Z * zz)
        Next
        load_colormap()
    End Sub

    Public Function Capture(exposure As Integer, focus As Integer) As Bitmap

        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)

        Width = BmpVimba.Width
        Height = BmpVimba.Height

        Bmp = CropFilter.Apply(BmpVimba)
        Flip.ApplyInPlace(Bmp)

        Return Bmp

    End Function


    Public Function Capture() As Bitmap

        mCamera.AcquireSingleImage(frame, 500)
        frame.Fill(BmpVimba)

        Width = BmpVimba.Width
        Height = BmpVimba.Height

        Bmp = Cropfilter.Apply(BmpVimba)
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
        Dim BmpVimbaFast As New FastBMP(Width, Height, PixelFormat.Format24bppRgb)

        Dim Edge As HomogenityEdgeDetector


        Pbar.Value = 0
        Stage.MoveAbsolute(Stage.Zaxe, Preview_Z - Z * Zsteps / 2, False)
        For zz = 0 To Z - 1
            Zx(zz) = zz * Zsteps + Preview_Z

            If zz > 0 Then Stage.MoveRelative(Stage.Zaxe, Zsteps, False)


            BmpVimbaFast = New FastBMP(Capture)
            BmpVimbaFast.GetGraysacleArray()

            Deivative.AnalyzeX(BmpVimbaFast.Greyimage, GreyEdge(zz))
            BLure.UpLoad(GreyEdge(zz))
            BLure.Process_FT_MTF()
            BLure.DownLoad(GreyEdge(zz))
            Buffer.BlockCopy(GreyEdge(zz), 0, GreyEdge2D(zz), 0, W * H * 4)
            ' BmpVimbaFast.bmp.Save("c:\temp\images\" + zz.ToString("D4") + ".bmp")
            'saveSinglePage32("c:\temp\" + zz.ToString("D4") + ".tif", GreyEdge(zz), Width, Height)
            '  saveSinglePage32("c:\temp\" + zz.ToString("D4") + ".tif", GreyEdge2D(zz))
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
            MaxmapPhasor(i) = ((Math.Atan2(Ss, Cs))) / (Math.PI * 2) * Z * Zsteps + Preview_Z
            If Ss < 0 Then
                MaxmapPhasor(i) = (Math.PI * 2 + (Math.Atan2(Ss, Cs))) / (Math.PI * 2) * Z * Zsteps + Preview_Z
            End If
        Next
        Dim MAxmap2D(H - 1, W - 1) As Single

        ReDim Zmap(W - 1, H - 1)
        Buffer.BlockCopy(MaxmapPhasor, 0, MAxmap2D, 0, W * H * 4)


        For yy = 0 To H - 1
            For xx = 0 To W - 1
                Zmap(xx, yy) = MAxmap2D(yy, xx)
            Next
        Next

        saveSinglePage32("c:\temp\maxmap2d.tif", MAxmap2D)

        ZmapBmp = New FastBMP(W, H, Imaging.PixelFormat.Format24bppRgb)


        Dim p As Integer = 0
        Dim r, g, b As Byte
        For yy = 0 To H - 1
            For xx = 0 To W - 1
                r = rr((Zmap(xx, yy) - Preview_Z) / (Z * Zsteps) * 1000)
                g = gg((Zmap(xx, yy) - Preview_Z) / (Z * Zsteps) * 1000)
                b = bb((Zmap(xx, yy) - Preview_Z) / (Z * Zsteps) * 1000)
                ZmapBmp.FillOriginalPixel(xx, yy, r, g, b)

            Next

        Next
        ZmapBmp.MakeNewFromBytes()


        saveSinglePage32("c:\temp\MaxMap.tif", MaxMap, W, H)
        saveSinglePage32("c:\temp\MaxMapPhasor.tif", MaxmapPhasor, W, H)
        Pbar.Value = 0
    End Sub


End Class
