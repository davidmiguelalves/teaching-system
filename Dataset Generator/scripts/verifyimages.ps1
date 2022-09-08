param(
     [Parameter()]
     [string]$folder
)

<#
$folder = "D:\Tese\shapes\teste"
#>

Get-ChildItem $folder -Include *.jpg | 
Foreach-Object {
    $file = $_.FullName

#verify if txt file exists!!!
    $content = Get-Content $file.Replace(".jpg",".txt")
    $destfolder = $folder + "\" + $content.split(" ")[0]

    if (-not (Test-Path -Path $destfolder)) {
        $ret = New-Item $destfolder -ItemType Directory
    }
    Copy-item -Path $file -Destination $destfolder -Force
}
