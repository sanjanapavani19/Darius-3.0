Imports xiApi.NET

Imports System.Windows
Public Class XimeaColor
    Private cam As New xiCam

    Public readoutnoise As Single
    Public Name As String
    Public W As Integer
    Public H As Integer
    Public status As Boolean
    Public FFsetup As Boolean
    Public Wbinned, Hbinned As Integer
    Public Type As String
    Public busy As Boolean
    Public ready As Boolean
    Public gain As Single
    Public exp As Single
    Public BmpRef As Bitmap
    Public bit_scale As Byte = 2 ^ 4
    Private timeout As Integer

    Public Bytes As Byte()
    Public TRG_MODE As Integer
    Public ExposureChanged As Boolean
    Public Dostop As Boolean




    Public Sub New()
        Dim numCams As Integer = cam.GetNumberDevices()
        status = False
        If numCams > 0 Then

            ' open first device
            cam.OpenDevice(0)


            Name = cam.GetParamString(PRM.DEVICE_NAME)
            cam.SetParam(PRM.BUFFER_POLICY, BUFF_POLICY.SAFE)
            cam.SetParam(PRM.IMAGE_DATA_FORMAT, IMG_FORMAT.RAW8)

            '    cam.SetParam(PRM.TRG_SELECTOR, 1)
            'cam.SetParam(PRM.ACQ_TIMING_MODE, ACQ_TIMING_MODE.FRAME_RATE)
            cam.SetParam(PRM.TRG_SOURCE, TRG_SOURCE.SOFTWARE)
            '  cam.SetParam(PRM.VERTICAL_FLIP, 1)
            ' cam.SetParam(PRM.HORIZONTAL_FLIP, 1)


            cam.SetParam(PRM.SENSOR_TAPS, 4)

            gain = Setting.Gett("Gain")

            setGain(gain)
            SetColorGain(Setting.Gett("GainR"), Setting.Gett("GainG"), Setting.Gett("GainB"))
            setGammaY(1)
            setGammaC(0)

            exp = Setting.Gett("exposureb")
            SetExposure(exp, True)
            SetBinning(False, 1)

            timeout = 5000
            busy = False
            status = True

            '------------------------------------------------------------
            cam.StartAcquisition()
            SetROI(0, 0, 100, 100)
        End If



    End Sub
    Public Sub SetROI(X As Integer, Y As Integer, Width As Integer, Height As Integer)


        cam.SetParam(PRM.WIDTH, Width)
        cam.SetParam(PRM.HEIGHT, Height)
        cam.SetParam(PRM.OFFSET_X, X)
        cam.SetParam(PRM.OFFSET_Y, Y)


    End Sub

    Public Sub SetColorGain(R As Integer, G As Integer, B As Integer)
        cam.SetParam(PRM.WB_KB, B)
        cam.SetParam(PRM.WB_KG, G)
        cam.SetParam(PRM.WB_KR, R)
        Setting.Sett("GainB", B)
        Setting.Sett("GainG", G)
        Setting.Sett("GainR", R)

    End Sub
    Public Sub SetBinning(yes As Boolean, size As Integer)
        If yes Then

            cam.SetParam(PRM.DOWNSAMPLING, size)
            cam.SetParam(PRM.DOWNSAMPLING_TYPE, BINNING_MODE.SUM)
            Wbinned = cam.GetParamInt(PRM.WIDTH)
            Hbinned = cam.GetParamInt(PRM.HEIGHT)

            BmpRef = New Bitmap(Wbinned, Hbinned, Imaging.PixelFormat.Format24bppRgb)
            ReDim Bytes(Wbinned * Hbinned - 1)

        Else
            cam.SetParam(PRM.DOWNSAMPLING, 1)
            W = cam.GetParamInt(PRM.WIDTH)
            H = cam.GetParamInt(PRM.HEIGHT)
            BmpRef = New Bitmap(W, H, Imaging.PixelFormat.Format24bppRgb)
            ReDim Bytes(W * H - 1)
        End If
    End Sub
    Public Sub SetPolicyToSafe()
        cam.SetParam(PRM.BUFFER_POLICY, BUFF_POLICY.SAFE)
    End Sub

    Public Sub SetPolicyToUNSafe()
        cam.SetParam(PRM.BUFFER_POLICY, BUFF_POLICY.UNSAFE)
    End Sub
    Public Sub Capture_Threaded()
        Dim Thread1 As New System.Threading.Thread(AddressOf Capture)
        Thread1.Start()

    End Sub


    Public Sub Capture()
        ready = False
        Try
            cam.SetParam(PRM.TRG_SOFTWARE, 1)
            cam.GetImageByteArray(Bytes, timeout)
        Catch ex As Exception

        End Try

        ready = True
    End Sub


    Public Sub SetFlatField(filename As String, bfilename As String)
        cam.SetParam(PRM.FFC_FLAT_FIELD_FILE_NAME, filename)
        cam.SetParam(PRM.FFC_DARK_FIELD_FILE_NAME, bfilename)
        cam.SetParam(PRM.FFC, 1)
        FFsetup = True
    End Sub

    Public Sub Flatfield(value As Integer)
        cam.SetParam(PRM.FFC, value)
    End Sub
    Public Sub Capture(ByRef framein)
        Try
            cam.SetParam(PRM.TRG_SOFTWARE, 1)
            cam.GetImageByteArray(framein, timeout)

        Catch ex As Exception

        End Try

    End Sub

    'Public Sub CaptureXIMG()
    '    Dim XIimage As xiApi.XI_IMG
    '    cam.SetParam(PRM.TRG_SOFTWARE, 1)
    '    cam.GetXI_IMG()
    'End Sub

    Public Function captureBmp() As Bitmap
        cam.SetParam(PRM.TRG_SOFTWARE, 1)
        cam.GetBitmap(BmpRef, timeout)
        Return BmpRef
    End Function

    Public Sub SetDataMode(type As Colortype)
        Select Case type
            Case Colortype.RGB
                cam.SetParam(PRM.IMAGE_DATA_FORMAT, IMG_FORMAT.RGB24)

                ReDim Bytes(W * H * 3 - 1)

            Case Colortype.Grey
                cam.SetParam(PRM.IMAGE_DATA_FORMAT, IMG_FORMAT.RAW8)
                ReDim Bytes(W * H - 1)
        End Select

    End Sub




    'Public Sub SetColorGain(R As Integer, G As Integer, B As Integer)
    '    cam.SetParam(PRM.WB_KB, B)
    '    cam.SetParam(PRM.WB_KG, G)
    '    cam.SetParam(PRM.WB_KR, R)
    '    Setting.Sett("GainB", B)
    '    Setting.Sett("GainG", G)
    '    Setting.Sett("GainR", R)


    'End Sub

    Public Sub setGain(Gai As Single)
        gain = Gai
        cam.SetParam(PRM.GAIN, gain)
        Setting.Sett("Gain", gain)

    End Sub

    Public Sub setGammaY(G As Single)
        cam.SetParam(PRM.GAMMAY, G)
    End Sub

    Public Sub setGammaC(G As Single)
        cam.SetParam(PRM.GAMMAC, G)
    End Sub

    Public Sub SetExposure(ex As Single, save As Boolean)
        If ex = 0 Then MsgBox("Richard : Please Enter a valid value for exposure time.") : Exit Sub
        cam.SetParam(PRM.EXPOSURE, ex * 1000000)
        timeout = ex * 100000
        If save Then Setting.Sett("exposure", ex)
        exp = ex
    End Sub

    Public Sub SetExposure()
        'Sets exposure time in microseconds.
        If exp = 0 Then MsgBox("Richard : Please Enter a valid value for exposure time." + vbCrLf + "Expsure is fixed!") : Exit Sub
        cam.SetParam(PRM.EXPOSURE, exp * 1000000)
        timeout = exp * 100000

    End Sub


End Class
