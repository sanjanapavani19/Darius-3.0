Imports Zaber.Motion
Imports Zaber.Motion.Binary

Public Class ZaberNew

    Dim Com As Object
    Public Elapsedtime As Long
    Dim Watch As New Stopwatch
    Public X, Y, Z As Single
    Public Xacc, Yacc, Zacc As Single
    Public Xspeed, Yspeed, Zspeed As Single
    Public xp, yp As Single
    Public FOVX, FOVY As Single
    Public SweptZ As Single
    Public Xaxe, Yaxe, Zaxe As Device

    Public Sub New(FOVX As Single, FOVY As Single)
        Dim com As Connection = Connection.OpenSerialPort("COM4")
        Dim Devicelist = com.DetectDevices()
        Xaxe = Devicelist(2)
        Yaxe = Devicelist(1)
        Zaxe = Devicelist(0)
        Me.FOVX = FOVX
        Me.FOVY = FOVY
        Home()
        MoveAbsolute(Xaxe, 11.7, False)
        MoveAbsolute(Xaxe, 37.6, False)
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

        MoveAbsolute(Zaxe, Setting.Gett("ZOFFSET"))
        StorePosition(Zaxe, 1)

        MoveAbsolute(Zaxe, Setting.Gett("FOCUS"))
        StorePosition(Zaxe, 2)

        Xspeed = 65
        Yspeed = 65
        Zspeed = 10


        SetSpeed(Xaxe, Xspeed)
        SetSpeed(Yaxe, Yspeed)
        SetSpeed(Zaxe, Zspeed)

        Xacc = 3000
        Yacc = 3000
        Zacc = 50

        SetAcceleration(Xaxe, Xacc)
        SetAcceleration(Yaxe, Yacc)
        SetAcceleration(Zaxe, Zacc)

    End Sub
    Public Sub MoveRelative(ByRef Axis As Device, R As Single, Optional update As Boolean = True)

        Try

            Axis.MoveRelative(R, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                If Tracking IsNot Nothing Then Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveRelativeAsync(ByRef Axis As Device, R As Single, Optional update As Boolean = True)
        Axis.MoveRelativeAsync(R, Units.Length_Millimetres)
        If update Then
            UpdatePositions()
            If Tracking IsNot Nothing Then Tracking.Update()
        Else

        End If
    End Sub
    Public Sub MoveAbsolute(ByRef Axis As Device, R As Single, Optional update As Boolean = True)
        Try
            Axis.MoveAbsolute(R, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                If Tracking IsNot Nothing Then Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveAbsoluteAsync(ByRef Axis As Device, R As Single, Optional update As Boolean = True)
        Try
            Axis.MoveAbsoluteAsync(R, Units.Length_Millimetres)
            If update Then
                UpdatePositions()
                If Tracking IsNot Nothing Then Tracking.Update()
            End If
        Catch ex As Exception

        End Try

    End Sub
    Public Sub SetSpeed(ByRef Axis As Device, S As Single)
        Try
            Axis.GenericCommandWithUnits(42, S, Units.Velocity_MillimetresPerSecond, Units.Velocity_MillimetresPerSecond, 100)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub SetAcceleration(ByRef Axis As Device, A As Integer)
        Axis.GenericCommand(43, A, 100, True)
    End Sub

    Public Function GetPosition(ByRef Axis As Device) As Single
        Return Axis.GetPosition(Units.Length_Millimetres)
    End Function

    Public Sub StorePosition(ByRef Axis As Device, position As Integer)
        Axis.GenericCommand(16, position, 0, True)
    End Sub


    Public Sub GoZero(ByRef Axis As Device, block As Boolean)
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

    Public Sub GoToFocus(block As Boolean)
        Try

            Dim ZZ As String = Setting.Gett("Focus")
            If block Then
                MoveAbsolute(Zaxe, ZZ - 5, True)
            Else
                MoveAbsolute(Zaxe, ZZ, True)
            End If

        Catch ex As Exception

        End Try

    End Sub
    Public Sub CalibrateZoffset(AutoFocusrange As Single)
        'Stage.MoveRelative(Stage.Zaxe, -AutoFocusrange / 2)
        Dim ZZ As Single = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("ZOFFSET", ZZ)
        StorePosition(Stage.Zaxe, 1)
        'Stage.MoveRelative(Stage.Zaxe, AutoFocusrange / 2)
        'ZZ = Stage.GetPosition(Stage.Zaxe)
        Setting.Sett("Focus", ZZ)
        StorePosition(Stage.Zaxe, 2)
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
        MoveAbsolute(Xaxe, 12.7)
        MoveAbsolute(Yaxe, 38)
    End Sub

    Public Sub SetSweptZ(SweptZ As Single)
        Me.SweptZ = SweptZ
    End Sub

    Public Sub MoveSweptZ()
        Zaxe.MoveRelative(SweptZ, Units.Length_Millimetres)

    End Sub
End Class
