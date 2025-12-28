# How to test

First cd into src directory `cd src` then run `docker compose up` or `podman compose up`.

Access grafana at `http://localhost:3000` the initial username and password are `admin` `admin`.

Configure the data sources as follows:

Loki at `http://loki:3100`
Tempo at `http://tempo:3200`
Prometheus at `http://prometheus:9090`

Have fun messing with grafana!
