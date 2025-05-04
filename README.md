# 🧱 Microservices-Based E-Commerce Backend with .NET 8

This project is a fully functional backend system for a sample e-commerce platform, built using **Microservices Architecture** in **.NET 8**. Each core business feature is developed as an isolated service, promoting scalability, maintainability, and resilience.

---

## 🚀 Features

### 🔐 Authentication Service
- JWT-based authentication & authorization
- Login and registration endpoints
- Role-based access control

### 📦 Product Service
- Full CRUD support
- Product availability & stock management
- Returns product data to the Order Service via HTTP calls

### 🛒 Order Service
- Place orders and fetch client-specific orders
- Validates product availability before confirming orders
- Communicates with the Product Service using `HttpClient` and retry policies

### 🌐 API Gateway (Ocelot)
- Centralized routing for all services
- Service discovery and load balancing
- Caching with `Ocelot.Cache`
- Resilience via `Polly` retry & fallback policies

---

## 🛠️ Tech Stack

| Layer | Technology |
|------|-------------|
| Backend | .NET 8, C# |
| ORM | Entity Framework Core |
| API Gateway | Ocelot + Ocelot.Cache |
| Resilience | Polly |
| Authentication | JWT |
| Testing | xUnit, FakeItEasy, FluentAssertions |
| Database | SQL Server |
| Architecture | Clean Architecture, Onion Architecture |

---

## ✅ Unit Testing

Thoroughly tested critical services, especially `OrderService`, using:
- `xUnit`
- `FakeItEasy` for mocking repositories and dependencies
- `HttpClient` mocking with custom message handlers
- `FluentAssertions` for readable and expressive assertions

---

## 🔄 Resilience with Polly

Implemented retry and fallback policies for inter-service HTTP communication using Polly to:
- Retry transient failures
- Fallback with a safe default or null result when needed
- Log retries for observability

---

## 🧪 Sample Test Code (OrderService)

```csharp
[Fact]
public async Task GetProduct_ValidProductId_ReturnProduct()
{
    int productId = 1;
    var result = await _orderService.GetProduct(productId);

    result.Should().NotBeNull();
    result.Id.Should().Be(productId);
}

🔧 How to Run
Prerequisites: .NET 8 SDK, SQL Server

bash
Copy
Edit
# Navigate into each service folder and run:
dotnet ef database update
dotnet run
Use Postman or Swagger to interact with services.

API Gateway runs on port 7000 by default.
