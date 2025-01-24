Imports Microsoft.VisualBasic
Imports System.Windows.Forms
Imports OpenSlideNET

Namespace DariusSVSViewer
    Public Class SVSViewer
        Inherits UserControl

        Private pictureBox As PictureBox

        Public Sub New(pictureBox As PictureBox)
            InitializeComponent()
            Me.pictureBox = pictureBox
        End Sub

        Public Sub LoadSVSImage(filePath As String)
            Using slide As OpenSlideImage = OpenSlideImage.Open(filePath)
                Dim thumbnail As Bitmap = slide.GetThumbnailImage(slide.Levels - 1)
                pictureBox.Image = thumbnail
            End Using
        End Sub

        Private Sub InitializeComponent()
            Me.SuspendLayout()
            ' 
            ' SVSViewer
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Name = "SVSViewer"
            Me.ResumeLayout(False)

        End Sub
    End Class
End Namespace

