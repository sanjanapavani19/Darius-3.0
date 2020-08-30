Imports BitMiracle.LibTiff.Classic
Imports System.Threading
Imports System.IO

Public Class PyramidalTiff
    Dim tiff As Tiff
    Public map As Bitmap
    Public tileGroup As Bitmap 'Debugging purposes
    Dim boxHeight, boxWidth As Integer 'Size of the picturebox
    Dim width As Integer 'Width of the whole image in pixels
    Dim height As Integer 'Height of the whole image in pixels
    Dim xTiles As Integer 'Number of tiles in the x direction
    Dim yTiles As Integer 'Number of tiles in the y direction
    Dim nBytes As Integer 'Number of bytes in a tile
    Dim pages As Integer 'Number of pages in the tiff file
    Dim nTiles As Integer 'n tiles to load in nxn tilesGroup
    Dim oldTileIndex As Integer 'Contains the index used last time loadTilesGroup was run
    Dim digZoomBool As Boolean 'True when during digital zoom
    Structure digitalPicturebox
        Dim width As Integer
        Dim height As Integer
        Dim bytes() As Byte
    End Structure
    Structure TileStructure
        Dim width As Integer 'Width of a tile 
        Dim height As Integer 'Height of a tile
        Dim index As Integer 'Index of the tile
        Dim page As Integer 'Page number of the tile
    End Structure
    Public tile As TileStructure
    Private zoomBox As digitalPicturebox
    Dim whiteFilter As New whiteBalance 'Auto white balance filter object
    Public unsharpFilter As New sharpening 'Unsharp mask filter object
    Dim tilesBytes() As Byte '3x3 tiles group together
    Dim pictureBytes() As Byte 'Lowest resolution picture
    Dim boxBytes() As Byte 'Image section viewable in picturebox

    Public Sub New(address As String, picHeight As Integer, picWidth As Integer, sharpen As Boolean, white As Boolean) 'Class constructor
        If Not IsNothing(address) Then 'Loads picture when an address is passed
            tiff = Tiff.Open(address, "r") 'Opens tiff stream
            pages = tiff.NumberOfDirectories()
            boxHeight = picHeight
            boxWidth = picWidth
            tile.page = pages - 1
            nTiles = 4 'nxn tilesGroup
            tiff.SetDirectory(tile.page) 'Start with the lowest resolution picture
            readPageInfo()
            map = New Bitmap(width, height, Imaging.PixelFormat.Format24bppRgb)
            tileGroup = New Bitmap(tile.width * nTiles, tile.height * nTiles, Imaging.PixelFormat.Format24bppRgb) 'Debugging
            nBytes = tile.height * tile.width * 3 - 1 'Number of bytes in a tile - 1
            xTiles = width / tile.width 'Number of tiles in the x direction
            yTiles = height / tile.height 'Number of tiles in the y direction
            digZoomBool = True
            zoomBox.width = width
            zoomBox.height = height
            ReDim tilesBytes((nBytes + 1) * (nTiles ^ 2) - 1) 'Resize the array to hold nxn tiles
            whiteFilter.setPercentage(0) 'Set the dafault percentage for white filter
            loadPicture(white)
        End If
    End Sub

    Public Sub digitalZoomIn(ByRef maplocation As Point, mouselocation As Point, sharpen As Boolean, white As Boolean)
        Dim xLocation, yLocation As Integer 'Original location of zoom in point
        Dim xScale, yScale As Decimal 'Ratio of the picturebox to the actual size of the array loaded in the picturebox
        If (zoomBox.width / 2) > boxWidth And (zoomBox.height / 2) > boxHeight Then 'Checks if to zoom in digitally or normally
            xScale = zoomBox.width / boxWidth
            yScale = zoomBox.height / boxHeight
            xLocation = (-maplocation.X) + mouselocation.X * xScale 'Finds the location of the mouse pointer on the picture
            yLocation = (-maplocation.Y) + mouselocation.Y * yScale 'Finds the location of the mouse pointer on the picture

            zoomBox.width /= 2 'Update the zoomBox size
            zoomBox.height /= 2 'Update the zoomBox size

            maplocation.X -= (xLocation + maplocation.X) / 2 'Finds the new maplocation to make transition seamless
            maplocation.Y -= (yLocation + maplocation.Y) / 2 'Finds the new maplocation to make transition seamless

            ReDim zoomBox.bytes(zoomBox.width * zoomBox.height * 3 - 1) 'Resizes the zoomBox array to the new size
            map = New Bitmap(zoomBox.width, zoomBox.height, Imaging.PixelFormat.Format24bppRgb) 'Prepares bitmap to hold the new size

            checkZoomBoxBounds(maplocation) 'Make sure its within bounds
            loadZoomBox(maplocation, white)
        Else
            Dim xTileOffset As Integer 'x coordinate of the mouse within the tile it's in
            Dim yTilesOffset As Integer 'y coordinate of the mouse within the tile it's in
            Dim temp As Integer 'temporary variable to hold a value needed for calculations
            digZoomBool = False

            xScale = zoomBox.width / boxWidth
            yScale = zoomBox.height / boxHeight
            'Skip a page if the next page doesn't have at least nTiles to load
            If height < boxHeight Or width < boxWidth Or xTiles < 4 Or yTiles < 4 Then
                tile.page -= 1
                tiff.SetDirectory(tile.page)
                readPageInfo()
                xTiles = width / tile.width 'Number of tiles in the x direction
                yTiles = height / tile.height 'Number of tiles in the y direction
                xScale = width / boxWidth
                yScale = height / boxHeight
            End If
            xLocation = (-maplocation.X) + mouselocation.X * xScale
            yLocation = (-maplocation.Y) + mouselocation.Y * yScale
            map = New Bitmap(boxWidth, boxHeight, Imaging.PixelFormat.Format24bppRgb)

            'Adjust the picturebox to the right location of zoom in
            xTileOffset = xLocation Mod tile.width 'Calculate the number of pixels the zoom in point is within the tile, it's 0 at the extreme edge
            yTilesOffset = yLocation Mod tile.height 'Calculate the number of pixels the zoom in point is within the tile, it's 0 at the extreme edge
            maplocation.X = -xTileOffset - tile.width + mouselocation.X 'Locate the tile groups appropriately to make zoom seamless
            maplocation.Y = -yTilesOffset - tile.height + mouselocation.Y 'Locate the tile groups appropriately to make zoom seamless
            'Check if the location is out of bounds
            If xLocation >= width Then
                xLocation = width - 1
                maplocation.X -= tile.width
            End If
            If yLocation >= height Then
                yLocation = height - 1
                maplocation.Y -= tile.height
            End If
            tile.index = tiff.ComputeTile(xLocation, yLocation, 0, 0) 'Finds the tile index from the coordinates
            'Adjust index to load tiles around the zoom in point
            If (tile.index - xTiles) >= 0 Then
                tile.index -= xTiles
            Else
                maplocation.Y += tile.height
            End If
            If (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1 Then
                tile.index -= 1
            Else
                maplocation.X += tile.width
            End If

            adjustZoom(mouselocation, maplocation)

            'Make sure there are enough tiles around the index tile to load a group, if not, adjust maplocation
            If tile.index Mod xTiles >= xTiles - nTiles Then 'Check right
                temp = (tile.index Mod xTiles) - (xTiles - nTiles)
                tile.index -= temp
                maplocation.X -= temp * tile.width
            End If

            If (tile.index + (nTiles - 1) * xTiles) >= (xTiles * yTiles) Then 'Check down
                temp = Math.Ceiling(((tile.index + (nTiles - 1) * xTiles) Mod (xTiles * yTiles - 1)) / xTiles) * xTiles
                tile.index -= temp
                maplocation.Y -= (temp / xTiles) * tile.height
            End If
            'Moves index tile around to make sure to load a group that will make the transition seamless
            While maplocation.X > 0 And (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1
                tile.index -= 1
                maplocation.X -= tile.width
            End While
            While maplocation.Y > 0 And tile.index - xTiles > 0
                tile.index -= xTiles
                maplocation.Y -= tile.height
            End While

            oldTileIndex = tile.index
            loadTilesGroup()
            checkBounds(maplocation)
            loadPictureBox(maplocation, white)
        End If

    End Sub

    Public Sub digitalZoomOut(ByRef maplocation As Point, mouselocation As Point, sharpen As Boolean, white As Boolean)
        Dim xLocation, yLocation As Integer 'Original location of zoom in point
        Dim xScale, yScale As Decimal 'Ratio of the picturebox to the actual size of the array loaded in the picturebox

        If digZoomBool Then 'Checks if transition between tile zoom to digital zoom is needed
            If zoomBox.width < width And zoomBox.height < height Then
                xScale = zoomBox.width / boxWidth
                yScale = zoomBox.height / boxHeight
                xLocation = (-maplocation.X) + mouselocation.X * xScale 'Finds the location of the mouse pointer on the picture
                yLocation = (-maplocation.Y) + mouselocation.Y * yScale 'Finds the location of the mouse pointer on the picture

                zoomBox.width *= 2 'Update the zoomBox size
                zoomBox.height *= 2 'Update the zoomBox size

                maplocation.X += xLocation + maplocation.X 'Finds the new maplocation to make transition seamless
                maplocation.Y += yLocation + maplocation.Y 'Finds the new maplocation to make transition seamless

                ReDim zoomBox.bytes(zoomBox.width * zoomBox.height * 3 - 1) 'Resizes the zoomBox array to the new size
                map = New Bitmap(zoomBox.width, zoomBox.height, Imaging.PixelFormat.Format24bppRgb) 'Prepares bitmap to hold the new size

                checkZoomBoxBounds(maplocation) 'Make sure its within bounds
                loadZoomBox(maplocation, white)

            End If
        Else
            Dim yTilesOffset, xTilesOffset As Integer
            Dim location As Point
            digZoomBool = True
            yTilesOffset = Math.Floor(tile.index / xTiles) 'Number of tiles above the currently loaded group
            xTilesOffset = tile.index Mod xTiles 'Number of tiles to the left of the currently loadedd 9x9 group
            location.X = (xTilesOffset * tile.width) + -(maplocation.X) + mouselocation.X 'Finds the location of the mouse on the picture
            location.Y = (yTilesOffset * tile.height) + -(maplocation.Y) + mouselocation.Y 'Finds the location of the mouse on the picture
            'Check if the location is out of bounds
            If location.X >= width Then
                location.X = width - 1
            End If
            If location.Y >= height Then
                location.Y = height - 1
            End If

            xScale = zoomBox.width / boxWidth
            yScale = zoomBox.height / boxHeight
            maplocation.X = -location.X + mouselocation.X * xScale 'Finds the new maplocation to make transition seamless
            maplocation.Y = -location.Y + mouselocation.Y * yScale 'Finds the new maplocation to make transition seamless

            ReDim zoomBox.bytes(zoomBox.width * zoomBox.height * 3 - 1) 'Resizes the zoomBox array to the new size
            map = New Bitmap(zoomBox.width, zoomBox.height, Imaging.PixelFormat.Format24bppRgb) 'Prepares bitmap to hold the new size

            oldTileIndex = tile.index
            checkZoomBoxBounds(maplocation) 'Make sure its within bounds
            loadZoomBox(maplocation, white)
            If whiteFilter.getupdateBalanced Then 'Check if we need to update update balancedBytes
                updateWhiteBalanceRGB(maplocation, white, False)
            End If
        End If
    End Sub

    Public Function zoomIn(ByRef maplocation As Point, mouseLocation As Point) 'Zooms into the tile that the mouse is hovering above
        Dim temp As Integer 'Variable to hold temporary value
        Dim oldHeight, oldWidth As Integer 'Image size before zoom in
        Dim xScale, yScale As Decimal 'X and Y scale to scale the coordinate on the old image to the new one
        Dim oldXLocation, oldYLocation As Integer 'Original location of zoom in point
        Dim newXLocation, newYLocation As Integer 'Scaled location in the new image
        Dim yTilesOffset, xTilesOffset As Integer
        If tile.page - 1 >= 0 And Not digZoomBool Then 'If there is a page to zoom in into
            'Find the index of the tile that the mouse is hovering above
            yTilesOffset = Math.Floor(tile.index / xTiles)  'Number of tiles above the currently loaded 9x9 group
            xTilesOffset = tile.index Mod xTiles 'Number of tiles to the left of the currently loadedd 9x9 group
            oldXLocation = (xTilesOffset * tile.width) + -(maplocation.X) + mouseLocation.X
            oldYLocation = (yTilesOffset * tile.height) + -(maplocation.Y) + mouseLocation.Y
            'Check if the location is out of bounds
            If oldXLocation >= width Then
                oldXLocation = width - 1
            End If
            If oldYLocation >= height Then
                oldYLocation = height - 1
            End If
            tile.index = tiff.ComputeTile(oldXLocation, oldYLocation, 0, 0) 'Compute the index of the tile the mouse is on

            'Load the next page for processing
            tile.page -= 1
            oldHeight = height 'Save height before loading new page
            oldWidth = width 'Save width before loading new page
            tiff.SetDirectory(tile.page) 'Load new page
            readPageInfo() 'Load image info into memory
            xScale = width / oldWidth 'Calculate scale value
            yScale = height / oldHeight 'Calculate scale value
            newXLocation = oldXLocation * xScale
            newYLocation = oldYLocation * yScale
            'Check if the location is out of bounds
            If newXLocation >= width Then
                newXLocation = width - 1
            End If
            If newYLocation >= height Then
                newYLocation = height - 1
            End If
            tile.index = tiff.ComputeTile(newXLocation, newYLocation, 0, 0) 'Finds the tile index from the coordinates
            xTiles = width / tile.width 'Find new number of tile rows
            yTiles = height / tile.height 'Find new number of tile columns

            'Adjust the picturebox to the right location of zoom in
            newXLocation = newXLocation Mod tile.width 'Calculate the number of pixels the zoom in point is within the tile
            newYLocation = newYLocation Mod tile.height 'Calculate the number of pixels the zoom in point is within the tile
            maplocation.X = -newXLocation - tile.width + mouseLocation.X 'Locate the tile groups appropriately to make zoom seamless
            maplocation.Y = -newYLocation - tile.height + mouseLocation.Y 'Locate the tile groups appropriately to make zoom seamless

            'Adjust index to load tiles around the zoom in point
            If (tile.index - xTiles) >= 0 Then
                tile.index -= xTiles
            Else
                maplocation.Y += tile.height
            End If
            If (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1 Then
                tile.index -= 1
            Else
                maplocation.X += tile.width
            End If

            adjustZoom(mouseLocation, maplocation)
            'Make sure there are enough tiles around the index tile to load a group, if not, adjust maplocation
            If tile.index Mod xTiles >= xTiles - nTiles Then 'Check right
                temp = (tile.index Mod xTiles) - (xTiles - nTiles)
                tile.index -= temp
                maplocation.X -= temp * tile.width
            End If

            If (tile.index + (nTiles - 1) * xTiles) >= (xTiles * yTiles) Then 'Check down
                temp = Math.Ceiling(((tile.index + (nTiles - 1) * xTiles) Mod (xTiles * yTiles - 1)) / xTiles) * xTiles
                tile.index -= temp
                maplocation.Y -= (temp / xTiles) * tile.height
            End If
            'Moves index tile around to make sure to load a group that will make the transition seamless
            While maplocation.X > 0 And (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1
                tile.index -= 1
                maplocation.X -= tile.width
            End While
            While maplocation.Y > 0 And tile.index - xTiles > 0
                tile.index -= xTiles
                maplocation.Y -= tile.height
            End While

            oldTileIndex = tile.index
            loadTilesGroup()
            Return True
        End If
        Return False
    End Function

    Public Function zoomOut(ByRef maplocation As Point, mouseLocation As Point) 'Zooms out into the tile that the mouse is hovering above
        Dim temp As Integer 'Variable to hold temporary value
        Dim oldHeight, oldWidth As Integer 'Image size before zoom in
        Dim xScale, yScale As Decimal 'X and Y scale to scale the coordinate on the old image to the new one
        Dim oldXLocation, oldYLocation As Integer 'Original location of zoom in point
        Dim newXLocation, newYLocation As Integer 'Scaled location in the new image
        Dim yTilesOffset, xTilesOffset As Integer


        If tile.page + 1 < pages And Not digZoomBool Then 'If there is a page to zoom out to
            'Find the index of the tile that the mouse is hovering above
            yTilesOffset = Math.Floor(tile.index / xTiles) 'Number of tiles above the currently loaded 9x9 group
            xTilesOffset = tile.index Mod xTiles 'Number of tiles to the left of the currently loadedd 9x9 group
            oldXLocation = (xTilesOffset * tile.width) + -maplocation.X + mouseLocation.X
            oldYLocation = (yTilesOffset * tile.height) + -maplocation.Y + mouseLocation.Y
            'Check if the location is out of bounds
            If oldXLocation >= width Then
                oldXLocation = width - 1
            End If
            If oldYLocation >= height Then
                oldYLocation = height - 1
            End If
            tile.index = tiff.ComputeTile(oldXLocation, oldYLocation, 0, 0) 'Compute the index of the tile the mouse is on

            'Load the next page for processing
            tile.page += 1
            oldHeight = height 'Save height before loading new page
            oldWidth = width 'Save width before loading new page
            tiff.SetDirectory(tile.page) 'Load new page
            readPageInfo() 'Load image info into memory
            xTiles = width / tile.width 'Find new number of tile rows
            yTiles = height / tile.height 'Find new number of tile columns

            If height < boxHeight Or width < boxWidth Or xTiles < 4 Or yTiles < 4 Then 'Check if the new page is bigger than the picturebox or if there are at least 4 tiles
                Return False
            End If

            xScale = width / oldWidth 'Calculate scale value
            yScale = height / oldHeight 'Calculate scale value
            newXLocation = oldXLocation * xScale
            newYLocation = oldYLocation * yScale
            'Check if the location is out of bounds
            If newXLocation >= width Then
                newXLocation = width - 1
            End If
            If newYLocation >= height Then
                newYLocation = height - 1
            End If
            tile.index = tiff.ComputeTile(newXLocation, newYLocation, 0, 0) 'Finds the tile index from the coordinates

            'Adjust the picturebox to the right location of zoom in
            newXLocation = newXLocation Mod tile.width 'Calculate the number of pixels the zoom in point is within the tile
            newYLocation = newYLocation Mod tile.height 'Calculate the number of pixels the zoom in point is within the tile
            maplocation.X = -newXLocation - tile.width + mouseLocation.X 'Locate the tile groups appropriately to make zoom seamless
            maplocation.Y = -newYLocation - tile.height + mouseLocation.Y 'Locate the tile groups appropriately to make zoom seamless

            'Adjust index to load tiles around the zoom in point
            If (tile.index - xTiles) >= 0 Then
                tile.index -= xTiles
            Else
                maplocation.Y += tile.height
            End If
            If (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1 Then
                tile.index -= 1
            Else
                maplocation.X += tile.width
            End If

            adjustZoom(mouseLocation, maplocation)
            'Make sure there are enough tiles around the index tile to load a group, if not, adjust maplocation
            If tile.index Mod xTiles >= xTiles - nTiles Then 'Check right
                temp = (tile.index Mod xTiles) - (xTiles - nTiles)
                tile.index -= temp
                maplocation.X -= temp * tile.width
            End If

            If (tile.index + (nTiles - 1) * xTiles) >= (xTiles * yTiles) Then 'Check down
                temp = Math.Ceiling(((tile.index + (nTiles - 1) * xTiles) Mod (xTiles * yTiles - 1)) / xTiles) * xTiles
                tile.index -= temp
                maplocation.Y -= (temp / xTiles) * tile.height
            End If
            'Moves index tile around to make sure to load a group that will make the transition seamless
            While maplocation.X > 0 And (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1
                tile.index -= 1
                maplocation.X -= tile.width
            End While
            While maplocation.Y > 0 And tile.index - xTiles > 0
                tile.index -= xTiles
                maplocation.Y -= tile.height
            End While

            oldTileIndex = tile.index
            loadTilesGroup()
            Return True
        End If
        Return False
    End Function

    Private Sub adjustZoom(mouselocation As Point, ByRef maplocation As Point) 'Depending on in which quadrant of the picturebox we are zooming in offsets the group of tile loaded to make transition seamless
        If mouselocation.X > boxWidth / 2 And (tile.index - 1) Mod xTiles >= 0 And Not (tile.index - 1) Mod xTiles = xTiles - 1 Then 'Right two quadrants of picturebox
            tile.index -= 1
            maplocation.X -= tile.width
        End If

        If mouselocation.Y > boxHeight / 2 And tile.index - xTiles > 0 Then 'Bottom two quadrant of the picturebox
            tile.index -= xTiles
            maplocation.Y -= tile.height
        End If
    End Sub

    Public Sub loadPictureBox(maplocation As Point, white As Boolean) 'Creates a bitmap the size of the picturebox to be shown
        ReDim boxBytes(boxWidth * boxHeight * 3 - 1) 'Creates an array the size of the picturebox

        For i = 0 To boxHeight - 1 'BlockCopy line by line from the tiles group to the boxBytes
            Buffer.BlockCopy(tilesBytes, 3 * nTiles * (-maplocation.Y) * tile.width + (-maplocation.X) * 3 + i * 3 * tile.width * nTiles, boxBytes, 3 * i * boxWidth, boxWidth * 3)
        Next

        If white Then whiteFilter.applyWhiteBalance(boxBytes) 'Check if white balance should be applied
        byteToBitmap(boxBytes, map)
    End Sub

    Private Sub loadTilesGroup() 'Loads a nxn group of tiles, using the index of top left corner tile as reference point for logic
        Dim currentIndex As Integer 'The index of the tile we are working with
        Dim tiles(nTiles - 1)() As Byte 'Row of n tiles
        'Initialize each element in the array to its correct size
        For i = 0 To nTiles - 1
            ReDim tiles(i)(nBytes)
        Next

        If tile.index = oldTileIndex Then
            'Loops to load the nxn group of tiles
            For n = 0 To nTiles - 1
                'Read row of n tiles into tiles(index)()
                For index = 0 To nTiles - 1
                    currentIndex = tile.index + n * xTiles + index
                    tiff.ReadTile(tiles(index), 0, (currentIndex Mod xTiles) * tile.width, Math.Floor(currentIndex / xTiles) * tile.height, 0, 0)
                Next
                'Copies the row of tiles to the correct position in tilesBytes()
                For i = 0 To tile.height - 1 'Goes through height of tile
                    For j = 0 To nTiles - 1 'Goes through n tiles
                        Buffer.BlockCopy(tiles(j), i * tile.width * 3, tilesBytes, 3 * (j * tile.width + i * nTiles * tile.width) + n * nTiles * (nBytes + 1), tile.width * 3)
                    Next
                Next
            Next
            BGRtoRGB(tilesBytes.Length - 1, tilesBytes) 'Convert the loaded tiles to RGB order

        Else
            If oldTileIndex = tile.index + xTiles Then 'Move up
                'Read row of n tiles into tiles(index)()
                For index = 0 To nTiles - 1
                    currentIndex = tile.index + index
                    tiff.ReadTile(tiles(index), 0, (currentIndex Mod xTiles) * tile.width, Math.Floor(currentIndex / xTiles) * tile.height, 0, 0)
                    BGRtoRGB(tiles(index).Length - 1, tiles(index))
                Next
                'Shift old tiles down and copies in new column of n tiles at the top
                For i = tile.height * nTiles - 1 To tile.height Step -1 'Goes through height of tile
                    Buffer.BlockCopy(tilesBytes, i * tile.width * nTiles * 3 - tile.height * tile.width * nTiles * 3, tilesBytes, i * tile.width * nTiles * 3, 3 * tile.width * nTiles)
                Next
                For i = 0 To tile.height - 1 'Goes through height of tile
                    For j = 0 To nTiles - 1 'Goes through tiles
                        Buffer.BlockCopy(tiles(j), i * tile.width * 3, tilesBytes, i * tile.width * nTiles * 3 + j * tile.width * 3, tile.width * 3)
                    Next
                Next

            ElseIf oldTileIndex = tile.index - xTiles Then 'Move down
                'Read row of n tiles into tiles(index)()
                For index = 0 To nTiles - 1
                    currentIndex = tile.index + index + (nTiles - 1) * xTiles
                    tiff.ReadTile(tiles(index), 0, (currentIndex Mod xTiles) * tile.width, Math.Floor(currentIndex / xTiles) * tile.height, 0, 0)
                    BGRtoRGB(tiles(index).Length - 1, tiles(index))
                Next
                'Shift old tiles up and copies in new column of n tiles at the bottom
                For i = 0 To tile.height * (nTiles - 1) - 1 'Goes through height of tile
                    Buffer.BlockCopy(tilesBytes, i * tile.width * nTiles * 3 + tile.height * tile.width * nTiles * 3, tilesBytes, i * tile.width * nTiles * 3, 3 * tile.width * nTiles)
                Next
                For i = 0 To tile.height - 1 'Goes through height of tile
                    For j = 0 To nTiles - 1 'Goes through tiles
                        Buffer.BlockCopy(tiles(j), i * tile.width * 3, tilesBytes, i * tile.width * nTiles * 3 + j * tile.width * 3 + (nTiles - 1) * tile.height * tile.width * nTiles * 3, tile.width * 3)
                    Next
                Next

            ElseIf oldTileIndex = tile.index - 1 Then 'Move right
                'Read col of n tiles into tiles(index)()
                For index = 0 To nTiles - 1
                    currentIndex = tile.index + index * xTiles + nTiles - 1
                    tiff.ReadTile(tiles(index), 0, (currentIndex Mod xTiles) * tile.width, Math.Floor(currentIndex / xTiles) * tile.height, 0, 0)
                    BGRtoRGB(tiles(index).Length - 1, tiles(index))
                Next
                'Moves old tiles over to the left and copies in new column of n tiles
                For i = 0 To tile.height * nTiles - 1 'Goes through height of tile
                    Buffer.BlockCopy(tilesBytes, i * nTiles * tile.width * 3 + tile.width * 3, tilesBytes, i * nTiles * tile.width * 3, 3 * tile.width * (nTiles - 1))
                    Buffer.BlockCopy(tiles(Math.Floor(i / tile.height)), (i * tile.width * 3) Mod (tile.width * tile.height * 3), tilesBytes, i * nTiles * tile.width * 3 + (nTiles - 1) * tile.width * 3, tile.width * 3)
                Next

            ElseIf oldTileIndex = tile.index + 1 Then 'Move left
                'Read col of n tiles into tiles(index)()
                For index = 0 To nTiles - 1
                    currentIndex = tile.index + index * xTiles
                    tiff.ReadTile(tiles(index), 0, (currentIndex Mod xTiles) * tile.width, Math.Floor(currentIndex / xTiles) * tile.height, 0, 0)
                    BGRtoRGB(tiles(index).Length - 1, tiles(index))
                Next
                'Moves old tiles over to the right and copies in new column of n tiles
                For i = 0 To tile.height * nTiles - 1 'Goes through height of tile
                    Buffer.BlockCopy(tilesBytes, i * nTiles * tile.width * 3, tilesBytes, i * nTiles * tile.width * 3 + tile.width * 3, 3 * tile.width * (nTiles - 1))
                    Buffer.BlockCopy(tiles(Math.Floor(i / tile.height)), (i * tile.width * 3) Mod (tile.width * tile.height * 3), tilesBytes, i * nTiles * tile.width * 3, tile.width * 3)
                Next
            End If

        End If

        oldTileIndex = tile.index
        byteToBitmap(tilesBytes, tileGroup) 'Debugging
    End Sub

    Public Sub loadPicture(white As Boolean)
        If IsNothing(pictureBytes) Then 'Check if picture has already been loaded in the array
            ReDim pictureBytes((nBytes + 1) * xTiles * yTiles - 1)
            Dim rowTiles(xTiles - 1)() As Byte
            For i = 0 To xTiles - 1
                ReDim rowTiles(i)(nBytes)
            Next

            'Goes through tiles row by row
            For rowTile = 0 To yTiles - 1
                'Retrieves tiles row by row
                For i = rowTile * xTiles To (rowTile * xTiles) + xTiles - 1
                    tiff.ReadEncodedTile(i, rowTiles(i Mod xTiles), 0, nBytes + 1)
                Next

                'Copy tiles into picturebytes
                For j = 0 To tile.height - 1
                    For k = 0 To xTiles - 1
                        Buffer.BlockCopy(rowTiles(k), j * tile.width * 3, pictureBytes, 3 * (k * tile.width + j * xTiles * tile.width) + (rowTile * tile.width * xTiles * tile.height * 3), tile.width * 3)
                    Next
                Next
            Next
            BGRtoRGB(pictureBytes.Length - 1, pictureBytes)
        End If

        byteToBitmap(pictureBytes, map)
        If IsNothing(whiteFilter.balancedBytes) Then whiteFilter.fetchPicture(pictureBytes, map, height, width) 'Check if picture has already been loaded in balancedbytes
        If white Then byteToBitmap(whiteFilter.balancedBytes, map) 'Checks if auto white balance should be applied or not
    End Sub

    Public Sub loadZoomBox(maplocation As Point, white As Boolean)
        If whiteFilter.getupdateBalanced Then
            whiteFilter.reloadBalancedBytes(pictureBytes)
        End If

        'Goes through the height of the picturebox
        For i = 0 To zoomBox.height - 1
            If white Then 'Check if auto white balance should be applied 
                Buffer.BlockCopy(whiteFilter.balancedBytes, i * width * 3 + 3 * width * (-maplocation.Y) + 3 * (-maplocation.X), zoomBox.bytes, i * zoomBox.width * 3, zoomBox.width * 3)
            Else
                Buffer.BlockCopy(pictureBytes, i * width * 3 + 3 * width * (-maplocation.Y) + 3 * (-maplocation.X), zoomBox.bytes, i * zoomBox.width * 3, zoomBox.width * 3)
            End If
        Next

        byteToBitmap(zoomBox.bytes, map)
    End Sub

    Private Sub readPageInfo() 'Reads size info from the current page in a tiff file
        Dim value As FieldValue()
        value = tiff.GetField(TiffTag.IMAGEWIDTH)
        width = value(0).ToInt()
        value = tiff.GetField(TiffTag.IMAGELENGTH)
        height = value(0).ToInt()
        value = tiff.GetField(TiffTag.TILEWIDTH)
        tile.width = value(0).ToInt()
        value = tiff.GetField(TiffTag.TILELENGTH)
        tile.height = value(0).ToInt()
    End Sub

    Private Sub BGRtoRGB(size As Integer, ByRef bytes() As Byte) 'Changes BGR array to RGB 
        Dim temp As Integer
        Dim index As Integer = 0
        For i = 0 To size / 3
            temp = bytes(index)
            bytes(index) = bytes(index + 2)
            bytes(index + 2) = temp
            index += 3
        Next
    End Sub

    Public Sub moveUp(ByRef y As Integer) 'Loads the group of tiles above the current one
        y -= tile.height 'Compensates the picturebox location to make the transition seamless
        tile.index = tile.index - xTiles
        loadTilesGroup()
    End Sub

    Public Sub moveDown(ByRef y As Integer) 'Loads the group of tiles below the current one
        y += tile.height 'Compensates the picturebox location to make the transition seamless
        tile.index = tile.index + xTiles
        loadTilesGroup()
    End Sub

    Public Sub moveLeft(ByRef x As Integer) 'Loads the group of tiles to the left of the current one
        x -= tile.width 'Compensates the picturebox location to make the transition seamless
        tile.index -= 1
        loadTilesGroup()
    End Sub

    Public Sub moveRight(ByRef x As Integer) 'Loads the group of tiles to the right of the current one
        x += tile.width 'Compensates the picturebox location to make the transition seamless
        tile.index += 1
        loadTilesGroup()
    End Sub

    Public Function rightCheck() 'Checks if there is enough room to call moveRight()
        If Not ((tile.index + nTiles - 1) Mod xTiles) = xTiles - 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function leftCheck() 'Checks if there is enough room to call moveLeft()
        If Not tile.index Mod xTiles = 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function upCheck() 'Checks if there is enough room to call moveUp()
        If tile.index - xTiles >= 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function downCheck() 'Checks if there is enough room to call moveDown()
        If tile.index + nTiles * xTiles < xTiles * yTiles Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub checkBounds(ByRef MapLocation As Point) 'Makes sure the user can't move the picture out of the bounds of the picturebox
        If MapLocation.X > 0 Then MapLocation.X = 0
        If MapLocation.X < -(tile.width * nTiles - boxWidth) Then MapLocation.X = -(tile.width * nTiles - boxWidth)
        If MapLocation.Y > 0 Then MapLocation.Y = 0
        If MapLocation.Y < -(tile.height * nTiles - boxHeight) Then MapLocation.Y = -(tile.height * nTiles - boxHeight)
    End Sub

    Public Sub checkZoomBoxBounds(ByRef maplocation As Point) 'Makes sure that the maplocation coordinates are within the bounds
        If maplocation.X > 0 Then maplocation.X = 0
        If maplocation.X < -(width - zoomBox.width) Then maplocation.X = -(width - zoomBox.width)
        If maplocation.Y > 0 Then maplocation.Y = 0
        If maplocation.Y < -(height - zoomBox.height) Then maplocation.Y = -(height - zoomBox.height)
    End Sub

    Public Function isDigital() 'Checks if we are in the digital zoom mode
        Return digZoomBool
    End Function

    Public Function pageZero() 'Makes sure we don't zoom past page 0
        If tile.page = 0 Then
            Return True
        End If
        Return False
    End Function

    Public Function mouseTreshold(mouselocation As Point) 'Tresholds the mouse pointer coordinate within the picturebox 
        If mouselocation.X < 0 Then Return False
        If mouselocation.Y < 0 Then Return False
        If mouselocation.X > boxWidth Then Return False
        If mouselocation.Y > boxHeight Then Return False
        Return True
    End Function

    '------------------------------------------------------------------------
    'Auto White Balance functions
    '------------------------------------------------------------------------

    Public Sub updateWhiteBalanceRGB(maplocation As Point, white As Boolean, refresh As Boolean) 'Calculates white balance RGB thresholds from whatever is currently loaded in the picturebox
        If digZoomBool Then 'Check if in digital zoom mode
            If zoomBox.width = width Then 'Looking at whole picture
                If white Then 'If white balance is checked
                    If refresh Then whiteFilter.getTresholds(pictureBytes) 'Calculate new thresholds only when we click the balance button
                    whiteFilter.reloadBalancedBytes(pictureBytes) 'Load the balancedBytes with new thresholds
                    loadPicture(white) 'Load whole picture
                Else 'White balance not checked
                    whiteFilter.getTresholds(pictureBytes) 'Calculate new thresholds only when we click the balance button
                End If
            Else 'Digital zoomed in the whole picture
                If white Then 'If white balance is checked
                    loadZoomBox(maplocation, False) 'Load unfiltered digitally zoomed picture in picturebox 
                    If refresh Then whiteFilter.getTresholds(zoomBox.bytes) 'Calculate new thresholds only when we click the balance button
                    whiteFilter.reloadBalancedBytes(pictureBytes) 'Load the balancedBytes with new thresholds
                    loadZoomBox(maplocation, True) 'Load picturebox with new white balance
                Else 'White balance not checked
                    If refresh Then whiteFilter.getTresholds(zoomBox.bytes) 'Calculate new thresholds only when we click the balance button
                    whiteFilter.reloadBalancedBytes(pictureBytes) 'Load the balancedBytes with new thresholds
                    loadZoomBox(maplocation, False) 'Load unfiltered digitally zoomed picture in picturebox 
                End If
            End If
        Else 'Tile zoom mode
            If white Then 'If white balance is checked
                loadPictureBox(maplocation, False) 'Load unfiltered tile zoomed picture in picturebox
                If refresh Then whiteFilter.getTresholds(boxBytes) 'Calculate new thresholds only when we click the balance button
                loadPictureBox(maplocation, True) 'Load picturebox with new white balance
            Else 'White balance not checked
                loadPictureBox(maplocation, False) 'Load unfiltered tile zoomed picture in picturebox
                If refresh Then whiteFilter.getTresholds(boxBytes) 'Calculate new thresholds only when we click the balance button
            End If
            whiteFilter.setupdateBalanced() 'balancedWhite needs to be updated
        End If
    End Sub

    Public Sub passPercentage(percent As Integer)
        whiteFilter.setPercentage(percent)
    End Sub
End Class
