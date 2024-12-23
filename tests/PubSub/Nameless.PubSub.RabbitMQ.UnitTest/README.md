**Running Tests**

Most of the tests present in this project should run only on dev's local machine since is the most likely place to have any system required, like RabbitMQ.

Use the command below to install and run a docker image at localhost to enable the tests:

```cmd
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest rabbitmq:4.0.5-management-alpine
```