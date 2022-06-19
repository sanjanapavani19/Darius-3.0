
Public Class Relay
    Private Declare Function FT_Open Lib "FTD2XX.dll" (ByVal intDeviceNumber As Integer, ByRef lnghandle As Long) As Long
    Private Declare Function FT_OpenEx Lib "FTD2XX.dll" (ByVal arg1 As String, ByVal arg2 As Long, ByRef lnghandle As Long) As Long
    Private Declare Function FT_Close Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_Read Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal lpszBuffer As String, ByVal lngBufferSize As Long, ByRef lngBytesReturned As Long) As Long
    Private Declare Function FT_Write Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal lpszBuffer As String, ByVal lngBufferSize As Long, ByRef lngBytesWritten As Long) As Long
    Private Declare Function FT_SetBaudRate Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal lngBaudRate As Long) As Long
    Private Declare Function FT_SetDataCharacteristics Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal byWordLength As Byte, ByVal byStopBits As Byte, ByVal byParity As Byte) As Long
    Private Declare Function FT_SetFlowControl Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal intFlowControl As Integer, ByVal byXonChar As Byte, ByVal byXoffChar As Byte) As Long
    Private Declare Function FT_ResetDevice Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_SetDtr Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_ClrDtr Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_SetRts Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_ClrRts Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_GetModemStatus Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByRef lngModemStatus As Long) As Long
    Private Declare Function FT_Purge Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal lngMask As Long) As Long
    Private Declare Function FT_GetStatus Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByRef lngRxBytes As Long, ByRef lngTxBytes As Long, ByRef lngEventsDWord As Long) As Long
    Private Declare Function FT_GetQueueStatus Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByRef lngRxBytes As Long) As Long
    Private Declare Function FT_GetEventStatus Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByRef lngEventsDWord As Long) As Long
    Private Declare Function FT_SetChars Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal byEventChar As Byte, ByVal byEventCharEnabled As Byte, ByVal byErrorChar As Byte, ByVal byErrorCharEnabled As Byte) As Long
    Private Declare Function FT_SetTimeouts Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal lngReadTimeout As Long, ByVal lngWriteTimeout As Long) As Long
    Private Declare Function FT_SetBreakOn Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_SetBreakOff Lib "FTD2XX.dll" (ByVal lnghandle As Long) As Long
    Private Declare Function FT_ListDevices Lib "FTD2XX.dll" (ByVal arg1 As Long, ByVal arg2 As String, ByVal dwFlags As Long) As Long
    Private Declare Function FT_GetNumDevices Lib "FTD2XX.dll" Alias "FT_ListDevices" (ByRef arg1 As Long, ByVal arg2 As String, ByVal dwFlags As Long) As Long
    Private Declare Function FT_SetBitMode Lib "FTD2XX.dll" (ByVal lnghandle As Long, ByVal mask As Byte, ByVal enable As Byte) As Long

    ' Return codes
    Const FT_OK = 0
    Const FT_INVALID_HANDLE = 1
    Const FT_DEVICE_NOT_FOUND = 2
    Const FT_DEVICE_NOT_OPENED = 3
    Const FT_IO_ERROR = 4
    Const FT_INSUFFICIENT_RESOURCES = 5
    Const FT_INVALID_PARAMETER = 6
    Const FT_INVALID_BAUD_RATE = 7


    ' Word Lengths
    Const FT_BITS_8 = 8
    Const FT_BITS_7 = 7

    ' Stop Bits
    Const FT_STOP_BITS_1 = 0
    Const FT_STOP_BITS_1_5 = 1
    Const FT_STOP_BITS_2 = 2

    ' Parity
    Const FT_PARITY_NONE = 0
    Const FT_PARITY_ODD = 1
    Const FT_PARITY_EVEN = 2
    Const FT_PARITY_MARK = 3
    Const FT_PARITY_SPACE = 4

    ' Flow Control
    Const FT_FLOW_NONE = &H0
    Const FT_FLOW_RTS_CTS = &H100
    Const FT_FLOW_DTR_DSR = &H200
    Const FT_FLOW_XON_XOFF = &H400

    ' Purge rx and tx buffers
    Const FT_PURGE_RX = 1
    Const FT_PURGE_TX = 2

    ' Flags for FT_OpenEx
    Const FT_OPEN_BY_SERIAL_NUMBER = 1
    Const FT_OPEN_BY_DESCRIPTION = 2

    ' Flags for FT_ListDevices
    Const FT_LIST_BY_NUMBER_ONLY = &H80000000
    Const FT_LIST_BY_INDEX = &H40000000
    Const FT_LIST_ALL = &H20000000
    Dim RelState As Long
    Public IsLED_ON As Boolean
    Public status As Boolean
    Dim lnghandle As Long




    Public Sub SetRelays(ByVal RelayN As Integer, State As Boolean)
        'Open the first device
        If status = False Then Exit Sub
        Dim temp As Long

        If (State = True) Then
            If RelayN = 1 Then temp = 1
            If RelayN = 2 Then temp = 2
            If RelayN = 3 Then temp = 4
            If RelayN = 4 Then temp = 8
            If RelayN = 5 Then temp = 16
            If RelayN = 6 Then temp = 32
            If RelayN = 7 Then temp = 64
            If RelayN = 8 Then temp = 128
            If RelayN = 255 Then temp = 255
            RelState = RelState Or temp
        End If

        If (State = False) Then
            If RelayN = 1 Then temp = 254
            If RelayN = 2 Then temp = 253
            If RelayN = 3 Then temp = 251
            If RelayN = 4 Then temp = 247
            If RelayN = 5 Then temp = 239
            If RelayN = 6 Then temp = 223
            If RelayN = 7 Then temp = 191
            If RelayN = 8 Then temp = 127
            If RelayN = 0 Then temp = 0
            RelState = RelState And temp
        End If

        'MsgBox (Str(RelState) + "," + Str(temp))


        Dim strWriteBuffer As String = " "
        Mid(strWriteBuffer, 1, 1) = Chr(RelState)
        Dim lngBytesWritten As Integer = 0
        If FT_Write(lnghandle, strWriteBuffer, Len(strWriteBuffer), lngBytesWritten) <> FT_OK Then
            '  Form1.logWinOutput("Write Failed")
            Exit Sub
        Else
            'Form1.logWinOutput("Write OK")
        End If




    End Sub
    Public Sub relay_Terminate()
        FT_Close(lnghandle)
    End Sub


    Public Sub New()


        status = False
        Try

            RelState = 0
            Dim arg1 As Long
            Dim arg2 As String = ""
            Dim dwFlag As Long

            FT_GetNumDevices(arg1, arg2, dwFlag)
            If FT_Open(1, lnghandle) <> FT_OK Then
                MsgBox("Error while opening")
                Exit Sub
            Else

            End If

            If FT_SetBitMode(lnghandle, 255, 4) <> FT_OK Then
                '    Form1.logWinOutput("Bit Bang Mode Error")
                Exit Sub
            Else

            End If

            If FT_SetBaudRate(lnghandle, 921600) <> FT_OK Then
                'Form1.logWinOutput("Baud rate setting error")
                Exit Sub
            Else
                ' Form1.logWinOutput("USB 8 Relay Board Opened")
                status = True
            End If

        Catch ex As Exception

        End Try

    End Sub

    Public Sub LED_ON()
        SetRelays(1, True)
        SetRelays(2, True)
        SetRelays(3, True)
        SetRelays(4, True)
        '     Form1.Button_LED.BackgroundImage = Image.FromFile(Application.StartupPath & "\images\led_diode-on.png")
    End Sub


    Public Sub LED_OFF()
        SetRelays(1, False)
        SetRelays(2, False)
        SetRelays(3, False)
        SetRelays(4, False)
        '   Form1.Button_LED.BackgroundImage = Image.FromFile(Application.StartupPath & "\images\led_diode-off.png")
    End Sub

End Class
