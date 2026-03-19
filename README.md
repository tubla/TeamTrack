# TeamTrack

# To Run Container App using Docker:
docker build --no-cache -t teamtrack-api .
docker run -d -p 8080:80 -e Swagger__Enabled=true -e ASPNETCORE_URLS="http://+:80" teamtrack-api