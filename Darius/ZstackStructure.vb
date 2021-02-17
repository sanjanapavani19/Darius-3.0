Public Class ZstackStructure
    Public W, H, Z As Integer
    Dim Pattern2D(,), ScalingUpPattern(), ScalingDownPattern() As Integer
    Dim GreenBytes(), GreenEdgeBytes()() As Single
    Public MaxMap() As Single
    Dim BLure As FFTW_VB_Real
    Dim bytes()() As Byte
    Dim zc As Integer
    Dim current As Long
    Dim ticks As Long
    Dim processDone() As Integer
    Dim Imagecreated() As Integer
    Public direction As Integer
    Dim Central As CentralDerivitavie
    Dim StepSize As Single = 0.01
    Dim Range As Single
    Public OutputBytes() As Byte

    Public Sub New(W As Integer, H As Integer, Range As Single, Stepsize As Single)
        Me.W = W
        Me.H = H
        Me.Range = (Range / 1000)
        Me.StepSize = (Stepsize / 1000)
        Z = Int(Range / Stepsize)

        ReDim GreenEdgeBytes(Z - 1)
        ReDim bytes(Z - 1)
        ReDim processDone(Z - 1)
        ReDim Imagecreated(Z - 1)
        For zi = 0 To Z - 1

            ReDim GreenEdgeBytes(zi)(W / 2 * H / 2 - 1)
            ReDim bytes(zi)(W * H * 3 - 1)
        Next
        ReDim GreenBytes(W / 2 * H / 2 - 1)
        BLure = New FFTW_VB_Real(W / 2, H / 2)
        BLure.MakeGaussianReal(0.01, BLure.MTF, 2)
        Central = New CentralDerivitavie(W / 2, H / 2)

        'For some stupid reason, the 2D rotates when it copied to a 1D array. It is wiered.... 
        ReDim Pattern2D(H - 1, W - 1)
        ReDim ScalingUpPattern(W * H - 1)
        ReDim ScalingDownPattern(W / 2 * H / 2 - 1)
        Dim j As Integer = 0
        For y = 0 To H - 1 Step 2
            For x = 0 To W - 1 Step 2

                For yb = y To y + 1
                    For xb = x To x + 1
                        Pattern2D(yb, xb) = j
                    Next
                Next
                j += 1

            Next
        Next
        Buffer.BlockCopy(Pattern2D, 0, ScalingUpPattern, 0, W * H * 4)

        Dim i As Integer = 0
        Dim p As Integer = 1
        For Y = 0 To H / 2 - 1
            For X = 0 To W / 2 - 1
                ScalingDownPattern(i) = p
                p = p + 6
                i += 1
            Next
            p = Y * W * 3 * 2 + 1
        Next
        ReDim MaxMap(W / 2 * H / 2 - 1)
        direction = 1
    End Sub
    Public Sub Clear()
        ReDim GreenEdgeBytes(Z - 1)
        ReDim bytes(Z - 1)
        For zi = 0 To Z - 1
            ReDim GreenEdgeBytes(zi)(W / 2 * H / 2 - 1)
            ReDim bytes(zi)(W * H * 3 - 1)
        Next
        ReDim GreenBytes(W / 2 * H / 2 - 1)
    End Sub
    Public Sub Upload(bytesin() As Byte, zi As Integer)

        Buffer.BlockCopy(bytesin, 0, bytes(zi), 0, bytesin.Length)
        zc = zi
    End Sub

    Public Sub AcquireThreaded(retrn As Boolean, Optional WithWrapup As Boolean = True)
        ticks = (Camera.exp * Stopwatch.Frequency / 1000)
        'MakeDelay()
        If retrn Then direction = 1
        Array.Clear(Imagecreated, 0, Z)
        Array.Clear(processDone, 0, Z)

        Dim Thread As New System.Threading.Thread(AddressOf ProcessThreaded)
        Thread.Start()

        For loopZ = 0 To Z - 1
            Camera.Trigger()
            MakeDelay()

            Stage.MoveRelativeAsync(Stage.Zaxe, StepSize * direction, False)
            Try
                Camera.cam.GetImageByteArray(bytes(loopZ), Camera.timeout)
            Catch ex As Exception

            End Try

            'Camera.TriggerOff()
            Imagecreated(loopZ) = 1
        Next
        If retrn Then Stage.MoveRelativeAsync(Stage.Zaxe, -StepSize * Z, False) Else direction = direction * -1
        If WithWrapup Then Wrapup()

    End Sub
    Public Sub ProcessThreaded()
        For loopZ = 0 To Z - 1
            Do Until Imagecreated(loopZ) = 1
                'Application.DoEvents()
            Loop
            GetColorBytes(bytes(loopZ), GreenBytes, W / 2, H / 2)
            Central.AnalyzeX(GreenBytes, GreenEdgeBytes(loopZ))
            BLure.UpLoad(GreenEdgeBytes(loopZ))
            BLure.Process_FT_MTF()
            BLure.DownLoad(GreenEdgeBytes(loopZ))
            processDone(loopZ) = 1
        Next

    End Sub


    Public Sub Process()
        GetColorBytes(bytes(zc), GreenBytes, W / 2, H / 2)
        Central.AnalyzeX(GreenBytes, GreenEdgeBytes(zc))
        BLure.UpLoad(GreenEdgeBytes(zc))
        BLure.Process_FT_MTF()
        BLure.DownLoad(GreenEdgeBytes(zc))
        processDone(zc) = 1


    End Sub
    Public Sub MakeDelay()

        current = System.Diagnostics.Stopwatch.GetTimestamp()

        While (System.Diagnostics.Stopwatch.GetTimestamp() - current) < ticks
            Application.DoEvents()
        End While

    End Sub
    Public Sub Acquire()
        For loopZ = 0 To Z - 1
            '56 ms of transfer time
            Camera.Capture()
            ' 38 ms moving to the next field
            Stage.MoveRelative(Stage.Zaxe, StepSize, False)
            ' 7ms upload
            Upload(Camera.Bytes, loopZ)
            ' 30 ms process
            zc = loopZ
            Process()
        Next
        Stage.MoveRelative(Stage.Zaxe, -StepSize * Z, False)
        Wrapup()

    End Sub

    Public Sub ProcessAll(bytesin()() As Byte)

        Me.bytes = bytes

        For zi = 0 To Z - 1

            GetColorBytes(bytes(zi), GreenBytes, W / 2, H / 2)
            Central.AnalyzeX(GreenBytes, GreenEdgeBytes(zi))
            BLure.UpLoad(GreenEdgeBytes(zi))
            BLure.Process_FT_MTF()
            BLure.DownLoad(GreenEdgeBytes(zi))

        Next



    End Sub
    Public Function Wrapup() As Byte()
        ReDim OutputBytes(W * H * 3 - 1)
        Dim max, maxZ As Single
        Dim maxi As Integer = W / 2 * H / 2 - 1
        Dim i As Integer
        ReDim MaxMap(W / 2 * H / 2 - 1)

        Do Until (processDone.Sum = Z)

        Loop

        For i = 0 To maxi
            max = 0
            For Zi = 0 To Z - 1
                If GreenEdgeBytes(Zi)(i) > max Then max = GreenEdgeBytes(Zi)(i) : maxZ = Zi : MaxMap(i) = maxZ
            Next
        Next
        'SaveSinglePageTiff16("c:\temp\maxmap.tif", MaxMap, W / 2, H / 2)
        'saveSinglePage32("c:\temp\maxtest.tif", maxtest, W / 2, H / 2)
        'For Zi = 0 To Z - 1
        '    saveSinglePage32("c:\temp\d\" + Zi.ToString, GreenEdgeBytes(Zi), W / 2, H / 2)
        '    SaveSinglePageTiff16("c:\temp\i\" + Zi.ToString, bytes(Zi), W, H)
        'Next
        i = 0
        Dim index As Integer
        Dim j As Integer
        Dim maxj = W * H * 3 - 1
        ' 78 ms 
        For j = 0 To maxj Step 3
            index = MaxMap(ScalingUpPattern(i))
            OutputBytes(j) = bytes(index)(j)
            OutputBytes(j + 1) = bytes(index)(j + 1)
            OutputBytes(j + 2) = bytes(index)(j + 2)
            i += 1
        Next
        Return OutputBytes
    End Function
    Sub GetColorBytes(BytesIn() As Byte, ByRef BytesOut() As Single, Wb As Integer, Hb As Integer)
        'to read green 
        ReDim BytesOut(Wb * Hb - 1)
        Dim max As Integer = Wb * Hb - 1
        For i = 0 To max
            BytesOut(i) = BytesIn(ScalingDownPattern(i))
        Next
    End Sub
End Class
