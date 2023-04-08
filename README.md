# Trakt Show & Season Rating Manager

This is a script that can be run in Docker, and can be set to run recurringly through starting the Docker container via cron. The script goes through all your rated episodes on Trakt, calculates the average rating per season and per show, and sets the ratings for the seasons and the show in Trakt.

## Installation

1. Create a folder on your Linux server with Docker and create the following `docker-compose.yml` inside of it:
```
version: '2'
services:
traktmanager:
image: 'koertlichtendonk/traktshowseasonratingmanager'
volumes:
- .:/usr/share/TraktShowSeasonRatingManager
```

2. Start the Docker container once, and it should create the following `Config.json`. Edit it with the appropriate values. You manually have to get the first access token/refresh token yourself, through, for example, Insomnia.REST.
```
{
"Token": {
"access_token": "",
"token_type": "",
"expires_in": 0,
"refresh_token": "",
"scope": "",
"created_at": 0
},
"AppInformation": {
"refresh_uri": "",
"client_id": "",
"client_secret": ""
},
"Username": "",
"Enabled": false
}
```


3. Check the Docker logs to make sure there aren't any exceptions ( `docker logs --follow [DOCKER CONTAINER]` ).

4. Add the docker start command to crontab (do `crontab -e`) and add the following line:
```
0 0 * * * docker start -a [DOCKER CONTAINER] > [PATH_TO_THE_FOLDER_YOU_MADE]/log.txt
```
5. With the above cron configuration, the script will now execute every night at midnight. The calculated average ratings for each season and show will be updated on Trakt accordingly.
