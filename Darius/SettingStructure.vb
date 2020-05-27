Imports System.Xml

Public Class SettingStructure
    'For reading the settings
    Public Xmld As XmlDocument
    Public Address As String

    Public Sub New(Addressin As String)

        Address = Addressin
        Xmld = New XmlDocument
        Xmld.Load(Address)
    End Sub

    Public Function Gett(node As String) As Object
        node = node.ToUpper
        Return Xmld.SelectSingleNode("/SETTINGS/" & node).InnerText
    End Function

    Public Sub Sett(node As String, value As String)
        node = node.ToUpper
        Xmld.SelectSingleNode("/SETTINGS/" & node).InnerText = value
        Xmld.Save(Address)
    End Sub



End Class
