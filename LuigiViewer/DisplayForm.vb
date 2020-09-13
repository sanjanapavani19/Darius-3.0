Imports System
Public Class DisplayForm
    Dim Pyramid As New PyramidalTiff(Nothing, 0, 0, False, False)
    Dim sharpenCheck As Boolean = False
    Dim autoWhiteCheck As Boolean = False
    Dim pictureLoaded As Boolean = False 'Boolean variable to keep track if there is a picture loaded or not
    Dim cantMove As Boolean = True 'Boolean variable that makes sure that user can't move the picture when the entirety of the picture is shown
    Private MouseLocation As Point = Point.Empty 'Location in the X and Y direction of the mouse pointer
    Private MapLocation As Point = Point.Empty 'Location in the X and Y direction of the loaded picture
    Private oldMapLocation As Point = Point.Empty 'Maplocation of previous mouse click
    Private MouseDownPoint As Point = Point.Empty 'Location in the X and Y direction of the mouse pointer when the left button is clicked
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(Address As String)
        ' This call is required by the designer.
        InitializeComponent()

        Dim temp As New PyramidalTiff(Address, PictureBox1.Height, PictureBox1.Width, sharpenCheck, autoWhiteCheck)
        cantMove = True 'Can't move if we load a new picture
        MapLocation.X = 0
        MapLocation.Y = 0
        Pyramid = temp
        pictureLoaded = True
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        If pictureLoaded Then 'Updates picturebox if there's a picture loaded
            If sharpenCheck Then
                PictureBox1.Image = New Bitmap(Pyramid.map)
                Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
            Else
                PictureBox1.Image = Pyramid.map
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown 'When the left mouse button is clicked and the picture is dragged, new tile groups are loaded as needed
        If e.Button = MouseButtons.Left And pictureLoaded Then
            MouseDownPoint = e.Location 'The location of the mouse pointer is saved
            oldMapLocation = MouseDownPoint 'The current maplocation coordinates are saved for later comparison
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        If e.Button = MouseButtons.Left And pictureLoaded And Not cantMove Then
            If pictureLoaded And Not e.Location = oldMapLocation Then 'Updates picturebox if there's a picture loaded and the picture has been moved around
                If sharpenCheck Then 'Check if to apply unsharp mask or not
                    PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
                    Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
                End If
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove 'When the left button is clicked and the picture is dragged in the picturebox
        If e.Button = MouseButtons.Left And pictureLoaded Then
            If Not Pyramid.isDigital Then 'Check if we are not in the digital zoom in phase
                If -MapLocation.X > (Pyramid.tile.width * 1.5) And Pyramid.rightCheck() Then 'Checks if the tile group to the right needs to be loaded, also checks if it is possible to do so
                    Pyramid.moveRight(MapLocation.X)
                ElseIf -MapLocation.X < (Pyramid.tile.width * 0.5) And Pyramid.leftCheck() Then 'Checks if the tile group to the left needs to be loaded, also checks if it is possible to do so
                    Pyramid.moveLeft(MapLocation.X)
                ElseIf -MapLocation.Y > (Pyramid.tile.height * 1.5) And Pyramid.downCheck() Then 'Checks if the tile group below needs to be loaded, also checks if it is possible to do so
                    Pyramid.moveDown(MapLocation.Y)
                ElseIf -MapLocation.Y < (Pyramid.tile.height * 0.5) And Pyramid.upCheck() Then 'Checks if the tile group above needs to be loaded, also checks if it is possible to do so
                    Pyramid.moveUp(MapLocation.Y)
                End If
            End If

            If Not cantMove Then 'Check if we are looking at the whole picture and shouldn't be able to move
                MapLocation.X -= (MouseDownPoint.X - e.X)
                MapLocation.Y -= (MouseDownPoint.Y - e.Y)
                MouseDownPoint = e.Location 'The location of the mouse pointer is saved
                If Pyramid.isDigital Then 'Check if we are moving in a digital zoomed in picture or a tile picture
                    Pyramid.checkZoomBoxBounds(MapLocation)
                    Pyramid.loadZoomBox(MapLocation, autoWhiteCheck)
                Else
                    Pyramid.checkBounds(MapLocation) 'Make sure the picture displayed is within the picturebox bounds
                    Pyramid.loadPictureBox(MapLocation, autoWhiteCheck)
                End If
                PictureBox1.Image = Pyramid.map
                Me.Update()
            End If
        End If
    End Sub

    Private Sub Form1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel 'When the mousewheel is scrolled 
        MouseLocation = PictureBox1.PointToClient(Cursor.Position)

        If Pyramid.mouseTreshold(MouseLocation) Then
            If e.Delta > 0 Then 'Zoom in
                If Pyramid.zoomIn(MapLocation, MouseLocation) Then 'Check if we are zooming in a tile or digital
                    Pyramid.checkBounds(MapLocation) 'Make sure the picture displayed is within the picturebox bounds
                    Pyramid.loadPictureBox(MapLocation, autoWhiteCheck)
                    PictureBox1.SizeMode = PictureBoxSizeMode.Normal
                ElseIf Not Pyramid.pageZero Then
                    Pyramid.digitalZoomIn(MapLocation, MouseLocation, sharpenCheck, autoWhiteCheck)
                    PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
                End If

                cantMove = False 'Can move after we zoom in
                If pictureLoaded Then 'Updates picturebox if there's a picture loaded
                    If sharpenCheck Then 'Check if to apply unsharp mask or not
                        PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
                        Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
                    Else
                        PictureBox1.Image = Pyramid.map
                    End If
                End If
                Me.Update()
            Else 'Zoom out
                If Pyramid.zoomOut(MapLocation, MouseLocation) Then 'Check if we are zooming out of a tile or digital
                    Pyramid.checkBounds(MapLocation) 'Make sure the picture displayed is within the picturebox bounds
                    Pyramid.loadPictureBox(MapLocation, autoWhiteCheck)
                    PictureBox1.SizeMode = PictureBoxSizeMode.Normal
                Else
                    Pyramid.digitalZoomOut(MapLocation, MouseLocation, sharpenCheck, autoWhiteCheck)
                    PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
                End If

                If pictureLoaded Then 'Updates picturebox if there's a picture loaded
                    If sharpenCheck Then 'Check if to apply unsharp mask or not
                        PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
                        Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
                    Else
                        PictureBox1.Image = Pyramid.map
                    End If
                End If
                Me.Update()
            End If
        End If
    End Sub

    Private Sub PictureBox1_MouseEnter(sender As System.Object, e As System.EventArgs) Handles PictureBox1.MouseEnter 'When mouse is in picturebox, picturebox has the focus of mouse commands
        PictureBox1.Focus()
    End Sub

    Private Sub Form1_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles PictureBox1.DragDrop
        Dim fileName As String() = e.Data.GetData(DataFormats.FileDrop) 'Extract filepath
        Dim test As String = "" 'Initialize test to empty string
        'Loop through the last 4 letters of the filepath to extract the file format
        For i = fileName(0).Length() - 4 To fileName(0).Length() - 1
            test += fileName(0).Chars(i) 'Append letter by letter
        Next

        If test = ".tif" Or test = "tiff" Then 'Check for desired format
            Dim temp As New PyramidalTiff(fileName(0), PictureBox1.Height, PictureBox1.Width, sharpenCheck, autoWhiteCheck)
            cantMove = True 'Can't move if we load a new picture
            MapLocation.X = 0
            MapLocation.Y = 0
            Pyramid = temp
            pictureLoaded = True
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
            If pictureLoaded Then 'Updates picturebox if there's a picture loaded
                If sharpenCheck Then
                    PictureBox1.Image = New Bitmap(Pyramid.map)
                    Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
                Else
                    PictureBox1.Image = Pyramid.map
                End If
            End If
            TrackBar1.Enabled = True 'Allow to move trackbar
            Button2.Enabled = True 'Allow to click auto white balance
        Else
            MessageBox.Show("Accepted extensions: .tif .tiff", "Wrong File Format", MessageBoxButtons.OK, MessageBoxIcon.Information) 'Warn user of wrong file format
        End If
    End Sub

    Private Sub Form1_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles PictureBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then 'Check if there is a file to drop
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.AllowDrop = True 'Allows pictures to be dropped in the picturebox
        TrackBar1.Enabled = True 'Stops user from using trackbar before a picture is loaded
        Button2.Enabled = True 'Stops user from pressing button before picture is loaded
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged 'Sharpen
        sharpenCheck = Not sharpenCheck 'Flip the boolean

        If pictureLoaded Then 'Updates picturebox if there's a picture loaded
            If sharpenCheck Then 'Check if to apply unsharp mask or not
                PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
                Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
            Else
                PictureBox1.Image = Pyramid.map
            End If
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged 'White balance
        autoWhiteCheck = Not autoWhiteCheck 'Flip the boolean

        If pictureLoaded Then 'Updates picturebox if there's a picture loaded
            If cantMove Then 'Checks if the user can drag the picture
                Pyramid.loadPicture(autoWhiteCheck)
            Else
                If Pyramid.isDigital Then 'Check if we are in digital zoom mode
                    Pyramid.loadZoomBox(MapLocation, autoWhiteCheck)
                Else
                    Pyramid.loadPictureBox(MapLocation, autoWhiteCheck)
                End If
            End If

            If pictureLoaded Then 'Updates picturebox if there's a picture loaded
                If sharpenCheck Then 'Check if to apply unsharp mask or not
                    PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
                    Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
                Else
                    PictureBox1.Image = Pyramid.map
                End If
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click 'White Balance button
        Pyramid.updateWhiteBalanceRGB(MapLocation, autoWhiteCheck, True)

        If sharpenCheck Then 'Check if to apply unsharp mask or not
            PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
            Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
        Else
            PictureBox1.Image = Pyramid.map
        End If
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Pyramid.passPercentage(TrackBar1.Value) 'Get the percent value everytime the scroller is moved

        Pyramid.updateWhiteBalanceRGB(MapLocation, autoWhiteCheck, False)

        If sharpenCheck Then 'Check if to apply unsharp mask or not
            PictureBox1.Image = New Bitmap(Pyramid.map) 'Copies bitmap so to leave the orignal unchanged
            Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
        Else
            PictureBox1.Image = Pyramid.map
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim OpenDialog As New OpenFileDialog
        Dim file As String

        If OpenDialog.ShowDialog() <> DialogResult.OK Then Exit Sub

        file = OpenDialog.FileName
        Dim temp As New PyramidalTiff(file, PictureBox1.Height, PictureBox1.Width, sharpenCheck, autoWhiteCheck)
        cantMove = True 'Can't move if we load a new picture
        MapLocation.X = 0
        MapLocation.Y = 0
        Pyramid = temp
        pictureLoaded = True
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        If pictureLoaded Then 'Updates picturebox if there's a picture loaded
            If sharpenCheck Then
                PictureBox1.Image = New Bitmap(Pyramid.map)
                Pyramid.unsharpFilter.applyMask(PictureBox1.Image)
            Else
                PictureBox1.Image = Pyramid.map
            End If
        End If
    End Sub
End Class
