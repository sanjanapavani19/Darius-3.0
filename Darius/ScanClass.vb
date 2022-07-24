Public Class SuperScan

    Public Structure ScanUnit
        Dim Zscan As ZstackStructure
        Public Sub New(Address As String, W As Integer, H As Integer, Range As Single, Stepsize As Single, scale As Integer)
            Zscan = New ZstackStructure(W, H, Range, Stepsize, scale)
        End Sub

    End Structure

    Dim X, Y, Z As Integer
    Dim W, H As Integer
    Dim Range, stepsize As Single
    Dim scale As Integer
    Dim Address As String
    Dim Buffersize As Integer
    Dim ScanUnits() As ScanUnit

    Public Sub New(X As Integer, Y As Integer, Z As Integer, BufferSize As Integer, Address As String, W As Integer, H As Integer, Range As Single, Stepsize As Single, scale As Integer)
        Me.X = X
        Me.Y = Y
        Me.Z = Z
        Me.Buffersize = BufferSize
        Me.Address = Address
        Me.Range = Range
        Me.stepsize = Stepsize
        Me.scale = scale
        ReDim ScanUnits(BufferSize - 1)

        For b = 0 To BufferSize - 1
            ScanUnits(b) = New ScanUnit(Address, W, H, Range, Stepsize, scale)
        Next


    End Sub
End Class
