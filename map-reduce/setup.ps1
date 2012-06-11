Function SendRequest ($method,$url,$data)
{
    $request = [net.webrequest]::Create($url)
    $request.Method = $method

    $bytes = [text.encoding]::UTF8.getbytes($data)
    $request.ContentType = "application/json"
    $request.ContentLength = $bytes.Length

    $writer = $request.GetRequestStream()
    $writer.Write($bytes, 0, $bytes.Length)
    $writer.Close()

    $response = $request.GetResponse()
}

SendRequest "DELETE" "http://localhost:5984/map-reduce-samples" ""
SendRequest "PUT" "http://localhost:5984/map-reduce-samples" ""

Write-Host "Inserting sample docs"
foreach ($file in ls docs/*.json) {
    $id=$file.BaseName
    $data=gc $file
    SendRequest "PUT" "http://localhost:5984/map-reduce-samples/$id" $data
}

Write-Host "Inserting sample design docs"
foreach ($file in ls views/*.json) {
    $id=$file.BaseName
    $data=gc $file
    SendRequest "PUT" "http://localhost:5984/map-reduce-samples/_design/$id" $data
}

Start-Process "http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?reduce=false"
Start-Process "http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?group_level=1"
Start-Process "http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?group_level=2"
Start-Process "http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?startkey=[2011]&endkey=[2011,{}]"
