
$foldertoverify = "D:\Tese\colors\full-recise"

$verifyfolder = "D:\Tese\tese\verify"
$folder0 = $verifyfolder+"\0"
$folder1 = $verifyfolder+"\1"
$folder2 = $verifyfolder+"\2"


if (-not (Test-Path -Path $verifyfolder)) {
    $ret = New-Item $verifyfolder -ItemType Directory
}
if (-not (Test-Path -Path $folder0)) {
    $ret = New-Item $folder0 -ItemType Directory
}
if (-not (Test-Path -Path $folder1)) {
    $ret = New-Item $folder1 -ItemType Directory
}
if (-not (Test-Path -Path $folder2)) {
    $ret = New-Item $folder2 -ItemType Directory
}


Get-ChildItem $foldertoverify -Include *.jpg -Recurse | 
Foreach-Object {
    $file = $_.FullName
    $content = Get-Content $file.Replace(".jpg",".txt")
    if($content.StartsWith("0")){
        Copy-item -Path $file -Destination $folder0
        Copy-item -Path $file.Replace(".jpg",".txt") -Destination $folder0
    }elseif($content.StartsWith("1")){
        Copy-item -Path $file -Destination $folder1
        Copy-item -Path $file.Replace(".jpg",".txt") -Destination $folder1
    }elseif($content.StartsWith("2")){
        Copy-item -Path $file -Destination $folder2
        Copy-item -Path $file.Replace(".jpg",".txt") -Destination $folder2
    }else{
        Write-Error "ERROR"
    }
}
