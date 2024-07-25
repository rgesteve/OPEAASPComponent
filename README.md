# Vary basic OPEA-compatible, SK-based component

To run (for testing, by default runs on port 5000):
```
dotnet run 
```

This assumes there's a redis-stack server running (for the moment connection string is hard-coded, but should be configurable from appsettings*json).

An easy way to set this up is to use the `redis/redis-stack-server` image from Dockerhub:
```sh
docker run -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server:latest
```


