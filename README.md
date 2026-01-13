# Customer Service

Microservice for managing customer information in the Mortgage Application system.

## Features

- Customer CRUD operations
- Address management
- Employment history
- Credit history tracking

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/customers | Get all customers |
| GET | /api/customers/{id} | Get customer by ID |
| GET | /api/customers/email/{email} | Get customer by email |
| POST | /api/customers | Create customer |
| PUT | /api/customers/{id} | Update customer |
| DELETE | /api/customers/{id} | Delete customer |
| GET | /api/customers/{id}/credit | Get credit history |
| PUT | /api/customers/{id}/credit | Update credit history |
| GET | /api/customers/{id}/employments | Get employments |
| POST | /api/customers/{id}/employments | Add employment |

## Running Locally

```bash
cd src/Customer.API
dotnet run
```

Access Swagger UI: http://localhost:5001/swagger

## Docker

```bash
docker build -t customer-service .
docker run -p 5001:5001 customer-service
```

## Dependencies

- None (Core Service)

## Consumed By

- Loans Service
- Payments Service
- Loan Application Service
