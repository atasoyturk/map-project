---
paths:
  - "BackendApi/Services/**"
  - "BackendApi/Settings/**"
  - "BackendApi/Program.cs"
---
# Outbound HTTP

- Every HttpClient to an external/public service sends a User-Agent from its settings (`<Service>Settings:UserAgent`, required).
- Log the status code of every non-2xx response before mapping it to an error.
