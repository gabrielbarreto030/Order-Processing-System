# 🛒 Order Processing System

REST API for order processing with a queue and asynchronous worker, built with .NET 8 following DDD, SOLID principles and scalable architecture.

The system simulates a real-world e-commerce scenario where orders are processed in the background using RabbitMQ.

---

## 📌 Description

This project demonstrates how to build a robust and scalable system using asynchronous processing.

Main Flow:

1. User creates an order via API
2. Order is saved in the database
3. Message is sent to RabbitMQ
4. Worker consumes the message
5. Order is processed (payment, inventory, shipping)
6. Order status is updated

---

## 🧠 Architecture

The project follows **Clean Architecture + DDD**:

- **Domain** → Entities and business rules
- **Application** → Use cases and services
- **Infrastructure** → Database, RabbitMQ, external services
- **API** → Controllers and endpoints
- **Worker** → Asynchronous message processing

### Applied Principles:

- SOLID
- Separation of Concerns
- Dependency Injection
- Asynchronous processing
- Horizontal scalability

---

## 🛠 Technologies

- **.NET 8** – Main Framework
- **C#**
- **SQL Server / EF Core** – Persistence
- **RabbitMQ** – Messaging
- **Serilog** – Structured Logging
- **Docker** – Containerization
- **Swagger/OpenAPI** – Documentation
- **HealthChecks** – Monitoring
- **xUnit** – Testing

---

## ⚡ Features

- ✅ Order Creation
- ✅ Asynchronous Processing with RabbitMQ
- ✅ Worker for Background Processing
- ✅ Order Status Control:

- Pending

- Processing

- Completed

- Failed
- ✅ Automatic Retry on Failures
- ✅ Dead-Letter Queue (DLQ)
- ✅ Structured Logging
- ✅ Clean and Scalable Architecture

---

## 🔄 Flow of System

```text
Client → API → Database → RabbitMQ → Worker → Update Order Status
▶️ How to run
1. Clone the repository
git clone https://github.com/your-repo.git
2. Add dependencies (RabbitMQ + DB)
docker-compose up -d
3. Run API
dotnet run --project src/API
4. Run Worker
dotnet run --project src/Worker
5. Access Swagger
http://localhost:5000/swagger
🧪 Tests
dotnet test
📊 Real-World Scenario

This type of architecture is widely used in:

E-commerce (Amazon, Mercado Livre)
Payment systems
Delivery platforms
Highly concurrent systems
🚀 Technical Advantages
Asynchronous processing for high Performance
Reduced load on the main API
Resilient system with retry and DLQ
Scalability with multiple workers
Low coupling between services
📅 Backlog (30 days)
Phase 1 – Fundamentals (Days 1–5)

Day 1: Create solution + Clean Architecture structure
Day 2: Configure API + Swagger + HealthCheck
Day 3: Create Order entity (Id, Status, Amount, CreatedAt)
Day 4: Configure EF Core + DbContext
Day 5: Repository Pattern for Orders

Phase 2 – Order API (Days 6–10)

Day 6: Endpoint to create order
Day 7: Endpoint to list orders
Day 8: Endpoint to search by ID
Day 9: Basic validations
Day 10: Logging with Serilog

Phase 3 – RabbitMQ (Days 11–15)

Day Phase 11: Configure RabbitMQ
Day 12: Publish message when creating order
Day 13: Create order queue
Day 14: Structure message (DTO)
Day 15: Test message sending

Phase 4 – Worker (Days 16–20)

Day 16: Create Worker Service
Day 17: Consume messages from the queue
Day 18: Update order status → Processing
Day 19: Simulate processing (delay/payment)
Day 20: Update status → Completed

Phase 5 – Resilience (Days 21–25)

Day 21: Implement automatic retry
Day 22: Create Dead Letter Queue (DLQ)
Day 23: Error handling in the worker
Day 24: Detailed failure logs
Day 25: Simulate controlled failures

Phase 6 – Quality (Days 26–28)

Day 26: Unit Tests (Domain)

Day 27: Unit Tests (Application)
Day 28: Integration Tests

Phase 7 – Polishing (Days 29–30)

Day 29: Architecture improvements + refactoring
Day 30: Final README + documentation

Phase 8 – Future Developments
Add Redis for caching
Add JWT authentication
Create order dashboard
Implement rate limiting
Deploy to Azure / AWS
Add metrics (Prometheus / Grafana)

👨‍💻 Author

Gabriel Barreto
Senior FullStack Developer

🌎 Open to international remote opportunities