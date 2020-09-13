'Filters that can be applied to the picture
Imports AForge.Imaging

Public Class whiteBalance 'Auto White Balancing
    Public balancedBytes() As Byte 'Holds a copy of the lowest resolution image with white balance
    Dim percentage(5) As Decimal 'Treshold percentage
    Dim percentChoice As Integer 'User choice of what percentage to apply
    Dim thresholds(5) As threshold
    Dim updateBalanced As Boolean = False 'Keeps track if balancedBytes needs to be updated with new thresholds 
    Structure threshold
        Dim rLow, rHigh As Integer 'Red tresholds for balancing 
        Dim gLow, gHigh As Integer 'Green tresholds for balancing
        Dim bLow, bHigh As Integer 'Blue tresholds for balancing
    End Structure

    Public Sub New()
        percentage = {0.002, 0.004, 0.006, 0.008, 0.01, 0.012}
    End Sub

    'Check the status of updateBalanced
    Public Function getupdateBalanced()
        Return updateBalanced
    End Function

    'Thresholds have changed and balancedBytes need an update
    Public Sub setupdateBalanced()
        updateBalanced = True
    End Sub

    'Sets the percentage of tresholding
    Public Sub setPercentage(percent As Integer)
        percentChoice = percent
    End Sub

    'Copies picture into local array
    Public Sub fetchPicture(bytes() As Byte, map As Bitmap, height As Integer, width As Integer)
        Dim resize As New Filters.ResizeNearestNeighbor(width / 8, height / 8) 'Declare resize object 
        Dim resizedMap As New Bitmap(width / 8, height / 8, Imaging.PixelFormat.Format24bppRgb) 'Declare new bitmap with resized size
        Dim temp((width / 8) * (height / 8) * 3 - 1) As Byte 'Declare temp variable to hold resized bytes

        ReDim balancedBytes(bytes.Length()) 'Initialize balancedBytes to size of picture
        Array.Copy(bytes, balancedBytes, bytes.Length()) 'Copy and store content of bytes
        resizedMap = resize.Apply(map) 'Apply resize filter 
        BitmapToBytes(resizedMap, temp) 'Convert resized pic to bytes
        getTresholds(temp) 'Calculate tresholds
        applyWhiteBalance(balancedBytes) 'Apply white balance on whole picture
    End Sub

    'Reload balancedBytes with new thresholds
    Public Sub reloadBalancedBytes(bytes() As Byte)
        Array.Copy(bytes, balancedBytes, bytes.Length()) 'Copy and store content of bytes
        applyWhiteBalance(balancedBytes)
        updateBalanced = False 'Turn off boolean after update
    End Sub

    'Calculates the RGB tresholds based on the bytes array and stores them in the class
    Public Sub getTresholds(bytes() As Byte)
        Dim index As Integer = 0
        Dim red(bytes.Length() / 3), green(bytes.Length() / 3), blue(bytes.Length() / 3) As Byte 'Declare arrays to hold red, green and blue values
        Dim length As Integer 'Length of current array looped through

        length = (bytes.Length() / 3) - 1
        'Extract rgb values from the bytes
        For i = 0 To length
            red(i) = bytes(index)
            green(i) = bytes(index + 1)
            blue(i) = bytes(index + 2)
            index += 3
        Next
        'Sort values 
        Array.Sort(red)
        Array.Sort(green)
        Array.Sort(blue)
        'Find the tresholds for all possible percentages
        For i = 0 To 5
            thresholds(i).rLow = red(red.Length() * percentage(i) - 1)
            thresholds(i).rHigh = red(red.Length() - red.Length() * percentage(i) - 1)
            thresholds(i).gLow = green(green.Length() * percentage(i) - 1)
            thresholds(i).gHigh = green(green.Length() - green.Length() * percentage(i) - 1)
            thresholds(i).bLow = blue(blue.Length() * percentage(i) - 1)
            thresholds(i).bHigh = blue(blue.Length() - blue.Length() * percentage(i) - 1)
        Next
    End Sub

    'Apply white balance filter on passed bytes
    Public Sub applyWhiteBalance(bytes() As Byte)
        Dim index As Integer = 0
        Dim length As Integer 'Length of current array looped through

        length = (bytes.Length() / 3) - 1
        'Go through bytes and apply auto white balance
        For i = 0 To length
            If bytes(index) < thresholds(percentChoice).rLow Then
                bytes(index) = 0
            Else
                bytes(index) -= thresholds(percentChoice).rLow
            End If

            If bytes(index) * (255 / (thresholds(percentChoice).rHigh - thresholds(percentChoice).rLow)) > 255 Then
                bytes(index) = 255
            Else
                bytes(index) *= (255 / (thresholds(percentChoice).rHigh - thresholds(percentChoice).rLow))
            End If


            If bytes(index + 1) < thresholds(percentChoice).gLow Then
                bytes(index + 1) = 0
            Else
                bytes(index + 1) -= thresholds(percentChoice).gLow
            End If

            If bytes(index + 1) * (255 / (thresholds(percentChoice).gHigh - thresholds(percentChoice).gLow)) > 255 Then
                bytes(index + 1) = 255
            Else
                bytes(index + 1) *= (255 / (thresholds(percentChoice).gHigh - thresholds(percentChoice).gLow))
            End If


            If bytes(index + 2) < thresholds(percentChoice).bLow Then
                bytes(index + 2) = 0
            Else
                bytes(index + 2) -= thresholds(percentChoice).bLow
            End If

            If bytes(index + 2) * (255 / (thresholds(percentChoice).bHigh - thresholds(percentChoice).bLow)) > 255 Then
                bytes(index + 2) = 255
            Else
                bytes(index + 2) *= (255 / (thresholds(percentChoice).bHigh - thresholds(percentChoice).bLow))
            End If
            index += 3
        Next
    End Sub
End Class

Public Class sharpening 'Unsharp Mask
    Dim unsharp As New Filters.Sharpen 'Sharpening filter

    Public Sub applyMask(map As Bitmap)
        unsharp.ApplyInPlace(map)
    End Sub
End Class