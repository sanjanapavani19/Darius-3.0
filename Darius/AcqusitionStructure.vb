Public Module AcqusitionStructure


    Public loop_Z As Integer

    Public Sub AcquireEDOF()

        'Piezo.MoveAbsolute(0)
        Dim numZ As Integer = ZEDOF.Z
        ReDim EDFbytes(Camera.W * Camera.H * 3 - 1)

        For loop_Z = 0 To numZ - 1
            'Camera.WaitUntillReady()
            'Camera.Capture_Threaded()
            'Wait(Camera.exp)
            Camera.Capture()
            Stage.MoveRelative(Stage.Zaxe, 0.01, False)
            ZEDOF.Upload(Camera.Bytes, loop_Z)
            ZEDOF.Process()
            'Application.DoEvents()
        Next
        Stage.MoveRelative(Stage.Zaxe, -0.01 * numZ, False)

        EDFbytes = ZEDOF.Wrapup

    End Sub




End Module
