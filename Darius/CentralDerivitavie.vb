Public Class CentralDerivitavie
    Dim W, H As Integer
    Dim count As Integer
    Dim fin2d(,), fout2d(,) As Single

    Public Sub New(W As Integer, H As Integer)
        Me.W = W
        Me.H = H
        ReDim fout2d(W - 1, H - 1)
        ReDim fin2d(W - 1, H - 1)
        count = W * H * 4
    End Sub

    Public Sub AnalyzeX(ByRef fin() As Single, ByRef fout As Single())
        Dim nMax = W * H - 2
        Dim T As Single
        fout(0) = 0
        fout(nMax + 1) = 0
        For j = 1 To nMax
            T = (fin(j - 1) - fin(j + 1)) / (fin(j) + 1)
            fout(j) = T * T
        Next
    End Sub

    Public Sub AnalyzeX(ByRef fin() As Byte, ByRef fout As Single())
        Dim nMax = W * H - 2
        Dim T As Single
        fout(0) = 0
        fout(nMax + 1) = 0
        For j = 1 To nMax
            T = (fin(j - 1) - fin(j + 1)) / (fin(j) + 1)
            fout(j) = T * T
        Next
    End Sub
    Public Sub AnalyzeY(ByRef fin() As Single, ByRef fout As Single())

        Dim nMax = W * H - W - 1
        Dim T As Single
        For j = W To nMax
            T = (fin(j - W) - fin(j + W)) / fin(j)
            fout(j) = T * T
        Next
    End Sub
End Class
