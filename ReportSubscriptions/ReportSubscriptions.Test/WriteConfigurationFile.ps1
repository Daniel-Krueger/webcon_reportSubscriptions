$filename = "$($env:TEMP)\report.subscription\configuration"
$files = get-childitem -path "..\ReportSubscriptions\bin\Debug\*.dll"
$files | foreach { [System.Reflection.Assembly]::LoadFile($_.FullName)}

$configuration = New-Object ReportSubscriptions.Model.Configuration
$serializer= New-Object System.Xml.Serialization.XmlSerializer( $configuration.GetType() )    
if (Test-Path -Path $filename){
    $stream = [System.IO.File]::OpenText($filename)    
    $configuration = $serializer.Deserialize( $stream)
    $stream.Close()
    $configuration.ImpersonationLogin  = Read-Host -Prompt "Provide the ImpersonationLogin"
    $configuration.ViewUrl = Read-Host -Prompt "Provide the view or report url"
}
else {
    $configuration.ClientId = Read-Host -Prompt "Provide the ClientId"
    $configuration.ClientSecret  = Read-Host -Prompt "Provide the ClientSecret"
    $configuration.ImpersonationLogin  = Read-Host -Prompt "Provide the ImpersonationLogin"
    $configuration.ViewUrl = Read-Host -Prompt "Provide the view or report url"
}
New-Item -Path $filename -ItemType File -Force
$writer = new-object System.IO.StreamWriter($filename)
$serializer.Serialize($writer,$configuration)
$writer.Close()