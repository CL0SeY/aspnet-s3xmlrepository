Push-Location .\src\S3XmlRepository
try {
  dotnet pack -c Release
}
finally {
  Pop-Location
}
