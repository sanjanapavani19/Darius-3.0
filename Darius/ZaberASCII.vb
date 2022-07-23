Imports Zaber.Motion
Imports Zaber.Motion.Ascii

Public Class ZaberASCII

    Dim Com As Object
    Public Elapsedtime As Long
    Dim Watch As New Stopwatch
    Public X, Y, Z As Single
    Public Xacc, Yacc, Zacc As Single
    Public Xspeed, Yspeed, Zspeed As Single
    Public xp, yp As Single
    Public FOVX, FOVY As Single
    Public SweptZ As Single
    Public Xaxe, Yaxe, Zaxe As Axis

    Public Sub New(FOVX As Single, FOVY As Single)
        Dim com As Connection = Connection.OpenSerialPort("COM3")
        Dim Devicelist = com.DetectDevices()
        Xaxe = Devicelist(1).GetAxis(1)
        Yaxe = Devicelist(2).GetAxis(1)
        Zaxe = Devicelist(0).GetAxis(1)
        Me.FOVX = FOVX
        Me.FOVY = FOVY
        Home()
        'MoveAbsolute(Xaxe, 3.7, False)
        MoveAbsolute(Yaxe, 9.9, False)
        'MoveAbsolute(Zaxe, 3, False)

    End Sub

    Public Sub SetFOV(FOVX As Single, FOVY As Single)
        Me.FOVX = FOVX
        Me.FOVY = FOVY
        Setting.Sett("FOVX", FOVX)
        Setting.Sett("FOVY", FOVY)
    End Sub
    Public Sub Home()
        Zaxe.Home()
        Xaxe.Home()
        Yaxe.Home()
        SetAcceleration(Zaxe, 1)
        SetSpeed(Zaxe, 100)
        MoveAbsolute(Zaxe, Setting.Gett("Focus"), False)
        MoveAbsolute(Xaxe, 34, False)
    End Sub
    Public Sub MoveRelative(ByRef Axe As Axis, R As Single, Optional update As Boolean = True)

        Try

            Axe.MoveRelative(R, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveRelativeAsync(ByRef Axe As Axis, R As Single, Optional update As Boolean = True)
        Axe.MoveRelativeAsync(R, Units.Length_Millimetres)
        If update Then
            UpdatePositions()
            Tracking.Update()
        Else

        End If
    End Sub
    Public Sub MoveAbsolute(ByRef Axe As Axis, A As Single, Optional update As Boolean = True)
        Try
            Axe.MoveAbsolute(A, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveAbsoluteAsync(ByRef Axe As Axis, A As Single, Optional update As Boolean = True)
        Try

            Axe.MoveAbsoluteAsync(A, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub SetSpeed(ByRef Axe As Axis, S As Single)
        Try

            Axe.Settings.Set("maxspeed", S, Units.Velocity_MillimetresPerSecond)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub SetAcceleration(ByRef Axe As Axis, A As Integer)
        Axe.Settings.Set("accel", A, Units.Acceleration_MetresPerSecondSquared)
    End Sub

    Public Function GetPosition(ByRef Axe As Axis) As Single
        Return Axe.GetPosition(Units.Length_Millimetres)
    End Function

    Public Sub StorePosition(ByRef Axis As Device, position As Integer)
        Axis.GenericCommand(16, position, 0, True)
    End Sub


    Public Sub UpdatePositions()
        X = Xaxe.GetPosition(Units.Length_Millimetres)
        Y = Yaxe.GetPosition(Units.Length_Millimetres)
        Z = Zaxe.GetPosition(Units.Length_Millimetres)
    End Sub

    Public Sub UpdateZPositions()
        Z = Zaxe.GetPosition(Units.Length_Millimetres)
    End Sub

    Public Sub Go_Middle()
        MoveAbsolute(Yaxe, 12.7)
        MoveAbsolute(Xaxe, 38)
    End Sub

    Public Sub SetSweptZ(SweptZ As Single)
        Me.SweptZ = SweptZ
    End Sub

    Public Sub MoveSweptZ()
        Zaxe.MoveRelative(SweptZ, Units.Length_Millimetres)

    End Sub

    Public Sub GoZero(block As Boolean)
        Try

            Dim ZZ As String = Setting.Gett("ZOFFSET")
            If block Then
                MoveAbsolute(Zaxe, ZZ - 5, True)
            Else
                MoveAbsolute(Zaxe, ZZ, True)
            End If


        Catch ex As Exception

        End Try

    End Sub

    Public Sub CalibrateZoffset(AutoFocusrange As Single)
        Stage.MoveRelative(Stage.Zaxe, -AutoFocusrange / 2)
        Dim ZZ As Single = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("ZOFFSET", ZZ)
        'StorePosition(Stage.Zaxe, 1)
        Stage.MoveRelative(Stage.Zaxe, AutoFocusrange / 2)
        ZZ = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("Focus", ZZ)
        'StorePosition(Stage.Zaxe, 2)
    End Sub

    Public Sub GoToFocus()
        Try

            Dim ZZ As String = Setting.Gett("Focus")

            MoveAbsolute(Zaxe, ZZ, True)


        Catch ex As Exception

        End Try

    End Sub
End Class
