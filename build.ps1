Push-Location .\src\S3XmlRepository
try {
  dotnet pack -c Release -o ..\..\artifacts
}
finally {
  Pop-Location
}
