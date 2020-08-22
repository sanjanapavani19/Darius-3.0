Public Class PreScan
    Private Sub PreScan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Height = Me.Height - 100
        PictureBox1.Width = PictureBox1.Height
    End Sub
End Class