# CurieBot
CO2/temperature measurement, reporting to Slack and InfluxDb


Installation:

1) Install InfluxDb (https://portal.influxdata.com/downloads)


2) Install Grafana (https://grafana.com/get)


3) Initial InfluxDb setup:
Start influxd.exe, then run following commands using influx.exe:

>create database co2
>create user grafana with password 'grafana'
>grant read on co2 to grafana
>create user backend with password 'backend'
>grant all on co2 to backend

You can view the existing databases and users using the following commands:
>show databases
>show users


4) Initial Grafana Setup:
Copy conf\sample.ini to a file named conf/custom.ini and change the web server port to 8080. The default Grafana port, 3000, requires special privileges on Windows.
Then you will be able to access grafana web ui via http://localhost:8080 using credentials admin/admin

Enable grafana anonymous access in config:
enable anonymous access
enabled = true

Using grafana web ui add InfluxDb datasource (http://localhost:8086 grafana/grafana)

Using grafana web ui import the grafana dashboard 'CO2 graph' from the template ..\SetUp\CO2-1493915595083_dashboard_template.json
Make changes if necessary.


5) Initial Slack setup:
Add a bot to your Slack team (useful info https://api.slack.com/bot-users)
Copy the api-token to the config file 'CurieBot.exe.config' (appSettings\SlackBotApiToken)

Add bot to the channel where it will be posting notifications. Copy this channel chatID to the config slackSettings\chatId


6) You might be willing to install the bundle influxdb/grafana/curiebot as windows services
To do that you can use nssm (https://nssm.cc/)
Command to use:
>"<path>\nssm.exe" install <servicename>
This will open a ui where you'll be able to configure the service parameters.
Some examples:
>"<path>\nssm.exe" install grafana
>"<path>\nssm.exe" install influxdb
>"<path>\nssm.exe" install curiebot




Possible issues:
1) Grafana throws 'There was an error writing history file: open : The system cannot find the file specified'
Adding environment variable HOME with value <path to exe location> should solve the problem.

