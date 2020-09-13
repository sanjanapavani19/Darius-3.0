Public Class mainForm
    Private Sub mainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim viewer As New LuigiViewer.DisplayForm()
        viewer.Show()
    End Sub
End Class