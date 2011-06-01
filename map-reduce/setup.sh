#!/bin/bash
echo "Creating database"
curl -X DELETE localhost:5984/map-reduce-samples
curl -X PUT localhost:5984/map-reduce-samples

echo "Inserting sample docs"
for f in docs/*.json
do
	id=$(basename ${f%%.json})
	curl -X PUT localhost:5984/map-reduce-samples/$id -H "Content-Type: application/json" --data @$f
done

echo "Inserting sample design docs"
for f in views/*.json
do
	id=$(basename ${f%%.json})
	curl -X PUT localhost:5984/map-reduce-samples/_design/$id -H "Content-Type: application/json" --data @$f
done

open http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?reduce=false
open http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?group_level=1
open http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?group_level=2
open "http://localhost:5984/map-reduce-samples/_design/purchases/_view/by_date?startkey=[2011]&endkey=[2011,{}]"
