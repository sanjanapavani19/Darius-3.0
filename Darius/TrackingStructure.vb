Imports AForge.Imaging.Filters
Imports AForge.Imaging





Public Class TrackingStructure

    Class ROIStructure


        Public Class DotsStructure
            Public Rect As Rectangle
            Public selected As Boolean
            Public pen As New Pen(Brushes.Green, 2)
            Public InitialX, InitialY As Integer
            Public IsMoved As Boolean
            Public Grabbed As Boolean
            Public Sub Move(Dx As Integer, Dy As Integer)
                Rect.X = InitialX + Dx
                Rect.Y = InitialY + Dy
            End Sub

        End Class



        Public Rect As Rectangle
        Public IsDragging As Boolean
        Public IsMade As Boolean
        Public IsMoved As Boolean
        Public Dots(2) As DotsStructure
        Public pen As New Pen(Brushes.White, 2)
        Public InitialX, InitialY As Integer
        Public ClickX, ClickY, width, height As Integer

        Public Sub New()
            Dots(0) = New DotsStructure
            Dots(1) = New DotsStructure
            Dots(2) = New DotsStructure
        End Sub

        Public Sub CorrectROI(W As Single, H As Single)

            ROIX = Math.Floor(W / Cursor.Width)
            ROIY = Math.Floor(H / Cursor.Height)

            width = ROIX * Cursor.Width
            height = ROIY * Cursor.Height
        End Sub

        Public Sub clear()
            IsMade = False
            IsDragging = False
        End Sub

        Public Sub Create()
            Rect = New Rectangle(ClickX, ClickY, width, height)

            InitialX = ClickX
            InitialY = ClickY

            Dim StepX, StepY As Integer
            StepX = Rect.Width / 3
            StepY = height / 3

            Dots(0).Rect = New Rectangle(Rect.X, Rect.Y, Cursor.Width, Cursor.Height)
            Dots(0).InitialX = Dots(0).Rect.X
            Dots(0).InitialY = Dots(0).Rect.Y

            Dots(1).Rect = New Rectangle(Rect.Right - Cursor.Width, Rect.Y, Cursor.Width, Cursor.Height)
            Dots(1).InitialX = Dots(1).Rect.X
            Dots(1).InitialY = Dots(1).Rect.Y

            Dots(2).Rect = New Rectangle(Rect.X, Rect.Bottom - Cursor.Height, Cursor.Width, Cursor.Height)
            Dots(2).InitialX = Dots(2).Rect.X
            Dots(2).InitialY = Dots(2).Rect.Y





            IsMade = True
        End Sub

        Public Sub Refresh()
            pen = New Pen(Brushes.White, 2)
            Pbox.Refresh()
            Pbox.CreateGraphics.DrawRectangle(pen, Rect)

            For i = 0 To 2
                Pbox.CreateGraphics.DrawRectangle(Dots(i).pen, Dots(i).Rect)
            Next
        End Sub

        Public Sub Move(Dx As Integer, Dy As Integer)
            Rect.X = InitialX + Dx
            Rect.Y = InitialY + Dy
            Form1.Label4.Text = Dx.ToString

            For i = 0 To 2
                Dots(i).Move(Dx, Dy)
            Next



        End Sub

        Public Sub Settle()
            InitialX = Rect.X
            InitialY = Rect.Y

            For i = 0 To 2
                Dots(i).InitialX = Dots(i).Rect.X
                Dots(i).InitialY = Dots(i).Rect.Y

            Next
        End Sub
    End Class


    Friend WithEvents pb As System.Windows.Forms.PictureBox
    Public Shared Pbox As PictureBox
    Public bmp As FastBMP
    Public Thumbnail As FastBMP
    Dim CursurClicked As Boolean
    Dim RecentX, RecentY As Single


    Public ROI As New ROIStructure


    'Public IsDragging As Boolean
    Public Shared ROIX, ROIY As Integer
    'Public ROImade As Boolean

    Dim T As New Timer
    Public Dots(2) As Rectangle
    Dim rectsIndex As Integer = 0
    'Position in pixels
    Dim X, Y As Single
    Dim XX, YY, ZZ As Single
    Public Shared Cursor As Button
    Public Scanned() As Button

    Public Sub New(ByRef Pb As PictureBox)
        Cursor = New Button


        Pb.Size = New Size(Pb.Width, Pb.Width * (Setting.Gett("Yrange") / Setting.Gett("Xrange")))
        Pb.SizeMode = PictureBoxSizeMode.StretchImage
        Pbox = Pb

        Cursor.Width = stage.FovX * Pb.Width / Setting.Gett("Xrange")
        Cursor.Height = Cursor.Width * stage.FovY / stage.FovX
        Cursor.FlatStyle = FlatStyle.Flat
        Cursor.ForeColor = Color.Yellow
        Cursor.BackColor = Color.Transparent

        Pbox.Controls.Add(Cursor)

        AddHandler T.Tick, AddressOf Update
        AddHandler Cursor.MouseDown, AddressOf GrabCursur
        AddHandler Cursor.MouseUp, AddressOf ReleaseCursur

        AddHandler Pb.DoubleClick, AddressOf Navigate
        AddHandler Pb.MouseDown, AddressOf MouseDown
        AddHandler Pb.MouseUp, AddressOf MouseUp
        AddHandler Pb.MouseMove, AddressOf MouseMove


        '    Pbox.Image = Image.FromFile("C:\2.jpg")
        Pbox.Size = New Size(Pbox.Width, Pbox.Width * (Setting.Gett("Yrange") / Setting.Gett("Xrange")))
        clear()

        T.Interval = 100
        '  Enable()

    End Sub

    Public Sub MouseDown(sender As Object, e As MouseEventArgs)
        ROI.IsDragging = True
        ROI.ClickY = e.Y
        ROI.ClickX = e.X

        For i = 0 To 2
            If ROI.Dots(i).Rect.Contains(e.X, e.Y) Then
                ROI.Dots(i).Grabbed = True

            End If
        Next

    End Sub

    Public Sub MouseMove(sender As Object, e As MouseEventArgs)
        If ROI.IsDragging And Not ROI.Ismade Then

            ROI.CorrectROI(e.X - ROI.ClickX, e.Y - ROI.ClickY)
            Pbox.Refresh()
            Pbox.CreateGraphics.DrawRectangle(ROI.pen, ROI.ClickX, ROI.ClickY, ROI.width, ROI.height)

        End If


        If ROI.IsDragging And ROI.IsMade And ROI.Rect.Contains(e.X, e.Y) Then

            For i = 0 To 2
                If ROI.Dots(i).Grabbed Then
                    ROI.Dots(i).Move(e.X - ROI.ClickX, e.Y - ROI.ClickY)
                    ROI.Dots(i).IsMoved = True
                End If
            Next

            If Not (ROI.Dots(0).IsMoved Or ROI.Dots(1).IsMoved Or ROI.Dots(2).IsMoved) Then
                ROI.IsMoved = True
                ROI.Move(e.X - ROI.ClickX, e.Y - ROI.ClickY)
            End If

            ROI.Refresh()
            End If


    End Sub

    Public Sub MouseUp(sender As Object, e As MouseEventArgs)

        If ROI.width > 0 And ROI.height > 0 And Not ROI.IsMade Then
            ROI.Create()
            ROI.Refresh()
            Form1.SetScan()
        End If

        If ROI.IsMade Then
            If ROI.IsMoved Then ROI.IsMoved = False
            For i = 0 To 2
                If ROI.Dots(i).IsMoved Then ROI.Dots(i).IsMoved = False
                If ROI.Dots(i).Grabbed Then ROI.Dots(i).Grabbed = False
            Next
            ROI.Settle()

        End If

        ROI.IsDragging = False

    End Sub




    Public Sub AddScanned(W As Integer, H As Integer, XX As Single, YY As Single, address As String)
            Dim index As Integer = Scanned.GetLength(0)

            ReDim Preserve Scanned(index)
        Scanned(index) = New Button With {
        .Width = Cursor.Width * W,
        .Left = ConvertCoordinatetoPixels(XX, YY).X - Cursor.Width / 2,
        .Height = Cursor.Height * H,
        .Top = ConvertCoordinatetoPixels(XX, YY).Y - Cursor.Height / 2,
        .FlatStyle = FlatStyle.Flat,
        .ForeColor = Color.Yellow,
        .BackColor = Color.Transparent,
        .Tag = address
            }

        AddHandler Scanned(index).Click, AddressOf ScannedClick
            Pbox.Controls.Add(Scanned(index))

        End Sub
        Public Sub ScannedClick(sender As Object, e As EventArgs)
            If MsgBox(" Do you want to open the image?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                ' open_Gimp(sender.tag)
            End If
        End Sub
    Public Sub UpdateBmp(bmpin As Bitmap)
        Tracking.bmp = New FastBMP(bmpin)
        Tracking.Pbox.Image = Tracking.bmp.bmp
    End Sub
    Public Sub Refreshbmp()
        Pbox.Image = bmp.bmp
    End Sub
    Public Sub clear()
        'bmp = New FastBMP(Pbox.Width, Pbox.Height, Imaging.PixelFormat.Format32bppArgb)


        ROI = New ROIStructure
        Pbox.Refresh()


        'Pbox.Image = bmp.bmp
        If Scanned IsNot Nothing Then
            For i = 0 To Scanned.GetUpperBound(0)
                Pbox.Controls.Remove(Scanned(i))
            Next
        End If

        ReDim Scanned(-1)

    End Sub

        Public Sub GrabCursur()
            CursurClicked = True

        End Sub

        Public Sub ReleaseCursur(sender As Object, e As MouseEventArgs)
            CursurClicked = False

        Stage.MoveAbsolute(Stage.Xaxe, (X + e.X - Cursor.Width / 2) * Setting.Gett("Xrange") / Pbox.Width + Setting.Gett("xMin"))
        Stage.MoveAbsolute(Stage.Yaxe, (Y + e.Y - Cursor.Height / 2) * Setting.Gett("Yrange") / Pbox.Height + Setting.Gett("yMin"))

    End Sub
    Private Sub Navigate(sender As Object, e As MouseEventArgs) Handles pb.MouseDoubleClick
        Stage.MoveAbsolute(Stage.Xaxe, ConvertPixeltoCoordinateX(e.X))
        Stage.MoveAbsolute(Stage.Yaxe, ConvertPixeltoCoordinateY(e.Y))
    End Sub
    Public Sub MovetoROIEdge()
        Stage.MoveAbsolute(Stage.Xaxe, ConvertPixeltoCoordinateX(ROI.Rect.X + Cursor.Width / 2))
        Stage.MoveAbsolute(Stage.Yaxe, ConvertPixeltoCoordinateY(ROI.Rect.Y + Cursor.Height / 2))
    End Sub


    Public Sub MovetoDots(i As Integer)
        Stage.MoveAbsolute(Stage.Xaxe, ConvertPixeltoCoordinateX(ROI.Dots(i).Rect.X + Cursor.Width / 2))
        Stage.MoveAbsolute(Stage.Yaxe, ConvertPixeltoCoordinateY(ROI.Dots(i).Rect.Y + Cursor.Height / 2))
    End Sub


    Public Sub Enable()
        T.Enabled = True
    End Sub

    Public Sub Disable()
        T.Enabled = False
    End Sub

    Public Sub Update()

        Cursor.Width = Stage.FOVX * Pbox.Width / Setting.Gett("Xrange")
        Cursor.Height = Cursor.Width * Stage.FOVY / Stage.FOVX

        ' These are in mm

        'XX = stage.GetPosition(stage.Xaxe)
        'YY = stage.GetPosition(stage.Yaxe)

        XX = Stage.X
        YY = Stage.Y
        ZZ = Stage.Z
        '   ZZ = stage.GetPosition(stage.Zport)

        Form1.ToolStripStatusLabel1.Text = "X: " + XX.ToString
            Form1.ToolStripStatusLabel2.Text = "Y: " + YY.ToString
            Form1.ToolStripStatusLabel3.Text = "Z: " + ZZ.ToString
            ' Now conversion 

            X = ConvertCoordinatetoPixels(XX, YY).X
            Y = ConvertCoordinatetoPixels(XX, YY).Y


            '  If X <> RecentX Or Y <> RecentY Then

            Cursor.Top = Y - Cursor.Height / 2
            Cursor.Left = X - Cursor.Width / 2
        '   End If

        ROI.Refresh()

        RecentX = X
            RecentY = Y
        End Sub

    Private Function ConvertCoordinatetoPixels(XX As Single, YY As Single) As Point
            Dim P As New Point
            XX = XX - Setting.Gett("xMin")
            YY = YY - Setting.Gett("yMin")
            P.X = Pbox.Width - XX * Pbox.Width / Setting.Gett("Xrange")
            P.Y = YY * Pbox.Height / Setting.Gett("Yrange")

            Return P
        End Function

        Public Function ConvertPixeltoCoordinateX(XX As Single) As Single

            X = Setting.Gett("Xrange") - XX * Setting.Gett("Xrange") / Pbox.Width + Setting.Gett("xMin")

            Return X
        End Function

        Public Sub MakeThumbnail(X As Single, y As Single, W As Integer, H As Integer)
        'bmp.Reset()
        'Thumbnail.MakeNew(bmp.width, bmp.height, Imaging.PixelFormat.Format24bppRgb)
        'Thumbnail.GR.DrawImage(bmp.bmp, 0, 0)

        'Thumbnail.GR.DrawRectangle(New Pen(Color.LawnGreen, 3), ConvertCoordinatetoPixels(X, y).X - CInt(Cursor.Width / 2), ConvertCoordinatetoPixels(X, y).Y - CInt(Cursor.Height / 2), Cursor.Width * W, Cursor.Height * H)
        'Thumbnail.GR.FillRectangle(New SolidBrush(Color.LawnGreen), ConvertCoordinatetoPixels(X, y).X - CInt(Cursor.Width / 2), ConvertCoordinatetoPixels(X, y).Y - CInt(Cursor.Height / 2), Cursor.Width * W, Cursor.Height * H)

    End Sub
        Public Function ConvertPixeltoCoordinateY(yy As Single) As Single

            Y = yy * Setting.Gett("Yrange") / Pbox.Height + Setting.Gett("yMin")

            Return Y
        End Function
    End Class