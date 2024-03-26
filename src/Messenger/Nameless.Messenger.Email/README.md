# Nameless Messenger E-mail

Messenger library for e-mail.

### Using SMTP4Dev for Test Purposes (Docker)

Create a container for SMTP4Dev using the command below:

```bash
docker run -d -p 8080:80 -p 2525:25 --name smtp4dev rnwood/smtp4dev:latest
```

This will start a simple SMTP server on Docker and will serve a page on [http://localhost:8080](http://localhost:8080)

The SMTP server port will be **2525**, if you follow this example.