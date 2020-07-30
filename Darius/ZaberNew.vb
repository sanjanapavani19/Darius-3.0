Imports Zaber.Motion
Imports Zaber.Motion.Binary

Public Class ZaberNew

    Dim Com As Object
    Public Elapsedtime As Long
    Dim Watch As New Stopwatch
    Public X, Y, Z As Single
    Public xp, yp As Single
    Public FOVX, FOVY As Single
    Public Xaxe, Yaxe, Zaxe As Device

    Public Sub New(FOVX As Single, FOVY As Single)
        Dim com As Connection = Connection.OpenSerialPort("COM7")
        Dim Devicelist = com.DetectDevices()
        Xaxe = Devicelist(2)
        Yaxe = Devicelist(1)
        Zaxe = Devicelist(0)
        Me.FOVX = FOVX
        Me.FOVY = FOVY
        Home()

    End Sub

    Public Sub Home()
        Xaxe.Home()
        Yaxe.Home()
        Zaxe.Home()
        MoveAbsolute(Zaxe, Setting.Gett("ZOFFSET"))
        StorePosition(Zaxe, 1)

        MoveAbsolute(Zaxe, Setting.Gett("FOCUS"))
        StorePosition(Zaxe, 2)



        SetSpeed(Xaxe, 25)
        SetSpeed(Yaxe, 25)
        SetSpeed(Zaxe, 48)



        SetAcceleration(Xaxe, 300)
        SetAcceleration(Yaxe, 300)
        SetAcceleration(Zaxe, 3000)

    End Sub
    Public Sub MoveRelative(ByRef Axis As Device, R As Single)
        Watch.Reset()
        Try

            Axis.MoveRelative(R, Units.Length_Millimetres)
            UpdatePositions()
            Tracking.Update()
        Catch ex As Exception

        End Try

    End Sub
    Public Sub MoveAbsolute(ByRef Axis As Device, R As Single)
        Try
            Axis.MoveAbsolute(R, Units.Length_Millimetres)
            UpdatePositions()
            Tracking.Update()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveAbsoluteAsync(ByRef Axis As Device, R As Single)
        Try
            Axis.MoveAbsoluteAsync(R, Units.Length_Millimetres)
            UpdatePositions()
            Tracking.Update()
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


    Public Sub GoZero(ByRef Axis As Device, position As Integer)
        Try
            Axis.GenericCommand(18, position, 0, True)
            UpdatePositions()
            Tracking.Update()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub GoToFocus()
        Try
            Zaxe.GenericCommand(18, 2, 0, True)
            UpdatePositions()
            Tracking.Update()
        Catch ex As Exception

        End Try

    End Sub

    Public Sub MoveRelativeAsync(ByRef Axis As Device, R As Single)
        Watch.Reset()
        Try
            Watch.Start()
            Axis.MoveRelativeAsync(R, Units.Length_Millimetres)
            Watch.Stop()
            Elapsedtime = Watch.ElapsedMilliseconds
        Catch ex As Exception

        End Try

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

End Class
