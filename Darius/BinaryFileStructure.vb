Public Class BinaryFileStructure
    Dim address As String
    Dim bytes() As Byte
    Dim inputFile As IO.FileStream

    Public Sub New(address As String, mode As IO.FileMode)
        Me.address = address
        inputFile = IO.File.Open(address, mode)
    End Sub

    Public Sub write(ByVal value As Byte)

        inputFile.WriteByte(value)
    End Sub

    Public Sub write(ByVal value As Integer)
        bytes = BitConverter.GetBytes(value)
        inputFile.Write(bytes, 0, bytes.GetLength(0))
    End Sub


    Public Sub write(ByVal value() As Byte, length As Integer)
        inputFile.Write(value, 0, length)
    End Sub

    Public Function ReadByteArray(length As Integer)
        ReDim bytes(length - 1)
        inputFile.Read(bytes, 0, length)
        Return bytes
    End Function

    Public Function ReadByte()
        Dim Onebyte As Byte
        Onebyte = inputFile.ReadByte()
        Return Onebyte
    End Function

    Public Function ReadInteger()
        ReDim bytes(3)
        inputFile.Read(bytes, 0, 4)
        Return BitConverter.ToInt16(bytes, 0)
    End Function

    Public Sub CLOSE()
        inputFile.Close()
    End Sub

End Class
