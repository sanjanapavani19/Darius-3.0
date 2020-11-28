Public Class ZstackStructure
    Public W, H, Z As Integer
    Dim Pattern2D(,), ScalingUpPattern(), ScalingDownPattern() As Integer
    Dim GreenBytes(), GreenEdgeBytes()() As Single
    Public MaxMap() As Single
    Dim BLure As FFTW_VB_Real
    Dim bytes()() As Byte
    Dim zc As Integer
    Dim Central As CentralDerivitavie
    Dim bytesout() As Byte

    Public Sub New(W As Integer, H As Integer, Z As Integer)
        Me.W = W
        Me.H = H
        Me.Z = Z
        ReDim GreenEdgeBytes(Z - 1)
        ReDim bytes(Z - 1)
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

    Public Sub Acquire()
        For loop_Z = 0 To Z - 1
            Camera.Capture()
            Stage.MoveRelative(Stage.Zaxe, 0.01, False)
            Upload(Camera.Bytes, loop_Z)
            Process()
        Next
        Stage.MoveRelative(Stage.Zaxe, -0.01 * Z, False)
        Wrapup()

    End Sub

    Public Sub Process()

        GetColorBytes(bytes(zc), GreenBytes, W / 2, H / 2)
        Central.AnalyzeX(GreenBytes, GreenEdgeBytes(zc))
        BLure.UpLoad(GreenEdgeBytes(zc))
        BLure.Process_FT_MTF()
        BLure.DownLoad(GreenEdgeBytes(zc))

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
        ReDim bytesout(W * H * 3 - 1)
        Dim max, maxZ As Single
        Dim maxi As Integer = W / 2 * H / 2 - 1
        Dim i As Integer
        ReDim MaxMap(W / 2 * H / 2 - 1)
        For i = 0 To maxi
            max = 0 : maxZ = 0
            For Zi = 0 To Z - 1
                If GreenEdgeBytes(Zi)(i) > max Then max = GreenEdgeBytes(Zi)(i) : maxZ = Zi
            Next
            MaxMap(i) = maxZ
        Next
        'SaveSinglePageTiff16("c:\temp\maxmap.tif", MaxMap, W / 2 - 1, H / 2 - 1)
        i = 0
        Dim index As Integer
        Dim j As Integer
        Dim maxj = W * H * 3 - 1
        For j = 0 To maxj Step 3
            index = MaxMap(ScalingUpPattern(i))
            bytesout(j) = bytes(index)(j)
            bytesout(j + 1) = bytes(index)(j + 1)
            bytesout(j + 2) = bytes(index)(j + 2)
            i += 1
        Next
        Return bytesout
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
