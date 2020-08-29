Imports System.Drawing.Imaging
Imports System.Threading
Imports BitMiracle.LibTiff.Classic

Public Class TileStructure
    Structure TileStructure
        Dim X, Y As Integer
        Dim width, height As Integer
        Dim numbers As Integer
        Dim index As Integer

        Dim bytes() As Byte

    End Structure

    Dim Scale As Integer

    Dim stride As Integer
    Public pages, page As Integer
    Dim address As String
    Dim tiff As Tiff
    Dim tsx, tsy As Integer
    Dim index, ii, indexYoffset, indexXoffset As Integer
    Public Ready As Boolean
    Dim bmpWidth, bmpheight As Integer
    Dim X, Y As Integer
    Dim XX, YY As Integer
    Dim bytes As Byte()
    Dim compress As Integer
    Dim Tile As TileStructure
    Public Sub New(X As Integer, Y As Integer, bmpWidth As Integer, bmpHeight As Integer, pages As Integer, page As Integer, address As String, compression As Integer)

        Me.pages = pages
        Me.X = X
        Me.Y = Y
        Tile.X = 8
        Tile.Y = 8

        Me.bmpWidth = bmpWidth
        Me.bmpheight = bmpHeight
        Tile.width = Int(bmpWidth / (16 * Tile.X)) * 16
        Tile.height = Int(bmpHeight / (16 * Tile.Y)) * 16
        compress = compression

        ' Stupid way of calculating stride
        Dim bmp = New Bitmap(bmpWidth, bmpHeight, Imaging.PixelFormat.Format24bppRgb)
        Dim rect As Rectangle = New Rectangle(0, 0, bmpWidth, bmpHeight)
        Dim bmpData As BitmapData = bmp.LockBits(rect, ImageLockMode.ReadWrite, Imaging.PixelFormat.Format24bppRgb)

        stride = bmpData.Stride
        tiff = Tiff.Open(address, "w")
        ReDim bytes(stride * bmpHeight - 1)


        Scale = 2 ^ page
        Tile.X = 8 / Scale
        Tile.Y = 8 / Scale
        ReDim Tile.bytes(X * Y * Tile.X * Tile.Y - 1)
        'Tile size shouldn't change 
        Tile.width = Int(bmpWidth / (16 * Tile.X * Scale)) * 16
        Tile.height = Int(bmpHeight / (16 * Tile.Y * Scale)) * 16
        Tile.numbers = Tile.X * Tile.Y * X * Y

        indexYoffset = (stride - Tile.width * 3) * Scale
        indexXoffset = 3 * Scale

        ReDim Tile.bytes(Tile.width * Tile.height * 3 - 1)
        SetTiff(page)

        Ready = True
    End Sub

    Public Sub SaveTile(XX As Integer, YY As Integer, bytesin As Byte())


        Tile.index = YY * X * Tile.X * Tile.Y + XX * Tile.X

        Buffer.BlockCopy(bytesin, 0, bytes, 0, bytes.GetLength(0))
        Me.XX = XX
        Me.YY = YY

        Dim Thread1 As New System.Threading.Thread(AddressOf SaveTileThreded)
        Thread1.Start()

        'SaveTileThreded()

    End Sub


    Public Sub SaveTileThreded()
        Ready = False
        For ty = 0 To Tile.Y - 1
            For tx = 0 To Tile.X - 1
                ' now reading the tile 
                tsx = tx * Tile.width
                tsy = ty * Tile.height
                ii = 0
                index = (tsy * stride + tsx * 3) * Scale
                For yt = 0 To Tile.height - 1
                    For xt = 0 To Tile.width - 1
                        Tile.bytes(ii + 2) = bytes(index)
                        Tile.bytes(ii + 1) = bytes(index + 1)
                        Tile.bytes(ii) = bytes(index + 2)
                        index = index + indexXoffset
                        ii += 3
                    Next
                    index += indexYoffset
                Next

                tiff.WriteEncodedTile(Tile.index, Tile.bytes, Tile.bytes.Length)
                Tile.index += 1

            Next
            Tile.index += (X - 1) * Tile.X
        Next

        Array.Clear(Tile.bytes, 0, Tile.bytes.Length())
        Ready = True

    End Sub

    Public Sub Close()
        tiff.WriteDirectory()
        tiff.Close()

    End Sub

    Private Sub SetTiff(page As Integer)
        Dim PIXEL_WIDTH As Integer = X * Tile.width * Tile.X
        Dim PIXEL_HEIGHT As Integer = Y * Tile.height * Tile.Y

        tiff.SetField(TiffTag.IMAGEWIDTH, PIXEL_WIDTH)
        tiff.SetField(TiffTag.IMAGELENGTH, PIXEL_HEIGHT)
        tiff.SetField(TiffTag.COMPRESSION, Compression.NONE)

        'tiff.SetField(TiffTag.JPEGQUALITY, compress)
        tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB)
        tiff.SetField(TiffTag.ROWSPERSTRIP, PIXEL_HEIGHT)
        tiff.SetField(TiffTag.XRESOLUTION, 96)
        tiff.SetField(TiffTag.YRESOLUTION, 96)
        tiff.SetField(TiffTag.BITSPERSAMPLE, 8)
        tiff.SetField(TiffTag.SAMPLESPERPIXEL, 3)
        tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG)
        tiff.SetField(TiffTag.TILEWIDTH, Tile.width)
        tiff.SetField(TiffTag.TILELENGTH, Tile.height)
        tiff.SetField(TiffTag.PAGENUMBER, page, pages)
    End Sub
End Class
