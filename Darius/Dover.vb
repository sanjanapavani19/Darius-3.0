Imports PMDLibrary
Imports PMDLibrary.PMD

Public Class Dover
    Dim periph1 As PMDPeripheral
    Dim device1 As PMDDevice
    Dim Axe As PMDAxis
    Dim Z As Integer
    Dim memory1 As PMDMemory
    Dim periph2 As PMDPeripheral
    Dim device2 As PMDDevice
    Dim axis2 As PMDAxis
    Dim major, minor As UInt32
    Dim MPmajor, MPminor, NumberAxes, special, custom, family As UInt16
    Dim MotorType As PMDMotorTypeVersion
    Dim DeviceType As PMDDeviceType
    Const TCPTimeout = 1000
    Public Sub CloseAll()
        If (memory1 IsNot Nothing) Then
            memory1.Close()
        End If
        If (Axe IsNot Nothing) Then
            Axe.Close()
        End If
        If (device1 IsNot Nothing) Then
            device1.Close()
        End If
        If (periph1 IsNot Nothing) Then
            periph1.Close()
        End If
        If (axis2 IsNot Nothing) Then
            axis2.Close()
        End If
        If (device2 IsNot Nothing) Then
            device2.Close()
        End If
        If (periph2 IsNot Nothing) Then
            periph2.Close()
        End If
    End Sub

    Public Sub New()

        ' DeviceType = PMDDeviceType.ResourceProtocol
        DeviceType = PMDDeviceType.MotionProcessor 'set PMDDeviceType to MotionProcessor if connecting to a Motion IC without a C-Motion Engine.

        Dim ra1(0 To 10) As UInt32
        Dim i As UInt32
        Dim IPAddr As UInt32



        periph1 = New PMDPeripheralCOM(PMDSerialPort.COM2, 57600, PMDSerialParity.None, PMDSerialStopBits.SerialStopBits1)
        device1 = New PMDDevice(periph1, DeviceType)
        Axe = New PMDAxis(device1, PMDAxisNumber.Axis1)
        Axe.GetVersion(family, MotorType, NumberAxes, special, custom, MPmajor, MPminor)

        If (DeviceType = PMDDeviceType.ResourceProtocol) Then
            device1.Version(major, minor)

            IPAddr = device1.GetDefault(PMDDefault.IPAddress)
            memory1 = New PMDMemory(device1, PMDDataSize.Size32Bit, PMDMemoryType.DPRAM)

            For i = 0 To ra1.Length() - 1
                ra1(i) = i

            Next
            memory1.Write(ra1, 100, ra1.Length())
        End If

        Axe.ActualPositionUnits = PMD.PMDActualPositionUnits.Counts
        Axe.ResetEventStatus(40960)
        Axe.ResetEventStatus(61439)
        Axe.OperatingMode = 55

    End Sub

    Public Sub MoveA(Z As Integer)

        Axe.Velocity = 1000000
        Axe.Acceleration = 1000000
        Axe.Position = Z
        Axe.Update()
        Me.Z = Z

    End Sub


    Public Sub MoveR(Z As Single)

        Axe.Velocity = 1000000
        Axe.Acceleration = 1000000
        Axe.Position = Z * 1000000
        Axe.Update()
    End Sub

    Public Sub WaitAxis()
        Do

        Loop Until Mid(ToBinary(Axe.ActivityStatus), 13, 1) = 0
    End Sub
    Private Function ToBinary(dec As Integer) As String
        Dim bin As Integer
        Dim output As String
        While dec <> 0
            If dec Mod 2 = 0 Then
                bin = 0
            Else
                bin = 1
            End If
            dec = dec \ 2
            output = Convert.ToString(bin) & output
        End While
        If output Is Nothing Then
            Return "0"
        Else
            Return output
        End If
    End Function

End Class
