# TrainSchedulesParserAPI
:monorail: Train Schedules Parser API to be used with Home Assistant :metal:

<h3>PC </h3>

```
docker build -t acamoes/trainschedulesapi:sixth .

docker push acamoes/trainschedulesapi:sixth
```

<h3>RPI </h3>

```
docker run -ti acamoes/trainschedulesapi:sixth

docker run -d --restart unless-stopped -p 4003:80 acamoes/trainschedulesapi:sixth
```
