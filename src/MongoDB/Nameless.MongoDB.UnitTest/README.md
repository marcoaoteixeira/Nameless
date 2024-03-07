# How to Install Mongo on Docker

Simple as run this command on your Terminal:

```
docker run -d --name mongodb -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=root mongo:latest
```

This will create an image for the latest version of Mongo and exposes the port 27017.
Your username and password will be "root".