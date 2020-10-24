Imports System.Runtime.InteropServices
Public Class piezo
    <DllImport("EO-Drive.dll")> Public Shared Function EO_InitHandle() As Integer
    End Function


    <DllImport("EO-Drive.dll")> Public Shared Function EO_InitAllHandles() As Integer
    End Function

    <DllImport("EO-Drive.dll")> Public Shared Function EO_GetAllHandles(ByRef handls As Integer(), size As Integer) As Integer
    End Function

    <DllImport("EO-Drive.dll")>
    Public Shared Function EO_NumberOfCurrentHandles() As Integer
    End Function
    <DllImport("EO-Drive.dll")>
    Public Shared Function EO_GetHandleBySerial(Serial As Short) As Integer
    End Function

    <DllImport("EO-Drive.dll")>
    Public Shared Sub EO_ReleaseHandle(handl As Integer)
    End Sub

    <DllImport("EO-Drive.dll")>
    Public Shared Sub EO_ReleaseAllHandles()
    End Sub

    'Device Movement & Information
    <DllImport("EO-Drive.dll")>
    Public Shared Function EO_Move(handl As Integer, position As Double) As Integer
    End Function

    <DllImport("EO-Drive.dll")>
    Public Shared Function EO_GetCommandPosition(handl As Integer, ByRef position As Double) As Integer
    End Function

    <DllImport("EO-Drive.dll")>
    Public Shared Function EO_GetSerialNumber(handl As Integer, ByRef serial As Integer) As Integer
    End Function
    Dim serial As Integer
    Dim err As Integer
    Dim last_position As Single
    Dim handl As Integer
    Dim sleep As Single
    Dim initialDelay As Single
    Dim dir As Integer
    Dim steps As Integer
    Dim stage As Integer
    Dim retrn As Boolean

    Public Sub initialize()
        handl = EO_InitHandle()
        If (handl = 0) Then
            MsgBox("Failed To initialize the Piezo stage. check if it is powered ")
            Return
        End If
        err = EO_GetSerialNumber(handl, serial)

        'Set the piezo at the middle range
        Move_A(0)
    End Sub

    Public Sub Move_r(ByVal position As Single)
        err = EO_GetCommandPosition(handl, last_position)
        EO_Move(handl, position + last_position)

    End Sub

    Public Sub Move_A(ByVal position As Single)
        err = EO_Move(handl, position)

    End Sub

    Public Sub PiezoScan()
        'err = EO_GetCommandPosition(handl, last_position)
        ''Debug.Print("Last position" + last_position.ToString)
        'If last_position < 50 Then

        '    Dim watch As New Stopwatch
        '    watch.Start()

        '    wait(sleep)
        '    For t = 1 To steps
        '        Move_r(100 / steps)
        '        wait(sleep)
        '    Next
        '    watch.Stop()
        '    'Debug.Print(watch.ElapsedMilliseconds)

        'Else

        '    Dim watch As New Stopwatch
        '    watch.Start()

        '    wait(sleep)
        '    For t = 1 To steps
        '        Move_r(-100 / steps)
        '        wait(sleep)
        '    Next
        '    watch.Stop()
        '    ' Debug.Print(watch.ElapsedMilliseconds)
        'End If


        'If retrn Then
        '    For t = steps To 0 Step -1
        '        Move_A(100 * t / steps)
        '    Next
        'End If
    End Sub

    Public Sub SetSteps(stp As Integer)
        steps = stp
        setSleep()
    End Sub
    Public Sub setSleep()
        'Calibration constant of 1002.808 is for sync...
        'It is documented on DEC 4 2015
        '    sleep = ((Form1.Ximea.exposure * 1002.808) - 12.724) / (steps + 1)
    End Sub


End Class
