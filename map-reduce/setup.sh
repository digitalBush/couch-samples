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